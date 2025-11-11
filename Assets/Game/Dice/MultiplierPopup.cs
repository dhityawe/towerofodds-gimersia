// MultiplierPopup.cs
// Balatro-style multiplier popup with juice effects
// Shows damage multiplier with smooth animations and auto-destroys

using UnityEngine;
using TMPro;
using DG.Tweening;

/// <summary>
/// Popup UI element that shows multiplier value with Balatro-style animations.
/// Automatically animates and destroys itself after playing.
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class MultiplierPopup : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float riseDuration = 1.2f;
    [SerializeField] private float riseDistance = 100f;
    [SerializeField] private float scalePunchAmount = 1.5f;
    [SerializeField] private float scalePunchDuration = 0.4f;
    
    [Header("Fade Settings")]
    [SerializeField] private float fadeInDuration = 0.2f;
    [SerializeField] private float fadeOutDuration = 0.5f;
    [SerializeField] private float fadeOutDelay = 0.7f;
    
    [Header("Effects")]
    [SerializeField] private bool enableGlow = true;
    [SerializeField] private Color glowColor = Color.yellow;
    [SerializeField] private float glowDuration = 0.3f;
    
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private TextMeshProUGUI textComponent;
    private Sequence animationSequence;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
        textComponent = GetComponentInChildren<TextMeshProUGUI>();
        
        // Start invisible
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }
    }

    void Start()
    {
        PlayAnimation();
    }

    void OnDestroy()
    {
        animationSequence?.Kill();
    }

    private void PlayAnimation()
    {
        Vector2 startPos = rectTransform.anchoredPosition;
        Vector2 endPos = startPos + Vector2.up * riseDistance;
        
        animationSequence = DOTween.Sequence();
        
        // 1. Fade in quickly
        animationSequence.Append(canvasGroup.DOFade(1f, fadeInDuration));
        
        // 2. Punch scale (Balatro-style pop)
        animationSequence.Join(
            rectTransform.DOScale(Vector3.one * scalePunchAmount, scalePunchDuration * 0.4f)
                .SetEase(Ease.OutBack)
        );
        animationSequence.Append(
            rectTransform.DOScale(Vector3.one, scalePunchDuration * 0.6f)
                .SetEase(Ease.OutElastic)
        );
        
        // 3. Rise upward
        animationSequence.Join(
            rectTransform.DOAnchorPos(endPos, riseDuration)
                .SetEase(Ease.OutQuad)
        );
        
        // 4. Color flash/glow effect
        if (enableGlow && textComponent != null)
        {
            Color originalColor = textComponent.color;
            animationSequence.Insert(
                fadeInDuration,
                textComponent.DOColor(glowColor, glowDuration)
                    .SetLoops(2, LoopType.Yoyo)
            );
        }
        
        // 5. Fade out after delay
        animationSequence.Insert(
            fadeOutDelay,
            canvasGroup.DOFade(0f, fadeOutDuration)
                .SetEase(Ease.InQuad)
        );
        
        // 6. Destroy when done
        animationSequence.OnComplete(() => {
            Destroy(gameObject);
        });
    }

    /// <summary>
    /// Set the multiplier text. Call before animation starts.
    /// </summary>
    public void SetMultiplier(float multiplier)
    {
        if (textComponent != null)
        {
            textComponent.text = $"×{multiplier:F1}";
        }
    }

    /// <summary>
    /// Set custom text. Call before animation starts.
    /// </summary>
    public void SetText(string text)
    {
        if (textComponent != null)
        {
            textComponent.text = text;
        }
    }
}
