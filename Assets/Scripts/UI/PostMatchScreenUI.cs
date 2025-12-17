using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Maç sonu ekranı - Rating, ödüller, röportaj
/// </summary>
public class PostMatchScreenUI : MonoBehaviour
{
    [Header("UI Referansları")]
    [SerializeField] private TextMeshProUGUI matchResultText;
    [SerializeField] private TextMeshProUGUI playerRatingText;
    [SerializeField] private TextMeshProUGUI bonusEarnedText;
    [SerializeField] private Image ratingColorIndicator;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button interviewButton;
    
    [Header("Interview Panel")]
    [SerializeField] private GameObject interviewPanel;
    [SerializeField] private TextMeshProUGUI interviewQuestionText;
    [SerializeField] private Button goodAnswerButton;
    [SerializeField] private Button badAnswerButton;
    [SerializeField] private Button neutralAnswerButton;
    [SerializeField] private Button closeInterviewButton;

    private float matchRating;
    private bool interviewCompleted = false;

    private void Start()
    {
        SetupButtons();
        LoadMatchResults();
        
        if (interviewPanel != null)
            interviewPanel.SetActive(false);
    }

    private void SetupButtons()
    {
        if (continueButton != null)
        {
            continueButton.onClick.AddListener(OnContinueButton);
        }
        
        if (interviewButton != null)
        {
            interviewButton.onClick.AddListener(OnInterviewButton);
            // Rating yüksekse röportaj butonu aktif
            interviewButton.interactable = matchRating >= 7.0f;
        }
        
        if (goodAnswerButton != null)
            goodAnswerButton.onClick.AddListener(() => OnAnswerInterview(InterviewAnswer.Good));
        
        if (badAnswerButton != null)
            badAnswerButton.onClick.AddListener(() => OnAnswerInterview(InterviewAnswer.Bad));
        
        if (neutralAnswerButton != null)
            neutralAnswerButton.onClick.AddListener(() => OnAnswerInterview(InterviewAnswer.Neutral));
        
        if (closeInterviewButton != null)
            closeInterviewButton.onClick.AddListener(OnCloseInterview);
    }

    /// <summary>
    /// Maç sonuçlarını yükle
    /// </summary>
    private void LoadMatchResults()
    {
        if (GameManager.Instance == null || GameManager.Instance.CurrentSave == null)
        {
            Debug.LogError("GameManager veya SaveData bulunamadı!");
            return;
        }
        
        SaveData saveData = GameManager.Instance.CurrentSave;
        
        // Maç sonuçlarını al (MatchContext'ten veya başka bir yerden)
        // Şimdilik örnek değerler
        int homeScore = 2;
        int awayScore = 1;
        matchRating = 7.5f; // Örnek rating
        
        if (matchResultText != null)
        {
            string homeTeam = saveData.clubData.clubName;
            matchResultText.text = $"{homeTeam} {homeScore} - {awayScore} Rakip";
        }
        
        // Rating göster
        DisplayRating(matchRating);
        
        // Bonus hesapla
        CalculateBonuses(saveData);
    }

    /// <summary>
    /// Rating'i göster
    /// </summary>
    private void DisplayRating(float rating)
    {
        if (playerRatingText != null)
            playerRatingText.text = $"Rating: {rating:F1}";
        
        if (ratingColorIndicator != null)
        {
            // Rating'e göre renk
            if (rating >= 8.0f)
                ratingColorIndicator.color = Color.green;
            else if (rating >= 6.0f)
                ratingColorIndicator.color = Color.yellow;
            else
                ratingColorIndicator.color = Color.red;
        }
    }

    /// <summary>
    /// Bonusları hesapla
    /// </summary>
    private void CalculateBonuses(SaveData saveData)
    {
        int totalBonus = 0;
        var contract = saveData.clubData.contract;
        var stats = saveData.playerProfile.seasonStats;
        
        // Gol bonusu
        int goalBonus = stats.goals * contract.goalBonus;
        
        // Asist bonusu
        int assistBonus = stats.assists * contract.assistBonus;
        
        // Galibiyet bonusu (maç kazanıldıysa)
        int winBonus = 0;
        // Bu bilgiyi maç sonucundan almak gerekir
        
        totalBonus = goalBonus + assistBonus + winBonus;
        
        if (bonusEarnedText != null)
        {
            if (totalBonus > 0)
                bonusEarnedText.text = $"Kazanılan Bonus: {totalBonus:N0} €";
            else
                bonusEarnedText.text = "Bonus kazanılmadı.";
        }
        
        // Paraya ekle
        saveData.economyData.money += totalBonus;
    }

    /// <summary>
    /// Röportaj butonu
    /// </summary>
    private void OnInterviewButton()
    {
        if (interviewPanel != null)
        {
            interviewPanel.SetActive(true);
            ShowInterviewQuestion();
        }
    }

    /// <summary>
    /// Röportaj sorusu göster
    /// </summary>
    private void ShowInterviewQuestion()
    {
        string[] questions = {
            "Maçtaki performansınız hakkında ne düşünüyorsunuz?",
            "Takımın genel performansı hakkında ne söyleyebilirsiniz?",
            "Hedefleriniz nelerdir?",
            "Teknik direktörle ilişkiniz nasıl?",
            "Transfer söylentileri hakkında ne düşünüyorsunuz?"
        };
        
        if (interviewQuestionText != null)
        {
            interviewQuestionText.text = questions[Random.Range(0, questions.Length)];
        }
    }

    /// <summary>
    /// Röportaj cevabı
    /// </summary>
    private void OnAnswerInterview(InterviewAnswer answer)
    {
        if (GameManager.Instance == null || GameManager.Instance.CurrentSave == null)
            return;
        
        SaveData saveData = GameManager.Instance.CurrentSave;
        
        // Cevaba göre etkiler
        switch (answer)
        {
            case InterviewAnswer.Good:
                // Popülerlik ve ilişkiler artar
                saveData.mediaData.popularity = Mathf.Clamp01(saveData.mediaData.popularity + 0.05f);
                saveData.relationsData.coachRelationship = Mathf.Clamp01(saveData.relationsData.coachRelationship + 0.1f);
                break;
            case InterviewAnswer.Bad:
                // Popülerlik ve ilişkiler düşer
                saveData.mediaData.popularity = Mathf.Clamp01(saveData.mediaData.popularity - 0.05f);
                saveData.relationsData.coachRelationship = Mathf.Clamp01(saveData.relationsData.coachRelationship - 0.1f);
                break;
            case InterviewAnswer.Neutral:
                // Değişiklik yok
                break;
        }
        
        interviewCompleted = true;
        OnCloseInterview();
    }

    private void OnCloseInterview()
    {
        if (interviewPanel != null)
            interviewPanel.SetActive(false);
    }

    /// <summary>
    /// Devam et butonu
    /// </summary>
    private void OnContinueButton()
    {
        // Maç sonu işlemleri
        if (GameManager.Instance != null && GameManager.Instance.CurrentSave != null)
        {
            SaveData saveData = GameManager.Instance.CurrentSave;
            
            // Rating'i kaydet
            saveData.playerProfile.seasonStats.AddMatchRating(matchRating);
            
            // Form/Moral güncelle
            if (FormMoralEnergySystem.Instance != null)
            {
                FormMoralEnergySystem.Instance.UpdateForm(saveData.playerProfile, matchRating);
                FormMoralEnergySystem.Instance.ConsumeEnergyAfterMatch(saveData.playerProfile, 90);
            }
            
            // Haber oluştur
            if (NewsSystem.Instance != null)
            {
                NewsSystem.Instance.CreateMatchNews(saveData.playerProfile, matchRating, "Rakip", true);
            }
            
            // Sosyal medya post
            if (SocialMediaSystem.Instance != null)
            {
                SocialMediaSystem.Instance.AutoPostAfterMatch(saveData.playerProfile, matchRating);
            }
            
            // Auto-save
            SaveSystem.AutoSave();
        }
        
        // Kariyer menüsüne dön
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.ChangeState(GameState.CareerHub);
        }
    }
}

public enum InterviewAnswer
{
    Good,
    Bad,
    Neutral
}


