// DiceVisualController.cs
// Visual representation of dice with face sprites
// Attach to UI Image or SpriteRenderer

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

/// <summary>
/// Controls visual display of a single die (1-6 faces).
/// Updates sprite/text based on current dice value.
/// Subscribe to DiceRoller events to animate.
/// Uses DOTween for smooth animations.
/// </summary>
public class DiceVisualController : MonoBehaviour
{
    [Header("Display Mode")]
    [Tooltip("Which die to display (1 or 2)")]
    [SerializeField] private int diceNumber = 1; // 1 or 2

    [Header("Visual Components")]
    [Tooltip("UI Image component (for UI dice)")]
    [SerializeField] private Image diceImage;
    
    [Tooltip("SpriteRenderer component (for world-space dice)")]
    [SerializeField] private SpriteRenderer diceSprite;
    
    [Tooltip("Optional text display (shows number 1-6)")]
    [SerializeField] private TextMeshProUGUI diceText;

    [Header("Dice Face Sprites")]
    [Tooltip("Sprites for dice faces 1-6 (array index 0 = face 1)")]
    [SerializeField] private Sprite[] diceFaces = new Sprite[6];

    [Header("Animation")]
    [Tooltip("Shake/rotate dice during roll")]
    [SerializeField] private bool enableShake = true;
    [SerializeField] private float shakeIntensity = 10f;
    [SerializeField] private bool enableRotation = true;
    [SerializeField] private float rotationSpeed = 360f;
    
    [Header("Animation Randomization")]
    [Tooltip("Randomize each dice's animation to prevent synchronization")]
    [SerializeField] private bool randomizeAnimationTiming = true;
    [SerializeField] private Vector2 shakeIntensityRange = new Vector2(8f, 15f);
    [SerializeField] private Vector2 rotationSpeedRange = new Vector2(200f, 400f);
    [SerializeField] private Vector2 animationDelayRange = new Vector2(0f, 0.15f);
    [SerializeField] private Vector2 finishTimeOffsetRange = new Vector2(-0.1f, 0.1f);
    
    [Header("DOTween Settings")]
    [Tooltip("Punch scale on roll start")]
    [SerializeField] private bool enablePunchScale = true;
    [SerializeField] private float punchScaleAmount = 1.3f;
    [SerializeField] private float punchDuration = 0.3f;
    
    [Tooltip("Bounce on roll complete")]
    [SerializeField] private bool enableBounce = true;
    [SerializeField] private float bounceScale = 1.2f;
    [SerializeField] private float bounceDuration = 0.5f;
    
    [Tooltip("Color flash on roll complete")]
    [SerializeField] private bool enableColorFlash = true;
    [SerializeField] private Color flashColor = Color.yellow;
    [SerializeField] private float flashDuration = 0.2f;
    [SerializeField] private int flashLoops = 2;
    
    [Tooltip("Glow pulse during roll")]
    [SerializeField] private bool enableGlowPulse = true;
    [SerializeField] private float glowMinAlpha = 0.7f;
    [SerializeField] private float glowPulseDuration = 0.3f;

    [Header("References")]
    [SerializeField] private DiceRoller diceRoller;

    private RectTransform rectTransform;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Vector3 originalScale;
    private int currentValue = 1;
    private Sequence rollSequence;
    private Tween shakeTween;
    private Tween rotateTween;
    private Tween glowTween;
    private Color originalColor;
    private float randomizedShakeIntensity;
    private float randomizedRotationSpeed;
    private float randomizedDelay;
    private int randomizedRotationDirection; // 1 for clockwise, -1 for counter-clockwise
    private float randomizedFinishTimeOffset;

    void Awake()
    {
        // Auto-find DiceRoller if not assigned
        if (diceRoller == null)
        {
            diceRoller = FindFirstObjectByType<DiceRoller>();
        }

        // Get transform for shake animation
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            originalPosition = rectTransform.localPosition;
            originalRotation = rectTransform.localRotation;
            originalScale = rectTransform.localScale;
        }
        else
        {
            originalPosition = transform.localPosition;
            originalRotation = transform.localRotation;
            originalScale = transform.localScale;
        }

        // Store original color
        if (diceImage != null)
        {
            originalColor = diceImage.color;
        }

        // Initialize to face 1
        SetDiceFace(1);
        
        // Randomize animation parameters for this dice
        RandomizeAnimationParameters();
    }
    
    private void RandomizeAnimationParameters()
    {
        if (randomizeAnimationTiming)
        {
            randomizedShakeIntensity = Random.Range(shakeIntensityRange.x, shakeIntensityRange.y);
            randomizedRotationSpeed = Random.Range(rotationSpeedRange.x, rotationSpeedRange.y);
            randomizedDelay = Random.Range(animationDelayRange.x, animationDelayRange.y);
            randomizedRotationDirection = Random.Range(0, 2) == 0 ? 1 : -1; // 50% chance for each direction
            randomizedFinishTimeOffset = Random.Range(finishTimeOffsetRange.x, finishTimeOffsetRange.y);
        }
        else
        {
            randomizedShakeIntensity = shakeIntensity;
            randomizedRotationSpeed = rotationSpeed;
            randomizedDelay = 0f;
            randomizedRotationDirection = 1; // Default clockwise
            randomizedFinishTimeOffset = 0f;
        }
    }

    void OnDestroy()
    {
        // Kill all tweens on destroy
        KillAllTweens();
    }

    void OnEnable()
    {
        if (diceRoller != null)
        {
            diceRoller.OnRollStarted += OnRollStarted;
            diceRoller.OnRollUpdate += OnRollUpdate;
            diceRoller.OnRollComplete += OnRollComplete;
        }
    }

    void OnDisable()
    {
        if (diceRoller != null)
        {
            diceRoller.OnRollStarted -= OnRollStarted;
            diceRoller.OnRollUpdate -= OnRollUpdate;
            diceRoller.OnRollComplete -= OnRollComplete;
        }
        
        // Kill tweens when disabled
        KillAllTweens();
    }

    void Update()
    {
        // DOTween handles animations, no manual update needed
        // Old shake/rotate code removed in favor of DOTween
    }

    private void OnRollStarted()
    {
        // Kill any existing tweens
        KillAllTweens();
        
        // Reset to default position and scale
        if (rectTransform != null)
        {
            rectTransform.localPosition = originalPosition;
            rectTransform.localRotation = originalRotation;
            rectTransform.localScale = originalScale;
        }
        else
        {
            transform.localPosition = originalPosition;
            transform.localRotation = originalRotation;
            transform.localScale = originalScale;
        }

        // Randomize animation parameters for this roll
        RandomizeAnimationParameters();

        // Punch scale animation on roll start (with randomized delay)
        if (enablePunchScale)
        {
            float delay = randomizedDelay;
            
            if (rectTransform != null)
            {
                DOVirtual.DelayedCall(delay, () => {
                    rectTransform?.DOPunchScale(Vector3.one * (punchScaleAmount - 1f), punchDuration, 5, 0.5f)
                        .SetEase(Ease.OutElastic);
                });
            }
            else
            {
                DOVirtual.DelayedCall(delay, () => {
                    if (transform != null)
                    {
                        transform.DOPunchScale(Vector3.one * (punchScaleAmount - 1f), punchDuration, 5, 0.5f)
                            .SetEase(Ease.OutElastic);
                    }
                });
            }
        }

        // Start shake and rotation loop (with randomized delay)
        if (enableShake)
        {
            DOVirtual.DelayedCall(randomizedDelay, () => {
                if (this != null) StartShakeAndRotation();
            });
        }
        
        // Start glow pulse effect (with randomized delay)
        if (enableGlowPulse && diceImage != null)
        {
            DOVirtual.DelayedCall(randomizedDelay, () => {
                if (diceImage != null)
                {
                    glowTween = diceImage.DOFade(glowMinAlpha, glowPulseDuration)
                        .SetEase(Ease.InOutSine)
                        .SetLoops(-1, LoopType.Yoyo);
                }
            });
        }
    }

    private void OnRollUpdate(int dice1, int dice2)
    {
        // Update visual based on which die this is
        int newValue = (diceNumber == 1) ? dice1 : dice2;
        SetDiceFace(newValue);
    }

    private void OnRollComplete(int dice1, int dice2)
    {
        // Apply randomized finish time offset (delays this dice's completion slightly)
        DOVirtual.DelayedCall(Mathf.Max(0f, randomizedFinishTimeOffset), () =>
        {
            if (this == null) return;
            
            // Set final value
            int finalValue = (diceNumber == 1) ? dice1 : dice2;
            SetDiceFace(finalValue);

            // Kill shake/rotation tweens
            KillAllTweens();

            // Reset position/rotation
            if (rectTransform != null)
            {
                rectTransform.localPosition = originalPosition;
                rectTransform.localRotation = originalRotation;
                rectTransform.localScale = originalScale;
            }
            else
            {
                transform.localPosition = originalPosition;
                transform.localRotation = originalRotation;
                transform.localScale = originalScale;
            }

            // Bounce animation on roll complete
            if (enableBounce)
            {
                if (rectTransform != null)
                {
                    rectTransform.DOScale(originalScale * bounceScale, bounceDuration * 0.3f)
                        .SetEase(Ease.OutQuad)
                        .OnComplete(() => {
                            if (rectTransform != null)
                            {
                                rectTransform.DOScale(originalScale, bounceDuration * 0.7f)
                                    .SetEase(Ease.OutBounce);
                            }
                        });
                }
                else
                {
                    transform.DOScale(originalScale * bounceScale, bounceDuration * 0.3f)
                        .SetEase(Ease.OutQuad)
                        .OnComplete(() => {
                            if (transform != null)
                            {
                                transform.DOScale(originalScale, bounceDuration * 0.7f)
                                    .SetEase(Ease.OutBounce);
                            }
                        });
                }
            }
            
            // Color flash effect
            if (enableColorFlash && diceImage != null)
            {
                diceImage.DOColor(flashColor, flashDuration)
                    .SetLoops(flashLoops * 2, LoopType.Yoyo)
                    .OnComplete(() => {
                        if (diceImage != null) diceImage.color = originalColor;
                    });
            }

            Debug.Log($"[DiceVisual {diceNumber}] Final value: {finalValue}");
        });
    }

    /// <summary>
    /// Set the dice face to show a specific value (1-6).
    /// </summary>
    public void SetDiceFace(int value)
    {
        value = Mathf.Clamp(value, 1, 6);
        currentValue = value;

        // Update sprite (array index 0 = face 1)
        int spriteIndex = value - 1;
        
        if (diceFaces != null && spriteIndex < diceFaces.Length && diceFaces[spriteIndex] != null)
        {
            if (diceImage != null)
            {
                diceImage.sprite = diceFaces[spriteIndex];
            }

            if (diceSprite != null)
            {
                diceSprite.sprite = diceFaces[spriteIndex];
            }
        }

        // Update text
        if (diceText != null)
        {
            diceText.text = value.ToString();
        }
    }

    /// <summary>
    /// Apply shake and rotation animation during roll.
    /// </summary>
    private void StartShakeAndRotation()
    {
        Transform targetTransform = (rectTransform != null) ? (Transform)rectTransform : transform;
        
        // Continuous shake with DOTween (using randomized intensity)
        shakeTween = targetTransform.DOShakePosition(
            duration: 99f, // Long duration (killed manually on roll complete)
            strength: randomizedShakeIntensity,
            vibrato: 10,
            randomness: 90,
            snapping: false,
            fadeOut: false
        ).SetLoops(-1); // Infinite loop

        // Continuous rotation (using randomized speed and direction)
        float rotationAngle = 360f * randomizedRotationDirection; // Positive = clockwise, negative = counter-clockwise
        rotateTween = targetTransform.DORotate(
            new Vector3(0f, 0f, rotationAngle),
            duration: 360f / randomizedRotationSpeed, // Time for one full rotation
            RotateMode.FastBeyond360
        ).SetEase(Ease.Linear)
         .SetLoops(-1, LoopType.Restart); // Infinite loop
    }

    /// <summary>
    /// Kill all active tweens on this dice.
    /// </summary>
    private void KillAllTweens()
    {
        shakeTween?.Kill();
        rotateTween?.Kill();
        glowTween?.Kill();
        rollSequence?.Kill();
        
        Transform targetTransform = (rectTransform != null) ? (Transform)rectTransform : transform;
        targetTransform.DOKill(); // Kill any other tweens on this transform
        
        if (diceImage != null)
        {
            diceImage.DOKill(); // Kill color tweens
        }
    }

    // ====== Public API ======

    public int GetCurrentValue() => currentValue;
    
    /// <summary>
    /// Manually set dice value (useful for testing).
    /// </summary>
    [ContextMenu("Test: Set to 6")]
    private void TestSetSix() => SetDiceFace(6);

    [ContextMenu("Test: Set to 1")]
    private void TestSetOne() => SetDiceFace(1);
}
