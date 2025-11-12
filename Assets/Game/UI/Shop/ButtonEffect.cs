using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(AudioSource))]
public class ButtonEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Click Effects")]
    [SerializeField] private float clickPunchScale = 1.2f;
    [SerializeField] private float clickPunchDuration = 0.2f;
    [SerializeField] private int clickVibrato = 10;
    [SerializeField] private float clickElasticity = 1f;
    [SerializeField] private bool enablePressDown = true;
    [SerializeField] private float pressDownScale = 0.9f;
    [SerializeField] private float pressDownDuration = 0.1f;

    [Header("Hover Effects")]
    [SerializeField] private bool enableHoverScale = true;
    [SerializeField] private float hoverScale = 1.08f;
    [SerializeField] private float hoverDuration = 0.15f;
    [SerializeField] private Ease hoverEase = Ease.OutBack;
    
    [SerializeField] private bool enableHoverBounce = true;
    [SerializeField] private float bounceAmount = 5f;
    [SerializeField] private float bounceDuration = 0.5f;

    [Header("Color Effects")]
    [SerializeField] private bool enableColorChange = false;
    [SerializeField] private Image targetImage;
    [SerializeField] private Color hoverColor = new Color(1f, 1f, 0.8f);
    [SerializeField] private Color pressColor = new Color(0.8f, 0.8f, 0.8f);
    [SerializeField] private float colorTransitionDuration = 0.15f;

    [Header("Rotation Effects")]
    [SerializeField] private bool enableRotationWiggle = true;
    [SerializeField] private float wiggleAngle = 5f;
    [SerializeField] private float wiggleDuration = 0.3f;

    [Header("Audio")]
    [SerializeField] private AudioClip clickClip;
    [SerializeField] private AudioClip hoverClip;
    [SerializeField] private float clickVolume = 1f;
    [SerializeField] private float hoverVolume = 0.5f;
    [SerializeField] private float pitchVariation = 0.1f;

    [Header("Particle Effects (Optional)")]
    [SerializeField] private ParticleSystem clickParticles;

    private Button button;
    private AudioSource audioSource;
    private Vector3 originalScale;
    private Vector3 originalPosition;
    private Color originalColor;
    private bool isHovering = false;
    private bool isInitialized = false;
    private Tween hoverTween;
    private Tween bounceTween;

    void Awake()
    {
        button = GetComponent<Button>();
        audioSource = GetComponent<AudioSource>();
        originalScale = transform.localScale;
        originalPosition = transform.localPosition;
        
        if (audioSource != null)
        {
            audioSource.playOnAwake = false;
        }

        if (targetImage == null)
        {
            targetImage = GetComponent<Image>();
        }

        if (targetImage != null && enableColorChange)
        {
            originalColor = targetImage.color;
        }
    }

    void Start()
    {
        // Capture original position after layout is settled (important for UI children)
        CaptureOriginalState();
        isInitialized = true;
    }

    private void CaptureOriginalState()
    {
        originalPosition = transform.localPosition;
        originalScale = transform.localScale;
        
        if (targetImage != null && enableColorChange)
        {
            originalColor = targetImage.color;
        }
    }

    void OnEnable()
    {
        // Don't subscribe to onClick - we handle effects in OnPointerDown/Up instead
        // This ensures effects play even if the button's onClick action disables/destroys it
        
        // Recapture position when re-enabled (in case parent moved)
        if (isInitialized)
        {
            CaptureOriginalState();
        }
    }

    void OnDisable()
    {
        KillAllTweens();
        ResetToOriginal();
    }

    void OnDestroy()
    {
        KillAllTweens();
    }

    private void ResetToOriginal()
    {
        // Reset all visual properties to original state
        transform.localScale = originalScale;
        transform.localPosition = originalPosition;
        transform.localRotation = Quaternion.identity; // Always reset rotation
        
        if (enableColorChange && targetImage != null)
        {
            targetImage.color = originalColor;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!button.interactable) return;

        isHovering = true;

        // Kill existing tweens and reset to original position first
        KillHoverTweens();
        transform.localPosition = originalPosition; // Reset position immediately to prevent drift

        // Hover scale
        if (enableHoverScale)
        {
            hoverTween = transform.DOScale(originalScale * hoverScale, hoverDuration)
                .SetEase(hoverEase);
        }

        // Hover bounce (always start from original position)
        if (enableHoverBounce)
        {
            Vector3 targetPos = originalPosition + new Vector3(0, bounceAmount, 0);
            bounceTween = transform.DOLocalMove(targetPos, bounceDuration)
                .SetEase(Ease.OutQuad);
        }

        // Color change
        if (enableColorChange && targetImage != null)
        {
            targetImage.DOColor(hoverColor, colorTransitionDuration);
        }

        // Hover sound
        if (hoverClip != null && audioSource != null)
        {
            audioSource.pitch = 1f + Random.Range(-pitchVariation * 0.5f, pitchVariation * 0.5f);
            audioSource.PlayOneShot(hoverClip, hoverVolume);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!button.interactable) return;

        isHovering = false;

        // Kill hover tweens
        KillHoverTweens();

        // Reset to original position and scale immediately to prevent drift
        transform.localPosition = originalPosition;

        // Animate back to original scale
        if (enableHoverScale)
        {
            hoverTween = transform.DOScale(originalScale, hoverDuration)
                .SetEase(Ease.OutQuad);
        }

        // Reset color
        if (enableColorChange && targetImage != null)
        {
            targetImage.DOColor(originalColor, colorTransitionDuration);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!button.interactable || !enablePressDown) return;

        // Kill scale tweens only, preserve rotation
        transform.DOKill(false);
        transform.DOScale(originalScale * pressDownScale, pressDownDuration)
            .SetEase(Ease.OutQuad);

        // Press color
        if (enableColorChange && targetImage != null)
        {
            targetImage.DOColor(pressColor, pressDownDuration);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!button.interactable || !enablePressDown) return;

        // Return to hover or original scale
        float targetScale = isHovering && enableHoverScale ? hoverScale : 1f;
        transform.DOScale(originalScale * targetScale, pressDownDuration * 1.5f)
            .SetEase(Ease.OutBack);

        // Return to hover color
        if (enableColorChange && targetImage != null)
        {
            Color targetColor = isHovering ? hoverColor : originalColor;
            targetImage.DOColor(targetColor, colorTransitionDuration);
        }

        // Play click effects on pointer up (ensures effects play even if button gets disabled during onClick)
        PlayClickEffects();
    }

    /// <summary>
    /// Play all click effects (sound, particles, rotation).
    /// Called on pointer up to ensure effects play even if button action disables/destroys the button.
    /// </summary>
    private void PlayClickEffects()
    {
        if (!button.interactable) return;

        // Kill all existing tweens and reset rotation to prevent spam-click drift
        transform.DOKill(true); // Complete existing tweens
        transform.localRotation = Quaternion.identity; // Reset rotation immediately

        // Play punch scale effect
        transform.DOPunchScale(Vector3.one * (clickPunchScale - 1f), clickPunchDuration, clickVibrato, clickElasticity)
            .SetEase(Ease.OutElastic);

        // Rotation wiggle (starts from zero rotation)
        if (enableRotationWiggle)
        {
            transform.DOPunchRotation(new Vector3(0, 0, wiggleAngle), wiggleDuration, 10, 1f);
        }

        // Play click sound with pitch variation
        if (clickClip != null && audioSource != null)
        {
            audioSource.pitch = 1f + Random.Range(-pitchVariation, pitchVariation);
            audioSource.PlayOneShot(clickClip, clickVolume);
        }

        // Spawn particles
        if (clickParticles != null)
        {
            clickParticles.Play();
        }
    }

    private void KillAllTweens()
    {
        transform.DOKill();
        hoverTween?.Kill();
        bounceTween?.Kill();
        if (targetImage != null)
        {
            targetImage.DOKill();
        }
    }

    private void KillHoverTweens()
    {
        hoverTween?.Kill();
        bounceTween?.Kill();
    }

#if UNITY_EDITOR
    void Reset()
    {
        // Set juicy defaults
        clickPunchScale = 1.2f;
        clickPunchDuration = 0.2f;
        clickVibrato = 10;
        clickElasticity = 1f;
        
        enableHoverScale = true;
        hoverScale = 1.08f;
        hoverDuration = 0.15f;
        
        enableHoverBounce = true;
        bounceAmount = 5f;
        
        enablePressDown = true;
        pressDownScale = 0.9f;
        
        clickVolume = 1f;
        hoverVolume = 0.5f;
        pitchVariation = 0.1f;
    }
#endif
}
