using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum MatchUIViewState
{
    Normal,     // Maç takibi – FieldPanel açık
    Chance      // Pozisyon geldiğinde – yakın HUD
}

public class MatchSceneUI : MonoBehaviour
{
    [Header("Genel Ayarlar")]
    [SerializeField] private MatchUIViewState startState = MatchUIViewState.Normal;

    [Header("Scoreboard")]
    [SerializeField] private TextMeshProUGUI homeTeamNameText;
    [SerializeField] private TextMeshProUGUI awayTeamNameText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI timeText;

    [Header("İstatistikler")]
    [SerializeField] private TextMeshProUGUI possessionText; // "Topla Oynama: %60 - %40"

    [Header("Normal Görünüm (Maç Takibi)")]
    [SerializeField] private GameObject normalRoot;          // FieldPanel + BottomPanel parent
    [SerializeField] private RectTransform fieldPanel;       // FieldPanel'in RectTransform'u

    [Header("Pozisyon Görünümü (Chance HUD)")]
    [SerializeField] private GameObject chanceRoot;          // Pozisyon HUD'ın tamamı
    [SerializeField] private TextMeshProUGUI ratingText;
    [SerializeField] private TextMeshProUGUI energyText;
    [SerializeField] private TextMeshProUGUI moraleText;

    [Header("Alt Kontroller (Pozisyon Ekranı)")]
    [SerializeField] private Button skipButton;
    [SerializeField] private Button objectivesButton;
    [SerializeField] private Button speedButton;
    [SerializeField] private TextMeshProUGUI speedButtonText;

    [Header("Maç Başlat")]
    [SerializeField] private Button startButton;             // Üstteki "Maçı Başlat" butonu

    // DIŞARI AÇTIĞIMIZ EVENT'LER
    public Action OnStartPressed;
    public Action OnSkipPressed;
    public Action OnObjectivesPressed;
    public Action<bool> OnSpeedToggled; // true = x2, false = x1

    MatchUIViewState _state;
    bool _isFastSpeed;

    void Awake()
    {
        if (startButton != null)
            startButton.onClick.AddListener(HandleStart);

        if (skipButton != null)
            skipButton.onClick.AddListener(HandleSkip);

        if (objectivesButton != null)
            objectivesButton.onClick.AddListener(HandleObjectives);

        if (speedButton != null)
            speedButton.onClick.AddListener(HandleSpeedToggle);
    }

    void Start()
    {
        SetState(startState);
        UpdateSpeedLabel();
    }

    // ─────────────────────────────────────────────
    //  STATE
    // ─────────────────────────────────────────────

    public void SetState(MatchUIViewState newState)
    {
        _state = newState;

        if (normalRoot != null)
            normalRoot.SetActive(_state == MatchUIViewState.Normal);

        if (chanceRoot != null)
            chanceRoot.SetActive(_state == MatchUIViewState.Chance);
    }

    public void EnterChanceView() => SetState(MatchUIViewState.Chance);
    public void ReturnToNormalView() => SetState(MatchUIViewState.Normal);

    // ─────────────────────────────────────────────
    //  SCORE / TIME / İSTATİSTİK
    // ─────────────────────────────────────────────

    public void SetTeams(string homeName, string awayName)
    {
        if (homeTeamNameText) homeTeamNameText.text = homeName;
        if (awayTeamNameText) awayTeamNameText.text = awayName;
    }

    public void SetScore(int home, int away)
    {
        if (scoreText) scoreText.text = $"{home} - {away}";
    }

    public void SetTimeText(string t)
    {
        if (timeText) timeText.text = t; // "23'"
    }

    public void SetPossession(float homePercent, float awayPercent)
    {
        if (possessionText)
            possessionText.text = $"Topla Oynama: %{Mathf.RoundToInt(homePercent)} - %{Mathf.RoundToInt(awayPercent)}";
    }

    public void SetPlayerStats(float rating, float energy, float morale)
    {
        if (ratingText) ratingText.text = $"Reyting: {rating:0.0}";
        if (energyText) energyText.text = $"Enerji: {energy:0}%";
        if (moraleText) moraleText.text = $"Moral: {morale:0}%";
    }

    // ─────────────────────────────────────────────
    //  BUTON HANDLER’LARI
    // ─────────────────────────────────────────────

    void HandleStart()
    {
        OnStartPressed?.Invoke();
    }

    void HandleSkip()
    {
        OnSkipPressed?.Invoke();
    }

    void HandleObjectives()
    {
        OnObjectivesPressed?.Invoke();
    }

    void HandleSpeedToggle()
    {
        _isFastSpeed = !_isFastSpeed;
        UpdateSpeedLabel();
        OnSpeedToggled?.Invoke(_isFastSpeed);
    }

    void UpdateSpeedLabel()
    {
        if (speedButtonText == null) return;
        speedButtonText.text = _isFastSpeed ? "x2" : "x1";
    }
}
