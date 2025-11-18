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
    [SerializeField] float popupFadeDuration = 0.25f;

    [Header("Intro")]
    [SerializeField] Image introImage;
    [SerializeField] CanvasGroup introCanvasGroup;
    [SerializeField] float introFadeDuration = 0.25f;

    [Header("Prompt")]
    [SerializeField] TextMeshProUGUI promptLabel;

    [Header("Scene")]
    [SerializeField] Button sceneSwitchButton;
    [SerializeField] TextMeshProUGUI interactedCount;

    Camera cam;

    Coroutine popupFadeRoutine;
    Coroutine introFadeRoutine;

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

        if (interactedCount != null)
            interactedCount.gameObject.SetActive(false);

        cam = Camera.main;

        if (popupCanvasGroup != null)
            popupCanvasGroup.alpha = 0f;

        if (introCanvasGroup != null)
            introCanvasGroup.alpha = 0f;
    }

    void OnEnable()
    {
        if (cam == null) cam = Camera.main;
    }

    public bool IsPopupVisible => popupImage != null && popupImage.gameObject.activeSelf;

    void Start()
    {
        // Fade intro in at game start
        if (introImage != null && introCanvasGroup != null)
        {
            introImage.gameObject.SetActive(true);
            StartIntroFade(1f);
        }
    }
    public void UpdateClues(int found, int total)
    {
        if (interactedCount == null)
            return;

        interactedCount.gameObject.SetActive(true);
        interactedCount.text = $"Clues Found: {found}/{total}";
    }
    public void ResetClues()
    {
        if (interactedCount != null)
        {
            interactedCount.gameObject.SetActive(false);
            interactedCount.text = string.Empty;
        }
    }

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

        if (introCanvasGroup != null)
        {
            StartIntroFade(0f);
        }
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
        if (popupCanvasGroup == null)
            return;

        if (popupFadeRoutine != null)
            StopCoroutine(popupFadeRoutine);

        popupFadeRoutine = StartCoroutine(
            FadeCanvasGroup(popupCanvasGroup, targetAlpha, popupFadeDuration,
                onComplete: () =>
                {
                    if (Mathf.Approximately(targetAlpha, 0f))
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
                })
        );
    }

    void StartIntroFade(float targetAlpha)
    {
        if (introCanvasGroup == null)
            return;

        if (introFadeRoutine != null)
            StopCoroutine(introFadeRoutine);

        introFadeRoutine = StartCoroutine(
            FadeCanvasGroup(introCanvasGroup, targetAlpha, introFadeDuration,
                onComplete: () =>
                {
                    if (Mathf.Approximately(targetAlpha, 0f))
                    {
                        if (introImage != null)
                            introImage.gameObject.SetActive(false);
                    }
                })
        );
    }
    IEnumerator FadeCanvasGroup(CanvasGroup cg, float target, float duration, System.Action onComplete = null)
    {
        float start = cg.alpha;
        float time = 0f;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(time / duration);
            cg.alpha = Mathf.Lerp(start, target, t);
            yield return null;
        }

        cg.alpha = target;
        onComplete?.Invoke();
    }
}