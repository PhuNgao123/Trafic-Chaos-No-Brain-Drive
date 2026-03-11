using UnityEngine;

/// <summary>
/// Fixes unwanted rotation on UI elements
/// Attach to any UI GameObject that should never rotate
/// </summary>
public class UIRotationFixer : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Force rotation to identity every frame")]
    public bool forceIdentityRotation = true;
    
    [Tooltip("Also fix local rotation")]
    public bool fixLocalRotation = true;
    
    [Tooltip("Disable all Animator components on this object and children")]
    public bool disableAnimators = true;

    void Start()
    {
        // Fix rotation on start
        FixRotation();
        
        // Disable animators if requested
        if (disableAnimators)
        {
            DisableAllAnimators();
        }
    }

    void Update()
    {
        if (forceIdentityRotation)
        {
            FixRotation();
        }
    }

    void FixRotation()
    {
        // Fix world rotation
        if (transform.rotation != Quaternion.identity)
        {
            transform.rotation = Quaternion.identity;
        }
        
        // Fix local rotation
        if (fixLocalRotation && transform.localRotation != Quaternion.identity)
        {
            transform.localRotation = Quaternion.identity;
        }
    }

    void DisableAllAnimators()
    {
        // Disable animator on this object
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.enabled = false;
        }

        // Disable animators on all children
        Animator[] childAnimators = GetComponentsInChildren<Animator>(true);
        foreach (Animator childAnimator in childAnimators)
        {
            if (childAnimator != null)
            {
                childAnimator.enabled = false;
            }
        }
    }
}