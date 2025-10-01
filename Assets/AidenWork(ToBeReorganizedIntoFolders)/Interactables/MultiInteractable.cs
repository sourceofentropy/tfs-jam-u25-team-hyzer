using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class MultiInteractable : MonoBehaviour, IInteractable
{
    private IInteractable[] interactables;

    void Awake()
    {
        // Get all IInteractable components EXCEPT this MultiInteractable itself
        interactables = GetComponents<IInteractable>()
            .Where(i => i != this as IInteractable)
            .ToArray();
    }

    public List<InteractionOption> GetOptions()
    {
        List<InteractionOption> allOptions = new List<InteractionOption>();
        foreach (var i in interactables)
        {
            if (i != null)
            {
                allOptions.AddRange(i.GetOptions());
            }
        }
        return allOptions;
    }

    public void Interact(KeyCode key, GameObject player)
    {
        foreach (var i in interactables)
        {
            if (i == null) continue;

            var options = i.GetOptions();
            foreach (var opt in options)
            {
                if (opt.key == key)
                {
                    i.Interact(key, player);
                    return;
                }
            }
        }
    }
}