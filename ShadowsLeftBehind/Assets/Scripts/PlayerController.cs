using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    Rigidbody2D rb;
    Camera cam;
    Vector2 halfSize;
    void Awake()
    {
        this.rb = GetComponent<Rigidbody2D>();
        this.rb.gravityScale = 0f;
        this.rb.freezeRotation = true;
        this.cam = Camera.main;

        var collider = GetComponent<Collider2D>();

        if (collider)
            this.halfSize = collider.bounds.extents;
    }

    void FixedUpdate()
    {
        Vector2 dir = Vector2.zero;
        var keyboard = Keyboard.current;

        if (keyboard != null)
        {
            dir.x = (keyboard.dKey.isPressed ? 1 : 0) - (keyboard.aKey.isPressed ? 1 : 0);
            dir.y = (keyboard.wKey.isPressed ? 1 : 0) - (keyboard.sKey.isPressed ? 1 : 0);
        }

        dir = dir.normalized;
        Vector3 pos = rb.position + dir * moveSpeed * Time.fixedDeltaTime;

        Vector3 min = cam.ViewportToWorldPoint(new Vector3(0, 0, cam.nearClipPlane));
        Vector3 max = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.nearClipPlane));

        pos.x = Mathf.Clamp(pos.x, min.x + this.halfSize.x, max.x - this.halfSize.x);
        pos.y = Mathf.Clamp(pos.y, min.y + this.halfSize.y, max.y - this.halfSize.y);

        rb.MovePosition(pos);
    }
}
