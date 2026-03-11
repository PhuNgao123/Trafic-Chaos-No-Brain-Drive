using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Button to reset all player data (coins, high score, unlocked vehicles)
/// Attach to a UI Button and assign the button reference
/// </summary>
public class DataResetButton : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Button that will trigger the reset")]
    public Button resetButton;
    
    [Header("Confirmation")]
    [Tooltip("Require confirmation before reset")]
    public bool requireConfirmation = true;
    
    [Tooltip("Show confirmation dialog (if available)")]
    public GameObject confirmationDialog;

    void Start()
    {
        // Auto-find button if not assigned
        if (resetButton == null)
            resetButton = GetComponent<Button>();
        
        // Setup button click event
        if (resetButton != null)
            resetButton.onClick.AddListener(OnResetButtonClicked);
    }

    void OnResetButtonClicked()
    {
        if (requireConfirmation)
        {
            // Show confirmation dialog if available
            if (confirmationDialog != null)
            {
                confirmationDialog.SetActive(true);
                return;
            }
            
            // Simple confirmation without dialog
            if (!ShouldConfirmReset())
                return;
        }
        
        ResetAllData();
    }
    
    bool ShouldConfirmReset()
    {
        // This would normally show a proper dialog
        // For now, just return true (you can implement proper dialog later)
        return true;
    }
    
    public void ResetAllData()
    {
        // Reset CurrencyManager data
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.ResetData();
        }
        
        // Reset GarageManager data
        if (GarageManager.Instance != null)
        {
            GarageManager.Instance.ResetData();
        }
        
        // Clear all PlayerPrefs to ensure everything is reset
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        
        // Reload data in managers
        ReloadManagerData();
    }
    
    void ResetGarageData()
    {
        // This method is no longer needed as GarageManager has its own ResetData
    }
    
    void ReloadManagerData()
    {
        // Force reload data in managers
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.ReloadData();
        }
        
        if (GarageManager.Instance != null)
        {
            GarageManager.Instance.ReloadData();
        }
    }
    
    // Public method that can be called from confirmation dialog
    public void ConfirmReset()
    {
        ResetAllData();
        
        // Hide confirmation dialog if it exists
        if (confirmationDialog != null)
            confirmationDialog.SetActive(false);
    }
    
    // Public method to cancel reset
    public void CancelReset()
    {
        // Hide confirmation dialog if it exists
        if (confirmationDialog != null)
            confirmationDialog.SetActive(false);
    }
}