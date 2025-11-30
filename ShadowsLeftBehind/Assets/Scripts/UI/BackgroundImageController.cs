using UnityEngine;
using UnityEngine.UI;
public class BackgroundImageController : MonoBehaviour
{
    [SerializeField] private RawImage backgroundImage;
    [SerializeField] private Texture lightTexture;
    [SerializeField] private Texture darkTexture;
    private void Awake()
    {
        if (backgroundImage == null)
            backgroundImage = GetComponent<RawImage>();
    }

    private void Start()
    {
        if (!lightTexture || !darkTexture)
            return;

        bool isDark = GameManager.instance != null && GameManager.instance.isNextRoomDark;

        backgroundImage.texture = isDark ? darkTexture : lightTexture;
    }
}
