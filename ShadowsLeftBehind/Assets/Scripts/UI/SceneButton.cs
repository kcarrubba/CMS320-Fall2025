using UnityEngine;
public class SceneButton : MonoBehaviour
{
    [SerializeField] string targetScene;

    public void LoadScene()
    {
        GameManager.instance.SwitchTo(targetScene);
        //print("pressed scene!");
    }
}