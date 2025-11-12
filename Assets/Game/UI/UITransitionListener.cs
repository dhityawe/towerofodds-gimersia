using UnityEngine;

/// <summary>
/// Listens to shop visibility events and triggers panel transitions.
/// Bridges ShopManager events with PanelTransitionHandle.
/// </summary>
[RequireComponent(typeof(PanelTransitionHandle))]
public class UITransitionListener : MonoBehaviour
{
    [Header("Shop Manager Reference")]
    [SerializeField] private ShopManager shopManager;

    private PanelTransitionHandle panelTransition;

    void Awake()
    {
        panelTransition = GetComponent<PanelTransitionHandle>();

        // Auto-find ShopManager if not assigned
        if (shopManager == null)
        {
            shopManager = FindFirstObjectByType<ShopManager>();
        }
    }

    void OnEnable()
    {
        // Subscribe to shop events
        if (shopManager != null)
        {
            shopManager.OnShowShop += ShowTransition;
            shopManager.OnHideShop += HideTransition;
        }
        else
        {
            Debug.LogWarning("[UITransitionListener] ShopManager not found! Assign it in Inspector.");
        }
    }

    void OnDisable()
    {
        // Unsubscribe from events
        if (shopManager != null)
        {
            shopManager.OnShowShop -= ShowTransition;
            shopManager.OnHideShop -= HideTransition;
        }
    }

    /// <summary>
    /// Show the panel with transition animation.
    /// </summary>
    public void ShowTransition()
    {
        if (panelTransition != null)
        {
            panelTransition.Show();
            Debug.Log($"[UITransitionListener] Showing {gameObject.name}");
        }
    }

    /// <summary>
    /// Hide the panel with transition animation.
    /// </summary>
    public void HideTransition()
    {
        if (panelTransition != null)
        {
            panelTransition.Hide();
            Debug.Log($"[UITransitionListener] Hiding {gameObject.name}");
        }
    }

#if UNITY_EDITOR
    [ContextMenu("Test Show Transition")]
    private void TestShow()
    {
        ShowTransition();
    }

    [ContextMenu("Test Hide Transition")]
    private void TestHide()
    {
        HideTransition();
    }
#endif
}
