using Oculus.Interaction;
using UnityEngine;

public class WrongCubeSelection : hw2Base
{
    public bool RightCubeSelected;

    [SerializeField] private FilterManager filterManager;

    protected override void OnRayStateChanged(InteractableStateChangeArgs args)
    {
        if (args.NewState == InteractableState.Select && filterManager.HasTriedBothFilters)
        {
            print("Wrong Cube Selected");
        }
    }
    

    protected override void ApplyTwoHandScale(float currentDist)
    {
        // Do nothing so this cube cannot be scaled.
    }
}
