# Titan Soccer - Tasarım Dokümanı

## 🎮 OYUN ÖZETİ

Titan Soccer, mobil platformlar için geliştirilen, klasik futbol oyunlarından farklı olarak refleks değil karar odaklı, gerçek zamanlı simülasyon + kontrollü oynanış birleşimi sunan yenilikçi bir futbol kariyer oyunudur.

### Temel Felsefe
- **Refleks değil, karar odaklı**: Oyuncu "Nasıl oynadım?" değil "Doğru kararı verdim mi?" sorusunu sorar.
- **Kontrollü oynanış**: Maçı baştan sona manuel oynamaz, maçın kaderini belirleyen anlarda doğrudan sahaya girer.
- **Hikâye odaklı**: Her maç bir hikâye üretir, spiker bu hikâyeyi anlatır.

---

## 🏠 ANA MENÜ VE AKIŞ

### Ana Menü Seçenekleri
1. **Kariyer Modu** → Save slot ekranına yönlendirir
2. **Gösteri Maçı** (şu an eklenmeyecek, seçenek olarak kalabilir)
3. **Antreman** → Oyuncu şut ve pas antremanı yapabilecek
4. **Paketler** → Data pack sistemi (eski mantık aynı)
5. **Ayarlar** → Ses seviyesi, dil (Türkçe/İngilizce), cloud save
6. **Çıkış** → Oyundan çıkma tuşu

---

## 💾 KAYIT SLOTLARI (SAVE SLOTS)

- Slotların üst tarafında "Kariyerler" yazısı
- Oyunda 3 kariyer slotu bulunur:
  - **Boş slot** → Yeni Kariyer Oluştur
  - **Dolu slot** → Oyuncu adı, kulüp, pozisyon, sezon gösterilir
- Her slot tamamen bağımsız bir kariyeri temsil eder

---

## 🎯 YENİ OYUN OLUŞTURMA

### Ekran Yapısı

**Alt Bölüm:**
- Oyuncu adı/soyadı
- Uyruk seçimi
- Oynamak istediği lig seçenekleri

**Üst Bölüm:**
- **Karakter Tasarım Ekranı**
  - Saç stili
  - Ten rengi
  - Forma uzunluğu (kısa kollu / uzun kollu)
  - Aksesuar seçimleri (eldiven, maske, vs.)

### Takım Teklifleri

Yeni oyun oluşturulduğunda:
- Seçilen ligin düşük güçlü **rastgele 3 takımından** teklif gelir
- Her takımın altında gösterilenler:
  - Aylık maaş (Euro cinsinden)
  - Oynayacağı zamanlar (rotasyon, ilk 11, yedek)
  - Sözleşme süresi

**Teklif Detay Pop-up:**
- Üstüne tıklandığında pop-up arayüz açılır
- Sözleşmenin tüm detayları gösterilir
- **Sağ altta**: İmzala tuşu
- **Sol üstte**: Geri tuşu

**İmza Animasyonu:**
- İmzala tuşuna basıldığında pop-up sayfasının alt bölmesinde imza animasyonu oynar
- Oyuncu isterse eğitime girer veya direkt kariyer menüsüne geçilir

---

## 🏆 KARİYER MENÜSÜ

### Üst Bölme
- Oyuncunun adı
- Overall
- Yaş
- Oynanan sezon

### Orta Bölme - Maça Git
- Bizim takımımız ve karşı takım gösterilir
- Üstüne tıklanırsa detaylı kadrolar gösterilir
- Oyuncu kadrodaysa bulunduğu yerde ismi **sarı renkle** yazılı olur
- "Maça Git" butonuna basılırsa maç öncesi ekranı açılır

### Maç Öncesi Ekranı
- İki takımın detaylı 11'i gösterilir
- **En alt sağda**: Kadro durumu ve sebepleri
  - **İlk 11** (yeşil renk):
    - Son maçtaki performansın iyiydi
    - Genel performansın iyi
    - Teknik direktör sana bir şans vermek istedi
  - **Yedeksin** (sarı renk):
    - Son maçta performansın iyi değildi
    - Teknik direktörle aran iyi değil
    - Takımla aran iyi değil
    - Magazinde ismin anılıyor (düşük seviye)
    - Genel performansın iyi değil
    - Enerjin düşük
  - **Kadroda değilsin** (kırmızı renk):
    - Son maçlarda performansın rezaletti
    - Teknik direktörle aran çok kötü
    - Magazinde ismin çok anılıyor (yüksek seviye)
    - Genel performansın rezalet seviyede
    - Enerjin çok düşük
- Enerji ve moral seviyesi gösterilir
- **En sağda**: Maça Git butonu

### Maç Ekranı
- **Üstte**: Skor tablosu
  - Sol: Maç dakikası
  - Sağ: Reytingimiz
- **Ortada**: Maç takip ekranı
  - Topun nerede olduğu
  - Topla oynama yüzdesi
- **Altında**: Enerji ve moral barı
- **Onun altında**: Spiker alanı (küçük dikdörtgen)
- **En altta**: Kontrol barı
  - **Hedefler** (ileri zamanlarda eklenecek)
  - **Kadro** tuşu → İki takımın aktif ilk 11'i ve yedekleri
    - Oyuncu kaptansa değişiklik önerebilir (ileri zamanlarda)
  - **Maç hızı**: 1x, 2x, 3x
  - **Maçı simüle et** tuşu
    - Oyun durur, pop-up açılır
    - "Maçı simüle etmek istediğinize emin misiniz?"
    - Evet → Maç simüle edilir (kalan dakika ve takım güçlerine göre)
    - Hayır → Maç kaldığı yerden devam eder

### Puan Durumu ve Maç Raporu
- **Puan Durumu**: Ligin aktif puan durumu
- **Maç Raporu**: Sonraki rakip takımın taktikleri ve olası 11'i

### Alt Menü
- Ana Sayfa (maç sayfası)
- Haberler
- Sosyal Medya
- Antreman
- Hayat
- Oyuncu
- Diğer

---

## 📰 BASIN EKRANI

### İçerikler
- Maç sonrası haberler
- Teknik direktör açıklamaları
- Transfer söylentileri
- Medya puanları

### Oyuncu Tepkileri
Oyuncu haberler için tepki verebilir:
- **Olumlu**
- **Olumsuz**
- **Umursamaz**

Bu tepkiler şunları etkiler:
- Moral
- Popülerlik
- Takım içi ilişkiler

### Haber Türleri
- Son maçımızın performansı
- Magazine düşme (iyi-kötü)
- Genel performans
- Transfer söylentileri
- Transfer haberleri
- Büyük maç sonuçları
- Kupa turnuva ilerlemeleri ve sonuçları
- Lig şampiyonlukları
- Şampiyonluk yarışları
- Oyuncumuzun ilişkilerine göre haberler:
  - Teknik direktörle arası bozuk
  - Takımla arası bozuk
- Oyuncu kaptan olunca haber bildirimleri
- Kız arkadaş ile magazine düşme, ayrılma, sevgili olma haberleri
- Kumar haberleri, yakalanma
- Şike haberleri

**Önemli Haberler:**
- Eğer haber ilişki etkileyen önemli bir haberse, oyuncu maça gitmeden önce pop-up ile bilgilendirilir
- Oyuncu haberi inceleyebilir ya da maça devam edebilir

---

## 📱 SOSYAL MEDYA SİSTEMİ

### Özellikler
- Takipçi sayısı
- Maç sonrası paylaşımlar
- Sponsor mesajları
- Taraftar yorumları

### Popülerlik Etkileri
Popülerlik arttıkça:
- Sponsorluklar
- Reklam gelirleri
- Özel etkinlikler açılır

### Paylaşım Sistemi
- Oyuncumuz hakkında veya diğer oyuncular hakkında haber sayfaları
- NPC kullanıcılar
- Performans, magazin haberleri vs. gibi şeylere göre post atabilir
- Son maçının performansına göre iyi/kötü/nötr paylaşımlar yapabilir
- Takipçi ve beğenme sayısı ona göre artar veya düşer
- NPC'lerin yanıtlarına iyi/kötü/nötr cevaplar verebilir
- Buna göre linçlenebilir ya da övülebilir

---

## 👥 İLİŞKİ SİSTEMİ

### İlişki Türleri
- Takım arkadaşları
- Teknik direktör
- Yönetim
- Sponsorlar
- Aile
- Sevgili
- Manajer

### İlişki Yönetimi
- Her ilişki günlük/maçlık limitlerle yönetilir
- Hediye, cevap, karar sistemiyle etkilenir
- İlişkiler oyun gidişatını ciddi şekilde değiştirir

### Takım Arkadaşları Sekmesi
- Her takım arkadaşı için farklı bir panel
- Panel tıklandığında:
  - Bara gidilebilir
  - Kumar oynanabilir
  - Antremana gidilebilir
  - Hediye alınabilir
  - Sohbet edilebilir
- Bu aktiviteler iyi/kötü olarak sonuçlanır
- İlişki seviyesi buna göre etkilenir
- Aktivite yapıldıktan sonra sonuç pop-up ekranı ile bilgilendirilir

Aynı sistem diğer ilişki seçenekleri için de geçerlidir (aile, kız arkadaş, vs.)

---

## 🩹 SAKATLIK SİSTEMİ

### 21.1 Amaç

- Antreman/maç riskini anlamlı yapmak
- Rotasyon, enerji, ekipman, karar sistemini derinleştirmek
- Transfer ve sözleşme kararlarına "risk" katmak

### 21.2 Temel Değişkenler

- **Energy (0–100):** düşükse sakatlık riski artar
- **Stamina statı:** risk azaltır
- **Injury Prone (gizli özellik, 0–100):** bazı oyuncular daha yatkın
- **Match Intensity / Tempo:** maç hızı ve fiziksel temas riski etkiler
- **Training Load (maç başı antreman sayısı):** 4 limit, ama 4'ü zorlamak risk yükseltir
- **Equipment Buffs:** krampon/ekipman sakatlık riskini azaltabilir

### 21.3 Sakatlık Riski Hesabı (Örnek)

Her maç ve her antreman sonrası bir "risk check" yapılır:

```
Risk = BaseRisk
     + (100 - Energy) * 0.15
     + TrainingLoad * 2
     + Intensity * 3
     + InjuryProne * 0.05
     - Stamina * 0.10
     - RecoveryItemsBonus
```

- **BaseRisk:** 2–6 arası (lig seviyesine göre)
- **Intensity:** 0–3 (düşük/normal/yüksek)

**Not:** Ara transfer döneminde sakat oyuncu teklif alabilir ama risk/maaş kırılır.

### 21.4 Sakatlık Türleri (Basit ama Etkili)

- **Hafif (1–3 maç):** burkulma, kas gerginliği
- **Orta (4–10 maç):** kas yırtığı, diz sorunu
- **Ağır (11+ maç):** bağ sakatlığı, uzun süreli

**Etki:**
- Oynamazsa moral düşebilir (özellikle büyük kulüpte)
- Form kaybı ve "geri dönüş maçı" ihtiyacı

### 21.5 Rehabilitasyon & Geri Dönüş

- **"Dinlen" seçeneği:** enerji hızlı toparlar
- **"Tedavi al" (market item):** süreyi kısaltır
- **"Riskli dönüş" (erken dönüş):** performans düşer + tekrar sakatlanma ihtimali artar

### 21.6 Sakatlığın Sistemlere Etkisi

- **Transfer AI:** "Transfer Riski" artırır
- **Menajer AI:** "Sezon sonunu bekle" önerisi verebilir
- **Sözleşme:** bazı bonuslar devre dışı kalabilir (maç sayısı bonusu vb.)

---

## 📄 SÖZLEŞME MADDELERİ SİSTEMİ (CONTRACT CLAUSES)

### 22.1 Amaç

Sözleşmeler sadece maaş değil; oyuncunun rolünü ve kariyerini belirleyen maddeler bütünüdür.

### 22.2 Sözleşme Temel Alanları

- Maaş (haftalık/aylık)
- Sözleşme süresi (1–5 yıl)
- Rol: İlk 11 / Rotasyon / Yedek
- İmza parası
- Bonuslar (aşağıda)

### 22.3 Madde Türleri (Önerilen Liste)

#### A) Serbest Kalma Maddesi (Release Clause)

- "X € ödenirse serbest kalır"
- Büyük kulüplerin ilgisini artırır ama kulüp bu maddeyi istemeyebilir
- Menajer pazarlıkla eklemeye çalışır

#### B) Oynama Süresi Maddesi

- "Sezonda en az X dakika oynatılacak"
- Tutulmazsa: moral düşüşü / transfer talebi / tazminat

#### C) Maaş Artışı Maddesi

- "Her sezon %X maaş artar"
- Gelişim odaklı uzun sözleşmelerde yaygın

#### D) Performans Maddesi

- "X gole ulaşırsan +Y €"
- "Sezon rating ortalaması 7.5 üstü olursa prim"

#### E) Takım Başarısı Maddesi

- "Şampiyonluk primi"
- "Avrupa/Playoff primi"
- "Kümede kalma primi" (ara transferde çok görülür)

#### F) Sakatlık / Sigorta Maddesi

- "Uzun süreli sakatlıkta maaşın %X'i ödenir"
- Büyük liglerde daha gerçekçi his verir

#### G) Sadakat Bonusu (Loyalty Bonus)

- "Sözleşme bitene kadar kalırsan Y €"
- Oyuncuyu erken transferden vazgeçirebilir

#### H) Kiralık Maddeleri (Loan)

- Satın alma opsiyonu
- Geri çağırma maddesi
- Maaş paylaşımı (%)

---

## 💰 BONUSLAR & PRİMLER SİSTEMİ

### 23.1 Bonusların Amaçları

- Performansı motive etmek
- Oyun ekonomisini çeşitlendirmek
- "Doğru maçta parlamak" hissini güçlendirmek

### 23.2 Bonus Türleri

#### A) İmza Parası (Signing Bonus)

- Transfer anında tek seferlik ödeme
- Büyük kulüpte yüksek, küçük kulüpte düşük

#### B) Maç Başı Primi

- "Oynadığın her maç için X €"
- Rotasyon oyuncularını motive eder

#### C) Gol/Asist Primi

**Pozisyona göre değişir:**
- ST için gol primi daha yüksek
- CM için asist primi daha yüksek

#### D) Temiz Sayfa Primi (Clean Sheet)

- Defansif pozisyonlar için
- Kaleci hariç sistemde bile defans oyuncularına anlam katar

#### E) Sezon Sonu Performans Primi

- "Sezon rating ortalaması"
- "Toplam gol/asist barajı"
- "Sezonun oyuncusu seçilme"

#### F) Takım Başarısı Primi

- Şampiyonluk
- Avrupa hedefi
- Kümede kalma
- Kupa kazanma

#### G) Sadakat Bonusu

- Sezon sonunda ödenir
- Transfer kararını etkiler

### 23.3 Bonusların Dengesi (Önemli Kurallar)

- Aynı anda her bonus aktif olmamalı (ekonomi patlamasın)
- Büyük lig: maaş yüksek, bonus daha kontrollü
- Alt lig: maaş düşük, bonuslar "hedef bazlı" daha yüksek olabilir

### 23.4 Bonusların Diğer Sistemlerle Bağlantısı

- **Basın:** bonus kazanınca haber olur ("Kaplan bonusu kaptı!")
- **Sosyal medya:** takipçi artar
- **İlişkiler:** sponsorlar memnun olur
- **Transfer:** yüksek bonus alan oyuncu "değerli" algısı yaratır
- **Sakatlık:** maç sayısı/gol bonuslarını kaçırma riski taşır

---

## 💰 EKONOMİ & MARKET

### Satın Alınabilecekler
- Enerji içecekleri
- Küçük stat boost'ları
- Kramponlar
- Kozmetik ürünler
- Ev
- Araba

**Not:** Bazı eşyalar performansı doğrudan etkiler.

---

## 🏟 TAKIMLAR SİSTEMİ (TEAMS SYSTEM)

### 1. AMAÇ

Takımlar sistemi; lig yapısını, kulüplerin gücünü, oyun içi dengeyi ve kariyer ilerlemesini belirler.
Oyuncu zayıf bir takımdan başlayıp güçlü kulüplere doğru yükselir.

### 2. TAKIM YAPISI

Her takım aşağıdaki temel verilere sahiptir:

- **Takım adı**
- **Lig**
- **Genel güç (Team Rating)**
- **Hücum gücü**
- **Orta saha gücü**
- **Savunma gücü**
- **Kadro derinliği**
- **Takım kimyası**

**Bu değerler:**
- Maç simülasyonunu
- Pozisyon sıklığını
- Gol ihtimalini
doğrudan etkiler.

### 3. TAKIM SEVİYELERİ

Takımlar seviye katmanlarına ayrılır:

- **Alt seviye** (düşme hattı, düşük bütçe)
- **Orta seviye** (istikrarlı lig takımları)
- **Üst seviye** (şampiyonluk adayları)

Oyuncu kariyere bilinçli olarak alt seviye bir takımda başlar.

### 4. TAKIM DAVRANIŞLARI

Takımların oyun içi davranışı:

- **Ofansif / dengeli / defansif oyun tarzı**
- **Gençlere şans verme eğilimi**
- **Yıldız oyuncu beklentisi**
- **Rotasyon kullanımı**

**Bu davranışlar:**
- Oyuncunun ilk 11 şansını
- Dakika süresini
- Pozisyon alma ihtimalini
etkiler.

### 5. TAKIM – OYUNCU İLİŞKİSİ

Takımla ilişkiler şu başlıklarda izlenir:

- **Teknik direktör memnuniyeti**
- **Takım arkadaşlarıyla uyum**
- **Taraftar beklentisi**
- **Yönetim güveni**

**Bu ilişkiler:**
- Kadro tercihlerini
- Sözleşme yenilemeyi
- Transfer kararlarını
belirler.

### 6. TRANSFER VE TEKLİF SİSTEMİ

Takımlar oyuncuya:

- **Sezon sonu**
- **Büyük performans sonrası**
- **Medya ve popülerlik artışıyla**

teklif gönderir.

**Teklifler:**
- **Maaş**
- **Sözleşme süresi**
- **Rol** (ilk 11 / rotasyon)
şeklinde sunulur.

---

## 🔄 TRANSFER SİSTEMİ

### 1. SİSTEMİN AMACI

Transfer sistemi, oyuncunun kariyerini şekillendiren en kritik uzun vadeli mekaniktir.

**Amaç:**
- Performansı ödüllendirmek
- Sabırlı oyuncuyu avantajlı kılmak
- Kısa vadeli risk – uzun vadeli kazanç dengesi kurmak

**Titan Soccer'da transfer asla rastgele değildir;**
her teklif oyuncunun istatistikleri, medyadaki algısı ve kulüp durumuna göre oluşur.

### 2. TRANSFER DÖNEMLERİ (WINDOWS)

#### 2.1 Yaz Transfer Dönemi (Ana Dönem)

- Sezon bitimi sonrası açılır
- Süresi uzun
- En yüksek teklif çeşitliliği
- Büyük kulüpler aktif

**Bu dönem:**
- Kariyer atlamaları
- Lig değişimleri
- Büyük maaş artışları
için ana fırsattır.

#### 2.2 Ara Transfer Dönemi (Kış Dönemi)

Ara transfer dönemi daha kısa, daha riskli ve daha stratejiktir.

- Sadece acil ihtiyaçlar
- Takımlar panik halindedir
- Oyuncudan hemen katkı beklenir

Bu dönem, "kurtarıcı transfer" mantığıyla işler.

### 3. TRANSFER TEKLİFİNİN OLUŞMA ALGORİTMASI

Bir transfer teklifinin oluşması için birden fazla koşul birlikte değerlendirilir.

#### 3.1 Oyuncu Performansı

**Son 5 maçtaki:**
- Ortalama rating
- Gol/asist katkısı
- Büyük maç performansları
- Maç başına dakika

#### 3.2 Oyuncu Profili

- **Yaş** (genç = potansiyel, yaşlı = tecrübe)
- **Pozisyon** (bazı pozisyonlar daha değerlidir)
- **OVR** (overall rating)
- **Gelişim eğrisi**

#### 3.3 Kulüp İhtiyacı

**Takım:**
- O pozisyonda eksik mi?
- Sakatlık krizi mi var?
- Düşme hattında mı?
- Avrupa hedefi mi var?

**Ara transferde kulüp ihtiyacı, oyuncu performansından bile daha baskın olabilir.**

#### 3.4 Medya & Popülerlik

- Basın puanları
- Sosyal medya takipçi sayısı
- Sponsor ilgisi

**Medya görünürlüğü yüksek oyuncu,**
aynı istatistiklere sahip başka bir oyuncudan daha çok teklif alır.

### 4. TRANSFER TEKLİF TÜRLERİ

#### 4.1 Gelişim Transferi

- Küçük / orta seviye kulüp
- İlk 11 garantisi
- Düşük maaş
- Uzun vadeli yatırım

**Genç oyuncular için idealdir.**

#### 4.2 Kurtarıcı Transfer (Ara Dönem Özel)

- Takım düşme hattında
- Oyuncudan anında etki beklenir
- Kısa sözleşme
- Yüksek baskı

**Başarısız olursa:**
- Oyuncu morali düşer
- Medya olumsuz tepki verir

#### 4.3 Yıldız Transferi

- Büyük kulüpler
- Yüksek maaş
- Yoğun rekabet
- İlk 11 garantisi yok

**Oyuncu hazır değilse kariyer duraklayabilir.**

#### 4.4 Kiralık Transfer

- Genç oyuncular için
- Gelişim odaklı
- Oynama süresi yüksek

**Kiralık süresi bitince:**
- Ana kulübe dönüş
- Kalıcı teklif ihtimali

### 5. ARA TRANSFER DÖNEMİ ÖZEL MEKANİKLERİ

#### 5.1 Zaman Baskısı

- Gün sayısı sınırlı
- Teklifler hızla gelir–gider
- Kararsızlık fırsat kaçırır

#### 5.2 Panik Teklifler

- Takımlar sakatlık sonrası hızlı teklif yapar
- Maaş yüksek olabilir
- Rol belirsiz olabilir

#### 5.3 Yönetim Baskısı

**Mevcut kulüp:**
- "Gitmeni istemiyoruz"
- "Yerine adam bulamayız"
gibi mesajlar verebilir.

**Bu durum:**
- Oyuncu–kulüp ilişkisini etkiler

### 6. TRANSFER PAZARLIĞI

- Oyuncu doğrudan pazarlık yapmaz; menajer üzerinden ilerler.

**Pazarlık edilebilen unsurlar:**
- Maaş
- Primler
- İlk 11 rolü
- Sözleşme süresi

**Her pazarlık:**
- Başarı / başarısızlık riski taşır
- Kulübün ilgisini azaltabilir

### 7. REDDETME & BEKLEME STRATEJİSİ

**Oyuncu:**
- Teklifi reddedebilir
- Sezon sonunu bekleyebilir
- Ara transferi pas geçebilir

**Risk:**
- Form düşerse teklifler kesilebilir

### 8. TRANSFER SONUÇLARI

#### Başarılı Transfer:
- Popülerlik artışı
- Medya ilgisi
- Sponsorluk fırsatları

#### Başarısız Transfer:
- Yedek kalma
- Moral düşüşü
- Transfer söylentileri

### 9. UI & SUNUM

**Transfer teklif ekranı:**
- Kulüp logosu
- Lig bilgisi
- Rol açıklaması
- Maaş & süre
- "Kabul Et / Reddet / Pazarlık" seçenekleri

**Ara transfer döneminde:**
- Geri sayım barı
- Aciliyet vurgusu

### 10. OYUNA KATKISI

**Bu sistem:**
- ✔ Kariyer hikâyesi yaratır
- ✔ Her oyunu farklı kılar
- ✔ Sabır ve risk dengesini öğretir
- ✔ Ara transferi gerçekten gerilimli hale getirir

---

### 11. 🤖 TRANSFER AI SİSTEMİ (DETAYLI)

#### 1. TRANSFER AI'NIN AMACI

Transfer AI'nin görevi:

- Gerçekçi teklif üretmek
- Oyuncunun kariyer seviyesine uygun kulüpler seçmek
- "Her sezon herkes teklif yapıyor" hissini engellemek
- Ara transfer dönemlerini daha kaotik ve riskli kılmak

**AI rastgele çalışmaz, puanlama sistemiyle karar verir.**

#### 2. TEMEL MANTIK (ÖZET)

Her kulüp, her transfer döneminde oyuncu için şu soruyu sorar:

**"Bu oyuncu bize şu an ne kadar gerekli ve değerli?"**

Bunun cevabı bir **Transfer Interest Score (TIS)** ile hesaplanır.

#### 3. TRANSFER INTEREST SCORE (TIS)

Her kulüp için ayrı ayrı hesaplanır.

```
TIS = OyuncuPerformansı
    + OyuncuPotansiyeli
    + Pozisyonİhtiyacı
    + KulüpHedefi
    + MedyaEtkisi
    - TransferRiski
```

**Eşik değer:**

- **TIS < 60** → teklif yok
- **60–75** → düşük seviye teklif
- **75–90** → ciddi teklif
- **90+** → yıldız transferi

#### 4. PUANLAMA BİLEŞENLERİ

##### 4.1 Oyuncu Performansı (0–30)

Son 5 maç baz alınır.

| Kriter | Puan |
|--------|------|
| Ortalama rating | 0–10 |
| Gol / Asist katkısı | 0–10 |
| Dakika oynama | 0–5 |
| Büyük maç etkisi | 0–5 |

📌 **Ara transfer döneminde bu puan %30 daha fazla ağırlık alır.**

##### 4.2 Oyuncu Potansiyeli (0–20)

| Durum | Etki |
|-------|------|
| Yaş < 22 | +10 |
| OVR artış hızı | +5 |
| Pozisyon nadirliği | +5 |

**Genç oyuncular için uzun vadeli yatırım kulüpleri daha agresif olur.**

##### 4.3 Pozisyon İhtiyacı (0–25)

Kulüp kadrosuna bakılır:

| Durum | Puan |
|-------|------|
| Pozisyonda sakatlık | +10 |
| Zayıf ilk 11 | +8 |
| Kadro derinliği düşük | +7 |

📌 **Ara transferde bu değer en önemli faktördür.**

##### 4.4 Kulüp Hedefi (0–15)

| Kulüp Durumu | Etki |
|---------------|------|
| Düşme hattı | +15 |
| Avrupa hedefi | +10 |
| Şampiyonluk yarışı | +8 |

**Düşme hattındaki kulüpler:**
- Daha yüksek maaş
- Daha kısa sözleşme
- Daha büyük baskı sunar

##### 4.5 Medya & Popülerlik (0–10)

| Kriter | Puan |
|--------|------|
| Sosyal medya takipçisi | 0–5 |
| Basın puanı | 0–5 |

**Popüler oyuncular:**
Aynı performansla daha üst seviye kulüplerden teklif alır.

##### 4.6 Transfer Riski (-20 → 0)

**Riskler:**
- Son dönemde form düşüşü
- Sakatlık geçmişi
- Disiplin sorunları
- Rüşvet / etik olayları

**Bu değer negatif çalışır.**

#### 5. TEKLİF OLUŞTURMA ADIMLARI

1. Transfer dönemi açılır
2. Liglere göre kulüpler sırayla çalışır
3. Her kulüp TIS hesaplar
4. Eşik geçilirse teklif hazırlanır
5. Teklif türü belirlenir

#### 6. TEKLİF TÜRÜ SEÇİMİ (AI KARARI)

AI şu soruyu sorar:

**"Bu oyuncu bizde ne rol oynayacak?"**

**Rol belirleme:**
- **OVR < takım ortalaması** → Rotasyon
- **OVR ≈ takım ortalaması** → İlk 11
- **OVR > takım ortalaması** → Yıldız

#### 7. ARA TRANSFER AI FARKLARI

Ara transferde AI:

- Daha az sabırlı
- Daha agresif
- Daha risk alıcı

**Özel kurallar:**
- TIS eşik değeri 5 puan düşürülür
- Maaş teklifleri %10–25 artar
- Sözleşmeler kısalır

#### 8. RED / PAZARLIK ETKİSİ

**Oyuncu:**
- Teklifi reddederse → kulüp TIS -10
- Pazarlık başarısız olursa → TIS -15

**Bu kulüp bir süre tekrar teklif yapmaz.**

#### 9. TRANSFER ZİNCİR REAKSİYONU

Bir transfer:

- Başka kulüpleri tetikleyebilir
- "Rakip kaptı" etkisi yaratır

**Bu durumda:**
Benzer kulüplerden son dakika teklifleri gelir

#### 10. NEDEN BU SİSTEM GÜÇLÜ?

- ✔ Rastgele değil
- ✔ Performans ödüllendiriliyor
- ✔ Ara transferler gerçekçi
- ✔ Kariyer hikâyeleri oluşuyor
- ✔ Kolay dengelenebilir

---

### 12. 🧠 MENAJER AI SİSTEMİ (AGENT / MANAGER AI)

#### 1) Amaç

Menajer AI'nin görevi, oyuncunun kariyerini "arka planda" yönetmek değil; oyuncuya seçenek sunan, riskleri açıklayan, pazarlık yapan ve uzun vadeli plan kuran bir karakter gibi çalışmaktır.

**Oyuncu menajeri:**
- Teklifleri filtreler
- Pazarlık yapar
- Zamanlama önerir (ara transfer mi, sezon sonu mu?)
- Rol/garanti süre pazarlığı yapar
- Sponsorluk ve imajla transferi bağlar
- Kriz yönetir (formsuzluk, yedek kalma, medya baskısı)

#### 2) Menajer Tipleri (Arketipler)

Oyun başında (veya ileride değiştirilebilir) menajer tipi seçilir. Her tip AI kararlarını etkiler.

##### A) Riskli / Agresif Menajer

- Büyük kulüplere hızlı zıplama ister
- Pazarlıkta sert çıkar
- "Şimdi gitmezsek fırsat kaçar" der

**Başarı:** hızlı yükseliş  
**Risk:** yedek kalma / kariyer duraklama

##### B) Gelişim Odaklı Menajer

- Önce garanti süre + düşük baskı önerir
- Kiralık/gelişim transferlerini sever
- "2 sezon burada oynayıp güçlen" der

**Başarı:** istikrarlı büyüme  
**Risk:** büyük kulüpler geç gelir

##### C) Para Odaklı Menajer

- Maaş + prim + imza parası maksimize eder
- Sponsorlukları agresif kovalar

**Başarı:** yüksek gelir  
**Risk:** sportif hedefler aksayabilir

##### D) İtibar / Marka Menajeri

- Medya, popülerlik, prestij kulüplerine odaklanır
- "Büyük lig = büyük vitrin" yaklaşımı

**Başarı:** şöhret / sponsor  
**Risk:** sportif baskı artar

#### 3) Menajer Statları (AI'nin "gücü")

Menajerin kendisinin de seviyeleri olur (oyuncu ilerledikçe daha iyi menajere geçilebilir).

- **Negotiation (Pazarlık):** maaş + rol + prim kazanımı
- **Network (Ağ):** teklif sayısı ve kulüp çeşitliliği
- **Reputation Shield (İtibar Kalkanı):** medya krizlerini azaltır
- **Scouting Insight (Öngörü):** kulübün gerçek planlarını daha doğru tahmin eder
- **Integrity (Etik):** rüşvet gibi olaylara yatkınlık/direnç

#### 4) Menajer AI'nin Karar Motoru

Menajer her teklif için bir **Deal Score (DS)** hesaplar ve öneri sunar.

```
DealScore = SportingFit + PlayingTime + Money + Prestige + RiskAdjust - ContractTraps
```

##### 4.1 SportingFit (0–30)

- Kulüp oyun tarzı oyuncuya uyuyor mu?
- Pozisyon ihtiyacı gerçek mi?
- Takımın güç seviyesi (çok yüksekse rekabet riski)

##### 4.2 PlayingTime (0–25)

- Rol: İlk 11 / rotasyon / yedek
- "Dakika garantisi" maddesi var mı?
- Takımda aynı pozisyonda kaç oyuncu var?

##### 4.3 Money (0–25)

- Maaş
- İmza parası
- Gol/asist primleri
- Bonuslar

##### 4.4 Prestige (0–15)

- Lig seviyesi
- Kulüp hedefi (Avrupa/şampiyonluk)
- Medya görünürlüğü

##### 4.5 RiskAdjust (-25 → 0)

- Baskı seviyesi
- Düşme hattı paniği
- Koç değişme ihtimali
- Taraftar tepkisi

##### 4.6 ContractTraps (-10 → 0)

- Serbest kalma maddesi yok
- Sözleşme çok uzun
- "Oynasa da oynamasa da" kulüp avantajlı

**DS Eşikleri:**

- **DS < 55:** "Uzak dur"
- **55–70:** "İdare eder"
- **70–85:** "Mantıklı"
- **85+:** "Fırsat transferi"

#### 5) Ara Transfer Dönemi (Kış) Menajer Davranışı

Ara transferde menajer daha "yangın söndürme" mantığıyla çalışır.

- Oynama süresi düşükse → "kiralık veya kısa kontrat" önerir
- Form çok iyiyse → "sezon sonunu bekle, daha büyük teklif alırız" diyebilir
- Düşme hattı kulüplerini "yüksek maaş ama yüksek risk" diye etiketler
- Zaman baskısı: "48 saat içinde cevap ver" gibi uyarı mesajları

#### 6) Pazarlık Sistemi (Menajer AI Aksiyonları)

Oyuncu pazarlığı menüden başlatır, menajer AI sonuç üretir.

##### Pazarlık Edilebilen Maddeler

- Maaş (+%)
- İmza parası
- Gol/asist primi
- Sözleşme süresi
- Rol (ilk 11 / rotasyon)
- Serbest kalma maddesi (release clause)
- Kiralıkta: satın alma opsiyonu

##### Pazarlık Başarı Şansı

```
Success = ManagerNegotiation + PlayerReputation + ClubUrgency - ClubStinginess - TimePressure
```

**Ara transferde:**
- **ClubUrgency (ihtiyaç) artar** → pazarlık şansı yükselir
- **TimePressure artar** → aşırı pazarlık ters tepebilir

##### Pazarlık Sonuçları

- **Başarılı:** daha iyi kontrat
- **Kısmi:** sadece maaş artar ama rol değişmez
- **Başarısız:** kulüp teklifi geri çekebilir (özellikle ara transferde)

#### 7) Teklif Filtreleme & "Shortlist"

Oyuncuya onlarca teklif yağdırmak yerine menajer:

- **3–5 tekliflik Kısa Liste** çıkarır
- Her teklif için **1 cümlelik özet** verir:
  - "Dakika garanti, düşük baskı"
  - "Yüksek maaş, yüksek rekabet"
  - "Prestijli kulüp, ama yedek riski"

Oyuncu isterse "tüm teklifleri göster" diyebilir.

#### 8) Menajer Olayları (Narrative + Sistem)

Menajer sadece matematik değil, hikâye olayları da üretir.

**Örnek olaylar:**
- "Yeni bir menajer firması seni istiyor" (menajer değiştirme)
- "Menajer komisyonu yükseltmek istiyor"
- "Menajer etik dışı bir teklif getiriyor" (rüşvet bağlantısı)
- "Menajer medya krizini yönetiyor" (olumsuz haberleri azaltma)

#### 9) Menajer Komisyonu ve Ekonomi

**Menajer komisyon alır:**
- Maaşın küçük yüzdesi
- İmza parasından pay

**Daha iyi menajer:**
- Daha pahalı komisyon
- Ama daha iyi kontrat getirir

**Bu da oyuncuya strateji sunar:**
"Ucuz menajer mi, iyi menajer mi?"

---

## 📦 DATA PACK SİSTEMİ (TEK PAKET MANTIGI – TITANDATA)

### 1. SİSTEMİN AMACI

Titan Soccer'daki Data Pack sistemi;

- Oyunu telif risklerinden korumak
- İçeriği güncellenebilir kılmak
- Oyuncuya kontrol ve seçim özgürlüğü vermek
- Oyunun çekirdeğini sade tutmak

amacıyla tasarlanmıştır.

**Önemli prensip:**

**Oyunda Data Pack olmadan da kariyer oynanabilir.**
**Data Pack yalnızca içerik katmanıdır.**

### 2. DATA PACK YAPISI (AYRI AYRI DEĞİL – TEK PAKET)

Titan Soccer'da oyuncu / takım / lig için ayrı ayrı paketler yoktur.

#### 🔹 Tek Paket Mantığı

Her Data Pack, kendi içinde tam bir futbol evreni taşır.

**Örnek:**

**TitanData Pack**
- Birden fazla lig
- O liglere ait takımlar
- Takımlara ait oyuncular
- Kulüp renkleri, isimleri, logolar
- Lig yapıları ve sezon kuralları

**Oyuncu:**
- "Şu lig gerçek, bu takım sahte" gibi parçalı bir yapı yaşamaz
- Ya tamamen kurgusal, ya da tamamen Data Pack'li bir deneyim yaşar

### 3. OYUNDA VAR OLAN İLK DATA PACK

#### 🎯 Varsayılan Paket: TitanData Pack

Oyun çıktığında:

- **1 adet Data Pack bulunur**
- **Adı:** TitanData Pack
- **İçeriği:**
  - Birçok gerçek lig
  - Birçok gerçek takım
  - Gerçek oyuncu isimleri
  - Gerçek lig ve takım yapıları

**Bu paket:**
- Oyuna dahil gelir
- Ama otomatik aktif değil
- Oyuncu bilinçli şekilde seçip kullanır

### 4. DATA PACK MENÜSÜ (ANA MENÜDEN ERİŞİM)

#### Menü Yolu

**Ana Menü → Data Pack'ler**

#### Data Pack Menüsü İçeriği

Her Data Pack kartı şunları gösterir:

- **Data Pack adı** (örnek: TitanData Pack)
- **İçerdiği lig sayısı**
- **İçerdiği takım sayısı**
- **İçerdiği oyuncu sayısı**
- **Durum etiketi:**
  - Yüklü değil
  - İndirildi
  - Aktif

#### Oyuncu Aksiyonları

- **İndir**
- **Aktif Et**
- **Pasif Et**

### 5. KULLANIM AKIŞI

#### İlk Oyun Açılışı

- Oyuncu isterse hiç Data Pack seçmeden oyuna başlayabilir
- Bu durumda:
  - Kurgusal ligler
  - Kurgusal takım ve oyuncu isimleri kullanılır

#### Data Pack İndirme

- Oyuncu Data Pack menüsünden:
  - Paketin üstüne tıklar
  - İndirir
  - İsterse aktif eder

#### Aktif Edilen Data Pack

**Aktif Data Pack:**
- Kariyer oluştururken kullanılır
- Lig seçimi ekranında görünür
- Takım teklifleri bu veriye göre gelir

### 6. AKTİF / PASİF DAVRANIŞ KURALLARI

**Aktif Data Pack varken:**
- Gerçek lig isimleri
- Gerçek takım isimleri
- Gerçek oyuncu adları
kullanılır.

**Data Pack pasif edilirse:**
- Oyun otomatik olarak kurgusal verilere döner
- Save dosyası bozulmaz
- Kariyer devam eder

**Ekranda uyarı gösterilir:**
*"Aktif Data Pack bulunamadı. Varsayılan verilerle devam ediliyor."*

### 7. SAVE DOSYASI İLE İLİŞKİ

**Save dosyası:**
- İsimleri değil
- **ID'leri** tutar

**Bu sayede:**
- Data Pack silinse bile kariyer çökmez
- Oyuncu istediği zaman tekrar paketi indirip gerçek isimlere geri dönebilir

**Save içinde saklanan bilgi:**
- `activeDataPackId`
- `dataPackVersion`

### 8. VERSİYON VE UYUMLULUK

Her Data Pack şu bilgileri taşır:

- `packId`
- `packVersion`
- `minGameVersion`

**Kural:**
- Oyun versiyonu yetersizse Data Pack aktif edilemez
- Menüde **"Uyumsuz"** etiketi görünür

### 9. İLERİDE EKLENEBİLECEK SENARYOLAR (AMA ŞİMDİ DEĞİL)

Bu yapı şunlara izin verir ama zorunlu değildir:

- Yeni sezon güncellemeleri
- Alternatif evren paketleri
- Eski sezon paketleri
- Topluluk odaklı mod benzeri içerikler

**Ama oyunun çekirdeği asla Data Pack'e bağımlı olmaz.**

### 10. TASARIM FELSEFESİ (ÇOK ÖNEMLİ)

**Data Pack:**
- Oyun mekaniğini değiştirmez
- Sadece veriyi değiştirir

**Oyuncu:**
- Ne kullandığını bilir
- Zorlanmaz
- İstediği zaman vazgeçebilir

**Bu sayede Titan Soccer:**
- Hukuki olarak güvenli
- Teknik olarak sağlam
- Uzun vadede sürdürülebilir
bir yapıya sahip olur.

---

## 🎮 OYNANIŞ SİSTEMİ

### Pozisyon Bazlı Oynanış
- Maç başladığında oyuncunun morali, overall'ı, takım ilişkisi, enerjisi, karşı takımın gücü baz alınarak pozisyon kazanılır
- Pozisyon başladığında oyun yavaş bir şekilde ilerler
- Herhangi bir aktivite yaptığımızda (şut, pas, hareket) oyun gerçek hızıyla devam eder
- Aktivite bittiğinde (oyuncu durduğunda) hala top oyuncudaysa oyun yavaşlar ve hareketimizi belirleriz

### Kontroller
- **Pas atmak**: Takım arkadaşının üstüne 1 kere tıkla → Yerden pas
- **Pas atmak**: Takım arkadaşının üstüne 2 kere tıkla → Havadan pas
- **Hareket etmek**: Sahanın rastgele bir noktasına tıkla → Hareket eder
  - Hareket ederken oyun normal hızla akar
  - Hareket edilirken top alınmadıysa (top hala oyuncudaysa) zaman yine yavaşlar ve bir sonraki hareketini seçer
- **Şut çekme**: Oyuncunun üstüne basıp parmağımızı sürüklediğimiz yöne doğru bir çizgi oluşur
  - Top ona göre düz, falsolu, havadan, yerden gider

### Yapay Zeka Sistemi
- Toplam 22 oyuncu (oyuncumuz dahil), 21'i yapay zeka
- Yapay zekalar mevkilerinde bulunur
- **Defans bölgesi**: Topun gelişine göre markaj veya baskı yapar ve pozisyon alır
- **Hücum**: Top ele geçirildiğinde pas alabilecek bir şekilde ileri çıkar
- Tüm oyuncular bir anda topa doğru koşmaz
- Topun yakınlığına göre, markaja göre yapay zeka ayarlanır
- Pas kesmek için pas atılabilir oyuncuların önünü de kapatmaya çalışır
- Bu sistem oyuncuların overall'ına ve takımın genel gücüne bağlı daha iyi çalışır

### Pas İsteme
- Oyuncu kendi üstüne tıklayarak pas isteyebilir
- Yapay zeka eğer oyuncuya pas atılabilir bir açı varsa pas atar, yoksa devam eder

### Takım Stilleri
- Takımlar oyun stillerine göre atağa çıkmaya çalışır
- Savunma çok ileri çıkmaz
- Oyun stilleri:
  - Tiki taka
  - Uzun pas
  - Pas oyunu
  - (Birkaç tane daha taktik)
- Taktikler takımdan takıma değişir
- Yapay zeka hareketleri bu sistemlere göre değişir
- Takımlar defansif, çok defansif, orta, ofansif, çok ofansif şeklinde oynatabilir
- Bu da takımdan takıma, maçtan maça, skora göre değişir

### Fizik
- Top kale direklerine çarpıp sekebilir

### Reyting Sistemi
- Oyuncunun yaptığı her bir hata veya doğru hareket reytingini doğrudan etkiler

### Kaleci Sistemi
- Kaleciler overall'larına göre hareket eder
- Overall yüksekse çok daha iyi kurtarışlar yaparken
- Overall düşükse en basit golleri bile yiyebilir

### Müdahale Sistemi (Defans)
- Eğer oyuncumuz ile savunma yapıyorsak:
- Oyuncunun üstüne basılı tutup bir yöne kayarsak o yöne doğru oyuncu kayar
- Eğer topa değerse topu kaydığı yöne doğru atar ve topu alıp uzaklaştırmaya çalışır
- Eğer topa dokunmayıp sadece oyuncuya müdahalede bulunursa:
  - Sarı kart
  - Kırmızı kart
  - Hiç kart görmeden faul yapabilir
- Bu müdahalenin derecesine göre değişir

### Spiker Sistemi
- Spikerimizin 100'lerce yorum seçeneği bulunur
- İyi veya kötü genellikle top kimdeyse oyuncunun ismini söyleyerek anlatır
- Yaptığı hareketi, olduğu bölgeyi falan söyler
- Gol atıldığında ona göre yorum yapar
- Birçok şeye yorum yapar, yani maçı anlatır

### Maç Sonu Ekranı
- Maç sonu reytingimiz derecesine göre yeşil/sarı/kırmızı renklerle ifade edilir
- Oyuncunun sözleşme bonusları varsa ve bonus yaptıysa kazandığı para gözükür
- Oyuncu maç sonu isteğe bağlı röportaj yapabilir
  - Reyting yüksekse röportajlara katılınabilir
  - Düşükse katılınamaz
- Oyuncuya rastgele sorular sorulur
- Oyuncu iyi/kötü/nötr cevaplar verebilir
- Buna göre basın ve ilişkiler etkilenebilir
- Yaptığınız açıklamalar ertesi gün haberlerde yaptığınız şekle (iyi/kötü/nötr) göre yayınlanır

---

## 🏋️ ANTRENMAN

### Antreman Sistemi
- Oyuncunun maç öncesi başına **2 hakkı** vardır
- Hakları bittikten sonra bir sonraki maçı oynadıktan sonra hakları yenilenir
- Antremanların zorluk seviyeleri vardır
- Yapılan antremanlara göre oyuncunun özellikleri küçük seviyelerde yavaş yavaş artar

### Antreman Türleri
- Pas antremanı
- Şut antremanı
- Sürat antremanı
- Dripling antremanı
- Orta açma antremanı
- Defans antremanı

Bu antremanlar zorluk seviyesine göre belli +'lar verir. Oyuncu isteğe bağlı olarak bu antremanları yapar.

---

## 🏠 HAYAT

### Hayat Bölmesi Seçenekleri
- **İlişkiler** (yukarıda anlatıldı)
- **Lüks**: Oyuncu kazandığı paralarla çeşitli evler, arabalar, kozmetik öğeler, apartmanlar, villalar, ünlü şirketler gibi birçok şeyi satın alabilir. Bunlardan gelir elde edebilir.
- **Kramponlar**: Ufak statlar sağlar ve sağladığı statlara göre fiyatları artar.
- **Bahis**: Mevcut hafta içinde maçlara kupon yapılabilir. Oyuncu kendi maçına da kupon yapabilir ama eğer ortaya çıkarsa ilişkileri çok kötü etkilenir. Sürekli devam ederse futbol kariyeri bitip hapse girebilir.
- **Kumar**: At yarışı, rulet, slot, blackjack gibi seçenekler olacak. İlerde daha fazlası da eklenebilir.

---

## 📈 İLERLEME & ZORLUK

### Başlangıç
- Düşük lig, zayıf takım

### Orta Oyun
- İlk 11, transfer teklifleri

### Geç Oyun
- Yıldız oyuncu, milli takım

### Zorluk
- Performansa göre dinamik ayarlanır

---

## 💸 RÜŞVET SİSTEMİ

### Rüşvet Verilebilen Taraflar

#### 🎩 Hakem
- Penaltı ihtimalini artırma
- Faul/sert müdahalelerin görmezden gelinmesi
- Ofsayt kararlarında küçük tolerans

#### 🧑‍💼 Teknik Direktör
- İlk 11'e girme şansı
- Daha fazla süre alma
- Pozisyon tercihlerinde öncelik

#### 🧑‍💼 Yönetim
- Transfer tekliflerinin gelmesi
- Maaş pazarlığında avantaj
- Sözleşme uzatma kolaylığı

#### 🗞 Medya (Dolaylı Rüşvet)
- Olumsuz haberlerin yumuşatılması
- Medya puanının korunması

### Rüşvet Mekaniği
- Rüşvet asla direkt bir tuşla yapılmaz
- Her zaman hikâye/olay kartı üzerinden gelir

**Örnek Senaryo:**
"Maçtan önce soyunma odasında bir menajer yanına yaklaşıyor. 'Hakemle aramız iyi… Küçük bir katkı işleri kolaylaştırır.'"

**Oyuncu Seçenekleri:**
- 💰 Rüşvet Ver
- ❌ Reddet
- 🤔 Ertele

### Başarı & Yakalanma Olasılığı
Her rüşvetin 3 temel değeri vardır:
1. **Miktar** (Az / Orta / Yüksek)
2. **Risk Seviyesi**
3. **Gizlilik Skoru**

**Başarı ihtimali şu faktörlere bağlıdır:**
- Oyuncunun popülerliği
- Daha önce rüşvet yapıp yapmadığı
- Ligin seviyesi (alt ligler daha gevşek)
- Medya ilgisi

### Yakalanma Durumunda Sonuçlar

#### ⚠ Hafif Yakalanma
- Medyada dedikodu
- Moral düşüşü
- Sponsor kaybı

#### 🚨 Ağır Yakalanma
- Maç cezası
- Takımdan gönderilme
- Maaş kesintisi
- Lig düşürülme

#### 💀 Kariyer Krizi (Nadir)
- Milli takım ihtimali sıfırlanır
- Büyük kulüpler transfer teklifini keser

### Ahlak & İtibar Sistemi
- Oyunda gizli bir **İtibar / Ahlak Değeri** bulunur
- Temiz oynarsan → uzun vadeli büyük kariyer
- Rüşveti alışkanlık yaparsan → kısa vadeli güç, uzun vadeli çöküş

**Bazı olaylar sadece yüksek ahlaklı oyunculara açılır:**
- Kaptanlık
- Milli takım
- Efsane statüsü

### UI / Sunum
- Rüşvet asla "RÜŞVET VER" diye yazmaz
- Her zaman üstü kapalı diyaloglar ile sunulur
- Oyuncu ne yaptığını anlar ama sistem açıkça söylemez

### Denge Kuralları (ÇOK ÖNEMLİ)
- Maç başına en fazla 1 rüşvet olayı
- Üst üste kullanıldığında risk katlanarak artar
- Büyük liglerde rüşvet çok daha riskli
- Oyuncu oyunu rüşvetle "kırıp geçemez"

### Örnek Senaryo
Son haftaya girilmiş, takım küme düşme hattında. Teknik direktör sana açıkça oynamayacağını söylüyor. Menajerin arıyor: "Bunu çözeriz… ama bedeli var."

Oyuncu kararı kariyerini belirler.

---

## 🧍‍♂️ OYUNCU İSTATİSTİKLERİ EKRANI (PLAYER STATS)

### Ekranın Amacı
Oyuncu istatistikleri ekranı, kullanıcının futbolcusunu tek bakışta analiz edebilmesini, gelişimini takip edebilmesini ve stratejik kararlar (antreman, ekipman, transfer) alabilmesini sağlar.

### Genel Yerleşim (Layout)
Ekran dikey (portrait) yapıdadır ve 4 ana bölümden oluşur:
1. Üst başlık ve geri butonu
2. Oyuncu özet kartı
3. Sekmeli istatistik alanı
4. Alt hızlı aksiyon butonları

### Üst Başlık Alanı
- **Orta üstte**: "PLAYER STATS" başlığı
- **Sol üstte**: Geri butonu
- Premium lacivert arka plan, altın çerçeve kullanılır

### Oyuncu Özet Kartı
**Kart İçeriği:**
- Oyuncu adı
- Kulüp adı
- Pozisyon
- Yaş
- **Sağ Tarafta:**
  - OVR (Overall Rating)
  - Piyasa Değeri

**Tasarım:**
- Büyük, yuvarlatılmış kart
- İnce altın çerçeve
- Koyu zemin üzerinde açık yazılar
- OVR değeri vurgulu ve büyük yazılır

### Sekmeli İstatistik Alanı (TABS)
**Sekmeler:**
- **Season** – Sezon istatistikleri
- **Career** – Tüm kariyer istatistikleri
- **Attributes** – Özellikler (statlar)

**Davranış:**
- Aktif sekme dolu arka planla vurgulanır
- Diğer sekmeler sadece çerçeveli görünür
- Sekme geçişleri animasyonlu yapılabilir

### Sezon İstatistikleri (SEASON TAB)
**Gösterilen İstatistikler:**
- Oynanan maç sayısı
- Oynanan dakika
- Atılan gol
- Yapılan asist
- Çekilen şut
- Pas başarı yüzdesi
- Ortalama maç puanı

**Tasarım:**
- Liste/tablo mantığı
- Her satır arasında ince ayırıcı çizgi
- Sağ tarafta sayısal değerler
- Sol tarafta istatistik adı

### Özellikler (ATTRIBUTES TAB)
**Temel Statlar:**
- Pace (Hız)
- Shooting (Şut)
- Passing (Pas)
- Dribbling
- Defense (Savunma)
- Stamina (Dayanıklılık)

**Gösterim:**
- Her stat yatay bar
- 0–100 arası değer
- Doluluk oranına göre bar uzar
- Değer sayısal olarak da gösterilir

**Kullanım:**
- Antreman ve ekipman etkileri burada net görülür
- Oyuncu gelişimini takip etmek kolaylaşır

### Alt Aksiyon Butonları
**Butonlar:**
- **Upgrade / Training** → Oyuncuyu antreman veya gelişim ekranına götürür
- **Equipment** → Krampon, ekipman ve kozmetik menüsüne gider

**Tasarım:**
- Büyük, dokunması kolay
- Yuvarlatılmış köşeler
- Altın çerçeve
- Net yazı

### Tasarım Felsefesi
- Bilgi yoğun ama boğucu değil
- Premium spor oyunu hissi
- Tek bakışta karar aldıran yapı
- FIFA Mobile tarzı okunabilirlik
- Titan Soccer'ın lacivert–altın kimliğiyle uyumlu

### Oyuna Katkısı
Bu ekran:
- Oyuncunun gelişimini somutlaştırır
- Antreman, transfer ve ekipman kararlarını anlamlı hale getirir
- Oyuncuya "kariyerini yönetiyorum" hissini güçlendirir

### Ek Mini Kartlar (Önerilen)

**Form Trendi (Son 5 Maç):**
- Son 5 maçın rating'lerini grafik/çizgi olarak gösterir
- Yükselen/düşen trendi görselleştirir

**Sakatlık Geçmişi:**
- Geçmiş sakatlıklar listesi
- Sakatlık türü, süre, tarih bilgisi
- Toplam sakatlık günü sayısı

**Disiplin (Kart Sayıları):**
- Sarı kart sayısı (sezon/career)
- Kırmızı kart sayısı (sezon/career)
- Toplam ceza günü

---

## 🧱 TEKNİK VE GELİŞİM

- Unity ile geliştirme
- 2D oynanış
- Modüler UI yapısı
- Genişletilebilir sistemler
- Uzun vadeli live-ops'a uygun

---

## 📱 EKRAN AKIŞI (SCREEN FLOW)

### Ana Akış
```
Main Menu → Save Slots → (Yeni Kariyer Akışı) → Career Hub → Match Pre → Match → Post Match → Career Hub
```

### Yeni Kariyer Akışı
1. **Data Pack Seçimi**
2. **Karakter Oluşturma**
3. **3 Takım Teklifi**

---

## ⚙️ MAÇ SİMÜLASYONU & POZİSYON ÜRETİM SİSTEMİ

### Maç Simülasyonu Mantığı

Maç normalde simülasyon akarken belli anlarda oyuncuya **"Chance"** düşer.

**Maç simülasyonu üretir:**
- Topa sahip olma dalgaları (possession segments)
- Atak şiddeti (attack threat)
- Şans kalitesi (chance quality)

### Pozisyon (Chance) Üretim Formülü (Özet)

Her 5–15 saniyede bir değerlendirme yapılır:

- Top kimde?
- Takım gücü farkı?
- Oyuncu sahada mı?
- Oyuncu pozisyonu uygun mu?

**ChanceBaseRate (lig seviyesine göre):**
- Alt lig: 0.8x
- Orta: 1.0x
- Üst lig: 1.2x (daha hızlı oyun)

**ChanceQuality etkileri:**
- Takım hücum gücü ↑ → kalite ↑
- Rakip savunma gücü ↑ → kalite ↓
- Oyuncu OVR ↑ → oyuncuya gelen şansların "tamamlama ihtimali" ↑

### Oyuncuya Gelen Şans Sayısı (Denge)

- **İlk 11 ise:** maç başı ortalama 2–6 chance
- **Rotasyon ise:** 1–4 chance
- **Yedek ise:** oyuna girerse dakika bazlı 0–2 chance

**Bu değerler tek maçta patlamasın diye "maksimum chance limiti" olur:**
- Maks 8 chance / maç (uzun vadede ayarlanabilir)

---

## 🏋️ ANTRENMAN LİMİTLERİ (ÇELİŞKİYİ ÇÖZELİM)

### Net Kural (Önerilen)

- **Maç başı antreman hakkı: 2**
- Market veya özel olaylarla nadiren +1 alınabilir (max 3)
- Antreman sayısı arttıkça sakatlık riski artar

**Bu sayede ekonomi ve sakatlık sistemi anlam kazanır.**

---

## 📈 GELİŞİM (PROGRESSION) & OVR HESAPLAMA

### Gelişim Kaynakları

- Maç rating
- Antreman başarı seviyesi
- Büyük maç performansı
- Disiplin/etik kararlar (uzun vadeli etkiler)

### Gelişim Puanı (XP) Örneği

**Maç sonu:**
- Rating 6.0 altı: düşük gelişim
- 6.0–7.0: normal
- 7.0–8.0: iyi
- 8.0+: bonus gelişim + popülerlik artışı

### OVR Hesaplama (Pozisyona Göre Ağırlık)

**Örnek (ST için):**
- Shooting 35%
- Pace 20%
- Dribbling 20%
- Passing 15%
- Stamina 10%

CM/CB gibi pozisyonlarda ağırlıklar değişir.

---

## 🧠 ZORLUK & AI SEVİYELERİ

### Zorluk Parametreleri

- AI pas isabeti
- Markaj mesafesi
- Top kapma başarısı
- Kaleci reaksiyon bonusu
- Şut engelleme sıklığı

### Dinamik Zorluk (DDA) Kuralı

**Oyuncu üst üste çok iyi giderse:**
- Rakip savunma reaksiyonu hafif artar

**Oyuncu çok düşerse:**
- Chance kalitesi biraz iyileşir (ama "hilesiz" hissedecek kadar)

**Asla:** "oyuncu kötü oynadı diye kesin gol yedirme/attırma" yapılmaz.

---

## 🗣 SPİKER SİSTEMİ (CONTENT TASARIMI)

### Template Sistemi

Spiker cümleleri sabit değil, template'lerle üretilir:

- `{minute}. dakika: {player} {zone} bölgesinde {action}!`
- `{team} baskıyı artırdı, {player} şimdi topu kontrol ediyor.`
- `Bu pozisyon kritik… {player} kararını veriyor!`

### Trigger Listesi

- Gol / kaçan gol / kurtarış
- Faul / kart
- Top kapma / pas arası
- Uzun pas / ara pas
- Şut türü (falso/sert/yerden/havadan)
- Sakatlık
- Büyük maç anları (son 10 dk)

### Dil Desteği

TR/EN için aynı trigger'lar, farklı template havuzları kullanır.

---

## 💰 EKONOMİ DENGESİ (ÖRNEK SAYI BANDLARI)

Bu bölüm oyunu "gerçek üretim" seviyesine getirir.

### Maaş Bantları (Aylık – Euro)

- Alt lig zayıf takım: €800–€2.500
- Orta lig: €2.500–€10.000
- Üst lig: €10.000–€60.000+

### Market Örnekleri

- Enerji içeceği: €150–€500 (tier)
- Rehab item: €800–€2.500
- Krampon (tier 1–5): €300 → €25.000
- Kozmetik: €200 → €5.000
- Araba/Ev: €10.000 → €500.000+ (bazıları "prestij + küçük bonus")

**Kural:** Pay-to-win yok. Büyük stat artışı değil, küçük farklar.

---

## 🎮 OYUN DURUMLARI (GAME STATES)

Oyun, UI ve simülasyon karmaşasını azaltmak için aşağıdaki state'lerle çalışır:

- **STATE: MainMenu**
- **STATE: SaveSlots**
- **STATE: NewCareerFlow**
- **STATE: CareerHub**
- **STATE: MatchPre**
- **STATE: MatchSim** (arka planda sim)
- **STATE: Chance** (oyuncu kontrolü)
- **STATE: Pause**
- **STATE: PostMatch**
- **STATE: TransferWindow**
- **STATE: NewsPopUp / CriticalEventPopUp** (maç öncesi önemli haber)

**Önemli Kural:**
Maça gitmeden önce kritik bir olay/önemli haber varsa öncelik pop-up'ındır, oyuncu "incele" veya "sonra" seçebilir.

---

## 💾 KAYIT SİSTEMİ & CLOUD SAVE (SAVE DESIGN)

### Save Slot İçeriği

Her slot şunları kaydeder:

- **PlayerProfile:** ad, uyruk, yaş, pozisyon, OVR, statlar, gizli özellikler (InjuryProne, Morality vb.)
- **ClubData:** kulüp, lig, sözleşme
- **SeasonData:** sezon, hafta, fikstür, puan durumu
- **RelationsData:** teknik direktör, takım, sponsor, aile, sevgili, menajer ilişkileri
- **EconomyData:** para (Euro), envanter (enerji içeceği, rehab item), ev/araba/kozmetik
- **MediaData:** popülerlik, basın puanı, sosyal medya takipçi
- **DataPackRefs:** aktif pack kimlikleri ve versiyon bilgisi

### Otomatik Kayıt Noktaları (Auto-save)

- Maç bitince
- Transfer kabul/ret/pazarlık sonucu
- Data pack seçimi değişince
- Market alışverişi sonrası
- İlişki aksiyonu sonrası (hediye/karar)

### Cloud Save (Opsiyonel)

- Oyuncu ayarlardan açar/kapatır
- Cloud açıkken: slot seçerken "Cloud senkron" uyarısı
- Data pack değişince: save bozulmaz, sadece isim/forma/logo verisi fallback'e döner

---

## 🏳️‍🌈 MİLLİ TAKIM SİSTEMİ

### 25.1 Seçilme Kriteri

Oyuncunun milli takıma seçilmesi için değerlendirilen faktörler:

- **OVR** – Genel yetenek seviyesi
- **Form** – Son maçlardaki performans
- **Popülerlik** – Medya ve taraftar ilgisi
- **Ahlak/İtibar** – Gizli değer, disiplin ve etik davranışlar
- **Disiplin** – Kart geçmişi, sakatlık geçmişi, takım içi ilişkiler

**Seçilme Eşiği:**
- Her pozisyon için belirli bir OVR eşiği vardır
- Form ve popülerlik eşiği düşürebilir veya yükseltebilir
- Ahlak/İtibar düşükse seçilme şansı azalır

### 25.2 Takvim Entegrasyonu

**Milli maç arası:**
- Hafta döngüsü farklılaşır
- Normal lig maçları arasına milli maçlar eklenir
- Ekstra basın ilgisi ve sponsor aktiviteleri tetiklenir

**Milli Maç Öncesi:**
- Özel basın konferansları
- Sponsor etkinlikleri
- Takım arkadaşlarıyla ilişki etkileşimleri

### 25.3 Ödüller

**Milli takım seçilmesi:**
- **Prestij + Sponsor Kapısı** – Yeni sponsor teklifleri
- **Daha Fazla Transfer İlgisi** – Üst seviye kulüplerden teklifler
- **Popülerlik Artışı** – Medya ve sosyal medya takipçi artışı
- **Maaş Pazarlığı Avantajı** – Kulüplerle pazarlıkta daha güçlü pozisyon

**Milli Maç Performansı:**
- Yüksek rating → daha fazla prestij
- Gol/Asist → sponsor bonusları
- Büyük turnuvalarda başarı → kariyer zirvesi

---

## 🏟 KULÜP HEDEFLERİ & YÖNETİM BEKLENTİSİ

### Kulüp Hedefi Türleri

Her kulüp sezon başında bir hedef belirler:

- **"Kümede Kal"** – Alt liglerde, düşme hattından uzak durma
- **"Playoff"** – Belirli bir sıralama hedefi
- **"İlk 5"** – Üst sıralarda yer alma
- **"Şampiyonluk"** – Lig birinciliği
- **"Genç Oynat"** – Genç oyunculara şans verme (uzun vadeli hedef)

### Yönetim Memnuniyeti Faktörleri

Yönetim memnuniyeti şu faktörlere bağlıdır:

- **Hedef Tutma** – Sezon sonunda hedefe ulaşma durumu
- **Disiplin** – Oyuncu disiplin sorunları, kart sayıları
- **Medya** – Basın ve sosyal medya ilişkileri
- **Sözleşme Maddeleri** – Bonus hedeflerinin tutma durumu

### Hedef Tutmazsa Sonuçlar

**Yönetim memnuniyeti düşerse:**

- **Koç Değişimi İhtimali** – Teknik direktör değişikliği
- **Oyuncu Satılabilir** – Transfer listesine çıkarılma riski
- **Maaş Pazarlığı Zorlaşır** – Sözleşme uzatma ve maaş artışı zorlaşır
- **İlk 11 Şansı Azalır** – Rotasyon veya yedek kulübesi riski

**Yönetim memnuniyeti yüksekse:**

- Sözleşme uzatma kolaylaşır
- Maaş artışı teklifleri gelir
- Kaptanlık şansı artar
- Transfer tekliflerinde kulüp daha az direnir

---

## 🧩 OYUN DURUMLARI (GAME STATES)

Oyun, farklı durumlar (states) arasında geçiş yapar. Her durum kendi UI ve mantığını içerir.

### Durumlar Listesi

1. **MainMenu** – Ana menü ekranı
2. **SaveSlots** – Kayıt slotları seçim ekranı
3. **NewCareerFlow** – Yeni kariyer oluşturma akışı
4. **CareerHub** – Kariyer ana ekranı (maç, haberler, antreman vb.)
5. **MatchPre** – Maç öncesi ekranı (kadro, enerji, moral)
6. **MatchSim** – Maç simülasyonu (arka plan simülasyonu)
7. **Chance** – Pozisyon kontrolü (oyuncu müdahale anı)
8. **Pause** – Maç duraklatma ekranı
9. **PostMatch** – Maç sonu ekranı (rating, ödüller, röportaj)
10. **TransferWindow** – Transfer dönemi ekranı
11. **CriticalEventPopUp** – Kritik olay pop-up'ı (haber, rüşvet, sakatlık vb.)

### Durum Geçiş Kuralları

**Kritik Olay Önceliği:**
- **CriticalEventPopUp**, "Maça git"ten önce önceliklidir
- Oyuncu kritik olayı görmeden maça gidemez
- Kritik olaylar: önemli haberler, rüşvet teklifleri, sakatlık uyarıları, transfer teklifleri

**Maç Akışı:**
```
CareerHub → MatchPre → MatchSim → (Chance) → PostMatch → CareerHub
```

**Transfer Dönemi:**
```
CareerHub → TransferWindow → (Teklifler) → CareerHub
```

---

## 💾 SAVE DESIGN & AUTO-SAVE (GENİŞLETİLDİ)

### Save İçerikleri (Detaylı)

**PlayerProfile:**
- Statlar (OVR, Pace, Shooting, Passing, Dribbling, Defense, Stamina)
- Gizli değerler:
  - `InjuryProne` – Sakatlık eğilimi
  - `Morality` – Ahlak/İtibar değeri
  - `Discipline` – Disiplin skoru
- İsim, uyruk, yaş, pozisyon
- Piyasa değeri

**ClubData:**
- Kulüp ID (Data Pack referansı)
- Lig ID
- Sözleşme bilgileri (maaş, süre, bonuslar)
- Kulüp hedefi ve yönetim memnuniyeti

**SeasonData:**
- Sezon numarası
- Hafta numarası
- Fikstür (oynanan/oynanacak maçlar)
- Puan durumu
- Takım sıralaması

**RelationsData:**
- Teknik direktör ilişkisi
- Takım arkadaşları ilişkileri
- Sponsor ilişkileri
- Aile ilişkileri
- Sevgili ilişkisi
- Menajer ilişkisi

**EconomyData:**
- Para (Euro)
- Envanter (enerji içeceği, rehab item sayıları)
- Sahip olunan eşyalar (ev, araba, kozmetik)

**MediaData:**
- Popülerlik seviyesi
- Basın puanı
- Sosyal medya takipçi sayısı
- Son haberler ve etkileşimler

**DataPackRefs:**
- Aktif pack kimlikleri (`activeDataPackId`)
- Pack versiyon bilgisi (`dataPackVersion`)

### Otomatik Kayıt Noktaları (Auto-save)

- **Maç bitince** – PostMatch ekranında
- **Transfer sonucu** – Kabul/ret/pazarlık sonrası
- **Market alışverişi** – Satın alma işlemi sonrası
- **İlişki aksiyonu** – Hediye/karar sonrası
- **Data pack değişimi** – Aktif pack değiştiğinde

**Auto-save göstergesi:**
- Kayıt sırasında ekranda küçük bir "Kaydediliyor..." göstergesi görünür
- Kritik noktalarda kayıt başarısız olursa uyarı verilir

---

## ✅ MVP KAPSAMI (v0.1 – İlk Yayınlanabilir Sürüm)

### v0.1'de OLMASI GEREKENLER

- **MainMenu + SaveSlots** – Ana menü ve kayıt slotları
- **NewCareerFlow** – DataPack seçimi + karakter oluşturma + 3 teklif + imza
- **CareerHub** – En az maç sayfası (ana ekran)
- **MatchSim + Chance** – Temel oynanış (hareket/pas/şut)
- **Basın** – Temel haber listesi + tepki sistemi
- **Sosyal Medya** – Temel post akışı + takipçi sistemi
- **Transfer** – Kabul/ret temel sistemi (yaz + ara transfer dönemleri)
- **Sakatlık** – Hafif/orta sakatlık + rehab item
- **Market** – Enerji içeceği + 2 krampon tier
- **Form/Moral/Energy** – Temel bağlantı sistemi

### v0.1'de SONRA (Gelecek Sürümler)

- **Kumar/Bahis Minigame** – At yarışı, rulet, slot, blackjack
- **Gelişmiş Koç Değişimi** – Detaylı taktik editörü
- **Online/PvP** – Çok oyunculu modlar
- **Tam Kapsamlı Taktik Editörü** – Formasyon, oyun stili, takım taktikleri
- **Milli Takım Sistemi** – Seçilme ve milli maçlar
- **Kulüp Hedefleri Detayı** – Yönetim beklentisi sistemi
- **Gelişmiş İlişki Sistemi** – Daha fazla aktivite ve etkileşim

---

## 🎛 DENGE & TELEMETRİ (GELİŞTİRİCİ NOTU)

### Amaç

Oyunu "hissiyatla" değil "veriyle" dengelemek için loglanacak değerler.

### Loglanacak Değerler

**Maç İstatistikleri:**
- **Maç başı chance sayısı** – Ortalama kaç pozisyon üretiliyor?
- **Şut → Gol oranı** – Şutların ne kadarı gol oluyor?
- **Pas başarı oranı** – Pasların ne kadarı başarılı?
- **Ortalama rating dağılımı** – Oyuncuların rating'leri nasıl dağılıyor? (6.0–7.0 arası mı, 8.0+ çok mu nadir?)

**Sistem İstatistikleri:**
- **Sakatlık frekansı** – Ne sıklıkla sakatlık oluyor? (maç başı, sezon başı)
- **Transfer teklif sıklığı** – Oyuncuya ne sıklıkla teklif geliyor?
- **Rüşvet olayları ve yakalanma oranı** – Rüşvet sistemi dengeli mi?

**Ekonomi İstatistikleri:**
- **Para kazanma hızı** – Oyuncu ne kadar hızlı para kazanıyor?
- **Market kullanımı** – Enerji içeceği ve rehab item satın alma sıklığı
- **Sözleşme bonusları** – Bonus hedeflerine ulaşma oranı

**İlişki İstatistikleri:**
- **İlişki değişim hızı** – İlişkiler ne kadar hızlı değişiyor?
- **Hediye/aktivite etkisi** – Hangi aktiviteler daha etkili?

### Dengeleme Hedefleri

**Maç Dengesi:**
- Maç başı 2–6 chance (ilk 11 için)
- Şut → Gol oranı: %15–25 (pozisyona göre değişir)
- Pas başarı oranı: %70–85
- Rating dağılımı: Çoğu maç 6.0–7.5 arası, 8.0+ nadir (%10–15)

**Sistem Dengesi:**
- Sakatlık: Sezon başı 0.5–1.5 ortalama
- Transfer teklifi: Sezon başı 2–5 (performansa göre)
- Rüşvet yakalanma: %20–30 (risk seviyesine göre)

**Ekonomi Dengesi:**
- Oyuncu haftalık gelir: Maaş + bonuslar
- Market ürünleri: Erişilebilir ama stratejik kullanım gerektirir
- Sözleşme bonusları: %60–80 ulaşılabilir hedefler

### Telemetri Kullanımı

**Geliştirme Aşamasında:**
- Her test oyununda bu değerler loglanır
- Dengeleme kararları veriye dayalı alınır

**Yayın Sonrası:**
- Anonim telemetri toplanır (oyuncu izniyle)
- Oyun güncellemelerinde denge ayarları yapılır

**Örnek Senaryo:**
- Eğer maç başı chance sayısı 8+ ise → Chance üretim formülü ayarlanır
- Eğer şut → gol oranı %5'ten az ise → Şut mekaniği güçlendirilir
- Eğer sakatlık çok sık ise → Sakatlık riski düşürülür

---

## 🧩 ÇELİŞKİ / NETLEŞTİRME NOTLARI

- Kaleci oynanamayacak (kariyer oyuncusu GK seçemez) ama maçlarda kaleci AI vardır.
- Antreman limiti maç başı 2 (ileride max 3).
- "Gösteri Maçı" menüde kalır ama Coming Soon.

---

## 📝 SON NOT

Bu sürümle doküman artık **"oyunu kurduracak" kadar complete:**

- ✅ Döngü net
- ✅ Sezon/takvim net
- ✅ Koç/management bağları net
- ✅ Disiplin + milli takım + form eklendi
- ✅ Data pack tek paket mantığı oturdu
- ✅ Chance'in sim'e bağlanacağı yer tariflendi
- ✅ Save sistemi detaylandırıldı
- ✅ Oyun durumları (Game States) tanımlandı
- ✅ MVP kapsamı netleştirildi
- ✅ Dengeleme ve telemetri sistemi eklendi

**Doküman hazır. Artık kodlamaya geçilebilir.**

---

## 📝 NOTLAR

Bu doküman, Titan Soccer oyununun kapsamlı tasarım rehberidir. Tüm sistemler ve mekanikler bu dokümana göre geliştirilmelidir.

