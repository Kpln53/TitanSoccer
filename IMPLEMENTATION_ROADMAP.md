# 🗺️ Script Yeniden Yapılandırma - Uygulama Yol Haritası

## ✅ Tamamlananlar
- [x] Tüm Enum'lar oluşturuldu (`Assets/Scripts/Data/Enums.cs`)

---

## 📋 Sıradaki Adımlar (Mantıklı Sıralama)

### AŞAMA 1: Temel Data Yapıları (Data Pack Sistemi)
**Amaç:** DataPack sistemi için gerekli temel veri yapılarını oluştur. Bunlar oyun verilerinden bağımsızdır.

**Öncelik:** 🔴 Yüksek (Diğer sistemler bunlara bağımlı)

1. **PlayerData.cs** ⏳
   - DataPack için oyuncu verisi
   - playerName, position, overall, skills, age, nationality

2. **TeamData.cs** ⏳
   - Takım bilgileri (DataPack için)
   - teamName, players list, teamPower, colors

3. **LeagueData.cs** ⏳
   - Lig bilgileri (DataPack için)
   - leagueName, teams list, country, tier

4. **DataPack.cs** ⏳
   - ScriptableObject (ana data paketi)
   - leagues list, standaloneTeams list, helper methods

**Bağımlılıklar:** PlayerPosition enum ✓

---

### AŞAMA 2: Core Sistemler (Temel Yöneticiler)
**Amaç:** Oyunun temel yönetim sistemlerini oluştur.

**Öncelik:** 🔴 Yüksek (Tüm sistemler bunları kullanır)

1. **GameManager.cs** ⏳
   - Singleton pattern
   - CurrentSave yönetimi
   - Scene geçişleri

2. **DataPackManager.cs** ⏳
   - Singleton pattern
   - DataPack yükleme ve erişim
   - Resources'dan DataPack arama

3. **SaveSystem.cs** ⏳
   - Static class
   - JSON kayıt/yükleme
   - Save/Load/Delete metodları

**Bağımlılıklar:** SaveData (Aşama 3'te oluşturulacak) - ama basit bir SaveData ile başlayabiliriz

---

### AŞAMA 3: Oyun Data Yapıları (Save Data Sistemi)
**Amaç:** Oyun içi kayıt sistemini oluştur. Bu veriler oyuncunun kariyerini temsil eder.

**Öncelik:** 🟡 Orta-Yüksek (Core sistemlerden sonra)

**Yardımcı Data Yapıları (Önce bunlar):**

1. **NewsItem.cs** ⏳
   - title, content, date, type, reaction

2. **SocialMediaPost.cs** ⏳
   - content, author, likes, comments, date, type

3. **OwnedItem.cs** ⏳
   - itemType, tier, purchaseDate

4. **BootsData.cs** ⏳
   - tier, statBonus, durability

5. **LuxuryItem.cs** ⏳
   - luxuryType, value, purchaseDate

6. **TransferOffer.cs** ⏳
   - clubName, salary, role, clauses, duration

7. **InjuryRecord.cs** ⏳
   - injuryType, duration, date, isRecovered

**Ana Data Yapıları:**

8. **ContractData.cs** ⏳
   - salary, bonuses list, clauses list, role, duration, startDate

9. **PlayerProfile.cs** ⏳
   - Temel bilgiler: name, age, nationality, position
   - Attributes: passing, shooting, dribbling, speed, stamina, defending, physical
   - Career stats: goals, assists, matches, ratings

10. **SeasonData.cs** ⏳
    - seasonNumber
    - matches list, goals, assists
    - ratings list, standings

11. **RelationsData.cs** ⏳
    - teammates relations, coach relation, management relation
    - family relations, girlfriend relation, manager relation

12. **EconomyData.cs** ⏳
    - money, ownedItems list, currentBoots, luxuryItems list
    - energyDrinkCount, rehabItemCount

13. **MediaData.cs** ⏳
    - recentNews list, socialMediaPosts list
    - socialMediaFollowers

14. **ClubData.cs** ⏳
    - clubName, leagueName
    - contract (ContractData)
    - objectives list

15. **SaveData.cs** ⏳
    - Ana kayıt dosyası
    - playerProfile, clubData, seasonData, relationsData, economyData, mediaData
    - saveDate, version

**Bağımlılıklar:** Tüm enum'lar ✓, Yardımcı data yapıları

---

### AŞAMA 4: Oyun Sistemleri
**Amaç:** Oyun mekaniklerini yöneten sistemleri oluştur.

**Öncelik:** 🟢 Orta (UI'lardan önce hazır olmalı)

**Temel Sistemler:**

1. **GameStateManager.cs** ⏳
   - Oyun durumu yönetimi (MainMenu, CareerHub, Match, vb.)
   - Scene geçiş kontrolü

**Core Sistemler:**

2. **NewsSystem.cs** ⏳
   - Singleton
   - Haber üretimi, haber listesi
   - GetRecentNews, RecordNewsReaction

3. **SocialMediaSystem.cs** ⏳
   - Singleton
   - Post üretimi, takipçi sistemi
   - CreatePost, LikePost, CommentPost

4. **MarketSystem.cs** ⏳
   - Singleton
   - Krampon ve lüks eşya satışı
   - BuyCleats, BuyLuxuryItem

5. **TransferSystem.cs** ⏳
   - Transfer işlemleri
   - AcceptOffer, RejectOffer

6. **TransferAISystem.cs** ⏳
   - TIS hesaplama, teklif oluşturma
   - GenerateTransferOffer

7. **ManagerAISystem.cs** ⏳
   - Deal Score, pazarlık sistemi
   - EvaluateOffer, Negotiate

8. **TrainingSystem.cs** ⏳
   - Skill artırma sistemi
   - TrainSkill

9. **InjurySystem.cs** ⏳
   - Sakatlık oluşturma ve iyileşme
   - GenerateInjury, RecoverFromInjury

10. **FormMoralEnergySystem.cs** ⏳
    - Form, moral, enerji yönetimi
    - UpdateForm, UpdateMoral, UpdateEnergy

11. **SeasonCalendarSystem.cs** ⏳
    - Sezon takvimi ve maç programı
    - GetNextMatch, GetMatchesForWeek

12. **ClubGoalsSystem.cs** ⏳
    - Kulüp hedefleri yönetimi
    - CheckObjectives, UpdateObjectiveStatus

13. **NationalTeamSystem.cs** ⏳
    - Milli takım sistemi
    - CheckSelectionCriteria

14. **BriberySystem.cs** ⏳
    - Rüşvet sistemi
    - AttemptBribery

15. **CommentatorSystem.cs** ⏳
    - Spiker yorum sistemi
    - GenerateCommentary, LoadTemplates

16. **CriticalEventSystem.cs** ⏳
    - Kritik olaylar sistemi
    - TriggerEvent, ShowEvent

**Bağımlılıklar:** Tüm data yapıları, Core sistemler

---

### AŞAMA 5: UI Sistemleri
**Amaç:** Kullanıcı arayüzlerini oluştur.

**Öncelik:** 🟢 Düşük-Orta (Son adım)

**Ana Menü UI:**
1. **MainMenuUI.cs** ⏳

**Kayıt Menüleri:**
2. **SaveSlotsMenu.cs** ⏳
3. **SaveSlotUI.cs** ⏳
4. **DataPackMenuUI.cs** ⏳

**Kariyer Menüleri:**
5. **CharacterCreationUI.cs** ⏳
6. **TeamOfferUI.cs** ⏳
7. **CareerHubUI.cs** ⏳
8. **HomePanelUI.cs** ⏳
9. **NewsUI.cs** ⏳
10. **SocialMediaUI.cs** ⏳
11. **MarketUI.cs** ⏳
12. **TrainingUI.cs** ⏳
13. **LifeUI.cs** ⏳
14. **PlayerStatsScreenUI.cs** ⏳

**Maç UI'ları:**
15. **MatchPreScreenUI.cs** ⏳
16. **MatchUI.cs** ⏳
17. **PostMatchScreenUI.cs** ⏳

**Diğer UI'lar:**
18. **CriticalEventPopUpUI.cs** ⏳
19. **StandingsUI.cs** ⏳
20. **SettingsUI.cs** ⏳

**Bağımlılıklar:** Tüm sistemler, Data yapıları

---

## 🎯 Önerilen İlk Adım

**AŞAMA 1'i başlatalım: Temel Data Yapıları**

1. Önce **PlayerData.cs** oluşturalım
2. Sonra **TeamData.cs**
3. Sonra **LeagueData.cs**
4. En son **DataPack.cs** (ScriptableObject)

Bu sıralama mantıklı çünkü:
- DataPack diğerlerine bağımlı
- LeagueData TeamData'ya bağımlı
- TeamData PlayerData'ya bağımlı
- PlayerData sadece enum'lara bağımlı (✓ hazır)

---

## 📊 İlerleme Takibi

- ✅ **Enum'lar:** Tamamlandı (17 enum)
- ✅ **Aşama 1 (Temel Data):** 4/4 (PlayerData, TeamData, LeagueData, DataPack)
- ✅ **Aşama 2 (Core Sistemler):** 4/4 (GameManager, DataPackManager, SaveSystem, GameStateManager)
- ✅ **Aşama 3 (Oyun Data):** 15/15 (7 Yardımcı + 8 Ana Data yapısı)
- ⏳ **Aşama 4 (Oyun Sistemleri):** 0/16
- ⏳ **Aşama 5 (UI):** 0/20

**Toplam İlerleme:** 1/58 (≈2%)

