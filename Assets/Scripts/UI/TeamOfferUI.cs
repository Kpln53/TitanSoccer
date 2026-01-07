using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Takım teklifi UI - Yeni oyuncuya takım teklifleri gösterir
/// </summary>
public class TeamOfferUI : MonoBehaviour
{
    [Header("Teklif Listesi")]
    public Transform offersListParent; // OffersScrollView > Viewport > Content (içinde OfferCard'lar var)
    public Button detailButtonTemplate; // Her OfferCard içindeki Detaylar butonu (referans için - ButtonRow içinde)
    
    [Header("Teklif Kartları (Inspector'da bağlanabilir veya otomatik bulunur)")]
    public List<GameObject> offerCardObjects = new List<GameObject>(); // Sahnedeki OfferCard'lar (opsiyonel - otomatik bulunur)

    [Header("Durum Metni")]
    public TextMeshProUGUI statusText;

    [Header("Detay Paneli")]
    public GameObject detailPanel;
    public TextMeshProUGUI detailTeamNameText;     // DetailTeamName (sahne: DetailTeamName)
    public TextMeshProUGUI detailSalaryText;       // DetailSalary (sahne: DetailSalary)
    public TextMeshProUGUI detailDurationText;     // DetailDuration (sahne: DetailDuration)
    public TextMeshProUGUI detailRoleText;         // DetailRole (sahne: DetailRole)
    public TextMeshProUGUI detailBonusText;        // DetailBonus (sahne: DetailBonus) - Bonuslar için
    public Button detailAcceptButton;              // İMZALA butonu
    public Button detailCloseButton;               // Geri butonu

    [Header("Geri Butonu")]
    public Button backButton;

    private List<TransferOffer> currentOffers = new List<TransferOffer>();
    private TransferOffer selectedOffer;

    private void Start()
    {
        Debug.Log("[TeamOfferUI] Start() called");
        SetupButtons();
        LoadOffers();
        if (detailPanel != null)
            detailPanel.SetActive(false);
    }

    private void SetupButtons()
    {
        if (detailAcceptButton != null)
            detailAcceptButton.onClick.AddListener(OnAcceptButton);

        if (detailCloseButton != null)
            detailCloseButton.onClick.AddListener(OnCloseDetailButton);

        if (backButton != null)
            backButton.onClick.AddListener(OnBackButton);
    }

    /// <summary>
    /// Teklifleri yükle ve göster
    /// </summary>
    private void LoadOffers()
    {
        Debug.Log("[TeamOfferUI] LoadOffers called");
        
        if (GameManager.Instance == null || !GameManager.Instance.HasCurrentSave())
        {
            Debug.LogWarning("[TeamOfferUI] No current save! Cannot load offers.");
            if (statusText != null)
                statusText.text = "Kayıt bulunamadı!";
            return;
        }

        PlayerProfile player = GameManager.Instance.CurrentSave.playerProfile;
        if (player == null)
        {
            Debug.LogWarning("[TeamOfferUI] No player profile! Cannot load offers.");
            if (statusText != null)
                statusText.text = "Oyuncu profili bulunamadı!";
            return;
        }
        
        Debug.Log($"[TeamOfferUI] Player profile found: {player.playerName}");

        // Durum metnini güncelle
        if (statusText != null)
            statusText.text = "Teklifler yükleniyor...";

        // Teklifleri oluştur
        GenerateOffers(player);

        // Teklifleri göster
        DisplayOffers();

        // Durum metnini güncelle
        if (statusText != null)
        {
            if (currentOffers.Count > 0)
                statusText.text = $"{currentOffers.Count} teklif bulundu";
            else
                statusText.text = "Henüz teklif yok";
        }
    }

    /// <summary>
    /// Teklifleri oluştur - Seçilen ligdeki düşük güçlü takımlardan 3 takım seç
    /// </summary>
    private void GenerateOffers(PlayerProfile player)
    {
        currentOffers.Clear();

        Debug.Log("[TeamOfferUI] GenerateOffers called");
        Debug.Log($"[TeamOfferUI] TransferAISystem.Instance: {(TransferAISystem.Instance != null ? "EXISTS" : "NULL")}");
        Debug.Log($"[TeamOfferUI] DataPackManager.Instance: {(DataPackManager.Instance != null ? "EXISTS" : "NULL")}");
        Debug.Log($"[TeamOfferUI] activeDataPack: {(DataPackManager.Instance != null && DataPackManager.Instance.activeDataPack != null ? "EXISTS" : "NULL")}");

        // TransferAISystem yoksa oluştur
        if (TransferAISystem.Instance == null)
        {
            GameObject transferAIObj = new GameObject("TransferAISystem");
            transferAIObj.AddComponent<TransferAISystem>();
            Debug.Log("[TeamOfferUI] TransferAISystem created automatically.");
        }

        if (TransferAISystem.Instance != null && DataPackManager.Instance != null && DataPackManager.Instance.activeDataPack != null)
        {
            // Seçilen ligi bul (SaveData'dan)
            string selectedLeagueName = null;
            if (GameManager.Instance != null && GameManager.Instance.HasCurrentSave() && 
                GameManager.Instance.CurrentSave.clubData != null)
            {
                selectedLeagueName = GameManager.Instance.CurrentSave.clubData.leagueName;
            }

            LeagueData selectedLeague = null;
            List<TeamData> leagueTeams = new List<TeamData>();

            // Eğer lig seçilmişse, o ligdeki takımları al
            if (!string.IsNullOrEmpty(selectedLeagueName))
            {
                selectedLeague = DataPackManager.Instance.GetLeague(selectedLeagueName);
                if (selectedLeague != null && selectedLeague.teams != null && selectedLeague.teams.Count > 0)
                {
                    leagueTeams = selectedLeague.teams;
                    Debug.Log($"[TeamOfferUI] Selected league: {selectedLeagueName}, found {leagueTeams.Count} teams.");
                }
                else
                {
                    Debug.LogWarning($"[TeamOfferUI] League '{selectedLeagueName}' not found or has no teams!");
                }
            }

            // Eğer lig bulunamadıysa veya seçilmemişse, tüm liglerden düşük güçlü takımları seç
            if (leagueTeams.Count == 0)
            {
                List<LeagueData> allLeagues = DataPackManager.Instance.GetAllLeagues();
                if (allLeagues != null && allLeagues.Count > 0)
                {
                    // İlk ligi kullan (varsayılan)
                    selectedLeague = allLeagues[0];
                    if (selectedLeague != null && selectedLeague.teams != null)
                    {
                        leagueTeams = selectedLeague.teams;
                        Debug.Log($"[TeamOfferUI] No league selected, using default league: {selectedLeague.leagueName}, found {leagueTeams.Count} teams.");
                    }
                }
            }

            if (leagueTeams != null && leagueTeams.Count > 0)
            {
                // Önce tüm takımların güçlerini hesapla (güçleri doğru almak için)
                foreach (var team in leagueTeams)
                {
                    if (team != null)
                    {
                        team.CalculateTeamPower();
                    }
                }

                // Takımları güçlerine göre sırala (düşükten yükseğe)
                List<TeamData> sortedTeams = leagueTeams
                    .Where(t => t != null) // Null kontrolü
                    .OrderBy(t => t.GetTeamPower()) // Düşük güçlüler önce
                    .ToList();

                // En düşük güçlü 3 takımı seç
                int offerCount = Mathf.Min(3, sortedTeams.Count);
                List<TeamData> selectedTeams = sortedTeams.Take(offerCount).ToList();

                Debug.Log($"[TeamOfferUI] Selected {selectedTeams.Count} lowest power teams from league '{selectedLeague?.leagueName ?? "Unknown"}'");
                
                // Debug: Tüm takımların güçlerini logla
                foreach (var team in sortedTeams)
                {
                    Debug.Log($"[TeamOfferUI] Team: {team.teamName}, Power: {team.GetTeamPower()}");
                }

                foreach (var team in selectedTeams)
                {
                    string leagueName = selectedLeague != null ? selectedLeague.leagueName : "Bilinmeyen Lig";

                    TransferOffer offer = TransferAISystem.Instance.GenerateTransferOffer(player, team.teamName, leagueName, selectedLeague, team);
                    if (offer != null)
                    {
                        currentOffers.Add(offer);
                        Debug.Log($"[TeamOfferUI] Generated offer from {team.teamName} (Power: {team.GetTeamPower()})");
                    }
                }
            }
            else
            {
                Debug.LogWarning("[TeamOfferUI] No teams found in any league!");
            }
        }
        else
        {
            // TransferAISystem yoksa basit teklifler oluştur (contractDuration yıl cinsinden)
            Debug.LogWarning("[TeamOfferUI] TransferAISystem or DataPackManager is null! Using fallback offers.");
            currentOffers.Add(CreateSimpleOffer("Galatasaray", "Süper Lig", 8000, 2, PlayingTime.Starter, 50000)); // 2 yıl = 24 ay
            currentOffers.Add(CreateSimpleOffer("Fenerbahçe", "Süper Lig", 10000, 2, PlayingTime.Rotation, 60000)); // 2.5 yıl ≈ 30 ay (2 yıl gösteriyoruz)
            currentOffers.Add(CreateSimpleOffer("Beşiktaş", "Süper Lig", 12000, 3, PlayingTime.Substitute, 70000)); // 3 yıl = 36 ay
            Debug.Log($"[TeamOfferUI] Created {currentOffers.Count} fallback offers.");
        }
    }

    /// <summary>
    /// Basit teklif oluştur (duration yıl cinsinden)
    /// </summary>
    private TransferOffer CreateSimpleOffer(string clubName, string leagueName, int salary, int durationYears, PlayingTime playingTime, int signingBonus)
    {
        return new TransferOffer
        {
            clubName = clubName,
            leagueName = leagueName,
            salary = salary,
            contractDuration = durationYears, // Yıl cinsinden
            playingTime = playingTime,
            signingBonus = signingBonus,
            offerDate = System.DateTime.Now
        };
    }

    /// <summary>
    /// Teklifleri göster (sahne üzerindeki mevcut OfferCard'ları kullan)
    /// </summary>
    private void DisplayOffers()
    {
        Debug.Log($"[TeamOfferUI] DisplayOffers called with {currentOffers.Count} offers.");
        
        List<GameObject> offerCards = new List<GameObject>();
        
        // Önce Inspector'da bağlı kartları kullan
        if (offerCardObjects != null && offerCardObjects.Count > 0)
        {
            offerCards = offerCardObjects.Where(card => card != null).ToList();
            Debug.Log($"[TeamOfferUI] Using {offerCards.Count} offer cards from Inspector.");
        }
        // Inspector'da yoksa otomatik bul
        else if (offersListParent != null)
        {
            Debug.Log($"[TeamOfferUI] Offer cards not found in Inspector, searching in parent: {offersListParent.name}");
            foreach (Transform child in offersListParent)
            {
                // Daha geniş arama kriterleri
                string childName = child.name.ToLower();
                if (childName.Contains("offercard") || childName.Contains("card") || 
                    childName.Contains("offer") || childName.Contains("teklif"))
                {
                    offerCards.Add(child.gameObject);
                    Debug.Log($"[TeamOfferUI] Found card: {child.name}");
                }
            }
            Debug.Log($"[TeamOfferUI] Auto-found {offerCards.Count} offer cards in parent.");
        }
        else
        {
            Debug.LogError("[TeamOfferUI] Neither offerCardObjects nor offersListParent is set! Cannot display offers.");
            return;
        }
        
        if (offerCards.Count == 0)
        {
            Debug.LogError("[TeamOfferUI] No offer cards found! Please add cards to offerCardObjects list in Inspector.");
            // Fallback: Eğer kart bulunamazsa bile devam et (fallback teklifleri göster)
            return;
        }
        
        Debug.Log($"[TeamOfferUI] Ready to display {currentOffers.Count} offers on {offerCards.Count} cards.");

        // Her teklif için mevcut card'ı güncelle veya yeni oluştur
        for (int i = 0; i < currentOffers.Count; i++)
        {
            if (i < offerCards.Count)
            {
                // Mevcut card'ı güncelle
                Debug.Log($"[TeamOfferUI] Updating card {i} with offer from {currentOffers[i].clubName}");
                UpdateOfferCard(offerCards[i], currentOffers[i]);
            }
            else
            {
                // Yeni card oluştur (gerekirse)
                Debug.LogWarning($"[TeamOfferUI] Not enough cards! Have {offerCards.Count}, need {currentOffers.Count}");
                CreateOfferItem(currentOffers[i]);
            }
        }

        // Kullanılmayan card'ları gizle
        for (int i = currentOffers.Count; i < offerCards.Count; i++)
        {
            offerCards[i].SetActive(false);
        }
    }

    /// <summary>
    /// Mevcut OfferCard'ı güncelle (sahne üzerindeki kartı kullan)
    /// </summary>
    private void UpdateOfferCard(GameObject cardObj, TransferOffer offer)
    {
        if (cardObj == null)
        {
            Debug.LogError("[TeamOfferUI] UpdateOfferCard: cardObj is null!");
            return;
        }

        Debug.Log($"[TeamOfferUI] UpdateOfferCard: Updating card '{cardObj.name}' with offer from '{offer.clubName}'");
        cardObj.SetActive(true);

        // TeamNameText'i bul ve güncelle (hem direkt child hem de recursive search)
        TextMeshProUGUI teamNameText = cardObj.transform.Find("TeamNameText")?.GetComponent<TextMeshProUGUI>();
        if (teamNameText == null)
        {
            // Recursive search
            teamNameText = cardObj.GetComponentsInChildren<TextMeshProUGUI>().FirstOrDefault(t => t.name.Contains("Team") || t.name.Contains("Name"));
        }
        if (teamNameText != null)
        {
            teamNameText.text = offer.clubName;
            Debug.Log($"[TeamOfferUI] Updated team name to: {offer.clubName}");
        }
        else
        {
            Debug.LogWarning($"[TeamOfferUI] TeamNameText not found in card '{cardObj.name}'!");
        }

        // SalaryText'i bul ve güncelle
        TextMeshProUGUI salaryText = cardObj.transform.Find("SalaryText")?.GetComponent<TextMeshProUGUI>();
        if (salaryText == null)
        {
            salaryText = cardObj.GetComponentsInChildren<TextMeshProUGUI>().FirstOrDefault(t => t.name.Contains("Salary") || t.name.Contains("Maaş"));
        }
        if (salaryText != null)
        {
            salaryText.text = $"Maaş: {offer.salary:N0} €/ay";
            Debug.Log($"[TeamOfferUI] Updated salary text");
        }
        else
        {
            Debug.LogWarning($"[TeamOfferUI] SalaryText not found in card '{cardObj.name}'!");
        }

        // InfoText'i bul ve güncelle
        TextMeshProUGUI infoText = cardObj.transform.Find("InfoText")?.GetComponent<TextMeshProUGUI>();
        if (infoText == null)
        {
            infoText = cardObj.GetComponentsInChildren<TextMeshProUGUI>().FirstOrDefault(t => t.name.Contains("Info") || t.name.Contains("Süre"));
        }
        if (infoText != null)
        {
            string playingTimeStr = offer.playingTime switch
            {
                PlayingTime.Starter => "İlk 11",
                PlayingTime.Rotation => "Rotasyon",
                PlayingTime.Substitute => "Yedek",
                _ => "Belirsiz"
            };
            int durationMonths = offer.contractDuration * 12; // Yılı aya çevir
            infoText.text = $"Süre: {durationMonths} ay - Rol: {playingTimeStr}";
            Debug.Log($"[TeamOfferUI] Updated info text");
        }
        else
        {
            Debug.LogWarning($"[TeamOfferUI] InfoText not found in card '{cardObj.name}'!");
        }

        // ButtonRow içindeki butonu bul ve event bağla
        Button detailButton = null;
        
        // Önce ButtonRow içinde ara
        Transform buttonRow = cardObj.transform.Find("ButtonRow");
        if (buttonRow != null)
        {
            // Eğer detailButtonTemplate referansı varsa, o isimde buton ara
            if (detailButtonTemplate != null)
            {
                Button[] buttons = buttonRow.GetComponentsInChildren<Button>();
                foreach (var btn in buttons)
                {
                    if (btn != null && btn.name == detailButtonTemplate.name)
                    {
                        detailButton = btn;
                        break;
                    }
                }
            }
            
            // Eğer hala bulunamadıysa, ButtonRow içindeki ilk butonu kullan
            if (detailButton == null)
            {
                detailButton = buttonRow.GetComponentInChildren<Button>();
            }
        }
        
        // ButtonRow yoksa veya buton bulunamadıysa, card içindeki butonu bul
        if (detailButton == null)
        {
            if (detailButtonTemplate != null)
            {
                Button[] allButtons = cardObj.GetComponentsInChildren<Button>();
                foreach (var btn in allButtons)
                {
                    if (btn != null && btn.name == detailButtonTemplate.name)
                    {
                        detailButton = btn;
                        break;
                    }
                }
            }
            
            // Hala bulunamadıysa, card içindeki ilk butonu kullan
            if (detailButton == null)
            {
                detailButton = cardObj.GetComponentInChildren<Button>();
            }
        }
        
        // Buton bulunduysa event bağla
        if (detailButton != null)
        {
            detailButton.onClick.RemoveAllListeners();
            detailButton.onClick.AddListener(() => OnOfferItemClicked(offer));
        }
    }

    /// <summary>
    /// Teklif item'ı oluştur (sadece gerekirse - genelde kullanılmayacak)
    /// </summary>
    private void CreateOfferItem(TransferOffer offer)
    {
        // Bu metod artık kullanılmıyor, sahne üzerindeki kartlar kullanılıyor
        Debug.LogWarning("[TeamOfferUI] CreateOfferItem çağrıldı ama sahne üzerinde yeterli kart yok! Yeni kart oluşturulmuyor.");
    }

    /// <summary>
    /// Teklif item'ına tıklandığında (Detaylar butonu)
    /// </summary>
    private void OnOfferItemClicked(TransferOffer offer)
    {
        selectedOffer = offer;
        ShowOfferDetail(offer);
    }

    /// <summary>
    /// Teklif detayını göster
    /// </summary>
    private void ShowOfferDetail(TransferOffer offer)
    {
        if (detailPanel != null)
            detailPanel.SetActive(true);

        // Takım adı
        if (detailTeamNameText != null)
            detailTeamNameText.text = offer.clubName;

        // Aylık maaş
        if (detailSalaryText != null)
            detailSalaryText.text = $"Aylık Maaş: {offer.salary:N0} €";

        // Sözleşme süresi (ay cinsinden)
        if (detailDurationText != null)
        {
            int durationMonths = offer.contractDuration * 12; // Yılı aya çevir
            detailDurationText.text = $"Sözleşme Süresi: {durationMonths} ay";
        }

        // Oynama rolü
        if (detailRoleText != null)
        {
            string playingTimeStr = offer.playingTime switch
            {
                PlayingTime.Starter => "İlk 11",
                PlayingTime.Rotation => "Rotasyon",
                PlayingTime.Substitute => "Yedek",
                _ => "Belirsiz"
            };
            detailRoleText.text = $"Oynama Rolü: {playingTimeStr}";
        }

        // Bonuslar (TransferOffer'da bonuslar yok, şimdilik örnek bonuslar gösterelim)
        if (detailBonusText != null)
        {
            // TransferOffer'da bonuslar yok, şimdilik örnek bonuslar göster
            // Gerçek implementasyonda TransferOffer'a bonuslar eklenebilir veya TransferSystem'de oluşturulabilir
            // Şimdilik maaşa göre hesaplanmış örnek bonuslar (görseldeki format: Gol: 1.000 € / Asist: 500 € / Galibiyet: 2.000 €)
            int goalBonus = Mathf.Max(500, Mathf.RoundToInt(offer.salary * 0.125f)); // Maaşın %12.5'i, min 500
            int assistBonus = Mathf.Max(250, Mathf.RoundToInt(offer.salary * 0.05f)); // Maaşın %5'i, min 250
            int winBonus = Mathf.Max(1000, Mathf.RoundToInt(offer.salary * 0.25f)); // Maaşın %25'i, min 1000
            
            detailBonusText.text = $"Gol: {goalBonus:N0} € / Asist: {assistBonus:N0} € / Galibiyet: {winBonus:N0} €";
        }
    }

    private void OnAcceptButton()
    {
        if (selectedOffer == null || GameManager.Instance == null || !GameManager.Instance.HasCurrentSave())
        {
            Debug.LogWarning("[TeamOfferUI] Cannot accept offer - missing data!");
            return;
        }

        SaveData saveData = GameManager.Instance.CurrentSave;
        int slotIndex = GameManager.Instance.CurrentSaveSlotIndex;
        
        Debug.Log($"[TeamOfferUI] Current save slot index: {slotIndex}");
        
        if (slotIndex < 0)
        {
            Debug.LogWarning($"[TeamOfferUI] Slot index is negative ({slotIndex}), setting to 0");
            slotIndex = 0;
            GameManager.Instance.SetSaveSlotIndex(slotIndex);
        }

        // TransferSystem yoksa oluştur
        if (TransferSystem.Instance == null)
        {
            GameObject transferSystemObj = new GameObject("TransferSystem");
            transferSystemObj.AddComponent<TransferSystem>();
            Debug.Log("[TeamOfferUI] TransferSystem created automatically.");
        }
        
        // Transfer teklifini kabul et (fikstür de burada oluşturulur)
        if (TransferSystem.Instance != null)
        {
            TransferSystem.Instance.AcceptOffer(saveData, selectedOffer);
        }
        else
        {
            // Fallback: TransferSystem yoksa manuel olarak ayarla
            if (saveData.clubData == null)
                saveData.clubData = new ClubData();
            
            if (saveData.clubData.contract == null)
                saveData.clubData.contract = new ContractData();
            
            saveData.clubData.clubName = selectedOffer.clubName;
            saveData.clubData.leagueName = selectedOffer.leagueName;
            saveData.clubData.contract.salary = selectedOffer.salary;
            saveData.clubData.contract.contractDuration = selectedOffer.contractDuration;
            saveData.clubData.contract.playingTime = selectedOffer.playingTime;
            
            // Fallback fikstür ve puan durumu oluşturma
            if (DataPackManager.Instance != null && !string.IsNullOrEmpty(saveData.clubData.leagueName))
            {
                LeagueData league = DataPackManager.Instance.GetLeague(saveData.clubData.leagueName);
                if (league != null && league.teams != null && league.teams.Count > 0)
                {
                    System.DateTime seasonStart = System.DateTime.Now;
                    var fixtures = FixtureGenerator.GenerateSeasonFixtures(league.teams, seasonStart);
                    
                    if (saveData.seasonData == null) saveData.seasonData = new SeasonData();
                    saveData.seasonData.fixtures = fixtures;
                    saveData.seasonData.seasonStartDate = seasonStart;
                    saveData.seasonData.seasonStartDateString = seasonStart.ToString("yyyy-MM-dd");
                    
                    // Puan durumunu başlat
                    saveData.seasonData.InitializeStandings(league.teams);
                    
                    Debug.Log($"[TeamOfferUI] Fallback: Generated {fixtures.Count} fixtures, {league.teams.Count} teams in standings.");
                }
            }
        }

        // Kayıt tarihini güncelle
        saveData.UpdateSaveDate();

        // Dosyaya kaydet
        SaveSystem.SaveGame(saveData, slotIndex);
        Debug.Log($"[TeamOfferUI] Save updated and saved to slot {slotIndex}");

        // CareerHub'a geç
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.ChangeState(GameState.CareerHub);
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("CareerHub");
        }
    }


    private void OnCloseDetailButton()
    {
        // Sadece detay panelini kapat, başka bir yere gitme
        if (detailPanel != null)
        {
            detailPanel.SetActive(false);
        }

        selectedOffer = null;
    }

    private void OnBackButton()
    {
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.ReturnToPreviousState();
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("CharacterCreation");
        }
    }
}
