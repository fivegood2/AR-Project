//remember: don't worry about base script

using UnityEngine;
using Oculus.Interaction;

public class hw2Skeleton : hw2Base
{
    public bool RightCubeSelected;

    [SerializeField] private FilterManager filterManager;

    protected override void OnRayStateChanged(InteractableStateChangeArgs args)
    {
        if (args.NewState == InteractableState.Select && filterManager.HasTriedBothFilters)
        {
            RightCubeSelected = true;
            print("Right Cube Selected");
        }
    }

    protected override void ApplyTwoHandScale(float currentDist)
    {
        // Do nothing so this cube cannot be scaled.
    }
}
