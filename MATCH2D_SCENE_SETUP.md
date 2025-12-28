# 2D Pozisyon Sahnesi Kurulum Rehberi

Bu rehber, `MatchChanceScene` sahnesini Unity'de kurmak için gereken tüm adımları içerir.

## 📋 İçindekiler

1. [Prefab Oluşturma](#1-prefab-oluşturma)
2. [MatchChanceScene Sahnesi Oluşturma](#2-matchchancescene-sahnesi-oluşturma)
3. [Saha Görseli Ekleme](#3-saha-görseli-ekleme)
4. [Script'leri Bağlama](#4-scriptleri-bağlama)
5. [UI Elemanlarını Ekleme](#5-ui-elemanlarını-ekleme)
6. [Kamera Ayarları](#6-kamera-ayarları)
7. [Test Etme](#7-test-etme)

---

## 1. Prefab Oluşturma

### Adım 1.1: Unity Editor'da Prefab'ları Oluştur

1. Unity Editor'ı açın
2. Üst menüden: **TitanSoccer > Create 2D Match Prefabs**
3. Başarı mesajı görünecek
4. Prefab'lar şu konumda oluşturulacak:
   - `Assets/Prefabs/Player.prefab`
   - `Assets/Prefabs/Ball.prefab`

### Adım 1.2: Prefab'ları Kontrol Et

Her iki prefab'ın da şu component'leri olduğundan emin olun:

**Player.prefab:**
- ✅ SpriteRenderer
- ✅ Rigidbody2D (gravity: 0, drag: 5)
- ✅ CircleCollider2D (radius: 0.4)
- ✅ PlayerController script
- ✅ Tag: "Player"

**Ball.prefab:**
- ✅ SpriteRenderer
- ✅ Rigidbody2D (gravity: 0, drag: 2, mass: 0.5)
- ✅ CircleCollider2D (radius: 0.3, isTrigger: true)
- ✅ BallController script
- ✅ Tag: "Ball"

---

## 2. MatchChanceScene Sahnesi Oluşturma

### Adım 2.1: Yeni Sahne Oluştur

1. `Assets/Scenes/` klasöründe sağ tık
2. **Create > Scene**
3. İsim: `MatchChanceScene`

### Adım 2.2: Build Settings'e Ekle

1. **File > Build Settings**
2. `MatchChanceScene` sahnesini açın
3. **Add Open Scenes** butonuna tıklayın

---

## 3. Saha Görseli Ekleme

### Adım 3.1: Saha Sprite'ı Oluştur (Geçici)

1. Sahne içinde boş bir GameObject oluşturun: **GameObject > Create Empty**
2. İsim: `Field`
3. `SpriteRenderer` component ekleyin
4. **Geçici olarak** yeşil bir sprite oluşturun veya basit bir Texture2D ekleyin

**Not:** İleride gerçek bir futbol sahası görseli ile değiştirilebilir.

### Adım 3.2: Saha Boyutunu Ayarlayın

1. `Field` GameObject'ini seçin
2. Transform: Scale (20, 30, 1) - Bu FieldManager'daki fieldWidth ve fieldHeight ile eşleşmeli

---

## 4. Script'leri Bağlama

### Adım 4.1: Ana Manager GameObject'i

1. Boş bir GameObject oluşturun: **GameObject > Create Empty**
2. İsim: `MatchChanceSceneManager`
3. `MatchChanceSceneManager` script'ini ekleyin
4. Inspector'da ayarlar:
   - **Slow Motion Speed**: 0.2 (varsayılan)
   - **Max Time Amount**: 10 (varsayılan)

### Adım 4.2: FieldManager GameObject'i

1. Boş bir GameObject oluşturun: **GameObject > Create Empty**
2. İsim: `FieldManager`
3. `FieldManager` script'ini ekleyin
4. Inspector'da ayarlar:
   - **Field Width**: 20
   - **Field Height**: 30
   - **Player Prefab**: `Assets/Prefabs/Player.prefab` (sürükleyip bırakın)

### Adım 4.3: AISystem GameObject'i

1. Boş bir GameObject oluşturun: **GameObject > Create Empty**
2. İsim: `AISystem`
3. `AISystem` script'ini ekleyin
4. Inspector'da ayarlar (varsayılan değerler uygundur)

### Adım 4.4: Ball GameObject'i

1. `Assets/Prefabs/Ball.prefab`'ı sahneye sürükleyin
2. Pozisyon: (0, 0, 0)
3. İsim: `Ball` (veya bırakın)

### Adım 4.5: Input Handler Ekleme

1. Canvas oluşturun: **GameObject > UI > Canvas**
2. Canvas'ı seçin
3. **Add Component > MatchChanceInputHandler** script'ini ekleyin
4. Inspector'da:
   - **Double Tap Time**: 0.3 (varsayılan)

---

## 5. UI Elemanlarını Ekleme

### Adım 5.1: Canvas Kontrolü

1. Canvas GameObject'ini seçin
2. Canvas component ayarları:
   - **Render Mode**: Screen Space - Overlay
   - **Canvas Scaler**: Scale With Screen Size

### Adım 5.2: MatchChanceUI Script'i

1. Canvas'a `MatchChanceUI` script'ini ekleyin
2. Inspector'da tüm referansları bağlayın:

#### Zaman Barı
1. **GameObject > UI > Slider** oluşturun
2. İsim: `TimeBarSlider`
3. Canvas içinde üst kısma yerleştirin
4. `MatchChanceUI` script'inde **Time Bar Slider** alanına sürükleyin
5. Slider'ın **Fill** alanını **Time Bar Fill**'e bağlayın
6. **GameObject > UI > TextMeshPro - Text** oluşturun (Slider'ın yanına)
7. İsim: `TimeBarText`
8. `MatchChanceUI` script'inde **Time Bar Text** alanına bağlayın

#### Pas Zinciri
1. **GameObject > UI > Panel** oluşturun
2. İsim: `PassChainPanel`
3. İçine iki TextMeshPro ekleyin:
   - `PassChainText` (Pas Zinciri: X)
   - `PassChainBonusText` (Bonus metni)
4. `MatchChanceUI` script'inde referansları bağlayın
5. Başlangıçta panel'i gizleyin (Inspector'da aktif değil)

#### Pozisyon Bilgileri
1. **GameObject > UI > TextMeshPro - Text** oluşturun
2. İsim: `MinuteText`
3. `MatchChanceUI` script'inde **Minute Text** alanına bağlayın

1. **GameObject > UI > TextMeshPro - Text** oluşturun
2. İsim: `PositionTypeText`
3. `MatchChanceUI` script'inde **Position Type Text** alanına bağlayın

#### Spiker Metni
1. **GameObject > UI > TextMeshPro - Text** oluşturun
2. İsim: `CommentatorText`
3. Alt kısma yerleştirin
4. `MatchChanceUI` script'inde **Commentator Text** alanına bağlayın

---

## 6. Kamera Ayarları

### Adım 6.1: Main Camera'yı Ayarlayın

1. **Main Camera**'yı seçin
2. `MatchCamera` script'ini ekleyin
3. Inspector'da ayarlar:
   - **Follow Speed**: 5 (varsayılan)
   - **Camera Distance**: 10 (varsayılan)
   - **Camera Angle**: 30 (varsayılan)

### Adım 6.2: Kamera Pozisyonu

1. Main Camera Transform:
   - Position: (0, 0, -10)
   - Rotation: (30, 0, 0)

### Adım 6.3: Kamera Sınırları (Opsiyonel)

`MatchCamera` script'inde sahne boyutuna göre ayarlayın:
- **Min X**: -10
- **Max X**: 10
- **Min Y**: -15
- **Max Y**: 15

---

## 7. Test Etme

### Adım 7.1: Scene Hierarchy Kontrolü

Sahneniz şu şekilde görünmeli:

```
MatchChanceScene
├── Main Camera (MatchCamera script)
├── MatchChanceSceneManager (MatchChanceSceneManager script)
├── FieldManager (FieldManager script)
├── AISystem (AISystem script)
├── Ball (Ball prefab)
└── Canvas
    ├── MatchChanceUI (MatchChanceUI script)
    ├── MatchChanceInputHandler (MatchChanceInputHandler script)
    ├── TimeBarSlider
    ├── PassChainPanel
    ├── MinuteText
    ├── PositionTypeText
    └── CommentatorText
```

### Adım 7.2: Test Senaryosu

1. `MatchChanceScene` sahnesini açın
2. Play'e basın
3. Console'da hataları kontrol edin
4. `FieldManager` otomatik olarak oyuncuları oluşturacak
5. `MatchChanceSceneManager` pozisyonu başlatacak

### Adım 7.3: Olası Hatalar ve Çözümleri

**Hata: "No save data available!"**
- `GameManager.Instance` veya `CurrentSave` null
- Test için `MatchChanceScene`'i direkt açmak yerine maç akışından gelmek gerekir

**Hata: "Player prefab is null"**
- `FieldManager`'da **Player Prefab** alanını kontrol edin
- Prefab'ın doğru yolda olduğundan emin olun

**Hata: "BallController.Instance is null"**
- Ball GameObject'inin sahneye ekli olduğundan emin olun
- `BallController` script'inin ekli olduğunu kontrol edin

---

## 8. Entegrasyon Kontrolü

### Adım 8.1: GameStateManager Kontrolü

`GameStateManager.cs`'de `MatchChance` state'i ekli olmalı:
- ✅ `GameState.MatchChance` enum'da mevcut
- ✅ `GetSceneNameForState()` içinde `"MatchChanceScene"` mapping'i var
- ✅ `SetStateFromScene()` içinde `"MatchChanceScene"` mapping'i var

### Adım 8.2: MatchUI Kontrolü

`MatchUI.cs`'de `HandlePlayerChance()` metodu `MatchChance` sahnesine yönlendirmeli.

### Adım 8.3: MatchChanceManager Kontrolü

`MatchChanceManager.cs` static class mevcut ve `CurrentChance` property'si var.

---

## 9. Son Kontroller

- [ ] Prefab'lar oluşturuldu (`Player.prefab`, `Ball.prefab`)
- [ ] `MatchChanceScene` sahnesi oluşturuldu
- [ ] Tüm script'ler doğru GameObject'lere bağlandı
- [ ] UI elemanları oluşturuldu ve `MatchChanceUI` script'ine bağlandı
- [ ] Kamera `MatchCamera` script'i ile yapılandırıldı
- [ ] `FieldManager`'da `Player Prefab` referansı atandı
- [ ] Sahne Build Settings'e eklendi
- [ ] Test edildi (hata yok)

---

## 🎮 Nasıl Çalışır?

1. Maç simülasyonu sırasında pozisyon geldiğinde
2. `MatchSimulationSystem` `OnPlayerChance` event'i fırlatır
3. `MatchUI` bu event'i yakalar ve `MatchChanceScene`'e geçer
4. `MatchChanceSceneManager` pozisyonu başlatır:
   - Zaman kırılması başlar (%20 hız)
   - Zaman barı aktif olur
5. Oyuncu kontrolleri:
   - Tek dokunma → Koşu/Yerden pas
   - Çift dokunma → Havadan pas
   - Basılı tut + sürükle → Şut
6. AI sistemi çalışır:
   - Savunma baskı yapar
   - Hücum boş alana koşar
7. Pozisyon sonuçlandığında `Match` sahnesine geri dönülür

---

## 💡 İpuçları

- **Sprite'lar**: Şu an basit renkli sprite'lar kullanılıyor. İleride gerçek oyuncu ve top sprite'ları ile değiştirilebilir.
- **Formasyon**: Şu an basit 4-4-2 formasyonu kullanılıyor. İleride farklı formasyonlar eklenebilir.
- **Saha Görseli**: Geçici bir yeşil alan kullanılıyor. İleride gerçek futbol sahası görseli eklenebilir.
- **UI Tasarımı**: UI elementlerinin pozisyonları ve boyutları oyun tasarımına göre ayarlanabilir.

---

**Hazırlayan:** AI Assistant  
**Tarih:** 2024  
**Versiyon:** 1.0
