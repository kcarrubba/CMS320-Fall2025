using UnityEngine;
using TMPro;
public class FlameTimerUI : MonoBehaviour
{
    [SerializeField] private FlameManager flameManager;
    [SerializeField] private TextMeshProUGUI timerText;

    private void Awake()
    {
        if (flameManager == null)
            flameManager = FindFirstObjectByType<FlameManager>();
    }

    private void Update()
    {
        if (flameManager == null || timerText == null)
            return;

        float time = flameManager.GetTimeRemaining();

        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);

        timerText.text = $"{minutes:00}:{seconds:00}";
    }
}
