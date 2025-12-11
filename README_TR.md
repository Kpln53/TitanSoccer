# Titan Soccer - Maç Akışı ve Kurulum

Bu depo, Unity **6000.2.13f1** sürümüyle hazırlanmış Titan Soccer prototipini içerir. Aşağıdaki adımlar kurulum ve yeni maç ekranını denemek için gerekli özet bilgileri sunar.

## Kurulum
1. Unity Hub üzerinden **6000.2.13f1** sürümünü kurun.
2. Depoyu klonladıktan sonra `TitanSoccer` klasörünü Unity Hub'da **Add project from disk** ile açın.
3. Proje açıldığında varsayılan **Universal Render Pipeline** ayarları otomatik olarak yüklenir; ekstra paket kurulumuna gerek yoktur.

## Maça Gitme Akışı
- Ana menüden “Maça Git” seçildiğinde önce kadro ve genel bilgileri gösterecek ara ekran planlanmıştır. Bu ara ekranın ardından `Scenes/MatchScene` içindeki yeni maç ekranı açılır.
- Maç ekranında üstte skorboard, reytingler ve maç saati; ortada mini saha durumu ve topla oynama yüzdesi; altında enerji/moral barları ve spiker yorumları; en altta hedefler, kadro, maç hızı, simülasyon ve başlat butonları yer alır.

## Maç Ekranını Test Etme
1. `Scenes/MatchScene` sahnesini açın.
2. **Başlat** tuşuna basarak maçı başlatın.
3. Zaman ilerledikçe enerji yavaşça düşer, moral gollerle değişir ve spiker topla oynama yüzdesine göre farklı cümleler kurar.
4. Moral/enerji ve topla oynama; oyuncuya rastgele pozisyon gelme ihtimalini artırır. Pozisyon geldiğinde koşu ve şut fazlarına geçilir.
5. **Maç Hızı** butonu ile 1x/2x arasında geçiş yapabilir, **Maçı Simüle Et** ile hızlıca sonucu görebilirsiniz.

> Not: Hedefler ve kadro butonları şimdilik bilgilendirici mesaj üretir; içerikleri gelecek iterasyonlarda doldurulacaktır.
