// ShopUIController.cs
// Example UI controller for switching between 3 shop types

using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Example UI controller showing how to switch between the 3 shop types.
/// Attach this to your Shop UI canvas.
/// </summary>
public class ShopUIController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ShopManager shopManager;

    [Header("Shop Type Buttons")]
    [SerializeField] private Button skillShopButton;
    [SerializeField] private Button itemShopButton;
    [SerializeField] private Button arcaneShopButton;

    [Header("Shop Display")]
    [SerializeField] private GameObject shopPanel;

    void Start()
    {
        // Wire up shop type buttons
        if (skillShopButton != null)
        {
            skillShopButton.onClick.AddListener(() => OpenShop(ShopType.Skills));
        }

        if (itemShopButton != null)
        {
            itemShopButton.onClick.AddListener(() => OpenShop(ShopType.Items));
        }

        if (arcaneShopButton != null)
        {
            arcaneShopButton.onClick.AddListener(() => OpenShop(ShopType.Arcanes));
        }
    }

    /// <summary>
    /// Open a specific shop type.
    /// </summary>
    public void OpenShop(ShopType shopType)
    {
        if (shopManager != null)
        {
            shopManager.RefreshShop(shopType);
        }

        // Show shop panel
        if (shopPanel != null)
        {
            shopPanel.SetActive(true);
        }

        Debug.Log($"Opened {shopType} Shop");
    }

    /// <summary>
    /// Switch to a different shop type while shop is open.
    /// </summary>
    public void SwitchShopType(ShopType shopType)
    {
        // If in OpenShopState, use the state's method
        var state = GameStateManager.Instance.CurrentState as OpenShopState;
        if (state != null)
        {
            state.SwitchShopType(shopType);
        }
        else
        {
            // Otherwise just refresh the shop manager
            OpenShop(shopType);
        }
    }

    /// <summary>
    /// Close shop and continue.
    /// </summary>
    public void CloseShop()
    {
        // Hide shop panel
        if (shopPanel != null)
        {
            shopPanel.SetActive(false);
        }

        // If in OpenShopState, transition back to RollDice
        var state = GameStateManager.Instance.CurrentState as OpenShopState;
        state?.CloseShop();
    }

    // ====== BUTTON HANDLERS (can be called from Unity UI) ======

    public void OnSkillShopButtonClicked()
    {
        SwitchShopType(ShopType.Skills);
    }

    public void OnItemShopButtonClicked()
    {
        SwitchShopType(ShopType.Items);
    }

    public void OnArcaneShopButtonClicked()
    {
        SwitchShopType(ShopType.Arcanes);
    }

    public void OnContinueButtonClicked()
    {
        CloseShop();
    }
}
