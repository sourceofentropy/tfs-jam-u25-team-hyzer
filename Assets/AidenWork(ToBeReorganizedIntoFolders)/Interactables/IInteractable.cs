using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    List<InteractionOption> GetOptions();
    void Interact(KeyCode key, GameObject player);
}