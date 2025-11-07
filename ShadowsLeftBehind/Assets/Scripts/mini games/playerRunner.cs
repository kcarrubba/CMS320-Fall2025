using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerRunner : MonoBehaviour {
    public float jumpForce = 14f;
    public LayerMask groundMask;
    Rigidbody2D rb;
    bool grounded;

    void Awake(){ rb = GetComponent<Rigidbody2D>(); }

    void Update(){
        if (!HallwayGameManager.I.alive) return;

        // Ground check (circle under feet)
        grounded = Physics2D.OverlapCircle((Vector2)transform.position + Vector2.down * 0.6f, 0.15f, groundMask);

        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) && grounded){
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    void OnCollisionEnter2D(Collision2D c){
        if (c.collider.CompareTag("Obstacle")){
            HallwayGameManager.I.GameOver();
        }
    }
}
