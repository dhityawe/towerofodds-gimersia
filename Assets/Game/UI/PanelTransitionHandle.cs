using UnityEngine;
using DG.Tweening;

/// <summary>
/// Handles smooth panel transitions with various animation effects.
/// Attach to any UI panel GameObject to enable show/hide animations.
/// </summary>
public class PanelTransitionHandle : MonoBehaviour
{
    [Header("Transition Settings")]
    [SerializeField] private TransitionType showTransition = TransitionType.Scale;
    [SerializeField] private TransitionType hideTransition = TransitionType.Scale;
    [SerializeField] private float showDuration = 0.3f;
    [SerializeField] private float hideDuration = 0.2f;
    [SerializeField] private Ease showEase = Ease.OutBack;
    [SerializeField] private Ease hideEase = Ease.InBack;

    [Header("Advanced Options")]
    [SerializeField] private bool disableOnHide = true;
    [SerializeField] private bool startHidden = false;
    [SerializeField] private float delayBeforeShow = 0f;
    [SerializeField] private float delayBeforeHide = 0f;

    public enum TransitionType
    {
        Scale,          // Pop in/out
        Fade,           // Fade in/out
        SlideFromTop,   // Slide from top
        SlideFromBottom,// Slide from bottom
        SlideFromLeft,  // Slide from left
        SlideFromRight, // Slide from right
        SlideToCenter,  // Slide from current position to Y=0 and back
        ScaleAndFade,   // Combined scale and fade
        Bounce          // Bounce in/out
    }

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector3 originalScale;
    private Vector2 originalPosition;
    private Vector2 centerPosition; // Position with Y=0
    private Sequence currentSequence;
    private bool isVisible = true;

    // Events
    public System.Action OnShowComplete;
    public System.Action OnHideComplete;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        
        // Ensure CanvasGroup exists for fade transitions
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null && NeedsFade())
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        // Store original values
        originalScale = rectTransform.localScale;
        originalPosition = rectTransform.anchoredPosition;
        centerPosition = new Vector2(originalPosition.x, 0f); // Keep X, set Y to 0

        // Start hidden if requested
        if (startHidden)
        {
            SetupHiddenState();
            isVisible = false;
        }
    }

    private void OnDestroy()
    {
        // Kill any running animations
        currentSequence?.Kill();
    }

    /// <summary>
    /// Show the panel with animation.
    /// </summary>
    public void Show()
    {
        if (isVisible) return;

        // Kill any existing animation
        currentSequence?.Kill();

        // Enable GameObject
        gameObject.SetActive(true);
        isVisible = true;

        // Setup initial state
        SetupHiddenState();

        // Create animation sequence
        currentSequence = DOTween.Sequence();
        currentSequence.AppendInterval(delayBeforeShow);
        
        switch (showTransition)
        {
            case TransitionType.Scale:
                currentSequence.Append(rectTransform.DOScale(originalScale, showDuration).SetEase(showEase));
                break;

            case TransitionType.Fade:
                currentSequence.Append(canvasGroup.DOFade(1f, showDuration).SetEase(showEase));
                break;

            case TransitionType.SlideFromTop:
                currentSequence.Append(rectTransform.DOAnchorPos(originalPosition, showDuration).SetEase(showEase));
                break;

            case TransitionType.SlideFromBottom:
                currentSequence.Append(rectTransform.DOAnchorPos(originalPosition, showDuration).SetEase(showEase));
                break;

            case TransitionType.SlideFromLeft:
                currentSequence.Append(rectTransform.DOAnchorPos(originalPosition, showDuration).SetEase(showEase));
                break;

            case TransitionType.SlideFromRight:
                currentSequence.Append(rectTransform.DOAnchorPos(originalPosition, showDuration).SetEase(showEase));
                break;

            case TransitionType.SlideToCenter:
                // Slide from original position (hidden) to Y=0 (shown)
                currentSequence.Append(rectTransform.DOAnchorPos(centerPosition, showDuration).SetEase(showEase));
                break;

            case TransitionType.ScaleAndFade:
                currentSequence.Append(rectTransform.DOScale(originalScale, showDuration).SetEase(showEase));
                currentSequence.Join(canvasGroup.DOFade(1f, showDuration).SetEase(showEase));
                break;

            case TransitionType.Bounce:
                rectTransform.localScale = originalScale * 0.5f;
                currentSequence.Append(rectTransform.DOScale(originalScale * 1.1f, showDuration * 0.6f).SetEase(Ease.OutQuad));
                currentSequence.Append(rectTransform.DOScale(originalScale, showDuration * 0.4f).SetEase(Ease.InOutQuad));
                break;
        }

        currentSequence.OnComplete(() => OnShowComplete?.Invoke());
    }

    /// <summary>
    /// Hide the panel with animation.
    /// </summary>
    public void Hide()
    {
        if (!isVisible) return;

        // Kill any existing animation
        currentSequence?.Kill();
        isVisible = false;

        // Create animation sequence
        currentSequence = DOTween.Sequence();
        currentSequence.AppendInterval(delayBeforeHide);

        switch (hideTransition)
        {
            case TransitionType.Scale:
                currentSequence.Append(rectTransform.DOScale(Vector3.zero, hideDuration).SetEase(hideEase));
                break;

            case TransitionType.Fade:
                currentSequence.Append(canvasGroup.DOFade(0f, hideDuration).SetEase(hideEase));
                break;

            case TransitionType.SlideFromTop:
                Vector2 topPos = originalPosition + new Vector2(0, Screen.height);
                currentSequence.Append(rectTransform.DOAnchorPos(topPos, hideDuration).SetEase(hideEase));
                break;

            case TransitionType.SlideFromBottom:
                Vector2 bottomPos = originalPosition - new Vector2(0, Screen.height);
                currentSequence.Append(rectTransform.DOAnchorPos(bottomPos, hideDuration).SetEase(hideEase));
                break;

            case TransitionType.SlideFromLeft:
                Vector2 leftPos = originalPosition - new Vector2(Screen.width, 0);
                currentSequence.Append(rectTransform.DOAnchorPos(leftPos, hideDuration).SetEase(hideEase));
                break;

            case TransitionType.SlideFromRight:
                Vector2 rightPos = originalPosition + new Vector2(Screen.width, 0);
                currentSequence.Append(rectTransform.DOAnchorPos(rightPos, hideDuration).SetEase(hideEase));
                break;

            case TransitionType.SlideToCenter:
                // Slide from Y=0 (shown) back to original position (hidden)
                currentSequence.Append(rectTransform.DOAnchorPos(originalPosition, hideDuration).SetEase(hideEase));
                break;

            case TransitionType.ScaleAndFade:
                currentSequence.Append(rectTransform.DOScale(Vector3.zero, hideDuration).SetEase(hideEase));
                currentSequence.Join(canvasGroup.DOFade(0f, hideDuration).SetEase(hideEase));
                break;

            case TransitionType.Bounce:
                currentSequence.Append(rectTransform.DOScale(originalScale * 1.05f, hideDuration * 0.3f).SetEase(Ease.OutQuad));
                currentSequence.Append(rectTransform.DOScale(Vector3.zero, hideDuration * 0.7f).SetEase(Ease.InBack));
                break;
        }

        currentSequence.OnComplete(() =>
        {
            if (disableOnHide)
            {
                gameObject.SetActive(false);
            }
            OnHideComplete?.Invoke();
        });
    }

    /// <summary>
    /// Toggle panel visibility.
    /// </summary>
    public void Toggle()
    {
        if (isVisible)
            Hide();
        else
            Show();
    }

    /// <summary>
    /// Show panel immediately without animation.
    /// </summary>
    public void ShowImmediate()
    {
        currentSequence?.Kill();
        gameObject.SetActive(true);
        ResetToVisible();
        isVisible = true;
        OnShowComplete?.Invoke();
    }

    /// <summary>
    /// Hide panel immediately without animation.
    /// </summary>
    public void HideImmediate()
    {
        currentSequence?.Kill();
        SetupHiddenState();
        if (disableOnHide)
        {
            gameObject.SetActive(false);
        }
        isVisible = false;
        OnHideComplete?.Invoke();
    }

    private void SetupHiddenState()
    {
        switch (hideTransition)
        {
            case TransitionType.Scale:
            case TransitionType.Bounce:
                rectTransform.localScale = Vector3.zero;
                break;

            case TransitionType.Fade:
                if (canvasGroup != null)
                    canvasGroup.alpha = 0f;
                break;

            case TransitionType.SlideFromTop:
                rectTransform.anchoredPosition = originalPosition + new Vector2(0, Screen.height);
                break;

            case TransitionType.SlideFromBottom:
                rectTransform.anchoredPosition = originalPosition - new Vector2(0, Screen.height);
                break;

            case TransitionType.SlideFromLeft:
                rectTransform.anchoredPosition = originalPosition - new Vector2(Screen.width, 0);
                break;

            case TransitionType.SlideFromRight:
                rectTransform.anchoredPosition = originalPosition + new Vector2(Screen.width, 0);
                break;

            case TransitionType.SlideToCenter:
                // Start at original position (off-screen or hidden)
                rectTransform.anchoredPosition = originalPosition;
                break;

            case TransitionType.ScaleAndFade:
                rectTransform.localScale = Vector3.zero;
                if (canvasGroup != null)
                    canvasGroup.alpha = 0f;
                break;
        }
    }

    private void ResetToVisible()
    {
        rectTransform.localScale = originalScale;
        
        // For SlideToCenter, visible position is centerPosition (Y=0)
        if (showTransition == TransitionType.SlideToCenter || hideTransition == TransitionType.SlideToCenter)
        {
            rectTransform.anchoredPosition = centerPosition;
        }
        else
        {
            rectTransform.anchoredPosition = originalPosition;
        }
        
        if (canvasGroup != null)
            canvasGroup.alpha = 1f;
    }

    private bool NeedsFade()
    {
        return showTransition == TransitionType.Fade || 
               showTransition == TransitionType.ScaleAndFade ||
               hideTransition == TransitionType.Fade || 
               hideTransition == TransitionType.ScaleAndFade;
    }

    public bool IsVisible => isVisible;

    // ====== EDITOR HELPERS ======

#if UNITY_EDITOR
    [ContextMenu("Test Show")]
    private void TestShow()
    {
        Show();
    }

    [ContextMenu("Test Hide")]
    private void TestHide()
    {
        Hide();
    }

    [ContextMenu("Test Toggle")]
    private void TestToggle()
    {
        Toggle();
    }
#endif
}
