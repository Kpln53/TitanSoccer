# 2D Maç Sahası Kurulum Rehberi

## Canvas ve Saha Ayarları

### Canvas Ayarları:
- **Canvas Size**: 1080 x 1920 (Portrait)
- **Render Mode**: Screen Space - Overlay
- **Canvas Scaler**: Scale With Screen Size
  - Reference Resolution: 1080 x 1920

### Field GameObject (Canvas Altında):
- **Position**: X: 540, Y: 960, Z: 0
- **Size**: Width: 120, Height: 120
- **Anchor**: Center (0.5, 0.5)
- **Pivot**: Center (0.5, 0.5)

## FieldManager Ayarları:
- **fieldWidth**: 120
- **fieldHeight**: 120
- **fieldCenter**: X: 540.6165, Y: 963.9995 (Inspector'da ayarla)
- **playerPrefab**: Assets/Prefabs/Player.prefab (sürükle-bırak)

## MatchCamera Ayarları:
- **orthographicSize**: 60 (120x120 saha için)
- **minX**: 480.6165 (540.6165 - 60)
- **maxX**: 600.6165 (540.6165 + 60)
- **minY**: 903.9995 (963.9995 - 60)
- **maxY**: 1023.9995 (963.9995 + 60)
- **fieldCenter**: X: 540.6165, Y: 963.9995 (Inspector'da ayarla)

## Formasyon Pozisyonları:
Saha merkezi (540.6165, 963.9995) üzerinden relative pozisyonlar:
- **Kaleci**: Y = ±55 (merkezden)
- **Defans**: Y = ±42, X = -48, -30, +30, +48
- **Orta Saha**: Y = ±12, X = -42, -22, +22, +42
- **Forvet**: Y = ±42, X = -35, +35

## Önemli Notlar:
1. Field GameObject Canvas altında olmalı
2. Field'ın RectTransform pozisyonu (540, 960) olmalı
3. FieldManager ve MatchCamera'da fieldCenter değerlerini Inspector'dan manuel ayarla
4. Oyuncular world space'te spawn olur, fieldCenter offset'i ile

