using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class Overlay : MonoBehaviour
{
    public static Overlay Instance { get; private set; }


    [Header("Popup")]
    [SerializeField] CanvasGroup popupCanvasGroup;
    [SerializeField] Image popupImage;
    [SerializeField] Image backgroundImage;
    [SerializeField] Image introImage;
    [SerializeField] float popupFadeDuration = 0.20f;

    [Header("Prompt")]
    [SerializeField] TextMeshProUGUI promptLabel;

    [Header("Scene")]
    [SerializeField] Button sceneSwitchButton;

    Camera cam;
    Coroutine popupFadeRoutine;

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
            backgroundImage.gameObject.SetActive(true);

        if (introImage != null)
            introImage.gameObject.SetActive(true);

        if (popupImage != null)
            popupImage.gameObject.SetActive(false);

        if (promptLabel != null)
            promptLabel.gameObject.SetActive(false);

        cam = Camera.main;

        if (popupCanvasGroup != null)
            popupCanvasGroup.alpha = 0f;
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

    public void ShowButton()
    {
        if (sceneSwitchButton == null)
            return;

        sceneSwitchButton.gameObject.SetActive(true);
    }

    public void HideButton()
    {
        if (sceneSwitchButton == null)
            return;

        sceneSwitchButton.gameObject.SetActive(false);
    }

    public void HideIntroImage()
    {
        if (introImage == null)
            return;

        introImage.gameObject.SetActive(false);
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
        if (popupImage == null || backgroundImage == null || popupCanvasGroup == null)
            return;

        popupImage.sprite = s;

        backgroundImage.gameObject.SetActive(true);
        popupImage.gameObject.SetActive(true);

        CenterPopup();
        StartPopupFade(1f);
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
        if (popupCanvasGroup == null)
        {
            if (popupImage != null)
                popupImage.gameObject.SetActive(false);
            if (backgroundImage != null)
                backgroundImage.gameObject.SetActive(false);

            return;
        }

        StartPopupFade(0f);
    }

    void StartPopupFade(float targetAlpha)
    {
        if (popupFadeRoutine != null)
            StopCoroutine(popupFadeRoutine);

        popupFadeRoutine = StartCoroutine(FadeCanvasGroup(popupCanvasGroup, targetAlpha));
    }

    IEnumerator FadeCanvasGroup(CanvasGroup cg, float target)
    {
        float start = cg.alpha;
        float time = 0f;

        while (time < popupFadeDuration)
        {
            time += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(time / popupFadeDuration);
            cg.alpha = Mathf.Lerp(start, target, t);
            yield return null;
        }

        cg.alpha = target;

        if (Mathf.Approximately(target, 0f))
        {
            if (popupImage != null)
                popupImage.gameObject.SetActive(false);
            if (backgroundImage != null)
                backgroundImage.gameObject.SetActive(false);
        }
        else
        {
            if (popupImage != null)
                popupImage.gameObject.SetActive(true);
            if (backgroundImage != null)
                backgroundImage.gameObject.SetActive(true);
        }
    }
}