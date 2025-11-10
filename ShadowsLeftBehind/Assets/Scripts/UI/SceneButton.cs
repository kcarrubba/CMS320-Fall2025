using UnityEngine;
public class SceneButton : MonoBehaviour
{
    [SerializeField] string targetScene;
    [SerializeField] bool movePlayer = true;

    public void LoadScene()
    {
        GameManager.instance.SwitchTo(targetScene, movePlayer);
    }
}