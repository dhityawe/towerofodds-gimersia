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
        if (item == null) return;

        // Remove old icon if exists
        if (spawnedIcons.ContainsKey(slotIndex))
        {
            Destroy(spawnedIcons[slotIndex]);
            spawnedIcons.Remove(slotIndex);
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
        if (itemIconPrefab == null || itemInventoryContainer == null) return;

        GameObject iconObj = Instantiate(itemIconPrefab, itemInventoryContainer);
        iconObj.name = $"ItemIcon_{slotIndex}_{item.GetName()}";

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
            Debug.Log($"[ItemVisualization] Icon spawned for slot {slotIndex}: {item.GetName()}");
        }
        else
        {
            Debug.LogWarning($"[ItemVisualization] No Image component or icon sprite found for {item.GetName()}");
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
