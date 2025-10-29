using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using System.Linq;
public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] GameObject playerPrefab;

    GameObject player;
    string pendingSpawnId = "default";
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        //load 1 eventsystem for all scenes
        if (EventSystem.current == null)
        {
            var eventSystem = new GameObject("EventSystem", typeof(EventSystem));
            DontDestroyOnLoad(eventSystem);

            eventSystem.AddComponent<InputSystemUIInputModule>();
        }

        //create the player instance (for all scenes)
        if (player == null && playerPrefab != null)
        {
            player = Instantiate(playerPrefab);
            DontDestroyOnLoad(player);

            SceneManager.MoveGameObjectToScene(player, gameObject.scene);
        }
    }

    //load the main menu first
    private void Start()
    {
        //unload any scenes we were debugging
        this.UnloadAllScenes();

        var initalScene = SceneManager.GetSceneByName("MainMenu");

        if (!initalScene.isLoaded)
            StartCoroutine(LoadAndActivate("MainMenu"));
        else
            SceneManager.SetActiveScene(initalScene);
    }

    private void UnloadAllScenes()
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            var scene = SceneManager.GetSceneAt(i);
            if (scene.isLoaded && scene.name != "GameManager")
            {
                SceneManager.UnloadSceneAsync(scene);
            }
        }
    }

    public void SwitchTo(string sceneName, string spawnId = "default")
    {
        pendingSpawnId = spawnId;
        StartCoroutine(SwitchRoutine(sceneName));
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    //callback
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (player == null) 
            return;

        var spawns = Object.FindObjectsByType<SpawnPoint>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        if (spawns == null || spawns.Length == 0) 
            return;

        var target =
            spawns.FirstOrDefault(s => s.id == pendingSpawnId) ??
            spawns.FirstOrDefault(s => s.id == "default") ??
            spawns.First();

        player.transform.position = target.transform.position;

        //face the sprite the correct way
        var pc = player.GetComponent<PlayerController>();

        if (pc) 
            pc.Face(target.facingDirection);
    }

    private IEnumerator LoadAndActivate(string sceneName)
    {
        var scene = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        while (!scene.isDone)
            yield return null;

        var loaded = SceneManager.GetSceneByName(sceneName);
        SceneManager.SetActiveScene(loaded);
    }

    public void SwitchScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    public IEnumerator SwitchRoutine(string nextScene)
    {
        var load = SceneManager.LoadSceneAsync(nextScene, LoadSceneMode.Additive);

        while (!load.isDone)
            yield return null;

        var newScene = SceneManager.GetSceneByName(nextScene);
        SceneManager.SetActiveScene(newScene);

        //unload previous scene contents
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            var s = SceneManager.GetSceneAt(i);

            if (s == null)
                continue;

            if (s.isLoaded && s.name != newScene.name && s.name != "GameManager")
                yield return SceneManager.UnloadSceneAsync(s);
        }
    }
}
