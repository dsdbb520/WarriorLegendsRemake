using UnityEngine;

[RequireComponent(typeof(InteractionIndicator))]
public abstract class BaseNPC : MonoBehaviour, IInteractable
{
    protected InteractionIndicator indicator;
    protected bool hasInteracted;

    protected virtual void Awake()
    {
        indicator = GetComponent<InteractionIndicator>();
    }

    public virtual void Interact()
    {
        if (indicator != null)
            indicator.OnInteracted();
        hasInteracted = true;
    }
}
