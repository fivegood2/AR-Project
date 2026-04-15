using UnityEngine;using UnityEngine.Rendering.Universal;


public class FilterWithDecal : FilterManager
{
    [Header("Decal Projector (child of object to filter)")]
    public DecalProjector decalProjector;

    [Header("Reveal color ('blue' or 'red')")]
    public string revealColor; 
    
    
    protected override void Start()
    {
        base.Start();
        decalProjector.gameObject.SetActive(false);
    }

    protected override void ApplyTint()
    {
        base.ApplyTint();
    }

    protected override void Update()
    {
        base.Update();

        if (revealColor == "blue") { decalProjector.gameObject.SetActive(!isRed); }
        else { decalProjector.gameObject.SetActive(isRed); }
        
        if(!filterOn) { decalProjector.gameObject.SetActive(false); }
    }
}
