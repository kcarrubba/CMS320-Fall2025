using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
public class GameManager : MonoBehaviour
{
    public static GameManager instance;

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
    }

    //load the main menu first
    private void Start()
    {
        const string initalScene = "MainMenu";
        var s = SceneManager.GetSceneByName(initalScene);

        if (!s.isLoaded)
            StartCoroutine(LoadAndActivate(initalScene));
        else
            SceneManager.SetActiveScene(s);
    }

    public void SwitchTo(string sceneName)
    {
        StartCoroutine(SwitchRoutine(sceneName));
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
