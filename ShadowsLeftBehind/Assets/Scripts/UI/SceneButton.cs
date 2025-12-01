using UnityEngine;
public class SceneButton : MonoBehaviour
{
    [SerializeField] string targetScene;
    [SerializeField] bool movePlayer = true;

    public void LoadScene()
    {
        Debug.Log("moving to scene");
        GameManager.instance.SwitchTo(targetScene, movePlayer);
    }
}