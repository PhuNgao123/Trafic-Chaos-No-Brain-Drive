using UnityEngine;
using UnityEngine.UI;

// Shows nitro bar and optional corner glow FX.
// Attach this to a Canvas object and wire the images in the inspector.
public class NitroUI : MonoBehaviour
{
    [Header("References")]
    public NitroController nitroController;

    [Header("Bar")]
    public Image nitroFillImage;      // Fill image (type = Filled)
    public Image nitroFrameImage;     // Optional frame image

    [Header("Corner Glow (optional)")]
    public Image[] cornerGlowImages;  // 4 images in screen corners

    [Header("Colors")]
    public Color fillNormalColor = Color.gray;
    public Color fillReadyColor = Color.cyan;
    public Color frameNormalColor = Color.white;
    public Color frameActiveColor = Color.yellow;
    
    [Header("Debug")]
    public bool enableDebugLogs = false; // Enable to see debug messages

    void Start()
    {
        RefreshPlayerReference();
        SetCornerGlows(false);
        
        if (enableDebugLogs)
        {
            Debug.Log($"NitroUI: Initialized. Fill Image: {(nitroFillImage != null ? "OK" : "NULL")}");
        }
    }
    
    public void RefreshPlayerReference()
    {
        if (nitroController == null)
        {
            nitroController = FindFirstObjectByType<NitroController>();
            if (enableDebugLogs)
            {
                if (nitroController != null)
                    Debug.Log("NitroUI: Found NitroController automatically");
                else
                    Debug.LogError("NitroUI: Could not find NitroController!");
            }
        }
        
        Debug.Log("NitroUI: Refreshed player references");
    }

    void Update()
    {
        if (nitroController == null)
        {
            if (enableDebugLogs)
                Debug.LogWarning("NitroUI: nitroController is null in Update!");
            return;
        }

        float percent = nitroController.NitroPercent;

        // Update bar fill and color
        if (nitroFillImage != null)
        {
            nitroFillImage.fillAmount = percent;
            nitroFillImage.color = nitroController.IsNitroReady ? fillReadyColor : fillNormalColor;
            
            if (enableDebugLogs && percent > 0)
                Debug.Log($"NitroUI: Updating fill to {percent * 100f}%");
        }
        else if (enableDebugLogs)
        {
            Debug.LogError("NitroUI: nitroFillImage is null!");
        }

        // Frame color: glow when nitro is active
        if (nitroFrameImage != null)
        {
            nitroFrameImage.color = nitroController.IsNitroActive ? frameActiveColor : frameNormalColor;
        }

        // Corner glow on when nitro active
        SetCornerGlows(nitroController.IsNitroActive);
    }

    void SetCornerGlows(bool enabled)
    {
        if (cornerGlowImages == null)
            return;

        foreach (var img in cornerGlowImages)
        {
            if (img == null) continue;
            img.enabled = enabled;
        }
    }
}

