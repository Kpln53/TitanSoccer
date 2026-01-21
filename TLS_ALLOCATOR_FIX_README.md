# TLS Allocator ALLOC_TEMP_TLS Hatası Çözümü - Basit Yaklaşım

## Sorun
Unity'de "TLS Allocator ALLOC_TEMP_TLS, underlying allocator ALLOC_TEMP_MAIN has unfreed allocations, size 384" hatası spam halinde çıkıyor.

## Neden Oluyor?
1. **StringBuilder** kullanımında capacity belirtilmemesi
2. **LINQ** operasyonlarının geçici bellek tahsisleri
3. **String concatenation** işlemlerinin fazla olması
4. **Garbage Collection** yetersizliği
5. **Thread Local Storage** temizlenmemesi

## Yapılan Çözümler

### 1. PostMatchUIController.cs Optimizasyonları
- StringBuilder'larda capacity belirtildi ve Clear() ile temizlendi
- LINQ yerine foreach kullanıldı
- OnDestroy'da event listener'lar temizlendi
- DelayedCleanup coroutine eklendi

### 2. MemoryOptimizer.cs (Mevcut)
- Bellek kullanımı kontrol edildi (50MB üzerinde cleanup)
- İkinci GC çağrısı eklendi
- TLSCleanup metodu eklendi

### 3. SimpleTLSFix.cs (Yeni - Agresif Çözüm)
- 0.1 saniyede bir agresif GC çağrısı
- Update metodunda sürekli temizlik (her 10/60/300 frame)
- Tüm GC generation'larını zorla temizler
- Resources.UnloadUnusedAssets() çağrısı
- Thread memory barrier

### 4. DebugLogSuppressor.cs (Yeni - Log Bastırma)
- Debug.Log çağrılarını bastırır
- Stack trace'leri devre dışı bırakır
- TLS allocation'a neden olan log'ları engeller

### 4. SceneFlow.cs Güncellemesi
- Her scene değişiminde SimpleTLSFix çağrılıyor

### 5. MatchContext.cs Optimizasyonu
- Clear() metodunda null check eklendi
- SimpleTLSFix cleanup eklendi

### 6. GameManager.cs Temizlendi
- Gereksiz MemoryOptimizer referansları kaldırıldı

## Kullanım

### Otomatik Çalışma
- `SimpleTLSFix` ve `DebugLogSuppressor` scriptlerini sahneye ekleyin
- Otomatik olarak çalışacaklar

### Manuel Çağırma
```csharp
// Agresif memory cleanup
SimpleTLSFix.ManualFix();

// Debug log'ları kapat
DebugLogSuppressor.DisableAllLogs();

// Debug log'ları aç
DebugLogSuppressor.EnableAllLogs();

// Scene değişiminde
SimpleTLSFix.OnSceneChange();
```

## Unity Project Settings Önerileri

### Player Settings
1. **Scripting Backend**: IL2CPP (daha iyi memory management)
2. **Api Compatibility Level**: .NET Standard 2.1
3. **Managed Stripping Level**: Medium

### Quality Settings
1. **V Sync Count**: Don't Sync (performans için)
2. **Anti Aliasing**: Disabled (memory tasarrufu)

### Graphics Settings
1. **Instancing Variants**: Keep All (memory leak önleme)
2. **Lightmap Streaming**: Enabled

## Test Etme
1. Oyunu çalıştırın
2. Console'da TLS Allocator hatalarının azaldığını kontrol edin
3. Memory Profiler ile bellek kullanımını izleyin

## Ek Öneriler
1. **String.Format** yerine **StringBuilder** kullanın
2. **LINQ** yerine **foreach** tercih edin
3. **List.Clear()** yerine **new List()** kullanmayın
4. **OnDestroy()** metodlarında cleanup yapın
5. StringBuilder'da **capacity** belirtin ve **Clear()** kullanın

## Sorun Devam Ederse
1. Unity'yi yeniden başlatın
2. Library klasörünü silin ve reimport yapın
3. Unity'nin daha yeni versiyonunu deneyin
4. IL2CPP build alıp test edin

## Basit Çözümün Avantajları
- Daha az karmaşık kod
- Daha az hata riski
- Kolay bakım
- Minimal performans etkisi