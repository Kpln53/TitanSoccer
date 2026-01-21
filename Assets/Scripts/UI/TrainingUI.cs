using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class TrainingUI : MonoBehaviour
{
    [Header("UI References")]
    public Transform cardsContainer;
    public GameObject resultPanel; // Popup
    
    [Header("Result Popup References")]
    public TextMeshProUGUI resultTitle;
    public TextMeshProUGUI resultStatChange;
    public TextMeshProUGUI resultEnergyChange;
    public TextMeshProUGUI resultNewEnergy;
    public TextMeshProUGUI resultNewMoral;
    public Button resultCloseButton;

    [Header("Limit Info")]
    public TextMeshProUGUI limitText;

    // Tanımlı Antrenmanlar
    private List<TrainingSession> availableSessions = new List<TrainingSession>();

    private void Start()
    {
        InitializeSessions();
        SetupUI();
        UpdateLimitText();
        
        if (resultPanel) resultPanel.SetActive(false);
        if (resultCloseButton) resultCloseButton.onClick.AddListener(CloseResultPanel);
    }

    private void InitializeSessions()
    {
        availableSessions.Clear();

        // 1. Hücum
        availableSessions.Add(new TrainingSession
        {
            id = "attack_1",
            title = "HÜCUM & BİTİRİCİLİK",
            type = TrainingType.Attack,
            energyCost = 15,
            statName = "Şut",
            statIncrease = 2,
            description = "Enerji: -15 | Şut: +2"
        });

        // 2. Fiziksel
        availableSessions.Add(new TrainingSession
        {
            id = "phys_1",
            title = "FİZİKSEL KONDİSYON",
            type = TrainingType.Physical,
            energyCost = 20,
            statName = "Güç",
            statIncrease = 3,
            description = "Enerji: -20 | Güç: +3"
        });

        // 3. Teknik
        availableSessions.Add(new TrainingSession
        {
            id = "tech_1",
            title = "TEKNİK & PAS",
            type = TrainingType.Technique,
            energyCost = 10,
            statName = "Pas",
            statIncrease = 1,
            description = "Enerji: -10 | Pas: +1"
        });
    }

    private void SetupUI()
    {
        // Kartları bul ve butonlarını bağla (Builder ile oluşturulan yapıyı kullanacağız)
        // Kartların isimleri: Card_0, Card_1, Card_2
        
        for (int i = 0; i < availableSessions.Count; i++)
        {
            if (i >= cardsContainer.childCount) break;

            Transform card = cardsContainer.GetChild(i);
            TrainingSession session = availableSessions[i];

            // Başlık
            var titleObj = card.Find("Content/Title");
            if (titleObj) titleObj.GetComponent<TextMeshProUGUI>().text = session.title;

            // Açıklama (Cost/Reward)
            var descObj = card.Find("Content/Description");
            if (descObj) descObj.GetComponent<TextMeshProUGUI>().text = session.description;

            // Buton
            var btnObj = card.Find("StartButton");
            if (btnObj)
            {
                Button btn = btnObj.GetComponent<Button>();
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => OnStartTraining(session));
            }
        }
    }

    private void OnStartTraining(TrainingSession session)
    {
        // Sistem yoksa oluştur (Test için)
        if (TrainingSystem.Instance == null)
        {
            GameObject sys = new GameObject("TrainingSystem");
            sys.AddComponent<TrainingSystem>();
        }

        string msg;
        bool success = TrainingSystem.Instance.ExecuteTraining(session, out msg);

        if (success)
        {
            ShowResult(session);
            UpdateLimitText();
            
            // Top paneli güncelle
            FindObjectOfType<TopPanelUI>()?.RefreshData();
        }
        else
        {
            Debug.LogWarning(msg);
            // Hata mesajı popup'ı eklenebilir
        }
    }

    private void ShowResult(TrainingSession session)
    {
        if (!resultPanel) return;

        resultTitle.text = $"ANTRENMAN SONUCU: {session.title}";
        resultStatChange.text = $"{session.statName}: +{session.statIncrease}";
        resultEnergyChange.text = $"Enerji: -{session.energyCost}";

        if (GameManager.Instance != null && GameManager.Instance.HasCurrentSave())
        {
            var data = GameManager.Instance.CurrentSave.seasonData;
            resultNewEnergy.text = $"Yeni Enerji: %{Mathf.RoundToInt(data.energy)}";
            resultNewMoral.text = $"Yeni Moral: %{Mathf.RoundToInt(data.moral)}";
        }

        resultPanel.SetActive(true);
    }

    private void CloseResultPanel()
    {
        if (resultPanel) resultPanel.SetActive(false);
    }

    private void UpdateLimitText()
    {
        if (limitText && TrainingSystem.Instance != null)
        {
            int left = TrainingSystem.MAX_DAILY_TRAINING - TrainingSystem.Instance.currentTrainingCount;
            limitText.text = $"Kalan Hak: {left}";
        }
    }
}
