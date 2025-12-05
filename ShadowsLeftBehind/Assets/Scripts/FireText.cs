using UnityEngine;
using TMPro;

public class FireText : MonoBehaviour
{
    [SerializeField] private float displayDuration = 10f;

    private void OnEnable()
    {
        // Start the countdown when this object becomes active
        Invoke(nameof(HideText), displayDuration);
    }

    private void HideText()
    {
        gameObject.SetActive(false);
    }
}
