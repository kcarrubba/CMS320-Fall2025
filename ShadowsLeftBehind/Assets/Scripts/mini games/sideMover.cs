using UnityEngine;

public class SideMover : MonoBehaviour {
    public float speed = 8f;
    void Update(){
        if (!HallwayGameManager.I.alive) return;
        transform.Translate(Vector3.left * speed * Time.deltaTime);
        if (transform.position.x < -20f) gameObject.SetActive(false);
    }
}