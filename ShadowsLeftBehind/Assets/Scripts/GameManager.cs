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
    string pendingSceneForMove = null;
    bool shouldMovePlayerOnNextLoad = true;

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

    private void Start()
    {
        StartCoroutine(Bootstrap());
    }

    private IEnumerator Bootstrap()
    {
        var toUnload = new System.Collections.Generic.List<Scene>();

        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            var scene = SceneManager.GetSceneAt(i);
            if (scene.isLoaded && scene.name != "GameManager")
            {
                toUnload.Add(scene);
            }
        }

        foreach (var s in toUnload)
        {
            var op = SceneManager.UnloadSceneAsync(s);
            if (op != null)
            {
                while (!op.isDone)
                    yield return null;
            }
        }

        yield return LoadAndActivate("Kitchen");
    }

    public void SwitchTo(string sceneName, bool movePlayer = true, string spawnId = "default")
    {
        pendingSpawnId = spawnId;

        if (movePlayer)
        {
            shouldMovePlayerOnNextLoad = true;
            pendingSceneForMove = sceneName;

            if (player != null && !player.activeSelf)
                player.SetActive(true);
        }
        else
        {
            shouldMovePlayerOnNextLoad = false;
            pendingSceneForMove = null;

            if (player != null)
                player.SetActive(false);
        }

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
        if (!shouldMovePlayerOnNextLoad)
            return;

        if (pendingSceneForMove == null || scene.name != pendingSceneForMove)
            return;

        if (player == null) 
            return;

        shouldMovePlayerOnNextLoad = false;
        pendingSceneForMove = null;

        var spawns = Object.FindObjectsByType<SpawnPoint>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        ).Where(s => s.gameObject.scene == scene).ToArray();

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
