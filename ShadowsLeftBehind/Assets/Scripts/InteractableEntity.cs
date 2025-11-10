using UnityEngine;
using UnityEngine.InputSystem;

public class InteractableEntity : MonoBehaviour
{
    public Sprite popupSprite;

    [TextArea] public string promptText = "Press E to Interact";
    public Vector3 promptWorldOffset = new Vector3(0f, 1.2f, 0f);

    bool playerInside;
    Transform player;

    void Reset()
    {
        var collider = GetComponent<BoxCollider2D>();

        if (collider != null)
            collider.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("player"))
            return;

        playerInside = true;
        player = other.transform;

        //if (Overlay.Instance != null)
        //    Overlay.Instance.ShowPrompt(promptText, transform.position + promptWorldOffset);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("player"))
            return;

        playerInside = false;
        player = null;

        if (Overlay.Instance != null)
        {
            //Overlay.Instance.HidePrompt();
            Overlay.Instance.HidePopup();
        }
    }

    void Update()
    {
        if (!playerInside || Overlay.Instance == null)
            return;

        //Overlay.Instance.UpdatePromptPosition(transform.position + promptWorldOffset);

        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (Overlay.Instance.IsPopupVisible)
            {
                Overlay.Instance.HidePopup();
                //Overlay.Instance.ShowPrompt(promptText, transform.position + promptWorldOffset);
            }
            else
            {
                //Overlay.Instance.HidePrompt();
                Overlay.Instance.ShowPopup(popupSprite);
            }
        }
    }
}
