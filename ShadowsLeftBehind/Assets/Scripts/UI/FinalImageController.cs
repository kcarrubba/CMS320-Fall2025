using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FinalImageController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image targetImage;
    [SerializeField] private Button backToMenuButton;

    [Header("Scene")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private bool shown;

    private void Awake()
    {
        if (!backToMenuButton || !targetImage)
            return;

        targetImage.enabled = false;
        backToMenuButton.gameObject.SetActive(false);

        backToMenuButton.onClick.RemoveListener(GoToMainMenu);
        backToMenuButton.onClick.AddListener(GoToMainMenu);
    }

    private void Update()
    {
        if (shown) 
            return;

        var mgr = InteractablesManager.Instance;

        if (!mgr || !mgr.AllCluesFoundInScene)
            return;

        Debug.Log("Clues Found for attic!");

        targetImage.enabled = true;
        backToMenuButton.gameObject.SetActive(true);

        this.shown = true;
    }

    private void GoToMainMenu()
    {
        GameManager.instance.SwitchTo(mainMenuSceneName);
    }
}
