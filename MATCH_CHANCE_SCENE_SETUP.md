# MatchChanceScene Kurulum Rehberi

Bu rehber, MatchChance sahnesini Unity'de kurmak için gereken tüm adımları içerir.

## 📋 İçindekiler

1. [Scene Oluşturma](#1-scene-oluşturma)
2. [Temel GameObject'ler](#2-temel-gameobjectler)
3. [Script'leri Ekleme](#3-scriptleri-ekleme)
4. [UI Kurulumu](#4-ui-kurulumu)
5. [Layer ve Tag Ayarları](#5-layer-ve-tag-ayarları)
6. [Test Etme](#6-test-etme)

---

## 1. Scene Oluşturma

### Adım 1.1: Yeni Scene Oluştur

1. Unity Editor'da `Assets/Scenes/` klasöründe **sağ tık**
2. **Create > Scene**
3. İsim: `MatchChanceScene`
4. Sahneyi aç

### Adım 1.2: Build Settings'e Ekle

1. **File > Build Settings**
2. `MatchChanceScene` sahnesini aç
3. **Add Open Scenes** butonuna tıkla

---

## 2. Temel GameObject'ler

### Adım 2.1: Main Camera

1. Sahne zaten `Main Camera` ile gelir
2. **Main Camera**'yı seç
3. **Add Component > MatchCamera** ekle
4. Inspector'da ayarlar (varsayılan değerler uygundur)

### Adım 2.2: FieldManager

1. **GameObject > Create Empty**
2. İsim: `FieldManager`
3. **Add Component > FieldManager** ekle
4. Inspector'da:
   - **Field Width**: 20
   - **Field Length**: 30
   - **Formation**: FourFourTwo
   - Player ve Ball prefab'ları **boş bırakılabilir** (runtime'da otomatik oluşturulur)

### Adım 2.3: MatchChanceSceneManager

1. **GameObject > Create Empty**
2. İsim: `MatchChanceSceneManager`
3. **Add Component > MatchChanceSceneManager** ekle
4. Inspector'da ayarlar:
   - **Slow Motion Speed**: 0.2
   - **Max Time Amount**: 10

### Adım 2.4: AISystem

1. **GameObject > Create Empty**
2. İsim: `AISystem`
3. **Add Component > AISystem** ekle
4. Inspector'da varsayılan değerler uygundur

### Adım 2.5: TimeFlowManager

1. **GameObject > Create Empty**
2. İsim: `TimeFlowManager`
3. **Add Component > TimeFlowManager** ekle
4. Inspector'da:
   - **Slow Motion Scale**: 0.2
   - **Transition Duration**: 0.3

### Adım 2.6: ShotAimSystem

1. **GameObject > Create Empty**
2. İsim: `ShotAimSystem`
3. **Add Component > ShotAimSystem** ekle
4. Inspector'da:
   - **Min Shot Power**: 8
   - **Max Shot Power**: 30
   - **Max Line Length**: 10
   - Camera referansı otomatik bulunacak

### Adım 2.7: PassSystem

1. **GameObject > Create Empty**
2. İsim: `PassSystem`
3. **Add Component > PassSystem** ekle
4. Inspector'da:
   - **Pass Speed**: 12
   - Referanslar runtime'da otomatik ayarlanacak

---

## 3. UI Kurulumu

### Adım 3.1: Canvas Oluştur

1. **GameObject > UI > Canvas**
2. Canvas'ı seç
3. Inspector'da:
   - **Render Mode**: Screen Space - Overlay
   - **Canvas Scaler > UI Scale Mode**: Scale With Screen Size
   - **Reference Resolution**: 1080 x 1920

### Adım 3.2: MatchChanceInputHandler

1. Canvas'ı seç
2. **Add Component > MatchChanceInputHandler** ekle
3. Inspector'da:
   - **Double Tap Time**: 0.3
   - Referanslar runtime'da otomatik bulunacak

### Adım 3.3: EventSystem Kontrolü

1. Scene'de `EventSystem` olmalı (Unity otomatik ekler)
2. Yoksa: **GameObject > UI > Event System**

**NOT:** UI elementleri kaldırıldı - sadece saf oynanış. Canvas sadece EventSystem için gerekli.

---

**UI Elementleri Kaldırıldı:**
- ❌ Zaman barı kaldırıldı
- ❌ Pas zinciri kaldırıldı
- ❌ Pozisyon bilgileri kaldırıldı
- ❌ Spiker metni kaldırıldı
- ✅ Sadece saf oynanış kalıyor

---

## 5. Layer ve Tag Ayarları

### Adım 5.1: Layer'ları Oluştur

1. **Edit > Project Settings > Tags and Layers**
2. **Layers** bölümünde:
   - **Layer 8**: `Player`
   - **Layer 9**: `Ball`
   - **Layer 10**: `Ground` (isteğe bağlı)

### Adım 5.2: Tag'leri Oluştur

1. Aynı pencerede **Tags** bölümü:
   - `Player` tag'i ekle (varsa atla)
   - `Ball` tag'i ekle (varsa atla)

---

## 6. Final Kontroller

### Adım 6.1: Hierarchy Kontrolü

Sahneniz şu şekilde görünmeli:

```
MatchChanceScene
├── Main Camera (MatchCamera script)
├── FieldManager (FieldManager script)
├── MatchChanceSceneManager (MatchChanceSceneManager script)
├── AISystem (AISystem script)
├── TimeFlowManager (TimeFlowManager script)
├── ShotAimSystem (ShotAimSystem script)
├── PassSystem (PassSystem script)
├── EventSystem
└── Canvas
    └── MatchChanceInputHandler (MatchChanceInputHandler script)
```

**Not:** UI elementleri kaldırıldı - sadece oynanış var.

### Adım 6.2: Script Referansları (Runtime'da otomatik)

Aşağıdaki referanslar **runtime'da otomatik bulunacak**:

- `MatchChanceInputHandler` → PlayerController, ShotAimSystem, PassSystem
- `ShotAimSystem` → Camera
- `PassSystem` → PlayerController, BallController
- `AISystem` → FieldManager
- `MatchCamera` → FieldManager (event'lerle bağlanır)

**Manuel bağlamanız gerekenler:**

✅ Hiçbir şey! Tüm referanslar runtime'da otomatik bulunur.
✅ `FieldManager` içindeki prefab referansları (opsiyonel - yoksa runtime'da oluşturur)

---

## 7. Test Senaryosu

### Adım 7.1: Test için MatchChanceData Hazırlama

Test için `MatchChanceManager`'a veri ekle:

```csharp
// Test için (örnek bir script oluştur):
MatchChanceData testChance = new MatchChanceData
{
    minute = 15,
    chanceType = MatchChanceType.Shot,
    description = "Şut fırsatı!",
    successChance = 0.6f
};
MatchChanceManager.CurrentChance = testChance;
```

Veya direkt `MatchChanceSceneManager.Start()` metodunu düzenleyerek test edebilirsin.

### Adım 7.2: Play ve Kontrol

1. Play'e bas
2. Console'da hataları kontrol et
3. FieldManager otomatik oyuncuları spawn edecek
4. TimeFlowManager zaman yönetimini başlatacak
5. Kamera sahneyi gösterecek

### Adım 7.3: Oynanış Testi (Saf Oynanış)

1. **Hareket**: Sahada boş bir yere tıkla → Oyuncu koşar
2. **Şut**: Oyuncuya basılı tut → Çizgi çiz → Bırak → Şut atılır
3. **Pas**: Takım arkadaşına tıkla → Pas atılır

**Not:** Hiç UI yok - sadece saf oynanış!

---

## 8. Olası Hatalar ve Çözümleri

### Hata: "NullReferenceException: FieldManager is null"

**Çözüm:** FieldManager GameObject'inin sahneye ekli olduğundan emin ol.

### Hata: "Player prefab is null"

**Çözüm:** FieldManager'da prefab'lar boş bırakılabilir - runtime'da otomatik oluşturulur. Veya Resources klasörüne prefab ekle.

### Hata: "MatchChanceManager.CurrentChance is null"

**Çözüm:** Test için Start() metodunda veri oluştur veya Match simülasyonundan sahneye geç.

### Hata: "Layer 'Player' not found"

**Çözüm:** Edit > Project Settings > Tags and Layers'da Layer 8'e "Player" ekle.

### Hata: "Tag 'Ball' not found"

**Çözüm:** Tags bölümüne "Ball" tag'i ekle.

---

## 9. Entegrasyon Kontrolü

### GameStateManager Kontrolü

`GameStateManager.cs`'de `MatchChance` state'i olmalı:

```csharp
public enum GameState
{
    // ...
    MatchChance,  // ✅ Olmalı
    // ...
}
```

Ve scene mapping:

```csharp
case GameState.MatchChance:
    return "MatchChanceScene";
```

---

## ✅ Tamamlandı!

Artık MatchChance sahnesi hazır! Test edebilirsin. 🎮

**Sonraki Adımlar:**
- Match simülasyonundan MatchChance sahnesine geçiş entegrasyonu
- Şut sonuçlarının MatchChanceSceneManager'a bildirilmesi
- Pas sonuçlarının işlenmesi

---

**Hazırlayan:** AI Assistant  
**Tarih:** 2025  
**Versiyon:** 1.0
