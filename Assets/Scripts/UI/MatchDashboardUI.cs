using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MatchDashboardUI : MonoBehaviour
{
    [Header("Skorboard")]
    [SerializeField] private TextMeshProUGUI homeTeamNameText;
    [SerializeField] private TextMeshProUGUI awayTeamNameText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI minuteText;
    [SerializeField] private TextMeshProUGUI ratingText;

    [Header("Mini Harita & Possession")]
    [SerializeField] private RectTransform miniMapRect;
    [SerializeField] private RectTransform ballIcon;
    [SerializeField] private TextMeshProUGUI possessionText;

    [Header("Enerji & Moral")]
    [SerializeField] private Image energyFillImage;
    [SerializeField] private Image moraleFillImage;

    [Header("Spiker")]
    [SerializeField] private TextMeshProUGUI commentaryText;

    [Header("Alt Butonlar")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button objectivesButton;
    [SerializeField] private Button lineupButton;
    [SerializeField] private Button speedButton;
    [SerializeField] private Button simulateButton;
    [SerializeField] private TextMeshProUGUI speedButtonLabel;

    public Action OnStartClicked;
    public Action OnObjectivesClicked;
    public Action OnLineupClicked;
    public Action<bool> OnSpeedToggled;
    public Action OnSimulateClicked;

    bool _isFastSpeed;

    void Awake()
    {
        if (startButton) startButton.onClick.AddListener(() => OnStartClicked?.Invoke());
        if (objectivesButton) objectivesButton.onClick.AddListener(() => OnObjectivesClicked?.Invoke());
        if (lineupButton) lineupButton.onClick.AddListener(() => OnLineupClicked?.Invoke());
        if (simulateButton) simulateButton.onClick.AddListener(() => OnSimulateClicked?.Invoke());
        if (speedButton) speedButton.onClick.AddListener(ToggleSpeed);
        UpdateSpeedLabel();
    }

    void ToggleSpeed()
    {
        _isFastSpeed = !_isFastSpeed;
        UpdateSpeedLabel();
        OnSpeedToggled?.Invoke(_isFastSpeed);
    }

    void UpdateSpeedLabel()
    {
        if (speedButtonLabel)
            speedButtonLabel.text = _isFastSpeed ? "x2" : "x1";
    }

    // ------- Public UI update metodlarý ---------

    public void SetTeams(string home, string away)
    {
        if (homeTeamNameText) homeTeamNameText.text = home;
        if (awayTeamNameText) awayTeamNameText.text = away;
    }

    public void SetScore(int home, int away)
    {
        if (scoreText) scoreText.text = $"{home} - {away}";
    }

    public void SetMinute(float minute)
    {
        if (minuteText) minuteText.text = $"{Mathf.FloorToInt(minute)}'";
    }

    public void SetPlayerRating(float rating)
    {
        if (ratingText) ratingText.text = $"Reyting: {rating:0.0}";
    }

    public void SetPossession(float homePercent, float awayPercent)
    {
        if (possessionText)
            possessionText.text = $"Topla Oynama: %{Mathf.RoundToInt(homePercent)} - %{Mathf.RoundToInt(awayPercent)}";
    }

    public void SetEnergy(float normalized01)
    {
        if (energyFillImage) energyFillImage.fillAmount = Mathf.Clamp01(normalized01);
    }

    public void SetMorale(float normalized01)
    {
        if (moraleFillImage) moraleFillImage.fillAmount = Mathf.Clamp01(normalized01);
    }

    public void SetCommentary(string text)
    {
        if (commentaryText) commentaryText.text = text;
    }

    /// <summary>Mini haritada topun konumunu -1..1 arasý normalize koordinat ile günceller.</summary>
    public void SetBallOnMiniMap(Vector2 normalizedPos)
    {
        if (!miniMapRect || !ballIcon) return;

        normalizedPos = Vector2.ClampMagnitude(normalizedPos, 1f);

        var halfSize = miniMapRect.rect.size * 0.5f;
        var localPos = new Vector2(
            normalizedPos.x * halfSize.x,
            normalizedPos.y * halfSize.y
        );

        ballIcon.anchoredPosition = localPos;
    }
}
