using UnityEngine;

/// <summary>
/// Central registry untuk shared projectiles.
/// Attach to a GameObject in scene atau make it ScriptableObject.
/// </summary>
public class ProjectileRegistry : MonoBehaviour
{
    public static ProjectileRegistry Instance { get; private set; }

    [Header("Common Projectiles")]
    public GameObject basicProjectile;
    public GameObject diceProjectile;
    public GameObject multiShotProjectile;
    public GameObject bombProjectile;
    public GameObject cardProjectile;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Get projectile by name. Returns null if not found.
    /// </summary>
    public GameObject GetProjectile(string projectileName)
    {
        switch (projectileName.ToLower())
        {
            case "basic": return basicProjectile;
            case "dice": return diceProjectile;
            case "multishot": return multiShotProjectile;
            case "bomb": return bombProjectile;
            case "card": return cardProjectile;
            default:
                Debug.LogWarning($"Projectile '{projectileName}' not found in registry!");
                return null;
        }
    }
}
