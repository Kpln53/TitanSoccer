using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Karakter oluşturma UI - Yeni kariyer için karakter oluşturma
/// </summary>
public class CharacterCreationUI : MonoBehaviour
{
    [Header("Oyuncu Bilgileri")]
    public TMP_InputField playerNameInput;     // İsim input'u (sahne: playerNameInput)
    public TMP_InputField playerSurnameInput;  // Soyisim input'u (sahne: playerSurnameInput)
    public TMP_Dropdown positionDropdown;      // Pozisyon dropdown'ı (ESKİ - artık kullanılmıyor)
    public TMP_Dropdown nationalityDropdown;   // Millet dropdown'ı
    public TMP_Dropdown leagueDropdown;        // Lig dropdown'ı

    [Header("Pozisyon Butonları - Normal Haller")]
    public GameObject defNormal;    // DEF (normal)
    public GameObject dosNormal;    // DOS (normal)
    public GameObject osNormal;     // OS (normal)
    public GameObject sfNormal;     // SF (normal)
    
    [Header("Pozisyon Butonları - Basılı Haller")]
    public GameObject defPressed;   // DEF (Basılı)
    public GameObject dosPressed;   // DOS (Basılı)
    public GameObject osPressed;    // OS (Basılı)
    public GameObject sfPressed;    // SF (Basılı)

    [Header("Butonlar")]
    public Button createButton;
    public Button backButton;

    private PlayerProfile newPlayerProfile;
    private PlayerPosition selectedPosition = PlayerPosition.MOO; // Varsayılan: Orta Saha

    private void Start()
    {
        // Referanslar bağlı değilse otomatik bul
        AutoFindPositionButtons();
        
        SetupUI();
        SetupButtons();
    }
    
    /// <summary>
    /// Pozisyon butonlarını otomatik bul (Inspector'da bağlı değilse)
    /// </summary>
    private void AutoFindPositionButtons()
    {
        // Mevki parent objesini bul
        Transform mevkiParent = GameObject.Find("Mevki")?.transform;
        
        if (mevkiParent == null)
        {
            // Canvas altında ara
            Canvas canvas = FindFirstObjectByType<Canvas>();
            if (canvas != null)
            {
                mevkiParent = canvas.transform.Find("Mevki");
            }
        }
        
        if (mevkiParent == null)
        {
            Debug.LogError("[CharacterCreationUI] Mevki parent not found! Cannot auto-find position buttons.");
            return;
        }
        
        Debug.Log($"[CharacterCreationUI] Mevki parent found: {mevkiParent.name}");
        
        // Normal butonları bul
        if (defNormal == null)
        {
            defNormal = mevkiParent.Find("DEF")?.gameObject;
            Debug.Log($"[CharacterCreationUI] Auto-found defNormal: {(defNormal != null ? "SUCCESS" : "FAILED")}");
        }
        
        if (dosNormal == null)
        {
            dosNormal = mevkiParent.Find("DOS")?.gameObject;
            Debug.Log($"[CharacterCreationUI] Auto-found dosNormal: {(dosNormal != null ? "SUCCESS" : "FAILED")}");
        }
        
        if (osNormal == null)
        {
            osNormal = mevkiParent.Find("OS")?.gameObject;
            Debug.Log($"[CharacterCreationUI] Auto-found osNormal: {(osNormal != null ? "SUCCESS" : "FAILED")}");
        }
        
        if (sfNormal == null)
        {
            sfNormal = mevkiParent.Find("SF")?.gameObject;
            Debug.Log($"[CharacterCreationUI] Auto-found sfNormal: {(sfNormal != null ? "SUCCESS" : "FAILED")}");
        }
        
        // Basılı butonları bul
        if (defPressed == null)
        {
            defPressed = mevkiParent.Find("DEF (Basılı)")?.gameObject;
            Debug.Log($"[CharacterCreationUI] Auto-found defPressed: {(defPressed != null ? "SUCCESS" : "FAILED")}");
        }
        
        if (dosPressed == null)
        {
            dosPressed = mevkiParent.Find("DOS (Basılı)")?.gameObject;
            Debug.Log($"[CharacterCreationUI] Auto-found dosPressed: {(dosPressed != null ? "SUCCESS" : "FAILED")}");
        }
        
        if (osPressed == null)
        {
            osPressed = mevkiParent.Find("OS (Basılı)")?.gameObject;
            Debug.Log($"[CharacterCreationUI] Auto-found osPressed: {(osPressed != null ? "SUCCESS" : "FAILED")}");
        }
        
        if (sfPressed == null)
        {
            sfPressed = mevkiParent.Find("SF (Basılı)")?.gameObject;
            Debug.Log($"[CharacterCreationUI] Auto-found sfPressed: {(sfPressed != null ? "SUCCESS" : "FAILED")}");
        }
    }

    private void SetupUI()
    {
        // Pozisyon dropdown'ı
        if (positionDropdown != null)
        {
            positionDropdown.ClearOptions();
            positionDropdown.AddOptions(new System.Collections.Generic.List<string>
            {
                "Stoper (STP)", "Sağ Bek (SĞB)", "Sol Bek (SLB)",
                "Merkez Orta Defans (MDO)", "Merkez Orta Ofans (MOO)",
                "Sağ Kanat (SĞK)", "Sol Kanat (SLK)", "Sağ Orta (SĞO)", "Sol Orta (SLO)",
                "Santrafor (SF)"
            });
        }

        // Millet dropdown'ı
        if (nationalityDropdown != null)
        {
            nationalityDropdown.ClearOptions();
            nationalityDropdown.AddOptions(new System.Collections.Generic.List<string>
            {
                "Türkiye", "Almanya", "Fransa", "İspanya", "İtalya", "İngiltere",
                "Brezilya", "Arjantin", "Portekiz", "Hollanda", "Belçika", "Hırvatistan",
                "Polonya", "İsveç", "Norveç", "Danimarka", "Yunanistan", "Romanya",
                "Bulgaristan", "Sırbistan", "Rusya", "Ukrayna", "Japonya", "Güney Kore",
                "Çin", "ABD", "Meksika", "Kolombiya", "Şili", "Uruguay", "Diğer"
            });
        }

        // Lig dropdown'ı - DataPackManager'dan ligleri yükle
        SetupLeagueDropdown();
    }

    /// <summary>
    /// Lig dropdown'ını aktif DataPack'ten yükle
    /// </summary>
    private void SetupLeagueDropdown()
    {
        if (leagueDropdown == null)
        {
            Debug.LogWarning("[CharacterCreationUI] League dropdown not found!");
            return;
        }

        leagueDropdown.ClearOptions();

        // DataPackManager'dan ligleri al
        if (DataPackManager.Instance != null && DataPackManager.Instance.activeDataPack != null)
        {
            var leagues = DataPackManager.Instance.GetAllLeagues();

            if (leagues != null && leagues.Count > 0)
            {
                System.Collections.Generic.List<string> leagueNames = new System.Collections.Generic.List<string>();
                foreach (var league in leagues)
                {
                    if (league != null && !string.IsNullOrEmpty(league.leagueName))
                    {
                        leagueNames.Add(league.leagueName);
                    }
                }

                if (leagueNames.Count > 0)
                {
                    leagueDropdown.AddOptions(leagueNames);
                    Debug.Log($"[CharacterCreationUI] Loaded {leagueNames.Count} league(s) into dropdown.");
                }
                else
                {
                    leagueDropdown.AddOptions(new System.Collections.Generic.List<string> { "Lig bulunamadı" });
                    Debug.LogWarning("[CharacterCreationUI] No valid leagues found in active DataPack!");
                }
            }
            else
            {
                leagueDropdown.AddOptions(new System.Collections.Generic.List<string> { "Lig bulunamadı" });
                Debug.LogWarning("[CharacterCreationUI] No leagues found in active DataPack!");
            }
        }
        else
        {
            leagueDropdown.AddOptions(new System.Collections.Generic.List<string> { "DataPack seçilmemiş" });
            Debug.LogWarning("[CharacterCreationUI] No active DataPack! Please select a DataPack first.");
        }
    }

    private void SetupButtons()
    {
        Debug.Log("[CharacterCreationUI] SetupButtons called");
        
        if (createButton != null)
            createButton.onClick.AddListener(OnCreateButton);

        if (backButton != null)
            backButton.onClick.AddListener(OnBackButton);
            
        // Pozisyon butonlarına listener ekle (Normal ve Basılı hallerin ikisine de)
        Debug.Log($"[CharacterCreationUI] Setting up position buttons...");
        Debug.Log($"[CharacterCreationUI] defNormal: {(defNormal != null ? "EXISTS" : "NULL")}");
        Debug.Log($"[CharacterCreationUI] dosNormal: {(dosNormal != null ? "EXISTS" : "NULL")}");
        Debug.Log($"[CharacterCreationUI] osNormal: {(osNormal != null ? "EXISTS" : "NULL")}");
        Debug.Log($"[CharacterCreationUI] sfNormal: {(sfNormal != null ? "EXISTS" : "NULL")}");
        
        SetupPositionButton(defNormal, PlayerPosition.STP, "DEF Normal");
        SetupPositionButton(defPressed, PlayerPosition.STP, "DEF Pressed");
        
        SetupPositionButton(dosNormal, PlayerPosition.MDO, "DOS Normal");
        SetupPositionButton(dosPressed, PlayerPosition.MDO, "DOS Pressed");
        
        SetupPositionButton(osNormal, PlayerPosition.MOO, "OS Normal");
        SetupPositionButton(osPressed, PlayerPosition.MOO, "OS Pressed");
        
        SetupPositionButton(sfNormal, PlayerPosition.SF, "SF Normal");
        SetupPositionButton(sfPressed, PlayerPosition.SF, "SF Pressed");
            
        // Başlangıçta OS'yi seçili yap
        Debug.Log("[CharacterCreationUI] Selecting default position: OS");
        SelectPosition(PlayerPosition.MOO);
    }
    
    /// <summary>
    /// Pozisyon butonuna listener ekle
    /// </summary>
    private void SetupPositionButton(GameObject buttonObj, PlayerPosition position, string debugName)
    {
        if (buttonObj == null)
        {
            Debug.LogWarning($"[CharacterCreationUI] {debugName} is NULL!");
            return;
        }
        
        Button btn = buttonObj.GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(() => {
                Debug.Log($"[CharacterCreationUI] Button clicked: {debugName}");
                SelectPosition(position);
            });
            Debug.Log($"[CharacterCreationUI] Listener added to {debugName}");
        }
        else
        {
            Debug.LogWarning($"[CharacterCreationUI] {debugName} has no Button component!");
        }
    }
    
    /// <summary>
    /// Pozisyon seç ve görünümü güncelle
    /// </summary>
    private void SelectPosition(PlayerPosition position)
    {
        Debug.Log($"[CharacterCreationUI] SelectPosition called with: {position}");
        selectedPosition = position;
        
        // Tüm butonları normal hale getir
        Debug.Log("[CharacterCreationUI] Resetting all buttons to normal...");
        ShowNormalButton(defNormal, defPressed);
        ShowNormalButton(dosNormal, dosPressed);
        ShowNormalButton(osNormal, osPressed);
        ShowNormalButton(sfNormal, sfPressed);
        
        // Seçilen pozisyonu basılı hale getir
        switch (position)
        {
            case PlayerPosition.STP:
                Debug.Log("[CharacterCreationUI] Showing DEF as pressed");
                ShowPressedButton(defNormal, defPressed);
                Debug.Log("[CharacterCreationUI] Position selected: DEF (Defans)");
                break;
            case PlayerPosition.MDO:
                Debug.Log("[CharacterCreationUI] Showing DOS as pressed");
                ShowPressedButton(dosNormal, dosPressed);
                Debug.Log("[CharacterCreationUI] Position selected: DOS (Defansif Orta Saha)");
                break;
            case PlayerPosition.MOO:
                Debug.Log("[CharacterCreationUI] Showing OS as pressed");
                ShowPressedButton(osNormal, osPressed);
                Debug.Log("[CharacterCreationUI] Position selected: OS (Orta Saha)");
                break;
            case PlayerPosition.SF:
                Debug.Log("[CharacterCreationUI] Showing SF as pressed");
                ShowPressedButton(sfNormal, sfPressed);
                Debug.Log("[CharacterCreationUI] Position selected: SF (Santrafor)");
                break;
        }
    }
    
    /// <summary>
    /// Normal butonu göster, basılı olanı gizle
    /// </summary>
    private void ShowNormalButton(GameObject normalObj, GameObject pressedObj)
    {
        if (normalObj != null)
        {
            normalObj.SetActive(true);
            Debug.Log($"[CharacterCreationUI] {normalObj.name} set to ACTIVE");
        }
        else
        {
            Debug.LogWarning("[CharacterCreationUI] normalObj is NULL in ShowNormalButton");
        }
        
        if (pressedObj != null)
        {
            pressedObj.SetActive(false);
            Debug.Log($"[CharacterCreationUI] {pressedObj.name} set to INACTIVE");
        }
        else
        {
            Debug.LogWarning("[CharacterCreationUI] pressedObj is NULL in ShowNormalButton");
        }
    }
    
    /// <summary>
    /// Basılı butonu göster, normal olanı gizle
    /// </summary>
    private void ShowPressedButton(GameObject normalObj, GameObject pressedObj)
    {
        if (normalObj != null)
        {
            normalObj.SetActive(false);
            Debug.Log($"[CharacterCreationUI] {normalObj.name} set to INACTIVE");
        }
        else
        {
            Debug.LogWarning("[CharacterCreationUI] normalObj is NULL in ShowPressedButton");
        }
        
        if (pressedObj != null)
        {
            pressedObj.SetActive(true);
            Debug.Log($"[CharacterCreationUI] {pressedObj.name} set to ACTIVE");
        }
        else
        {
            Debug.LogWarning("[CharacterCreationUI] pressedObj is NULL in ShowPressedButton");
        }
    }

    private void OnCreateButton()
    {
        Debug.Log("[CharacterCreationUI] OnCreateButton called!");
        
        // İsim ve soyisim kontrolü
        string firstName = playerNameInput != null ? playerNameInput.text.Trim() : "";
        string lastName = playerSurnameInput != null ? playerSurnameInput.text.Trim() : "";

        Debug.Log($"[CharacterCreationUI] First name: '{firstName}', Last name: '{lastName}'");

        if (string.IsNullOrEmpty(firstName))
        {
            Debug.LogWarning("[CharacterCreationUI] İsim gereklidir!");
            return;
        }

        if (string.IsNullOrEmpty(lastName))
        {
            Debug.LogWarning("[CharacterCreationUI] Soyisim gereklidir!");
            return;
        }

        // İsim ve soyisimi birleştir
        string fullName = $"{firstName} {lastName}".Trim();

        // Yeni oyuncu profili oluştur
        newPlayerProfile = new PlayerProfile
        {
            playerName = fullName
        };

        // Pozisyon seç (YENİ: Butonlardan seçilen pozisyon)
        newPlayerProfile.position = selectedPosition;
        Debug.Log($"[CharacterCreationUI] Selected position: {selectedPosition}");

        // Millet seç
        if (nationalityDropdown != null && nationalityDropdown.options.Count > 0)
        {
            newPlayerProfile.nationality = nationalityDropdown.options[nationalityDropdown.value].text;
        }
        else
        {
            newPlayerProfile.nationality = "Türkiye"; // Varsayılan
        }

        // Yaş otomatik belirlenecek (18-22 arası rastgele)
        newPlayerProfile.age = UnityEngine.Random.Range(18, 23);

        // Avatar ID (1 veya 2 şimdilik)
        newPlayerProfile.avatarId = UnityEngine.Random.Range(1, 3);

        // Overall otomatik belirlenecek (60-75 arası rastgele)
        int randomOverall = UnityEngine.Random.Range(60, 76);
        newPlayerProfile.overall = randomOverall;

        // Yetenekleri pozisyona göre otomatik ayarla
        InitializePlayerSkills(newPlayerProfile, randomOverall);

        // Lig seç
        string selectedLeagueName = null;
        if (leagueDropdown != null && leagueDropdown.options.Count > 0 && 
            leagueDropdown.value >= 0 && leagueDropdown.value < leagueDropdown.options.Count)
        {
            selectedLeagueName = leagueDropdown.options[leagueDropdown.value].text;
            
            // "Lig bulunamadı" veya "DataPack seçilmemiş" gibi placeholder'ları kontrol et
            if (selectedLeagueName == "Lig bulunamadı" || selectedLeagueName == "DataPack seçilmemiş")
            {
                selectedLeagueName = null;
                Debug.LogWarning("[CharacterCreationUI] Invalid league selected!");
            }
        }

        // Yeni SaveData oluştur
        SaveData newSave = new SaveData
        {
            saveName = fullName, // Oyuncu adını kayıt adı olarak kullan
            saveDate = System.DateTime.Now,
            saveDateString = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            version = Application.version, // Oyun versiyonu
            playerProfile = newPlayerProfile,
            clubData = new ClubData(),
            seasonData = new SeasonData(),
            relationsData = new RelationsData(),
            economyData = new EconomyData(),
            mediaData = new MediaData()
        };

        // Seçilen ligi clubData'ya kaydet
        if (!string.IsNullOrEmpty(selectedLeagueName))
        {
            newSave.clubData.leagueName = selectedLeagueName;
            Debug.Log($"[CharacterCreationUI] Selected league: {selectedLeagueName}");
        }

        Debug.Log("[CharacterCreationUI] Starting save process...");
        Debug.Log($"[CharacterCreationUI] GameManager.Instance: {(GameManager.Instance != null ? "EXISTS" : "NULL")}");
        
        // GameManager yoksa oluştur
        if (GameManager.Instance == null)
        {
            Debug.LogWarning("[CharacterCreationUI] GameManager.Instance is NULL! Creating one...");
            GameObject gameManagerObj = new GameObject("GameManager");
            gameManagerObj.AddComponent<GameManager>();
            Debug.Log("[CharacterCreationUI] GameManager created automatically.");
        }
        
        // GameManager'a kaydet
        if (GameManager.Instance != null)
        {
            int slotIndex = GameManager.Instance.CurrentSaveSlotIndex;
            Debug.Log($"[CharacterCreationUI] Current save slot index: {slotIndex}");
            
            if (slotIndex < 0)
            {
                slotIndex = 0; // Varsayılan slot
                GameManager.Instance.SetSaveSlotIndex(slotIndex);
                Debug.Log($"[CharacterCreationUI] Slot index was negative, set to {slotIndex}");
            }
            
            Debug.Log($"[CharacterCreationUI] Setting current save to slot {slotIndex}...");
            Debug.Log($"[CharacterCreationUI] SaveData - Player: {newSave.playerProfile?.playerName ?? "NULL"}, Club: {newSave.clubData?.clubName ?? "NULL"}");
            
            GameManager.Instance.SetCurrentSave(newSave, slotIndex);
            
            Debug.Log($"[CharacterCreationUI] Calling SaveSystem.SaveGame for slot {slotIndex}...");
            Debug.Log($"[CharacterCreationUI] SaveData reference: {(newSave != null ? "NOT NULL" : "NULL")}");
            
            // Dosyaya kaydet
            SaveSystem.SaveGame(newSave, slotIndex);
            Debug.Log($"[CharacterCreationUI] SaveSystem.SaveGame call completed for slot {slotIndex}");
        }
        else
        {
            Debug.LogError("[CharacterCreationUI] GameManager.Instance is still NULL after creation attempt! Cannot save game.");
        }

        // TeamOffer sahnesine geç
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.ChangeState(GameState.TeamOffer);
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("TeamOffer");
        }
    }

    /// <summary>
    /// Oyuncu yeteneklerini pozisyona göre otomatik ayarla
    /// </summary>
    private void InitializePlayerSkills(PlayerProfile profile, int overall)
    {
        int baseSkill = overall;
        int variation = 10; // ±10 varyasyon

        if (profile.position == PlayerPosition.KL)
        {
            // Kaleci yetenekleri
            profile.saveReflex = Mathf.Clamp(baseSkill + UnityEngine.Random.Range(-variation, variation), 0, 100);
            profile.goalkeeperPositioning = Mathf.Clamp(baseSkill + UnityEngine.Random.Range(-variation, variation), 0, 100);
            profile.aerialAbility = Mathf.Clamp(baseSkill + UnityEngine.Random.Range(-variation, variation), 0, 100);
            profile.oneOnOne = Mathf.Clamp(baseSkill + UnityEngine.Random.Range(-variation, variation), 0, 100);
            profile.handling = Mathf.Clamp(baseSkill + UnityEngine.Random.Range(-variation, variation), 0, 100);
            
            // Diğer yetenekleri düşük tut
            profile.passingSkill = Mathf.Clamp(baseSkill - UnityEngine.Random.Range(0, variation * 2), 0, 100);
            profile.shootingSkill = Mathf.Clamp(baseSkill - UnityEngine.Random.Range(0, variation * 2), 0, 100);
            profile.dribblingSkill = Mathf.Clamp(baseSkill - UnityEngine.Random.Range(0, variation * 2), 0, 100);
            profile.falsoSkill = Mathf.Clamp(baseSkill - UnityEngine.Random.Range(0, variation * 2), 0, 100);
            profile.speed = Mathf.Clamp(baseSkill - UnityEngine.Random.Range(0, variation), 0, 100);
            profile.stamina = Mathf.Clamp(baseSkill + UnityEngine.Random.Range(-variation, variation), 0, 100);
            profile.defendingSkill = Mathf.Clamp(baseSkill + UnityEngine.Random.Range(-variation, variation), 0, 100);
            profile.physicalStrength = Mathf.Clamp(baseSkill + UnityEngine.Random.Range(-variation, variation), 0, 100);
        }
        else
        {
            // Pozisyona göre yetenekleri ayarla
            switch (profile.position)
            {
                case PlayerPosition.STP:
                case PlayerPosition.SĞB:
                case PlayerPosition.SLB:
                    // Defans oyuncuları
                    profile.defendingSkill = Mathf.Clamp(baseSkill + UnityEngine.Random.Range(-variation, variation), 0, 100);
                    profile.physicalStrength = Mathf.Clamp(baseSkill + UnityEngine.Random.Range(-variation, variation), 0, 100);
                    profile.passingSkill = Mathf.Clamp(baseSkill + UnityEngine.Random.Range(-variation, variation), 0, 100);
                    profile.shootingSkill = Mathf.Clamp(baseSkill - UnityEngine.Random.Range(0, variation), 0, 100);
                    profile.dribblingSkill = Mathf.Clamp(baseSkill - UnityEngine.Random.Range(0, variation), 0, 100);
                    profile.falsoSkill = Mathf.Clamp(baseSkill - UnityEngine.Random.Range(0, variation), 0, 100);
                    profile.speed = Mathf.Clamp(baseSkill + UnityEngine.Random.Range(-variation, variation), 0, 100);
                    profile.stamina = Mathf.Clamp(baseSkill + UnityEngine.Random.Range(-variation, variation), 0, 100);
                    break;
                    
                case PlayerPosition.MDO:
                    // Defansif orta saha
                    profile.defendingSkill = Mathf.Clamp(baseSkill + UnityEngine.Random.Range(-variation, variation), 0, 100);
                    profile.passingSkill = Mathf.Clamp(baseSkill + UnityEngine.Random.Range(-variation, variation), 0, 100);
                    profile.physicalStrength = Mathf.Clamp(baseSkill + UnityEngine.Random.Range(-variation, variation), 0, 100);
                    profile.shootingSkill = Mathf.Clamp(baseSkill - UnityEngine.Random.Range(0, variation), 0, 100);
                    profile.dribblingSkill = Mathf.Clamp(baseSkill + UnityEngine.Random.Range(-variation, variation), 0, 100);
                    profile.falsoSkill = Mathf.Clamp(baseSkill + UnityEngine.Random.Range(-variation, variation), 0, 100);
                    profile.speed = Mathf.Clamp(baseSkill + UnityEngine.Random.Range(-variation, variation), 0, 100);
                    profile.stamina = Mathf.Clamp(baseSkill + UnityEngine.Random.Range(-variation, variation), 0, 100);
                    break;
                    
                case PlayerPosition.MOO:
                    // Ofansif orta saha
                    profile.passingSkill = Mathf.Clamp(baseSkill + UnityEngine.Random.Range(-variation, variation), 0, 100);
                    profile.dribblingSkill = Mathf.Clamp(baseSkill + UnityEngine.Random.Range(-variation, variation), 0, 100);
                    profile.shootingSkill = Mathf.Clamp(baseSkill + UnityEngine.Random.Range(-variation, variation), 0, 100);
                    profile.falsoSkill = Mathf.Clamp(baseSkill + UnityEngine.Random.Range(-variation, variation), 0, 100);
                    profile.speed = Mathf.Clamp(baseSkill + UnityEngine.Random.Range(-variation, variation), 0, 100);
                    profile.stamina = Mathf.Clamp(baseSkill + UnityEngine.Random.Range(-variation, variation), 0, 100);
                    profile.defendingSkill = Mathf.Clamp(baseSkill - UnityEngine.Random.Range(0, variation), 0, 100);
                    profile.physicalStrength = Mathf.Clamp(baseSkill + UnityEngine.Random.Range(-variation, variation), 0, 100);
                    break;
                    
                case PlayerPosition.SĞK:
                case PlayerPosition.SLK:
                case PlayerPosition.SĞO:
                case PlayerPosition.SLO:
                    // Kanat oyuncuları
                    profile.speed = Mathf.Clamp(baseSkill + UnityEngine.Random.Range(-variation, variation), 0, 100);
                    profile.dribblingSkill = Mathf.Clamp(baseSkill + UnityEngine.Random.Range(-variation, variation), 0, 100);
                    profile.falsoSkill = Mathf.Clamp(baseSkill + UnityEngine.Random.Range(-variation, variation), 0, 100);
                    profile.passingSkill = Mathf.Clamp(baseSkill + UnityEngine.Random.Range(-variation, variation), 0, 100);
                    profile.shootingSkill = Mathf.Clamp(baseSkill + UnityEngine.Random.Range(-variation, variation), 0, 100);
                    profile.stamina = Mathf.Clamp(baseSkill + UnityEngine.Random.Range(-variation, variation), 0, 100);
                    profile.defendingSkill = Mathf.Clamp(baseSkill - UnityEngine.Random.Range(0, variation), 0, 100);
                    profile.physicalStrength = Mathf.Clamp(baseSkill + UnityEngine.Random.Range(-variation, variation), 0, 100);
                    break;
                    
                case PlayerPosition.SF:
                    // Forvet
                    profile.shootingSkill = Mathf.Clamp(baseSkill + UnityEngine.Random.Range(-variation, variation), 0, 100);
                    profile.physicalStrength = Mathf.Clamp(baseSkill + UnityEngine.Random.Range(-variation, variation), 0, 100);
                    profile.dribblingSkill = Mathf.Clamp(baseSkill + UnityEngine.Random.Range(-variation, variation), 0, 100);
                    profile.falsoSkill = Mathf.Clamp(baseSkill + UnityEngine.Random.Range(-variation, variation), 0, 100);
                    profile.speed = Mathf.Clamp(baseSkill + UnityEngine.Random.Range(-variation, variation), 0, 100);
                    profile.passingSkill = Mathf.Clamp(baseSkill + UnityEngine.Random.Range(-variation, variation), 0, 100);
                    profile.stamina = Mathf.Clamp(baseSkill + UnityEngine.Random.Range(-variation, variation), 0, 100);
                    profile.defendingSkill = Mathf.Clamp(baseSkill - UnityEngine.Random.Range(0, variation * 2), 0, 100);
                    break;
            }
            
            // Kaleci yeteneklerini düşük tut
            profile.saveReflex = Mathf.Clamp(baseSkill - UnityEngine.Random.Range(0, variation * 2), 0, 100);
            profile.goalkeeperPositioning = Mathf.Clamp(baseSkill - UnityEngine.Random.Range(0, variation * 2), 0, 100);
            profile.aerialAbility = Mathf.Clamp(baseSkill - UnityEngine.Random.Range(0, variation * 2), 0, 100);
            profile.oneOnOne = Mathf.Clamp(baseSkill - UnityEngine.Random.Range(0, variation * 2), 0, 100);
            profile.handling = Mathf.Clamp(baseSkill - UnityEngine.Random.Range(0, variation * 2), 0, 100);
        }

        // Overall'ı yeniden hesapla
        profile.CalculateOverall();
    }

    private PlayerPosition ConvertPositionIndexToEnum(int index)
    {
        // KL (Kaleci) kaldırıldığı için indexler kaydı
        return index switch
        {
            0 => PlayerPosition.STP,
            1 => PlayerPosition.SĞB,
            2 => PlayerPosition.SLB,
            3 => PlayerPosition.MDO,
            4 => PlayerPosition.MOO,
            5 => PlayerPosition.SĞK,
            6 => PlayerPosition.SLK,
            7 => PlayerPosition.SĞO,
            8 => PlayerPosition.SLO,
            9 => PlayerPosition.SF,
            _ => PlayerPosition.MOO
        };
    }

    private void OnBackButton()
    {
        // MainMenu'ye dön
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}

