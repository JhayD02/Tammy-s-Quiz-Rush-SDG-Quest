using UnityEngine;
using UnityEngine.UI;

public class HolographicUI : MonoBehaviour
{
    public Image[] layers; // Assign duplicates in Inspector
    public float flickerSpeed = 3f;
    public float alphaRange = 0.2f;
    public float scaleRange = 0.05f;

    private Vector3 baseScale;

    void Start()
    {
        baseScale = transform.localScale;
    }

    void Update()
    {
        float flicker = Mathf.Sin(Time.time * flickerSpeed);

        // Scale pulse
        transform.localScale = baseScale * (1f + flicker * scaleRange);

        // Alpha flicker per layer
        foreach (Image img in layers)
        {
            if (img != null)
            {
                Color c = img.color;
                c.a = 0.5f + flicker * alphaRange;
                img.color = c;
            }
        }
    }
}
