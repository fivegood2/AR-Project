using UnityEngine;


public class FilterManager : MonoBehaviour
{
    public OVRPassthroughLayer passthroughLayer;
    public Color redColor = new Color(1f, 0.2f, 0.2f, 1f);
    public Color blueColor = new Color(0.2f, 0.2f, 1f, 1f);

    //don't change these in inspector unless you need to test them (preventing clutter)
    [HideInInspector] public bool filterOn;
    [HideInInspector] public bool isRed = true;

    //https://developer.oculus.com/documentation/unity/unity-passthrough-color-mapping
    //https://developer.oculus.com/reference/unity/latest/class_o_v_r_passthrough_layer
    
    
    public Renderer overlayRenderer;
    private Material overlayMat;

    protected virtual void Start()
    {
        overlayMat = overlayRenderer.material;
        overlayRenderer.enabled = false;
        Color[] neutral = new Color[256];
        for (int i = 0; i < 256; i++)
        {
            float t = i / 255f;
            neutral[i] = new Color(t, t, t, 1f);
        }
        passthroughLayer.SetColorMap(neutral);
        overlayRenderer.enabled = false;
    }

    protected virtual void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch))
        {
            filterOn = !filterOn;

            if (filterOn)
            {
                isRed = true;
                ApplyTint();
            }
            else
            {
                Color[] neutral = new Color[256];
                for (int i = 0; i < 256; i++)
                {
                    float t = i / 255f;
                    neutral[i] = new Color(t, t, t, 1f);
                }
                passthroughLayer.SetColorMap(neutral);
                overlayRenderer.enabled = false;
            }
        }

        if (filterOn && OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.RTouch))
        {
            isRed = !isRed;
            ApplyTint();
        }

        //print(isRed);
    }

    protected virtual void ApplyTint()
    {
        Color tint = isRed ? redColor : blueColor;
        Color[] colorMap = new Color[256];

        for (int i = 0; i < 256; i++)
        {
            float t = i / 255f;
            colorMap[i] = new Color(tint.r * t, tint.g * t, tint.b * t, 1f);
        }

        passthroughLayer.SetColorMap(colorMap);
        overlayRenderer.enabled = true;
        overlayMat.color = new Color(tint.r, tint.g, tint.b, 0.7f);
    }
}
