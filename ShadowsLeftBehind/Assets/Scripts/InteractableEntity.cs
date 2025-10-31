using UnityEngine;
using UnityEngine.InputSystem;
public class InteractableEntity : MonoBehaviour
{
    //per prefab
    public Sprite popupSprite;
    public Vector3 popupWorldOffset = new Vector3(0f, 1.6f, 0f);

    [TextArea] public string promptText = "Press E to Interact";
    public Vector3 promptWorldOffset = new Vector3(0f, 1.2f, 0f);

    bool playerInside;
    Transform player;

    void Reset()
    {
        var collider = GetComponent<BoxCollider2D>();
        collider.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("player")) 
            return;

        playerInside = true;
        player = other.transform;
        Overlay.Instance.ShowPrompt(promptText, transform.position + promptWorldOffset);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("player"))
            return;

        playerInside = false;
        player = null;
        Overlay.Instance.HidePrompt();
    }

    void Update()
    {
        if (playerInside)
        {
            Overlay.Instance.UpdatePromptPosition(transform.position + promptWorldOffset);
            Overlay.Instance.UpdatePopupPosition(transform.position + popupWorldOffset);
        }

        if (playerInside && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            Overlay.Instance.HidePrompt();
            Overlay.Instance.ShowPopup(popupSprite, transform.position, popupWorldOffset);
        }
    }
}

