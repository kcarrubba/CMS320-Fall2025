using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractablesManager : MonoBehaviour
{
    public static InteractablesManager Instance { get; private set; }

    InteractableEntity[] allInteractables; //we fill this from script
    HashSet<InteractableEntity> discovered = new HashSet<InteractableEntity>();
    int TotalClues => allInteractables != null ? allInteractables.Length : 0;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        RefreshInteractablesForScene(SceneManager.GetActiveScene());
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RefreshInteractablesForScene(scene);
    }

    void RefreshInteractablesForScene(Scene scene)
    {
        discovered.Clear();

        var all = Object.FindObjectsByType<InteractableEntity>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        allInteractables = all
            .Where(i => i.gameObject.scene == scene)
            .ToArray();

        Debug.Log($"[InteractablesManager] Scene '{scene.name}' has {allInteractables.Length} interactables.");

        if (Overlay.Instance != null)
        {
            if (TotalClues > 0)
                Overlay.Instance.UpdateClues(0, TotalClues);
            else
                Overlay.Instance.ResetClues();

            Overlay.Instance.HideButton();
        }
    }

    void UpdateUI()
    {
        if (Overlay.Instance != null)
        {
            Overlay.Instance.UpdateClues(discovered.Count, TotalClues);
        }
    }
    public void OnInteractableDiscovered(InteractableEntity entity)
    {
        if (entity == null)
            return;

        if (!discovered.Contains(entity))
        {
            discovered.Add(entity);

            this.UpdateUI();

            if (discovered.Count == TotalClues && TotalClues > 0)
            {
                Debug.Log("All clues discovered!");
                Overlay.Instance.ShowButton();
                //move scene button here from game manager ui button script
            }
        }
    }
}
