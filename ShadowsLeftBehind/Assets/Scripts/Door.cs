using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(BoxCollider2D))]
public class Door : MonoBehaviour
{
    [Header("Destination")]
    [SerializeField] string targetScene;
    [SerializeField] string spawnId = "default";
    [SerializeField] bool movePlayer = true;

    [Header("Visuals")]
    [SerializeField] SpriteRenderer doorSprite;
    [SerializeField] Collider2D triggerCollider;

    bool playerInside;
    bool isUnlocked = false;

    private void Awake()
    {
        if (triggerCollider == null)
            triggerCollider = GetComponent<Collider2D>();
        if (doorSprite == null)
            doorSprite = GetComponent<SpriteRenderer>();

        SetUnlocked(false);
    }

    void Reset()
    {
        var col = GetComponent<BoxCollider2D>();

        if (col == null)
            col = gameObject.AddComponent<BoxCollider2D>();

        col.isTrigger = true;
    }

    void SetUnlocked(bool unlocked)
    {
        isUnlocked = unlocked;

        if (doorSprite != null)
            doorSprite.enabled = unlocked;

        if (triggerCollider != null)
            triggerCollider.enabled = unlocked;
    }

    void Update()
    {
        if (!isUnlocked)
        {
            if (InteractablesManager.Instance != null &&
                InteractablesManager.Instance.AllCluesFoundInScene)
            {
                SetUnlocked(true);
            }
            else
            {
                return;
            }
        }

        if (!playerInside)
            return;

        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (string.IsNullOrEmpty(targetScene))
            {
                Debug.LogWarning($"Door on '{name}' has no targetScene set.");
                return;
            }

            if (GameManager.instance == null)
            {
                Debug.LogError("No GameManager.instance found for Door.");
                return;
            }

            GameManager.instance.SwitchTo(targetScene, movePlayer, spawnId);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("player"))
            return;

        playerInside = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("player"))
            return;

        playerInside = false;
    }
}