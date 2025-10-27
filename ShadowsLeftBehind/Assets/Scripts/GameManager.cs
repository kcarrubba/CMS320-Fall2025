using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    //load the main menu first
    private void Start()
    {
        StartCoroutine(LoadAndActivate("MainMenu"));
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

    private IEnumerator SwitchRoutine(string nextScene)
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

            if (s.isLoaded && s.name != newScene.name && s.name != "Bootstrapper")
                yield return SceneManager.UnloadSceneAsync(s);
        }
    }
}
