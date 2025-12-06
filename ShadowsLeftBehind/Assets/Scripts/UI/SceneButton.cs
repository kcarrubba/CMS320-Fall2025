using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneButton : MonoBehaviour
{
    [SerializeField] string targetScene;
    [SerializeField] bool movePlayer = true;

    public void LoadScene()
    {
        Debug.Log($"Play button clicked, targetScene = '{targetScene}'");

        if (string.IsNullOrEmpty(targetScene))
        {
            Debug.LogError("SceneButton: targetScene is empty! Set it in the Inspector.");
            return;
        }

        // If the GameManager singleton isn't available for some reason,
        // fall back to a simple single-scene load so the build still works.
        if (GameManager.instance == null)
        {
            Debug.LogError("SceneButton: GameManager.instance is NULL, loading scene directly.");
            SceneManager.LoadScene(targetScene, LoadSceneMode.Single);
            return;
        }

        try
        {
            GameManager.instance.SwitchTo(targetScene, movePlayer);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"SceneButton: SwitchTo threw an exception: {ex.Message}\n{ex.StackTrace}");
            // Fallback so the player isn’t stuck on the menu
            SceneManager.LoadScene(targetScene, LoadSceneMode.Single);
        }
    }
}
