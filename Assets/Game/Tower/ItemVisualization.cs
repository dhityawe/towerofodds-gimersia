// ItemVisualization.cs
// UI component that visualizes tower items
// Listens to TowerRuntime events and spawns/updates item sprite icons

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Visualizes tower items by spawning item icon prefabs in the ItemInventory container.
/// Attach this to a UI GameObject (e.g., ItemInventoryPanel).
/// </summary>
public class ItemVisualization : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TowerRuntime towerRuntime;
    [SerializeField] private Transform itemInventoryContainer; // Parent for item icons
    [SerializeField] private GameObject itemIconPrefab; // Prefab with Image component

    [Header("Settings")]
    [SerializeField] private bool autoRefreshOnStart = true;

    private Dictionary<int, GameObject> spawnedIcons = new Dictionary<int, GameObject>();

    void Awake()
    {
        // Auto-find tower if not assigned
        if (towerRuntime == null)
        {
            towerRuntime = FindFirstObjectByType<TowerRuntime>();
        }

        if (itemInventoryContainer == null)
        {
            Debug.LogError("[ItemVisualization] itemInventoryContainer is not assigned!");
        }

        if (itemIconPrefab == null)
        {
            Debug.LogError("[ItemVisualization] itemIconPrefab is not assigned!");
        }
    }

    void OnEnable()
    {
        if (towerRuntime != null)
        {
            towerRuntime.OnItemAdded += OnItemAdded;
            towerRuntime.OnItemRemoved += OnItemRemoved;
            Debug.Log("[ItemVisualization] Subscribed to TowerRuntime item events");
        }
        else
        {
            Debug.LogWarning("[ItemVisualization] TowerRuntime is null, cannot subscribe to events!");
        }

        if (autoRefreshOnStart)
        {
            RefreshAll();
        }
    }

    void OnDisable()
    {
        if (towerRuntime != null)
        {
            towerRuntime.OnItemAdded -= OnItemAdded;
            towerRuntime.OnItemRemoved -= OnItemRemoved;
        }
    }

    /// <summary>
    /// Called when an item is added to the tower.
    /// </summary>
    private void OnItemAdded(int slotIndex, TowerItem item)
    {
        Debug.Log($"[ItemVisualization] OnItemAdded event received! Slot: {slotIndex}, Item: {(item != null ? item.GetName() : "NULL")}");
        
        if (item == null)
        {
            Debug.LogWarning("[ItemVisualization] Item is null!");
            return;
        }

        // Remove old icon if exists
        if (spawnedIcons.ContainsKey(slotIndex))
        {
            Destroy(spawnedIcons[slotIndex]);
            spawnedIcons.Remove(slotIndex);
            Debug.Log($"[ItemVisualization] Removed old icon from slot {slotIndex}");
        }

        // Spawn new icon
        SpawnItemIcon(slotIndex, item);
    }

    /// <summary>
    /// Called when an item is removed from the tower.
    /// </summary>
    private void OnItemRemoved(int slotIndex)
    {
        if (spawnedIcons.ContainsKey(slotIndex))
        {
            Destroy(spawnedIcons[slotIndex]);
            spawnedIcons.Remove(slotIndex);
            Debug.Log($"[ItemVisualization] Icon removed from slot {slotIndex}");
        }
    }

    /// <summary>
    /// Spawn an item icon for the given slot.
    /// </summary>
    private void SpawnItemIcon(int slotIndex, TowerItem item)
    {
        Debug.Log($"[ItemVisualization] SpawnItemIcon called - Slot: {slotIndex}, Item: {item.GetName()}");
        
        if (itemIconPrefab == null)
        {
            Debug.LogError("[ItemVisualization] itemIconPrefab is NULL! Cannot spawn icon.");
            return;
        }
        
        if (itemInventoryContainer == null)
        {
            Debug.LogError("[ItemVisualization] itemInventoryContainer is NULL! Cannot spawn icon.");
            return;
        }

        GameObject iconObj = Instantiate(itemIconPrefab, itemInventoryContainer);
        iconObj.name = $"ItemIcon_{slotIndex}_{item.GetName()}";
        
        Debug.Log($"[ItemVisualization] Icon GameObject instantiated: {iconObj.name}");

        // Set the icon sprite
        Image iconImage = iconObj.GetComponent<Image>();
        if (iconImage == null)
        {
            iconImage = iconObj.GetComponentInChildren<Image>();
        }

        if (iconImage != null && item.icon != null)
        {
            iconImage.sprite = item.icon;
            iconImage.color = Color.white; // Ensure visible
            Debug.Log($"[ItemVisualization] Icon spawned successfully for slot {slotIndex}: {item.GetName()}, Sprite: {item.icon.name}");
        }
        else
        {
            Debug.LogWarning($"[ItemVisualization] Failed to set icon! Image component: {(iconImage != null ? "Found" : "NULL")}, Item icon: {(item.icon != null ? item.icon.name : "NULL")}");
        }

        // Store reference
        spawnedIcons[slotIndex] = iconObj;
    }

    /// <summary>
    /// Refresh all item icons based on current tower state.
    /// Clears existing icons and re-spawns all.
    /// </summary>
    public void RefreshAll()
    {
        // Clear all existing icons
        foreach (var kvp in spawnedIcons)
        {
            if (kvp.Value != null)
            {
                Destroy(kvp.Value);
            }
        }
        spawnedIcons.Clear();

        // Spawn icons for all equipped items
        if (towerRuntime != null)
        {
            TowerItem[] items = towerRuntime.GetAllItems();
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] != null)
                {
                    SpawnItemIcon(i, items[i]);
                }
            }

            Debug.Log($"[ItemVisualization] Refreshed all icons - {spawnedIcons.Count} items");
        }
    }

    /// <summary>
    /// Clear all item icons.
    /// </summary>
    public void ClearAll()
    {
        foreach (var kvp in spawnedIcons)
        {
            if (kvp.Value != null)
            {
                Destroy(kvp.Value);
            }
        }
        spawnedIcons.Clear();
        Debug.Log("[ItemVisualization] All icons cleared");
    }

    [ContextMenu("Test: Refresh All")]
    private void TestRefreshAll()
    {
        RefreshAll();
    }

    [ContextMenu("Test: Clear All")]
    private void TestClearAll()
    {
        ClearAll();
    }
}
