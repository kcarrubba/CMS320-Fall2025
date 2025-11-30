using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Flame : MonoBehaviour
{
    private FlameManager flameManager;
    private bool extinguished;
    private bool playerInRange;

    [SerializeField] private string playerTag = "player";
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    public void Init(FlameManager manager)
    {
        flameManager = manager;
    }

    private void Awake()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void Update()
    {
        if (extinguished)
            return;

        if (playerInRange && Input.GetKeyDown(interactKey))
            Extinguish();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInRange = false;
        }
    }
    private void Extinguish()
    {
        if (extinguished) 
            return;

        extinguished = true;

        if (flameManager != null)
            flameManager.RegisterFlameExtinguished();

        Destroy(gameObject);
    }
}
