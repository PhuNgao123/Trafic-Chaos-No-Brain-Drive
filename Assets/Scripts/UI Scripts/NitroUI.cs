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

    void Start()
    {
        SetCornerGlows(false);
        
        // Force initial update
        if (nitroFillImage != null)
        {
            nitroFillImage.fillAmount = 0f;
            nitroFillImage.color = fillNormalColor;
        }
    }
    
    public void RefreshPlayerReference()
    {
        // Find object with tag "Player" and get NitroController from it
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            nitroController = player.GetComponent<NitroController>();
        }
    }

    void Update()
    {
        // Try to find NitroController if not found yet
        if (nitroController == null)
        {
            // Find object with tag "Player" and get NitroController from it
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                nitroController = player.GetComponent<NitroController>();
            }
            
            if (nitroController == null)
            {
                // Still not found, skip this frame
                return;
            }
        }

        if (nitroFillImage == null)
        {
            return;
        }

        float percent = nitroController.NitroPercent;

        // Update bar fill and color
        nitroFillImage.fillAmount = percent;
        
        // Color logic: gradual color change based on percentage
        if (nitroController.IsNitroActive)
        {
            // Active: use frame active color (yellow)
            nitroFillImage.color = frameActiveColor;
        }
        else if (nitroController.IsNitroReady)
        {
            // Ready (100%): use ready color (cyan)
            nitroFillImage.color = fillReadyColor;
        }
        else if (percent > 0.01f)
        {
            // Filling (1-99%): lerp between normal and ready color
            nitroFillImage.color = Color.Lerp(fillNormalColor, fillReadyColor, percent);
        }
        else
        {
            // Empty (0%): use normal color (gray)
            nitroFillImage.color = fillNormalColor;
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

