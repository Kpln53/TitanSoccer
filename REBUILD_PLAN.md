# 🏗️ Script Yeniden Yapılandırma Planı

## 📋 Yapılacaklar Listesi

### 1. 🔢 Enum'lar (Temel Tanımlar)
**Dosya:** `Assets/Scripts/Data/Enums.cs`
- PlayerPosition (KL, STP, SĞB, SLB, MDO, MOO, SĞK, SLK, SĞO, SLO, SF)
- ContractRole (Starter, Rotation, Substitute)
- PlayingTime (Starter, Rotation, Substitute)
- ClauseType (ReleaseClause, BuyoutClause, vb.)
- BonusType (SigningBonus, MatchFee, GoalBonus, AssistBonus, CleanSheet, SeasonEnd, TeamSuccess, Loyalty)
- NewsType (Transfer, League, Injury, Achievement, Rumour, vb.)
- NewsReaction (Positive, Negative, Neutral)
- SocialMediaPostType (Normal, Match, Transfer, Achievement)
- ItemType (Cleats, Luxury, EnergyDrink, RehabItem)
- CleatsTier (Basic, Standard, Premium, Elite)
- LuxuryType (House, Car, Watch, Jewelry)
- ManagerType (Strict, Supportive, Balanced)
- ClubObjective (LeaguePosition, CupWin, ChampionsLeague, RelegationAvoid)
- TransferWindowType (Summer, Winter)
- TransferOfferType (Permanent, Loan)
- CriticalEventType (TransferOffer, Injury, ContractRenewal, vb.)
- CommentTrigger (Goal, Assist, Save, Foul, MatchStart, HalfTime, vb.)

### 2. 📦 Data Yapıları

#### 2.1. Temel Data (Data Pack için)
- **TeamData.cs** - Takım bilgileri (teamName, players, teamPower, vb.)
- **PlayerData.cs** - DataPack için oyuncu verisi (zaten var, kontrol et)
- **LeagueData.cs** - Lig bilgileri (zaten var, kontrol et)
- **DataPack.cs** - ScriptableObject (zaten var, kontrol et)

#### 2.2. Oyun Data Yapıları
- **PlayerProfile.cs** - Oyuncu profili (attributes, age, nationality, career stats)
- **ClubData.cs** - Kulüp bilgileri (clubName, league, contract, objectives)
- **SeasonData.cs** - Sezon verileri (matches, goals, assists, ratings, standings)
- **RelationsData.cs** - İlişkiler (teammates, coach, management, family, girlfriend, manager)
- **EconomyData.cs** - Ekonomi (money, items, cleats, luxury items)
- **MediaData.cs** - Medya (news, social media posts, followers)
- **ContractData.cs** - Sözleşme (salary, bonuses, clauses, role, duration)
- **SaveData.cs** - Ana kayıt dosyası (tüm data yapılarını içeren)

#### 2.3. Yardımcı Data Yapıları
- **NewsItem.cs** - Haber item'ı (title, content, date, type, reaction)
- **SocialMediaPost.cs** - Sosyal medya postu (content, likes, comments, date)
- **OwnedItem.cs** - Sahip olunan item (type, tier, purchaseDate)
- **BootsData.cs** - Krampon verisi (tier, statBonus, durability)
- **LuxuryItem.cs** - Lüks eşya (type, value, purchaseDate)
- **TransferOffer.cs** - Transfer teklifi (club, salary, role, clauses)
- **InjuryRecord.cs** - Sakatlık kaydı (type, duration, date)

### 3. 🎮 Core Sistemler

#### 3.1. Ana Yöneticiler
- **GameManager.cs** - Oyun durumu yönetimi (singleton, CurrentSave, scene management)
- **DataPackManager.cs** - DataPack yükleme ve erişim (singleton)
- **GameStateManager.cs** - Oyun durumu geçişleri (MainMenu, CareerHub, Match, vb.)
- **SaveSystem.cs** - Kayıt/yükleme sistemi (zaten var, kontrol et)

#### 3.2. Oyun Sistemleri
- **NewsSystem.cs** - Haber sistemi (singleton, haber üretimi, haber listesi)
- **SocialMediaSystem.cs** - Sosyal medya sistemi (singleton, post üretimi, takipçi)
- **MarketSystem.cs** - Market sistemi (krampon, lüks eşya satışı)
- **TransferSystem.cs** - Transfer sistemi (teklif kabul/red, transfer işlemleri)
- **TransferAISystem.cs** - Transfer AI (TIS hesaplama, teklif oluşturma)
- **ManagerAISystem.cs** - Menajer AI (Deal Score, pazarlık)
- **TrainingSystem.cs** - Antrenman sistemi (skill artırma)
- **InjurySystem.cs** - Sakatlık sistemi (sakatlık oluşturma, iyileşme)
- **FormMoralEnergySystem.cs** - Form/Moral/Enerji sistemi
- **SeasonCalendarSystem.cs** - Sezon takvimi (maç programı)
- **ClubGoalsSystem.cs** - Kulüp hedefleri sistemi
- **NationalTeamSystem.cs** - Milli takım sistemi
- **BriberySystem.cs** - Rüşvet sistemi
- **CommentatorSystem.cs** - Spiker sistemi (yorum üretimi)
- **CriticalEventSystem.cs** - Kritik olaylar sistemi (popup olaylar)

### 4. 🎨 UI Sistemleri

#### 4.1. Ana Menü UI
- **MainMenuUI.cs** - Ana menü (zaten var, kontrol et)

#### 4.2. Kayıt Menüleri
- **SaveSlotsMenu.cs** - Kayıt slotları (zaten var, kontrol et)
- **SaveSlotUI.cs** - Kayıt slotu UI (zaten var, kontrol et)
- **DataPackMenuUI.cs** - DataPack seçim menüsü (zaten var, kontrol et)

#### 4.3. Kariyer Menüleri
- **CharacterCreationUI.cs** - Karakter oluşturma (zaten var, kontrol et)
- **TeamOfferUI.cs** - Takım teklifi ekranı
- **CareerHubUI.cs** - Kariyer hub (zaten var, güncelle)
- **HomePanelUI.cs** - Ana panel (maç bilgisi, hızlı erişim)
- **NewsUI.cs** - Haberler paneli
- **SocialMediaUI.cs** - Sosyal medya paneli
- **MarketUI.cs** - Market paneli
- **TrainingUI.cs** - Antrenman paneli
- **LifeUI.cs** - Hayat paneli (ilişkiler, lüks, kramponlar)
- **PlayerStatsScreenUI.cs** - Oyuncu istatistikleri ekranı

#### 4.4. Maç UI'ları
- **MatchPreScreenUI.cs** - Maç öncesi ekran
- **MatchUI.cs** - Maç ekranı
- **PostMatchScreenUI.cs** - Maç sonrası ekran

#### 4.5. Diğer UI'lar
- **CriticalEventPopUpUI.cs** - Kritik olay popup
- **StandingsUI.cs** - Puan durumu ekranı
- **SettingsUI.cs** - Ayarlar ekranı

### 5. 📁 Klasör Yapısı

```
Assets/Scripts/
├── Core/
│   ├── GameManager.cs
│   ├── DataPackManager.cs
│   ├── GameStateManager.cs
│   └── Systems/ (tüm sistemler buraya)
│       ├── NewsSystem.cs
│       ├── SocialMediaSystem.cs
│       ├── MarketSystem.cs
│       ├── TransferSystem.cs
│       └── ...
├── Data/
│   ├── Enums.cs
│   ├── TeamData.cs
│   ├── PlayerData.cs
│   ├── LeagueData.cs
│   ├── DataPack.cs
│   ├── PlayerProfile.cs
│   ├── ClubData.cs
│   ├── SeasonData.cs
│   ├── RelationsData.cs
│   ├── EconomyData.cs
│   ├── MediaData.cs
│   ├── ContractData.cs
│   ├── SaveData.cs
│   └── Helpers/ (yardımcı data yapıları)
│       ├── NewsItem.cs
│       ├── SocialMediaPost.cs
│       └── ...
├── UI/
│   ├── MainMenuUI.cs
│   ├── SaveSlotsMenu.cs
│   └── ... (tüm UI'lar)
├── Match/
│   └── (maç ile ilgili scriptler)
└── Editor/
    └── (editor scriptleri)
```

## ⚡ Uygulama Sırası

1. **Enum'ları oluştur** - Tüm enum'ları tek dosyada topla
2. **Temel Data yapılarını oluştur** - TeamData, PlayerProfile, vb.
3. **SaveData'yı tam yapıyla oluştur** - Tüm data yapılarını içeren ana kayıt
4. **Core sistemleri oluştur** - GameManager, DataPackManager, GameStateManager
5. **Oyun sistemlerini oluştur** - NewsSystem, SocialMediaSystem, vb.
6. **UI scriptlerini güncelle** - Mevcut UI'ları yeni yapıya göre güncelle

## ✅ Kontrol Listesi

- [ ] Enum'lar oluşturuldu
- [ ] Temel Data yapıları oluşturuldu
- [ ] SaveData tam yapıyla oluşturuldu
- [ ] Core sistemler oluşturuldu
- [ ] Oyun sistemleri oluşturuldu
- [ ] UI scriptleri güncellendi
- [ ] Tüm referanslar düzeltildi
- [ ] Compile hataları giderildi

