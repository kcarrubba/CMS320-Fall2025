using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float moveSpeed = 5f;

    [Header("Animator")]
    public Animator animator;                 // Controller should have a Blend Tree state named in walkStateName
    [SerializeField] string walkStateName = "Walk"; // <-- exact name of your Walk Blend Tree state

    Rigidbody2D rb;
    Camera cam;
    Vector2 halfSize;

    // Input / facing
    Vector2 inputDir;
    Vector2 lastMoveDir = Vector2.down;       // default facing
    bool isMoving;

    // Animator parameter hashes (optional but safer)
    static readonly int HashMoveX = Animator.StringToHash("MoveX");
    static readonly int HashMoveY = Animator.StringToHash("MoveY");
    bool hasMoveX, hasMoveY;

    void Awake()
    {
        if (!animator) animator = GetComponent<Animator>();

        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        cam = Camera.main;

        var col = GetComponent<Collider2D>();
        if (col) halfSize = col.bounds.extents;

        // Cache which params exist to avoid warnings if names differ
        if (animator)
        {
            foreach (var p in animator.parameters)
            {
                if (p.nameHash == HashMoveX) hasMoveX = true;
                else if (p.nameHash == HashMoveY) hasMoveY = true;
            }
        }
    }

    void Update()
    {
        // --- Read WASD (new Input System) ---
        inputDir = Vector2.zero;
        var kb = Keyboard.current;
        if (kb != null)
        {
            inputDir.x = (kb.dKey.isPressed ? 1 : 0) - (kb.aKey.isPressed ? 1 : 0);
            inputDir.y = (kb.wKey.isPressed ? 1 : 0) - (kb.sKey.isPressed ? 1 : 0);
        }
        inputDir = inputDir.normalized;
        isMoving = inputDir.sqrMagnitude > 0.0001f;

        // Snap to a single cardinal so the 4-way tree doesn't flicker on diagonals
        Vector2 snapped = isMoving ? SnapToCardinal(inputDir) : lastMoveDir;
        if (isMoving) lastMoveDir = snapped;

        // Animator driving (single Walk tree + freeze on first frame when idle)
        if (animator)
        {
            if (hasMoveX) animator.SetFloat(HashMoveX, snapped.x);
            if (hasMoveY) animator.SetFloat(HashMoveY, snapped.y);

            if (isMoving)
            {
                if (animator.speed == 0f) animator.speed = 1f; // resume animation
            }
            else
            {
                // Freeze on first frame of the direction we’re facing
                animator.Play(walkStateName, 0, 0f);
                animator.speed = 0f;
            }
        }
    }

    void FixedUpdate()
    {
        // Physics movement
        Vector2 target = rb.position + inputDir * moveSpeed * Time.fixedDeltaTime;

        if (cam)
        {
            Vector3 min = cam.ViewportToWorldPoint(new Vector3(0f, 0f, cam.nearClipPlane));
            Vector3 max = cam.ViewportToWorldPoint(new Vector3(1f, 1f, cam.nearClipPlane));

            target.x = Mathf.Clamp(target.x, min.x + halfSize.x, max.x - halfSize.x);
            target.y = Mathf.Clamp(target.y, min.y + halfSize.y, max.y - halfSize.y);
        }

        rb.MovePosition(target);
    }

    // Public so GameManager (or others) can set facing explicitly
    public void Face(Vector2 dir)
    {
        if (dir == Vector2.zero) return;

        Vector2 snapped = SnapToCardinal(dir);
        lastMoveDir = snapped;

        if (animator)
        {
            if (hasMoveX) animator.SetFloat(HashMoveX, snapped.x);
            if (hasMoveY) animator.SetFloat(HashMoveY, snapped.y);
        }

        var sr = GetComponentInChildren<SpriteRenderer>();
        if (sr && Mathf.Abs(lastMoveDir.x) > 0.5f)
            sr.flipX = lastMoveDir.x < 0f;
    }

    // Helpers
    static Vector2 SnapToCardinal(Vector2 v)
    {
        if (v == Vector2.zero) return Vector2.zero;
        return Mathf.Abs(v.x) >= Mathf.Abs(v.y)
            ? new Vector2(Mathf.Sign(v.x), 0f)
            : new Vector2(0f, Mathf.Sign(v.y));
    }
}
