using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public interface IInteractable
{

    public UnityAction<IInteractable> OnInteract { get; set; }
    public void Interact(Interactor interactor, out bool success);
    public void StopInteract(); 


}
