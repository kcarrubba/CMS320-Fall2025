using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(BoxCollider2D))]
public class Door : MonoBehaviour
{
    [Header("Destination")]
    [SerializeField] string targetScene;
    [SerializeField] string spawnId = "default";
    [SerializeField] bool movePlayer = true;

    bool playerInside;

    void Reset()
    {
        var col = GetComponent<BoxCollider2D>();

        if (col == null)
            col = gameObject.AddComponent<BoxCollider2D>();

        col.isTrigger = true;
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

    void Update()
    {
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
}