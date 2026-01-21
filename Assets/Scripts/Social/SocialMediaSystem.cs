using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TitanSoccer.Social
{
    public class SocialMediaSystem : MonoBehaviour
    {
        public static SocialMediaSystem Instance { get; private set; }

        // Takipçi sayısı
        public int Followers { get; private set; } = 1400000; 

        // Maç sonrası paylaşım hakkı
        public int PostsRemaining { get; private set; } = 2;

        // Son oynanan maçın verisi
        public MatchData LastMatchContext { get; private set; }
        
        // Oyuncu bu maç hakkında zaten post attı mı?
        private bool _hasPostedAboutLastMatch = false;

        // Ana Feed Listesi
        public List<SocialPostData> Feed { get; private set; } = new List<SocialPostData>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void AddFollowers(int amount)
        {
            Followers += amount;
            if (Followers < 0) Followers = 0;
        }

        public void ResetDailyLimit()
        {
            PostsRemaining = 2;
            _hasPostedAboutLastMatch = false; // Yeni maç, yeni reaksiyon hakkı
        }

        public bool CanPost()
        {
            return PostsRemaining > 0;
        }

        public void UsePostRight()
        {
            if (PostsRemaining > 0) PostsRemaining--;
            _hasPostedAboutLastMatch = true; // Artık maç hakkında konuştuk, sıradaki postlar genel olacak
        }

        public void SetLastMatch(MatchData match)
        {
            LastMatchContext = match;
            ResetDailyLimit();
            
            // Maç bittiğinde otomatik dünya içeriklerini üret
            GenerateWorldContent(match);
        }

        // --- DÜNYA SİMÜLASYONU (Haberler & Diğer Takımlar) ---
        public void GenerateWorldContent(MatchData playerMatch)
        {
            // 1. Oyuncunun Maçı Hakkında Haberler
            GenerateMatchNews(playerMatch);

            // 2. Diğer Takımların Postları (Simülasyon)
            GenerateAIClubPosts();
        }

        private void GenerateMatchNews(MatchData match)
        {
            string[] newsHandles = { "@SporManşet", "@FutbolGlobal", "@SonDakikaSpor", "@LigMerkezi" };
            string handle = newsHandles[Random.Range(0, newsHandles.Length)];
            
            string content = "";
            string resultStr = $"{match.homeTeamName} {match.homeScore}-{match.awayScore} {match.awayTeamName}";

            if (match.playerRating > 8.0f)
            {
                content = $"MAÇ SONUCU: {resultStr}. Can Yıldız sahanın yıldızıydı! ({match.playerRating} Puan) 🔥";
            }
            else if (match.IsDraw())
            {
                content = $"Zorlu mücadelede kazanan çıkmadı! {resultStr}. İki takım da puanları paylaştı.";
            }
            else
            {
                string winner = match.homeScore > match.awayScore ? match.homeTeamName : match.awayTeamName;
                content = $"MAÇ SONUCU: {resultStr}. {winner} 3 puanı hanesine yazdırdı.";
            }

            SocialPostData newsPost = new SocialPostData
            {
                authorName = handle.Substring(1),
                handle = handle,
                content = content,
                type = PostType.NewsPost,
                timeAgo = "Az önce"
            };

            // Haberlere rastgele yorumlar ekle
            newsPost.comments = GenerateRandomComments(5);
            CalculateEngagement(newsPost, 500000); // Haber sayfalarının takipçisi çok olur

            AddToFeed(newsPost);
        }

        private void GenerateAIClubPosts()
        {
            // Ligdeki rastgele takımlardan postlar (Örnek veriler)
            string[] otherTeams = { "Galatasaray", "Fenerbahçe", "Beşiktaş", "Trabzonspor", "Man City", "Liverpool", "Bayern" };
            
            // 2 tane rastgele takım postu oluştur
            for (int i = 0; i < 2; i++)
            {
                string team = otherTeams[Random.Range(0, otherTeams.Length)];
                if (LastMatchContext != null && (team == LastMatchContext.homeTeamName || team == LastMatchContext.awayTeamName)) continue; // Bizim maçtaki takımlar olmasın

                bool isWin = Random.value > 0.5f;
                string content = isWin ? 
                    $"Önemli bir galibiyet! Taraftarımıza armağan olsun. 🦁 #{team}" : 
                    $"Bugün istediğimiz sonucu alamadık. Haftaya telafi edeceğiz. #{team}";

                SocialPostData clubPost = new SocialPostData
                {
                    authorName = team,
                    handle = $"@{team}Official",
                    content = content,
                    type = PostType.ClubPost,
                    timeAgo = "10dk önce"
                };

                clubPost.comments = GenerateRandomComments(3);
                CalculateEngagement(clubPost, 2000000); // Büyük kulüp
                AddToFeed(clubPost);
            }
        }

        public void AddToFeed(SocialPostData post)
        {
            Feed.Insert(0, post);
            // Feed çok şişmesin, son 50 postu tut
            if (Feed.Count > 50) Feed.RemoveAt(Feed.Count - 1);
        }

        // --- POST SEÇENEKLERİ ÜRETME ---
        public List<PostOption> GeneratePostOptions(MatchData matchData)
        {
            if (matchData == null) matchData = LastMatchContext;
            List<PostOption> options = new List<PostOption>();

            // DURUM 1: Maç hakkında henüz post atılmadıysa -> MAÇ REAKSİYONU
            if (!_hasPostedAboutLastMatch && matchData != null)
            {
                bool isWin = (matchData.isHomeTeam && matchData.homeScore > matchData.awayScore) || (!matchData.isHomeTeam && matchData.awayScore > matchData.homeScore);
                bool isLoss = (matchData.isHomeTeam && matchData.homeScore < matchData.awayScore) || (!matchData.isHomeTeam && matchData.awayScore < matchData.homeScore);

                if (isWin)
                {
                    options.Add(new PostOption { buttonText = "KUTLAMA", description = "Galibiyeti kutla!", postContent = $"Harika bir galibiyet! {matchData.homeScore}-{matchData.awayScore}! Takım arkadaşlarımı tebrik ediyorum. 🔥⚽", predictedOutcome = Sentiment.Positive });
                    options.Add(new PostOption { buttonText = "ALÇAKGÖNÜLLÜ", description = "Rakibi tebrik et.", postContent = "Zorlu bir maçtı, rakibimizi mücadelesinden dolayı kutlarım. Önümüzdeki maçlara odaklanacağız. 🙏", predictedOutcome = Sentiment.Positive });
                    options.Add(new PostOption { buttonText = "İDDİALI", description = "Gücünü göster.", postContent = "Bizi kimse durduramaz! Şampiyonluk geliyor! 🏆", predictedOutcome = Sentiment.Neutral });
                }
                else if (isLoss)
                {
                    options.Add(new PostOption { buttonText = "ÖZÜR DİLE", description = "Taraftardan özür dile.", postContent = "Bugün istediğimiz sonucu alamadık. Sizi üzdüğümüz için özür dileriz. Daha çok çalışacağız. 😔", predictedOutcome = Sentiment.Positive });
                    options.Add(new PostOption { buttonText = "MOTİVASYON", description = "Takımı ateşle.", postContent = "Düşsek de kalkmasını biliriz. Bu mağlubiyet bize ders olacak. Asla pes etmek yok! 💪", predictedOutcome = Sentiment.Positive });
                    options.Add(new PostOption { buttonText = "HAKEM/ŞANS", description = "Şanssızlıktan bahset.", postContent = "Bugün şans bizden yana değildi. Hakem kararları da tartışılır... Önümüze bakacağız.", predictedOutcome = Sentiment.Negative });
                }
                else // Beraberlik
                {
                    options.Add(new PostOption { buttonText = "ANALİZ", description = "Maçı değerlendir.", postContent = "Zorlu bir mücadeleydi. 1 puan iyidir ama daha fazlasını istiyorduk.", predictedOutcome = Sentiment.Neutral });
                    options.Add(new PostOption { buttonText = "MOTİVASYON", description = "Geleceğe bak.", postContent = "Mücadeleye devam. Sonraki maçta 3 puan bizim olacak!", predictedOutcome = Sentiment.Positive });
                }
            }
            // DURUM 2: Maç postu atıldıysa -> GELECEK MAÇ / GENEL KONULAR
            else
            {
                options.Add(new PostOption 
                { 
                    buttonText = "ANTRENMAN", 
                    description = "Çalışmaya devam ettiğini göster.", 
                    postContent = "Durmak yok! Bir sonraki maç için hazırlıklar tam gaz devam ediyor. 🏋️‍♂️⚽", 
                    predictedOutcome = Sentiment.Positive 
                });

                options.Add(new PostOption 
                { 
                    buttonText = "DİNLENME", 
                    description = "Kafa dağıt.", 
                    postContent = "Maç sonrası biraz dinlenme ve toparlanma zamanı. 🎮☕", 
                    predictedOutcome = Sentiment.Neutral 
                });

                options.Add(new PostOption 
                { 
                    buttonText = "TARAFTAR", 
                    description = "Taraftara teşekkür et.", 
                    postContent = "Mesajlarınız ve desteğiniz için teşekkürler. Sizler dünyanın en iyi taraftarısınız! ❤️", 
                    predictedOutcome = Sentiment.Positive 
                });
            }

            return options;
        }

        // --- ETKİLEŞİM HESAPLAMA ---
        public void CalculateEngagement(SocialPostData post, int targetFollowers = -1)
        {
            int baseFollowers = targetFollowers == -1 ? Followers : targetFollowers;
            
            // %2 ile %8 arası etkileşim oranı
            float engagementRate = Random.Range(0.02f, 0.08f);
            
            post.likes = (int)(baseFollowers * engagementRate);
            
            // Yorum sayısı like'ın %1'i ile %5'i arası
            post.commentsCount = (int)(post.likes * Random.Range(0.01f, 0.05f));
        }

        // --- YORUM ÜRETME ---
        public List<SocialCommentData> GenerateComments(SocialPostData post, Sentiment choiceSentiment)
        {
            List<SocialCommentData> comments = new List<SocialCommentData>();
            // Yorum sayısı postun popülerliğine göre ama UI için max 20 tane üretelim
            int displayCount = Mathf.Min(post.commentsCount, Random.Range(5, 15));

            for (int i = 0; i < displayCount; i++)
            {
                comments.Add(GenerateSingleComment(choiceSentiment));
            }

            return comments;
        }
        
        private List<SocialCommentData> GenerateRandomComments(int count)
        {
            List<SocialCommentData> list = new List<SocialCommentData>();
            for(int i=0; i<count; i++) list.Add(GenerateSingleComment(Sentiment.Neutral));
            return list;
        }

        private SocialCommentData GenerateSingleComment(Sentiment sentiment)
        {
            string[] positiveComments = { "Harikasın kaptan! ❤️", "Bu maçı tek başına alırsın!", "Gözümüz üzerinde, harika oynadın.", "Kral sahalara döndü!", "İşte beklediğimiz performans 🔥", "Adamsın!", "Gururumuzsun." };
            string[] negativeComments = { "Daha çok çalışman lazım.", "Bu performans yakışmadı.", "Pas hatalarını düzeltmelisin.", "Takım oyununa odaklan.", "Beklentilerin altındasın.", "Böyle oynayacaksan git." };
            string[] neutralComments = { "Bir sonraki maç daha iyi olacak.", "Başarılar.", "Takipteyiz.", "Güzel maçtı.", "Hayırlısı olsun.", "Maç kaç kaç bitti?" };

            string content = "";
            Sentiment commentSentiment = Sentiment.Neutral;
            float roll = Random.value;

            if (sentiment == Sentiment.Positive)
            {
                if (roll < 0.8f) { content = GetRandom(positiveComments); commentSentiment = Sentiment.Positive; }
                else { content = GetRandom(neutralComments); commentSentiment = Sentiment.Neutral; }
            }
            else if (sentiment == Sentiment.Negative)
            {
                if (roll < 0.7f) { content = GetRandom(negativeComments); commentSentiment = Sentiment.Negative; }
                else { content = GetRandom(neutralComments); commentSentiment = Sentiment.Neutral; }
            }
            else 
            {
                content = GetRandom(neutralComments);
            }

            return new SocialCommentData
            {
                authorName = GenerateRandomUser(),
                handle = "@user" + Random.Range(100, 9999),
                content = content,
                likes = Random.Range(0, 500),
                sentiment = commentSentiment
            };
        }

        private string GetRandom(string[] array) => array[Random.Range(0, array.Length)];

        private string GenerateRandomUser()
        {
            string[] names = { "Ahmet", "Mehmet", "Ayşe", "Fatma", "Can", "Cem", "Elif", "Zeynep", "FutbolDelisi", "Madridista_TR", "GolMakinesi", "UltraAslan", "Fenerli", "Kartal" };
            return names[Random.Range(0, names.Length)];
        }
        
        public string FormatFollowers(int count)
        {
            if (count >= 1000000) return (count / 1000000f).ToString("0.0") + "M";
            if (count >= 1000) return (count / 1000f).ToString("0.0") + "K";
            return count.ToString();
        }
    }
}
