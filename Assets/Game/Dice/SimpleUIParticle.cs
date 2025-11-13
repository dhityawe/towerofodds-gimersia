// SimpleUIParticle.cs
// Simple UI particle for multiplier effect
// Just a colored image that can be animated

using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Simple UI particle - just an image that can be animated by MultTextEffect.
/// Create a UI Image GameObject, attach this script, and save as prefab.
/// </summary>
[RequireComponent(typeof(Image))]
public class SimpleUIParticle : MonoBehaviour
{
    // This component doesn't need any logic
    // It's just a marker for the particle prefab
    // The animation is handled by MultTextEffect.cs
}
