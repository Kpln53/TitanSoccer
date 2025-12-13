using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MatchChanceUI : MonoBehaviour
{
    [Header("Pozisyon Bilgisi")]
    [SerializeField] private TextMeshProUGUI minuteText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI possessionText;
    [SerializeField] private TextMeshProUGUI situationDescriptionText;

    [Header("Oyuncu Durumu")]
    [SerializeField] private TextMeshProUGUI energyText;
    [SerializeField] private TextMeshProUGUI moraleText;
    [SerializeField] private TextMeshProUGUI ratingText;

    [Header("Seçenekler")]
    [SerializeField] private Button shootButton;
    [SerializeField] private Button passButton;
    [SerializeField] private Button dribbleButton;
    [SerializeField] private Button crossButton;

    [Header("Sonuç Gösterimi")]
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private Button continueButton;

    private MatchContext _context;
    private bool _hasMadeChoice = false;

    void Start()
    {
        _context = MatchContext.I;

        if (_context == null)
        {
            Debug.LogError("MatchContext bulunamadı!");
            return;
        }

        // UI'ı güncelle
        UpdateUI();

        // Buton eventlerini bağla
        if (shootButton) shootButton.onClick.AddListener(() => HandleChoice(ChanceAction.Shoot));
        if (passButton) passButton.onClick.AddListener(() => HandleChoice(ChanceAction.Pass));
        if (dribbleButton) dribbleButton.onClick.AddListener(() => HandleChoice(ChanceAction.Dribble));
        if (crossButton) crossButton.onClick.AddListener(() => HandleChoice(ChanceAction.Cross));

        if (continueButton) continueButton.onClick.AddListener(OnContinueClicked);

        if (resultPanel) resultPanel.SetActive(false);
    }

    void UpdateUI()
    {
        if (_context == null) return;

        // Dakika
        if (minuteText)
            minuteText.text = $"{Mathf.FloorToInt(_context.minute)}'";

        // Skor
        if (scoreText)
            scoreText.text = $"{_context.homeScore} - {_context.awayScore}";

        // Topla oynama
        if (possessionText)
            possessionText.text = $"Topla Oynama: %{Mathf.RoundToInt(_context.homePossession)}";

        // Enerji ve Moral
        if (energyText)
            energyText.text = $"Enerji: {_context.energy01 * 100f:F0}%";
        if (moraleText)
            moraleText.text = $"Moral: {_context.morale01 * 100f:F0}%";
        if (ratingText)
            ratingText.text = $"Reyting: {_context.rating:F1}";

        // Pozisyon açıklaması
        if (situationDescriptionText)
            situationDescriptionText.text = "Pozisyon geldi! Ne yapmak istersin?";
    }

    void HandleChoice(ChanceAction action)
    {
        if (_hasMadeChoice || _context == null) return;

        _hasMadeChoice = true;

        // Seçenek butonlarını devre dışı bırak
        SetButtonsInteractable(false);

        // Sonucu hesapla
        ChanceOutcome outcome = CalculateOutcome(action);

        // Sonucu göster
        ShowResult(outcome, action);

        // MatchContext'e sonucu kaydet
        _context.SetOutcome(outcome);
    }

    ChanceOutcome CalculateOutcome(ChanceAction action)
    {
        // Temel başarı şansı (enerji, moral, rating'e göre)
        float baseSuccess = (_context.energy01 * 0.3f + _context.morale01 * 0.3f + (_context.rating / 10f) * 0.4f);
        baseSuccess = Mathf.Clamp01(baseSuccess);

        // Pozisyon tipine göre farklı başarı şansları
        float successChance = baseSuccess;
        
        switch (action)
        {
            case ChanceAction.Shoot:
                // Şut için biraz daha zor
                successChance = baseSuccess * 0.7f;
                if (Random.value < successChance)
                {
                    // Gol mü yoksa kaçtı mı?
                    if (Random.value < 0.4f) // %40 gol şansı
                        return ChanceOutcome.Goal;
                    else
                        return ChanceOutcome.ShotMiss;
                }
                else
                {
                    return ChanceOutcome.ShotMiss;
                }

            case ChanceAction.Pass:
                // Pas için daha kolay
                successChance = baseSuccess * 1.2f;
                if (Random.value < successChance)
                {
                    // Asist mi yoksa sadece başarılı pas?
                    if (Random.value < 0.3f) // %30 asist şansı
                        return ChanceOutcome.Assist;
                    else
                        return ChanceOutcome.None; // Başarılı pas ama asist değil
                }
                else
                {
                    return ChanceOutcome.Turnover;
                }

            case ChanceAction.Dribble:
                // Dribling için orta zorluk
                successChance = baseSuccess * 0.9f;
                if (Random.value < successChance)
                {
                    // Başarılı dribling sonrası şans
                    if (Random.value < 0.25f)
                        return ChanceOutcome.Goal;
                    else
                        return ChanceOutcome.None;
                }
                else
                {
                    return ChanceOutcome.Turnover;
                }

            case ChanceAction.Cross:
                // Orta için orta zorluk
                successChance = baseSuccess * 0.85f;
                if (Random.value < successChance)
                {
                    if (Random.value < 0.2f)
                        return ChanceOutcome.Assist;
                    else
                        return ChanceOutcome.None;
                }
                else
                {
                    return ChanceOutcome.Turnover;
                }
        }

        return ChanceOutcome.None;
    }

    void ShowResult(ChanceOutcome outcome, ChanceAction action)
    {
        if (resultPanel == null || resultText == null) return;

        resultPanel.SetActive(true);

        string resultMessage = "";
        switch (outcome)
        {
            case ChanceOutcome.Goal:
                resultMessage = "GOOOOL! Harika bir vuruş!";
                break;
            case ChanceOutcome.Assist:
                resultMessage = "Mükemmel bir pas/asist!";
                break;
            case ChanceOutcome.ShotMiss:
                resultMessage = "Şut kaçtı...";
                break;
            case ChanceOutcome.Turnover:
                resultMessage = "Top kaybedildi...";
                break;
            case ChanceOutcome.None:
                resultMessage = "Pozisyon devam ediyor...";
                break;
        }

        resultText.text = resultMessage;
    }

    void SetButtonsInteractable(bool interactable)
    {
        if (shootButton) shootButton.interactable = interactable;
        if (passButton) passButton.interactable = interactable;
        if (dribbleButton) dribbleButton.interactable = interactable;
        if (crossButton) crossButton.interactable = interactable;
    }

    void OnContinueClicked()
    {
        // MatchScene'e geri dön
        SceneManager.LoadScene("MatchScene");
    }
}

public enum ChanceAction
{
    Shoot,
    Pass,
    Dribble,
    Cross
}

