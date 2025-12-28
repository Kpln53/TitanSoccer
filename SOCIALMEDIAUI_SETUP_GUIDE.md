# 📱 SocialMediaUI (Sosyal Medya Panel) Manuel Kurulum Rehberi

SocialMediaPanel içeriğini Unity Editor'da manuel olarak kurmak için bu rehberi kullan.

## 📋 Gerekli UI Elemanları

SocialMediaPanel içinde şu UI elemanları olmalı:

### 1. 🏷️ Başlık, Takipçi Sayısı ve Geri Butonu

**GameObject:** `SocialMediaHeader` veya istediğin isim

**İçinde olması gerekenler:**
- **TextMeshProUGUI** - `titleText` (Başlık: "📱 Sosyal Medya")
- **TextMeshProUGUI** - `followerCountText` (Takipçi sayısı: "10,000 Takipçi")
- **Button** - `backButton` (Geri butonu: "← Geri")

### 2. 📜 Post Listesi (ScrollView)

**GameObject:** `PostScrollView` (ScrollView component'i ile)

**İçinde olması gerekenler:**
- **ScrollRect** component
- **Viewport** (child GameObject)
- **Content** (Viewport içinde) → Bu `postListParent` olacak

**Yapı:**
```
PostScrollView (ScrollRect)
└── Viewport (Mask + Image)
    └── Content (VerticalLayoutGroup)
        └── (Post item'ları buraya runtime'da eklenecek)
```

### 3. ➕ Yeni Post Butonu

**GameObject:** `NewPostButton` (Button component'i ile)

**İçinde olması gerekenler:**
- Button içinde TextMeshProUGUI → "📝 Yeni Post" veya "+ Yeni Post"

### 4. 📝 Yeni Post Paneli (Modal - Başlangıçta Kapalı)

**GameObject:** `NewPostPanel` (Panel, başlangıçta `SetActive(false)`)

**İçinde olması gerekenler:**
- **TextMeshProUGUI** - Başlık (opsiyonel, "Yeni Post Paylaş" gibi)
- **TMP_InputField** - `postInputField` (Çok satırlı text input, word wrap açık)
- **Button** - `postButton` ("Paylaş" butonu)
- **Button** - `cancelPostButton` ("İptal" butonu)

**Örnek Layout:**
```
NewPostPanel (Panel - başlangıçta kapalı, merkezde, modal)
├── TitleText (TextMeshPro - "Yeni Post Paylaş", opsiyonel)
├── PostInputField (TMP_InputField - Çok satırlı, placeholder: "Ne düşünüyorsun?")
├── ButtonsContainer (Horizontal Layout Group)
│   ├── CancelButton ("İptal")
│   └── PostButton ("Paylaş")
```

### 5. 📄 Post Detay Paneli (Modal - Başlangıçta Kapalı)

**GameObject:** `PostDetailPanel` (Panel, başlangıçta `SetActive(false)`)

**İçinde olması gerekenler:**
- **TextMeshProUGUI** - `postAuthorText` ("@OyuncuAdı" - bold, mavi renk)
- **TextMeshProUGUI** - `postContentText` (Post içeriği - word wrap açık)
- **TextMeshProUGUI** - `postLikesText` ("❤️ 1250" - beğeni sayısı)
- **TextMeshProUGUI** - `postCommentsText` ("💬 45 Yorum" - yorum sayısı)
- **ScrollView** - `commentsScrollView` (Yorum listesi için)
  - **Content** → `commentsParent` (yorum item'ları buraya eklenecek)
- **TMP_InputField** - `commentInputField` (Yorum yazma alanı)
- **Button** - `likePostButton` ("❤️ Beğen")
- **Button** - `commentButton` ("💬 Yorum Yap")
- **Button** - Geri butonu (veya header'da)

**Örnek Layout:**
```
PostDetailPanel (Panel - başlangıçta kapalı, modal)
├── HeaderSection
│   ├── PostAuthorText (@OyuncuAdı - bold, mavi)
│   └── PostContentText (Post içeriği, word wrap açık)
├── StatsSection (Horizontal Layout)
│   ├── PostLikesText (❤️ 1250)
│   └── PostCommentsText (💬 45 Yorum)
├── CommentsScrollView
│   └── Viewport
│       └── CommentsContent (VerticalLayoutGroup) → commentsParent
├── InteractionSection (Horizontal Layout)
│   ├── LikePostButton (❤️ Beğen)
│   └── CommentButton (💬 Yorum Yap)
├── CommentInputSection
│   └── CommentInputField (TMP_InputField)
└── BackButton (← Geri)
```

## 🔧 Unity Editor'da Kurulum Adımları

### Adım 1: SocialMediaPanel GameObject'ini Bul

1. Unity Editor'da **Hierarchy** penceresini aç
2. **CareerHub** sahnesini aç
3. Canvas → MainPanel → ContentArea → **SocialMediaPanel** GameObject'ini bul
4. **SocialMediaPanel** seçiliyken **Inspector** penceresinde `SocialMediaUI` script'ini ekle (yoksa Add Component)

### Adım 2: UI Elemanlarını Oluştur

#### 1. Başlık, Takipçi ve Geri Butonu:

1. **SocialMediaPanel** içinde sağ tık → **UI → Panel** → `SocialMediaHeader` olarak adlandır
2. **SocialMediaHeader** içinde:
   - **UI → Text - TextMeshPro** → `TitleText` ("📱 Sosyal Medya" yaz, Font Size: 24, Bold)
   - **UI → Text - TextMeshPro** → `FollowerCountText` ("10,000 Takipçi" yaz, Font Size: 18, sağ tarafa hizala)
   - **UI → Button - TextMeshPro** → `BackButton` ("← Geri" yaz, Font Size: 18)

**Layout Önerisi:**
- Horizontal Layout Group ekle (SocialMediaHeader'a)
- Child Control Width: ✅ (Width: Flexible)
- TitleText: Sol taraf
- FollowerCountText: Orta (Flexible Width)
- BackButton: Sağ taraf (Fixed Width: 120)

#### 2. Post Listesi ScrollView:

1. **SocialMediaPanel** içinde sağ tık → **UI → Scroll View** → `PostScrollView` olarak adlandır
2. **PostScrollView** otomatik olarak şu yapıyı oluşturur:
   - `Viewport` (Mask + Image)
   - `Content` (Viewport içinde)
3. **Content** GameObject'ini seç ve adını `PostListContent` yap (bu `postListParent` olacak)
4. **PostListContent** GameObject'ine:
   - **Vertical Layout Group** component ekle
     - Spacing: 15
     - Padding: 10 (her taraftan)
     - Child Control Width: ✅
     - Child Control Height: ❌
     - Child Force Expand Width: ✅
   - **Content Size Fitter** component ekle
     - Vertical Fit: Preferred Size

#### 3. Yeni Post Butonu:

1. **SocialMediaPanel** içinde sağ tık → **UI → Button - TextMeshPro** → `NewPostButton` olarak adlandır
2. Button Text: "📝 Yeni Post" veya "+ Yeni Post"
3. Font Size: 18
4. Pozisyon: PostScrollView'ın altında veya üstünde (sen belirle)

#### 4. Yeni Post Paneli (Modal):

1. **SocialMediaPanel** içinde sağ tık → **UI → Panel** → `NewPostPanel` olarak adlandır
2. **NewPostPanel**'i seç, **Inspector**'da **Active** checkbox'ını kapat (başlangıçta kapalı)
3. **RectTransform** ayarları:
   - Anchor: Center (0.5, 0.5)
   - Position: (0, 0)
   - Size: (600, 400) - veya istediğin boyut
   - Arka plan rengi: Koyu (0.1, 0.1, 0.15, 0.95)
4. **NewPostPanel** içinde:
   - **UI → Text - TextMeshPro** → `TitleText` (opsiyonel, "Yeni Post Paylaş")
   - **UI → Input Field - TextMeshPro** → `PostInputField` olarak adlandır
     - Placeholder: "Ne düşünüyorsun?"
     - Multi-line: ✅
     - Character Limit: 280 (Twitter gibi)
   - **Horizontal Layout Group** → `ButtonsContainer`
     - **UI → Button - TextMeshPro** → `CancelPostButton` ("İptal")
     - **UI → Button - TextMeshPro** → `PostButton` ("Paylaş")

**NewPostPanel Layout Önerisi:**
```
NewPostPanel (Panel)
├── Vertical Layout Group
│   ├── TitleText (TextMeshPro, Font Size: 20, Bold)
│   ├── PostInputField (TMP_InputField, Flexible Height)
│   └── ButtonsContainer (Horizontal Layout Group)
│       ├── CancelButton (Fixed Width: 150)
│       └── PostButton (Fixed Width: 150)
```

#### 5. Post Detay Paneli (Modal):

1. **SocialMediaPanel** içinde sağ tık → **UI → Panel** → `PostDetailPanel` olarak adlandır
2. **PostDetailPanel**'i seç, **Inspector**'da **Active** checkbox'ını kapat (başlangıçta kapalı)
3. **RectTransform** ayarları:
   - Anchor: Center (0.5, 0.5)
   - Position: (0, 0)
   - Size: (700, 600) - veya istediğin boyut
   - Arka plan rengi: Koyu (0.1, 0.1, 0.15, 0.95)
4. **PostDetailPanel** içinde:
   - **UI → Text - TextMeshPro** → `PostAuthorText` ("@OyuncuAdı", Font Size: 18, Bold, Mavi renk: 0.3, 0.6, 1.0)
   - **UI → Text - TextMeshPro** → `PostContentText` (Post içeriği, Font Size: 16, Word Wrap: ✅)
   - **Horizontal Layout Group** → `StatsContainer`
     - **UI → Text - TextMeshPro** → `PostLikesText` ("❤️ 1250", Font Size: 14)
     - **UI → Text - TextMeshPro** → `PostCommentsText` ("💬 45 Yorum", Font Size: 14)
   - **UI → Scroll View** → `CommentsScrollView`
     - **Viewport → Content** → `CommentsContent` olarak adlandır (bu `commentsParent` olacak)
     - **CommentsContent**'e Vertical Layout Group ekle
   - **UI → Input Field - TextMeshPro** → `CommentInputField` (Placeholder: "Yorum yaz...")
   - **Horizontal Layout Group** → `InteractionContainer`
     - **UI → Button - TextMeshPro** → `LikePostButton` ("❤️ Beğen")
     - **UI → Button - TextMeshPro** → `CommentButton` ("💬 Yorum Yap")
   - **UI → Button - TextMeshPro** → `BackButton` ("← Geri")

**PostDetailPanel Layout Önerisi:**
```
PostDetailPanel (Panel)
├── Vertical Layout Group
│   ├── PostAuthorText (TextMeshPro, Bold, Mavi)
│   ├── PostContentText (TextMeshPro, Word Wrap)
│   ├── StatsContainer (Horizontal Layout)
│   │   ├── PostLikesText
│   │   └── PostCommentsText
│   ├── CommentsScrollView (Flexible Height)
│   │   └── Viewport
│   │       └── CommentsContent (VerticalLayoutGroup) → commentsParent
│   ├── CommentInputField (TMP_InputField)
│   ├── InteractionContainer (Horizontal Layout)
│   │   ├── LikePostButton
│   │   └── CommentButton
│   └── BackButton
```

### Adım 3: SocialMediaUI Script Referanslarını Bağla

1. **SocialMediaPanel** GameObject'ini seç
2. **Inspector**'da **SocialMediaUI** component'ini bul
3. Şu referansları ata:

#### UI Referansları:
- **Title Text**: `TitleText` GameObject'ini sürükle-bırak
- **Follower Count Text**: `FollowerCountText` GameObject'ini sürükle-bırak
- **Back Button**: `BackButton` GameObject'ini sürükle-bırak
- **Post List Parent**: `PostListContent` GameObject'ini sürükle-bırak (PostScrollView > Viewport > Content)
- **Post Item Prefab**: (boş bırakılabilir, script runtime'da oluşturur)

#### Yeni Post Paneli:
- **New Post Panel**: `NewPostPanel` GameObject'ini sürükle-bırak
- **Post Input Field**: `PostInputField` GameObject'ini sürükle-bırak
- **Post Button**: `PostButton` GameObject'ini sürükle-bırak
- **Cancel Post Button**: `CancelPostButton` GameObject'ini sürükle-bırak
- **New Post Button**: `NewPostButton` GameObject'ini sürükle-bırak

#### Post Detay Paneli:
- **Post Detail Panel**: `PostDetailPanel` GameObject'ini sürükle-bırak
- **Post Author Text**: `PostAuthorText` GameObject'ini sürükle-bırak
- **Post Content Text**: `PostContentText` GameObject'ini sürükle-bırak
- **Post Likes Text**: `PostLikesText` GameObject'ini sürükle-bırak
- **Post Comments Text**: `PostCommentsText` GameObject'ini sürükle-bırak
- **Comments Parent**: `CommentsContent` GameObject'ini sürükle-bırak (CommentsScrollView > Viewport > Content)
- **Comment Input Field**: `CommentInputField` GameObject'ini sürükle-bırak
- **Like Post Button**: `LikePostButton` GameObject'ini sürükle-bırak
- **Comment Button**: `CommentButton` GameObject'ini sürükle-bırak

### Adım 4: Stil ve Düzen

#### Renk Önerileri:
- **Arka Plan Panelleri**: (0.1, 0.1, 0.15, 0.8-0.95)
- **Post Item Arka Plan**: (0.1, 0.1, 0.15, 0.8)
- **Yazar Rengi**: (0.3, 0.6, 1.0) - Mavi
- **Beğeni Rengi**: (1.0, 0.3, 0.3) - Kırmızı
- **Yorum Rengi**: (0.7, 0.7, 0.7) - Gri

#### Font Boyutları:
- Başlık: 24-28
- Yazar: 18, Bold
- İçerik: 16
- İstatistikler: 14
- Butonlar: 18

### Adım 5: Test

1. Unity'de Play moduna geç
2. CareerHub sahnesinde "Sosyal Medya" butonuna tıkla
3. Post listesinin göründüğünü kontrol et
4. Takipçi sayısının göründüğünü kontrol et
5. "Yeni Post" butonuna tıkla → Modal açılmalı
6. Bir post'a tıkla → Detay paneli açılmalı
7. Geri butonlarının çalıştığını kontrol et

## ✅ Kontrol Listesi

- [ ] SocialMediaPanel GameObject'i var
- [ ] SocialMediaUI script'i eklenmiş
- [ ] Header (Başlık, Takipçi, Geri) oluşturulmuş
- [ ] PostScrollView oluşturulmuş
- [ ] PostListContent (postListParent) doğru yere bağlanmış
- [ ] NewPostButton oluşturulmuş
- [ ] NewPostPanel oluşturulmuş ve başlangıçta kapalı
- [ ] PostInputField oluşturulmuş
- [ ] PostDetailPanel oluşturulmuş ve başlangıçta kapalı
- [ ] CommentsScrollView ve CommentsContent oluşturulmuş
- [ ] Tüm script referansları bağlanmış
- [ ] Layout Group'lar doğru ayarlanmış
- [ ] Font boyutları ve renkler ayarlanmış

## 🐛 Sorun Giderme

### Post Listesi Görünmüyor:
- PostListParent referansının doğru olduğundan emin ol (PostScrollView > Viewport > Content)
- Vertical Layout Group'un PostListContent'te olduğunu kontrol et

### Modal Paneller Açılmıyor:
- Panellerin başlangıçta SetActive(false) olduğundan emin ol
- Panel referanslarının doğru bağlandığını kontrol et

### Takipçi Sayısı Görünmüyor:
- FollowerCountText referansının bağlı olduğundan emin ol
- GameManager.Instance ve CurrentSave'in null olmadığını kontrol et (Console'da hata var mı bak)

### Butonlar Çalışmıyor:
- Tüm buton referanslarının bağlı olduğundan emin ol
- Console'da hata mesajları olup olmadığını kontrol et

## 📝 Notlar

- Post item'ları runtime'da oluşturulur (prefab yoksa)
- SocialMediaSystem'den postlar yüklenir (yoksa placeholder postlar gösterilir)
- Takipçi sayısı MediaData.socialMediaFollowers'dan gelir
- Yeni post paylaşıldığında SocialMediaSystem'e kaydedilir
- Post beğenme ve yorum yapma işlevleri çalışır durumda

