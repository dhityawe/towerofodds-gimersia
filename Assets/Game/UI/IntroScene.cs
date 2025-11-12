using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles the intro scene animation sequence.
/// Flow: BG appears → Title fades in with glow → BG dissolves → Title dissolves → Next scene
/// </summary>
public class IntroScene : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image bgImage;
    [SerializeField] private TextMeshProUGUI titleText;

    [Header("Timing")]
    [SerializeField] private float delayBeforeTitleAppear = 1f;
    [SerializeField] private float titleFadeInDuration = 1.5f;
    [SerializeField] private float titleGlowDuration = 0.8f;
    [SerializeField] private float delayBeforeBgDissolve = 1f;
    [SerializeField] private float bgDissolveDuration = 1.5f;
    [SerializeField] private float delayBeforeTitleDissolve = 0.5f;
    [SerializeField] private float titleDissolveDuration = 1f;

    [Header("Title Glow Effect")]
    [SerializeField] private Color glowColor = new Color(1f, 0.9f, 0.5f, 1f); // Warm glow
    [SerializeField] private float glowIntensity = 2f; // Emission intensity
    [SerializeField] private Vector3 glowScale = new Vector3(1.05f, 1.05f, 1f);

    [Header("Scene Transition")]
    [SerializeField] private bool loadNextSceneAfterIntro = false;
    [SerializeField] private string nextSceneName = "MainGame";
    [SerializeField] private bool autoStartIntro = true;

    [Header("Skip Settings")]
    [SerializeField] private bool allowSkip = true;
    [SerializeField] private KeyCode skipKey = KeyCode.Space;

    private Color originalTitleColor;
    private Vector3 originalTitleScale;
    private Sequence introSequence;
    private bool isSkipped = false;

    void Awake()
    {
        // Validate references
        if (bgImage == null)
        {
            Debug.LogError("[IntroScene] BgImage is not assigned!");
            return;
        }

        if (titleText == null)
        {
            Debug.LogError("[IntroScene] TitleText is not assigned!");
            return;
        }

        // Store original values
        originalTitleColor = titleText.color;
        originalTitleScale = titleText.transform.localScale;

        // Setup initial states
        SetupInitialState();
    }

    void Start()
    {
        if (autoStartIntro)
        {
            PlayIntro();
        }
    }

    void Update()
    {
        // Allow skipping
        if (allowSkip && !isSkipped && Input.GetKeyDown(skipKey))
        {
            SkipIntro();
        }
    }

    private void SetupInitialState()
    {
        // BG: Full opacity
        Color bgColor = bgImage.color;
        bgColor.a = 1f;
        bgImage.color = bgColor;

        // Title: Hidden (alpha 0)
        Color titleColor = titleText.color;
        titleColor.a = 0f;
        titleText.color = titleColor;
    }

    /// <summary>
    /// Play the full intro sequence.
    /// </summary>
    public void PlayIntro()
    {
        // Kill any existing sequence
        introSequence?.Kill();

        // Create intro sequence
        introSequence = DOTween.Sequence();

        // 1. Start with BG visible (already set in SetupInitialState)

        // 2. Delay → Title appears with reverse dissolve + glow
        introSequence.AppendInterval(delayBeforeTitleAppear);
        introSequence.AppendCallback(() => AnimateTitleAppearWithGlow());

        // 3. Wait for title animation to complete
        introSequence.AppendInterval(titleFadeInDuration + titleGlowDuration);

        // 4. Delay → BG dissolves to alpha 0
        introSequence.AppendInterval(delayBeforeBgDissolve);
        introSequence.AppendCallback(() => AnimateBgDissolve());

        // 5. Wait for BG dissolve
        introSequence.AppendInterval(bgDissolveDuration);

        // 6. Delay → Title dissolves to alpha 0
        introSequence.AppendInterval(delayBeforeTitleDissolve);
        introSequence.AppendCallback(() => AnimateTitleDissolve());

        // 7. Wait for title dissolve, then load next scene (if enabled)
        introSequence.AppendInterval(titleDissolveDuration);
        introSequence.OnComplete(() => OnIntroComplete());

        Debug.Log("[IntroScene] Intro sequence started");
    }

    private void AnimateTitleAppearWithGlow()
    {
        // Fade in text from alpha 0 → 1 with glow effect
        Sequence titleSeq = DOTween.Sequence();

        // Calculate glow scale
        Vector3 targetGlowScale = new Vector3(
            originalTitleScale.x * glowScale.x,
            originalTitleScale.y * glowScale.y,
            originalTitleScale.z * glowScale.z
        );

        // Phase 1: Fade in with glow color and scale up
        titleSeq.Append(titleText.DOColor(glowColor, titleFadeInDuration * 0.5f));
        titleSeq.Join(titleText.DOFade(1f, titleFadeInDuration * 0.5f));
        titleSeq.Join(titleText.transform.DOScale(targetGlowScale, titleFadeInDuration * 0.5f));

        // Phase 2: Hold glow briefly
        titleSeq.AppendInterval(titleGlowDuration);

        // Phase 3: Fade to normal color, remove glow, scale back
        titleSeq.Append(titleText.DOColor(originalTitleColor, titleFadeInDuration * 0.5f));
        titleSeq.Join(titleText.transform.DOScale(originalTitleScale, titleFadeInDuration * 0.5f));

        Debug.Log("[IntroScene] Title appearing with glow effect");
    }

    private void AnimateBgDissolve()
    {
        // Fade BG from alpha 1 → 0
        bgImage.DOFade(0f, bgDissolveDuration)
            .SetEase(Ease.InQuad);

        Debug.Log("[IntroScene] BG dissolving");
    }

    private void AnimateTitleDissolve()
    {
        // Fade title from alpha 1 → 0
        titleText.DOFade(0f, titleDissolveDuration)
            .SetEase(Ease.InQuad);

        Debug.Log("[IntroScene] Title dissolving");
    }

    /// <summary>
    /// Skip the intro and immediately complete.
    /// </summary>
    public void SkipIntro()
    {
        if (isSkipped) return;

        isSkipped = true;
        introSequence?.Kill();

        Debug.Log("[IntroScene] Intro skipped");
        OnIntroComplete();
    }

    private void OnIntroComplete()
    {
        Debug.Log("[IntroScene] Intro sequence completed");

        // Disable BG GameObject after intro ends
        if (bgImage != null)
        {
            bgImage.gameObject.SetActive(false);
            Debug.Log("[IntroScene] BG GameObject disabled");
        }

        // Notify GameStateManager that intro is complete
        if (GameStateManager.Instance != null)
        {
            var introState = GameStateManager.Instance.CurrentState as IntroState;
            if (introState != null)
            {
                introState.OnIntroComplete();
            }
        }

        if (loadNextSceneAfterIntro)
        {
            LoadNextScene();
        }
        else
        {
            Debug.Log("[IntroScene] Staying in current scene (loadNextSceneAfterIntro is disabled)");
        }
    }

    private void LoadNextScene()
    {
        if (string.IsNullOrEmpty(nextSceneName))
        {
            Debug.LogWarning("[IntroScene] Next scene name is not set!");
            return;
        }

        Debug.Log($"[IntroScene] Loading scene: {nextSceneName}");
        SceneManager.LoadScene(nextSceneName);
    }

    void OnDestroy()
    {
        // Clean up tweens
        introSequence?.Kill();
        titleText?.DOKill();
        bgImage?.DOKill();
    }

#if UNITY_EDITOR
    [ContextMenu("Play Intro")]
    private void TestPlayIntro()
    {
        SetupInitialState();
        PlayIntro();
    }

    [ContextMenu("Skip Intro")]
    private void TestSkipIntro()
    {
        SkipIntro();
    }

    [ContextMenu("Reset State")]
    private void TestResetState()
    {
        introSequence?.Kill();
        SetupInitialState();
        isSkipped = false;
    }
#endif
}
