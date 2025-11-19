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

    [Header("Glow Settings")]
    [SerializeField] bool useGlow = true;
    [Range(0f, 1f)] [SerializeField] float minAlpha = 0.3f;
    [Range(0f, 1f)] [SerializeField] float maxAlpha = 1f;
    [SerializeField] float glowSpeed = 2f;

    bool playerInside;
    bool isUnlocked = false;
    float glowT;

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
        // Unlock logic
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

        // Glow effect once the door is unlocked
        if (useGlow && doorSprite != null && isUnlocked)
        {
            glowT += Time.deltaTime * glowSpeed;
            float pingPong = (Mathf.Sin(glowT) + 1f) * 0.5f;   // 0 to 1
            float alpha = Mathf.Lerp(minAlpha, maxAlpha, pingPong);

            Color c = doorSprite.color;
            c.a = alpha;
            doorSprite.color = c;
        }

        if (!playerInside)
            return;

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
