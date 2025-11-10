using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Overlay : MonoBehaviour
{
    public static Overlay Instance { get; private set; }

    [SerializeField] Image popupImage;
    [SerializeField] Image backgroundImage;
    [SerializeField] TextMeshProUGUI promptLabel;

    Camera cam;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (backgroundImage != null)
            backgroundImage.gameObject.SetActive(false);

        if (popupImage != null)
            popupImage.gameObject.SetActive(false);

        if (promptLabel != null)
            promptLabel.gameObject.SetActive(false);

        cam = Camera.main;
    }

    void OnEnable()
    {
        if (cam == null) cam = Camera.main;
    }

    public bool IsPopupVisible => popupImage != null && popupImage.gameObject.activeSelf;

    public void ShowPrompt(string text, Vector3 worldPos)
    {
        if (promptLabel == null) return;

        promptLabel.text = text;
        promptLabel.gameObject.SetActive(true);
        UpdatePromptPosition(worldPos);
    }

    public void UpdatePromptPosition(Vector3 worldPos)
    {
        if (!promptLabel.gameObject.activeSelf)
            return;

        if (cam == null)
            cam = Camera.main;

        Vector3 screen = cam.WorldToScreenPoint(worldPos);
        promptLabel.rectTransform.position = screen;
    }

    public void HidePrompt()
    {
        if (promptLabel != null)
            promptLabel.gameObject.SetActive(false);
    }

    public void ShowPopup(Sprite s)
    {
        if (popupImage == null || backgroundImage == null)
            return;

        popupImage.sprite = s;
        popupImage.gameObject.SetActive(true);
        backgroundImage.gameObject.SetActive(true);

        CenterPopup();
    }

    void CenterPopup()
    {
        var rt = popupImage.rectTransform;

        // force it to be centered in the canvas
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero;
    }

    public void HidePopup()
    {
        if (popupImage != null)
            popupImage.gameObject.SetActive(false);

        if (backgroundImage != null)
            backgroundImage.gameObject.SetActive(false);
    }
}
