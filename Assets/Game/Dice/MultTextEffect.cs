// MultTextEffect.cs
// Balatro-style multiplier text popup with particle effects
// Handles scale animation, wiggle effect, and particle burst

using UnityEngine;
using TMPro;
using DG.Tweening;

/// <summary>
/// Multiplier text popup effect inspired by Balatro.
/// Features: Pop-in animation, wiggle effect, particle burst, auto-destroy.
/// Attach to multiplier popup prefab.
/// </summary>
[RequireComponent(typeof(TextMeshProUGUI))]
public class MultTextEffect : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float popInDuration = 0.5f;
    [SerializeField] private float popInScale = 2.0f;
    [SerializeField] private Ease popInEase = Ease.OutBack;
    [SerializeField] private float holdDuration = 2.5f;
    [SerializeField] private float fadeOutDuration = 0.6f;

    [Header("Wiggle Effect")]
    [SerializeField] private bool enableWiggle = true;
    [SerializeField] private float wiggleAmount = 5f;
    [SerializeField] private float wiggleSpeed = 20f;

    [Header("Particle Effect")]
    [SerializeField] private GameObject particlePrefab;
    [SerializeField] private int particleCount = 10;
    [SerializeField] private float particleSpread = 50f;
    [SerializeField] private Color particleColor = Color.yellow;
    [SerializeField] private float particleLifetime = 1f;
    [SerializeField] private float particleSpeed = 100f;

    [Header("Movement")]
    [SerializeField] private Vector3 floatDirection = new Vector3(0, 50f, 0);
    [SerializeField] private float floatDuration = 1f;

    private TextMeshProUGUI textMesh;
    private TMPWiggleEffect wiggleEffect;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Sequence animationSequence;

    void Awake()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        rectTransform = GetComponent<RectTransform>();
        
        // Add canvas group for fading
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        // Add wiggle effect if enabled
        if (enableWiggle)
        {
            wiggleEffect = GetComponent<TMPWiggleEffect>();
            if (wiggleEffect == null)
            {
                wiggleEffect = gameObject.AddComponent<TMPWiggleEffect>();
            }
            // Note: TMPWiggleEffect uses its own serialized settings
            // Adjust wiggle in Inspector after component is added
        }
        
        // Start with scale zero so we don't see a flash before animation
        transform.localScale = Vector3.zero;
    }

    void Start()
    {
        // Play effect after text is set by DiceRollUI
        PlayEffect();
    }

    void OnDestroy()
    {
        // Clean up tweens
        animationSequence?.Kill();
        DOTween.Kill(transform);
        DOTween.Kill(canvasGroup);
    }

    /// <summary>
    /// Play the full popup effect sequence.
    /// </summary>
    public void PlayEffect()
    {
        Debug.Log($"[MultTextEffect] PlayEffect started for {gameObject.name}");
        
        // Start invisible and scaled down
        transform.localScale = Vector3.zero;
        canvasGroup.alpha = 1f;
        
        Debug.Log($"[MultTextEffect] Initial state - Scale: {transform.localScale}, Alpha: {canvasGroup.alpha}, Position: {transform.position}");

        // Create animation sequence
        animationSequence = DOTween.Sequence();

        // Pop in animation
        animationSequence.Append(
            transform.DOScale(Vector3.one * popInScale, popInDuration)
                .SetEase(popInEase)
                .OnStart(() => Debug.Log($"[MultTextEffect] Pop-in animation started"))
                .OnComplete(() => Debug.Log($"[MultTextEffect] Pop-in complete, scale: {transform.localScale}"))
        );

        // Slight scale down to normal
        animationSequence.Append(
            transform.DOScale(Vector3.one, 0.15f)
                .SetEase(Ease.OutQuad)
        );

        // Hold at full opacity
        animationSequence.AppendInterval(holdDuration);

        // Fade out while floating up
        animationSequence.Append(
            canvasGroup.DOFade(0f, fadeOutDuration)
                .SetEase(Ease.InQuad)
        );

        // Float upward during fade out
        animationSequence.Join(
            transform.DOMove(transform.position + floatDirection, floatDuration)
                .SetEase(Ease.OutQuad)
        );

        // Destroy after animation completes
        animationSequence.OnComplete(() => {
            Destroy(gameObject);
        });

        // Spawn particles on pop-in
        DOVirtual.DelayedCall(popInDuration * 0.5f, () => {
            SpawnParticles();
        });
    }

    /// <summary>
    /// Spawn particle burst effect.
    /// </summary>
    private void SpawnParticles()
    {
        if (particlePrefab != null)
        {
            // Use prefab-based particles
            for (int i = 0; i < particleCount; i++)
            {
                GameObject particle = Instantiate(particlePrefab, transform.position, Quaternion.identity, transform.parent);
                
                // Random direction in circle
                float angle = (360f / particleCount) * i + Random.Range(-10f, 10f);
                Vector3 direction = Quaternion.Euler(0, 0, angle) * Vector3.right;
                Vector3 targetPos = transform.position + direction * particleSpread;
                
                // Animate particle
                particle.transform.DOMove(targetPos, particleLifetime)
                    .SetEase(Ease.OutQuad);
                
                // Fade out
                var particleCanvasGroup = particle.GetComponent<CanvasGroup>();
                if (particleCanvasGroup == null)
                {
                    particleCanvasGroup = particle.AddComponent<CanvasGroup>();
                }
                particleCanvasGroup.DOFade(0f, particleLifetime)
                    .SetEase(Ease.InQuad)
                    .OnComplete(() => Destroy(particle));
            }
        }
        else
        {
            // Fallback: Create simple particle effect using Unity ParticleSystem
            CreateSimpleParticleEffect();
        }
    }

    /// <summary>
    /// Create a simple particle system if no prefab is assigned.
    /// </summary>
    private void CreateSimpleParticleEffect()
    {
        GameObject particleObj = new GameObject("MultiplierParticles");
        particleObj.transform.position = transform.position;
        particleObj.transform.SetParent(transform.parent);

        ParticleSystem ps = particleObj.AddComponent<ParticleSystem>();
        
        // Stop the system first to configure it
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        
        var main = ps.main;
        main.duration = 0.5f;
        main.startLifetime = particleLifetime;
        main.startSpeed = particleSpeed;
        main.startSize = 5f;
        main.startColor = particleColor;
        main.maxParticles = particleCount;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.loop = false;
        main.playOnAwake = false;

        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] {
            new ParticleSystem.Burst(0f, particleCount)
        });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 10f;

        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { 
                new GradientColorKey(particleColor, 0f), 
                new GradientColorKey(particleColor, 1f) 
            },
            new GradientAlphaKey[] { 
                new GradientAlphaKey(1f, 0f), 
                new GradientAlphaKey(0f, 1f) 
            }
        );
        colorOverLifetime.color = gradient;

        // Now play the system after configuration
        ps.Play();

        // Destroy particle system after it's done
        Destroy(particleObj, main.duration + main.startLifetime.constantMax);
    }

    /// <summary>
    /// Set the multiplier text value.
    /// </summary>
    public void SetMultiplierText(float multiplier, string skillName = "")
    {
        if (textMesh != null)
        {
            textMesh.text = skillName != "" 
                ? $"{skillName}\n×{multiplier:F1}"
                : $"×{multiplier:F1}";
            
            // Ensure text is visible (set alpha to full)
            Color textColor = textMesh.color;
            textColor.a = 1f;
            textMesh.color = textColor;
            
            // Force update
            textMesh.ForceMeshUpdate();
        }
    }

    /// <summary>
    /// Stop the effect immediately.
    /// </summary>
    public void StopEffect()
    {
        animationSequence?.Kill();
        DOTween.Kill(transform);
        DOTween.Kill(canvasGroup);
        Destroy(gameObject);
    }
}
