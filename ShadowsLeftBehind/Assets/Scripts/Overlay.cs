using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Overlay : MonoBehaviour
{
    public static Overlay Instance { get; private set; }

    //one of these with no art
    [SerializeField] Image popupImage;
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
        cam = Camera.main;
    }

    void OnEnable()
    {
        if (cam == null) cam = Camera.main;
    }

    public void ShowPrompt(string text, Vector3 worldPos)
    {
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

    public void HidePrompt() => promptLabel.gameObject.SetActive(false);

    public void ShowPopup(Sprite s, Vector3 worldPos, Vector3 worldOffset)
    {
        popupImage.sprite = s;
        popupImage.gameObject.SetActive(true);
        UpdatePopupPosition(worldPos + worldOffset);
    }

    public void UpdatePopupPosition(Vector3 worldPos)
    {
        if (!popupImage.gameObject.activeSelf)
            return;

        if (cam == null)
            cam = Camera.main;

        Vector3 screen = RectTransformUtility.WorldToScreenPoint(cam, worldPos);
        popupImage.rectTransform.position = screen;
    }

    public void HidePopup()
    {
        popupImage.gameObject.SetActive(false);
    }
}
