using UnityEngine;
using TMPro;
using DG.Tweening;

/// <summary>
/// UI controller for displaying player's current chips.
/// Automatically updates when chip amount changes.
/// Add TMPWiggleEffect component separately for Balatro-style wiggle.
/// </summary>
[RequireComponent(typeof(TextMeshProUGUI))]
public class ChipsDisplayUI : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private TextMeshProUGUI chipsText;

    [Header("Animation Settings")]
    [SerializeField] private bool animateOnChange = true;
    [SerializeField] private float punchScale = 1.2f;
    [SerializeField] private float punchDuration = 0.3f;
    [SerializeField] private Color gainColor = Color.green;
    [SerializeField] private Color spendColor = Color.red;
    [SerializeField] private float colorDuration = 0.5f;

    [Header("Format Settings")]
    [SerializeField] private string prefix = "";
    [SerializeField] private string suffix = "";
    [SerializeField] private bool useThousandsSeparator = true;

    private Color originalColor;
    private int displayedChips = 0;

    void Awake()
    {
        // Get TextMeshProUGUI component
        if (chipsText == null)
        {
            chipsText = GetComponent<TextMeshProUGUI>();
        }

        if (chipsText != null)
        {
            originalColor = chipsText.color;
        }
    }

    void Start()
    {
        // Initial update
        UpdateDisplay(PlayerDataManager.Instance.Chips);
    }

    void OnEnable()
    {
        // Subscribe to chip events
        if (PlayerDataManager.Instance != null)
        {
            PlayerDataManager.Instance.OnChipsChanged += OnChipsChanged;
            PlayerDataManager.Instance.OnChipsGained += OnChipsGained;
            PlayerDataManager.Instance.OnChipsSpent += OnChipsSpent;
            PlayerDataManager.Instance.OnGameReset += OnGameReset;
        }
    }

    void OnDisable()
    {
        // Unsubscribe from events
        if (PlayerDataManager.Instance != null)
        {
            PlayerDataManager.Instance.OnChipsChanged -= OnChipsChanged;
            PlayerDataManager.Instance.OnChipsGained -= OnChipsGained;
            PlayerDataManager.Instance.OnChipsSpent -= OnChipsSpent;
            PlayerDataManager.Instance.OnGameReset -= OnGameReset;
        }
    }

    private void OnChipsChanged(int newAmount)
    {
        UpdateDisplay(newAmount);
    }

    private void OnChipsGained(int amount)
    {
        if (animateOnChange)
        {
            AnimateChange(gainColor);
        }
    }

    private void OnChipsSpent(int amount)
    {
        if (animateOnChange)
        {
            AnimateChange(spendColor);
        }
    }

    private void OnGameReset()
    {
        // Reset to starting chips without animation
        UpdateDisplay(PlayerDataManager.Instance.Chips);
    }

    /// <summary>
    /// Update the text display with current chip amount.
    /// </summary>
    private void UpdateDisplay(int chipAmount)
    {
        displayedChips = chipAmount;

        if (chipsText != null)
        {
            string formattedChips;
            if (useThousandsSeparator)
            {
                formattedChips = chipAmount.ToString("N0"); // 1,000
            }
            else
            {
                formattedChips = chipAmount.ToString();
            }

            chipsText.text = $"{prefix}{formattedChips}{suffix}";
        }
    }

    /// <summary>
    /// Play animation when chips change.
    /// </summary>
    private void AnimateChange(Color flashColor)
    {
        if (chipsText == null) return;

        // Kill any existing animations
        chipsText.transform.DOKill();
        chipsText.DOKill();

        // Punch scale animation
        chipsText.transform.DOPunchScale(Vector3.one * (punchScale - 1f), punchDuration, 5, 0.5f);

        // Color flash animation
        Sequence colorSeq = DOTween.Sequence();
        colorSeq.Append(chipsText.DOColor(flashColor, colorDuration * 0.3f));
        colorSeq.Append(chipsText.DOColor(originalColor, colorDuration * 0.7f));
    }

    /// <summary>
    /// Manually refresh the display (useful for initialization).
    /// </summary>
    public void RefreshDisplay()
    {
        if (PlayerDataManager.Instance != null)
        {
            UpdateDisplay(PlayerDataManager.Instance.Chips);
        }
    }

    /// <summary>
    /// Set custom format prefix and suffix.
    /// </summary>
    public void SetFormat(string newPrefix, string newSuffix)
    {
        prefix = newPrefix;
        suffix = newSuffix;
        RefreshDisplay();
    }

#if UNITY_EDITOR
    [ContextMenu("Test Gain Animation")]
    private void TestGainAnimation()
    {
        AnimateChange(gainColor);
    }

    [ContextMenu("Test Spend Animation")]
    private void TestSpendAnimation()
    {
        AnimateChange(spendColor);
    }

    [ContextMenu("Refresh Display")]
    private void EditorRefreshDisplay()
    {
        RefreshDisplay();
    }
#endif
}
