using System.Collections.Generic;
using UnityEngine;

public class InteractablesManager : MonoBehaviour
{
    public static InteractablesManager Instance { get; private set; }

    InteractableEntity[] allInteractables; //we fill this from script
    HashSet<InteractableEntity> discovered = new HashSet<InteractableEntity>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this; 
        //possibly have this persist between scenes?
    }

    void Start()
    {
        //fill the array
        this.allInteractables = FindObjectsByType<InteractableEntity>(FindObjectsSortMode.None);
    }

    public void OnInteractableDiscovered(InteractableEntity entity)
    {
        if (entity == null)
            return;

        if (!discovered.Contains(entity))
        {
            discovered.Add(entity);

            if (discovered.Count == allInteractables.Length)
            {
                Debug.Log("All clues discovered!");
                Overlay.Instance.ShowButton();
                //move scene button here from game manager ui button script
            }
        }
    }
}
