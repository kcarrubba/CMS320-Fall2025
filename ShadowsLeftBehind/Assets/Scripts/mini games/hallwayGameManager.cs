using UnityEngine;
using UnityEngine.SceneManagement;

public class HallwayGameManager : MonoBehaviour {
    public static HallwayGameManager I;
    public bool alive = true;
    void Awake(){ I = this; Time.timeScale = 1f; }
    public void GameOver(){
        if (!alive) return;
        alive = false;
        Time.timeScale = 0f;
        Debug.Log("Game Over - Press R to restart");
    }
    void Update(){
        if (!alive && (Input.GetKeyDown(KeyCode.R))) {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
