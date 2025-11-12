using UnityEngine;
using TMPro;

/// <summary>
/// Balatro-style character wiggle effect for TextMeshProUGUI.
/// Attach to any TMP text to make each character wiggle independently.
/// Optimized for performance with toggle and dirty checking.
/// </summary>
[RequireComponent(typeof(TextMeshProUGUI))]
public class TMPWiggleEffect : MonoBehaviour
{
    [Header("Wiggle Settings")]
    [SerializeField] private bool enableWiggle = true;
    [SerializeField] private float wiggleSpeed = 3f;
    [SerializeField] private float wiggleAmplitude = 2f;
    [SerializeField] private float wiggleRotation = 5f;
    [SerializeField] private float wiggleOffset = 0.3f; // Wave offset between characters

    [Header("Performance")]
    [SerializeField] private bool updateWhenInvisible = false; // Disable when off-screen for performance

    private TextMeshProUGUI tmpText;
    private bool wasEnabled;
    private string lastText = "";

    void Awake()
    {
        tmpText = GetComponent<TextMeshProUGUI>();
    }

    void OnEnable()
    {
        wasEnabled = enableWiggle;
    }

    void Update()
    {
        // Performance: Skip if disabled or text is invisible
        if (!enableWiggle || tmpText == null)
            return;

        if (!updateWhenInvisible && !tmpText.enabled)
            return;

        ApplyWiggle();
    }

    /// <summary>
    /// Apply wiggle effect to all visible characters.
    /// </summary>
    private void ApplyWiggle()
    {
        // Force mesh update
        tmpText.ForceMeshUpdate();

        var textInfo = tmpText.textInfo;
        if (textInfo == null || textInfo.characterCount == 0) 
            return;

        // Check if text changed (reset mesh if needed)
        if (lastText != tmpText.text)
        {
            lastText = tmpText.text;
        }

        // Animate each character
        for (int i = 0; i < textInfo.characterCount; i++)
        {
            var charInfo = textInfo.characterInfo[i];

            // Skip invisible characters (spaces, etc.)
            if (!charInfo.isVisible) 
                continue;

            int materialIndex = charInfo.materialReferenceIndex;
            int vertexIndex = charInfo.vertexIndex;

            // Get the vertices for this character (4 vertices per character)
            var vertices = textInfo.meshInfo[materialIndex].vertices;

            // Calculate wave animation based on character index and time
            float timeOffset = i * wiggleOffset;
            float wave = Mathf.Sin((Time.time * wiggleSpeed) + timeOffset);

            // Calculate character center point
            Vector3 charCenter = (vertices[vertexIndex + 0] + 
                                 vertices[vertexIndex + 1] + 
                                 vertices[vertexIndex + 2] + 
                                 vertices[vertexIndex + 3]) / 4f;

            // Vertical offset
            Vector3 offset = new Vector3(0, wave * wiggleAmplitude, 0);

            // Rotation around Z-axis
            float rotation = wave * wiggleRotation;
            Quaternion rot = Quaternion.Euler(0, 0, rotation);

            // Apply transformations to all 4 vertices
            for (int j = 0; j < 4; j++)
            {
                Vector3 vertex = vertices[vertexIndex + j];

                // Rotate vertex around character center
                vertex = charCenter + rot * (vertex - charCenter);

                // Add vertical wiggle offset
                vertex += offset;

                vertices[vertexIndex + j] = vertex;
            }
        }

        // Update mesh geometry
        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            var meshInfo = textInfo.meshInfo[i];
            meshInfo.mesh.vertices = meshInfo.vertices;
            tmpText.UpdateGeometry(meshInfo.mesh, i);
        }
    }

    /// <summary>
    /// Enable or disable wiggle effect.
    /// </summary>
    public void SetWiggleEnabled(bool enabled)
    {
        enableWiggle = enabled;

        // Reset mesh to default state when disabling
        if (!enabled && tmpText != null)
        {
            tmpText.ForceMeshUpdate();
        }
    }

    /// <summary>
    /// Set wiggle intensity (0-1 range recommended).
    /// </summary>
    public void SetWiggleIntensity(float intensity)
    {
        wiggleAmplitude = 2f * intensity;
        wiggleRotation = 5f * intensity;
    }

    /// <summary>
    /// Set custom wiggle parameters.
    /// </summary>
    public void SetWiggleParams(float speed, float amplitude, float rotation, float offset)
    {
        wiggleSpeed = speed;
        wiggleAmplitude = amplitude;
        wiggleRotation = rotation;
        wiggleOffset = offset;
    }

    public bool IsWiggleEnabled => enableWiggle;

#if UNITY_EDITOR
    [ContextMenu("Toggle Wiggle")]
    private void ToggleWiggle()
    {
        enableWiggle = !enableWiggle;
        Debug.Log($"Wiggle effect: {(enableWiggle ? "ON" : "OFF")}");
    }

    [ContextMenu("Wiggle Intensity: Low")]
    private void SetLowIntensity()
    {
        SetWiggleParams(2f, 1f, 3f, 0.3f);
        Debug.Log("Wiggle set to LOW intensity");
    }

    [ContextMenu("Wiggle Intensity: Medium")]
    private void SetMediumIntensity()
    {
        SetWiggleParams(3f, 2f, 5f, 0.3f);
        Debug.Log("Wiggle set to MEDIUM intensity");
    }

    [ContextMenu("Wiggle Intensity: High")]
    private void SetHighIntensity()
    {
        SetWiggleParams(5f, 4f, 10f, 0.3f);
        Debug.Log("Wiggle set to HIGH intensity");
    }
#endif
}
