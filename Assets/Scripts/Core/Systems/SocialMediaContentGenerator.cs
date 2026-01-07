using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Sosyal medya içeriği üreticisi (Singleton)
/// Maç sonuçlarına göre dinamik metinler üretir.
/// </summary>
public class SocialMediaContentGenerator : MonoBehaviour
{
    public static SocialMediaContentGenerator Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Oyuncu için maç sonu paylaşım seçenekleri üretir
    /// </summary>
    public List<SocialMediaPost> GeneratePlayerPostOptions(MatchData match)
    {
        List<SocialMediaPost> options = new List<SocialMediaPost>();

        // 1. Övgü (Positive)
        options.Add(GenerateOption(match, SocialMediaPostTone.Positive));
        
        // 2. Eleştiri / Öz eleştiri (Negative/Critical)
        options.Add(GenerateOption(match, SocialMediaPostTone.Negative));

        // 3. Motivasyon (Motivational)
        options.Add(GenerateOption(match, SocialMediaPostTone.Motivational));

        return options;
    }

    private SocialMediaPost GenerateOption(MatchData match, SocialMediaPostTone tone)
    {
        SocialMediaPost post = new SocialMediaPost();
        post.tone = tone;
        post.type = SocialMediaPostType.Normal;
        post.author = "@Player"; 
        post.context = $"{match.homeTeamName} vs {match.awayTeamName} Maçı";
        
        bool isWin = false;
        // Basit kazanan kontrolü (Varsayım: PlayerTeam ev sahibi veya deplasman olabilir, şimdilik skor üzerinden genel mantık)
        // Gerçekte: if (match.GetWinner() == GameManager.Instance.PlayerTeamName) ...
        // Şimdilik oyuncunun performansı yüksekse ve skor iyiyse "iyi sonuç" varsayalım.
        
        // Cümle Havuzları
        switch (tone)
        {
            case SocialMediaPostTone.Positive:
                if (match.playerGoals >= 3)
                    post.content = "Hat-trick topu evime gidiyor! İnanılmaz bir geceydi! ⚽⚽⚽";
                else if (match.playerGoals > 0)
                    post.content = $"Gol atmak harika hissettiriyor! Takım arkadaşlarıma teşekkürler. 🔥";
                else if (match.playerRating >= 8.0f)
                    post.content = "Sahada her şeyimi verdim. Bu takımın bir parçası olmaktan gurur duyuyorum! 💪";
                else if (match.playerAssists > 0)
                    post.content = "Takıma katkı sağlamak her şeyden önemli. Güzel asistti! 🅰️";
                else
                    post.content = "3 puan bizim! Destekleyen herkese teşekkürler. Yolumuza devam ediyoruz!";
                
                post.potentialFeedbackScore = 90;
                break;

            case SocialMediaPostTone.Negative:
                if (match.playerRating < 5.0f)
                    post.content = "Bugün sahada kendim gibi değildim. Bunun telafisi olacak. 🙏";
                else if (match.homeScore == match.awayScore) // Beraberlik
                    post.content = "Kazanabileceğimiz bir maçtı. 1 puanla yetinmek üzücü.";
                else
                    post.content = "Skor tabelası istediğimiz gibi değil. Daha çok çalışıp geri döneceğiz.";
                
                post.potentialFeedbackScore = 65; 
                break;

            case SocialMediaPostTone.Motivational:
                if (match.matchType == MatchData.MatchType.Derby) // Derbi varsayımı
                    post.content = "Derbiler her zaman zordur. Savaşmaya devam edeceğiz! ⚡";
                else
                    post.content = "Düşsek de kalkmasını biliriz. Odak noktamız bir sonraki maç. #NeverGiveUp";
                
                post.potentialFeedbackScore = 85;
                break;
                
            default:
                post.content = "Maç bitti.";
                break;
        }

        return post;
    }

    /// <summary>
    /// Maç sonrası feed için taraftar/medya yorumları üretir
    /// </summary>
    public List<SocialMediaPost> GenerateFeedPosts(MatchData match)
    {
        List<SocialMediaPost> feed = new List<SocialMediaPost>();
        
        // 1. Kulüp Resmi Hesabı (@Club)
        SocialMediaPost clubPost = new SocialMediaPost();
        // Basitleştirilmiş kulüp adı
        string clubName = "ClubOfficial";
        // Eğer oyuncunun takımı biliniyorsa o kullanılır, şimdilik homeTeam üzerinden gidiyoruz
        clubPost.author = "@" + clubName;
        clubPost.content = $"MAÇ SONUCU | {match.homeTeamName} {match.homeScore} - {match.awayScore} {match.awayTeamName}";
        clubPost.likes = Random.Range(2000, 10000);
        clubPost.tone = SocialMediaPostTone.Neutral;
        feed.Add(clubPost);

        // 2. Medya Yorumu (Gazeteci/Spor Sayfası)
        SocialMediaPost mediaPost = new SocialMediaPost();
        mediaPost.author = "@SporMerkezi";
        if (match.playerGoals >= 2)
        {
            mediaPost.content = $"{match.homeTeamName} maçında yıldızlaşan isim yine aynı! Gol makinesi iş başında. 🔥";
            mediaPost.likes = Random.Range(500, 2000);
        }
        else if (match.playerRating > 8.5f)
        {
            mediaPost.content = "Sahada basmadık yer bırakmadı. Maçın adamı performansı! 👏";
            mediaPost.likes = Random.Range(400, 1500);
        }
        else
        {
            mediaPost.content = "Zorlu mücadeleden geriye kalanlar... Haftanın özeti yakında.";
            mediaPost.likes = Random.Range(100, 500);
        }
        feed.Add(mediaPost);

        // 3. Taraftar Yorumları (Fan)
        int fanCount = Random.Range(2, 5); // 2-4 arası fan yorumu
        for (int i = 0; i < fanCount; i++)
        {
            SocialMediaPost fanPost = new SocialMediaPost();
            fanPost.author = "@Fan_" + Random.Range(1000, 9999);
            
            float rnd = Random.value;
            if (match.playerRating > 7.5f)
            {
                if (rnd > 0.5f) fanPost.content = "Forma aşkı budur! Helal olsun. ❤️💙";
                else fanPost.content = "Böyle oynasın canımı yesin. Büyük topçu.";
                fanPost.tone = SocialMediaPostTone.Positive;
                fanPost.likes = Random.Range(50, 300);
            }
            else if (match.playerRating < 5.5f)
            {
                if (rnd > 0.5f) fanPost.content = "Yine sahada yoktu. Ne zaman düzelecek bu performans?";
                else fanPost.content = "Hoca neden hala ısrar ediyor anlamıyorum.";
                fanPost.tone = SocialMediaPostTone.Negative;
                fanPost.likes = Random.Range(20, 150);
            }
            else
            {
                fanPost.content = "İyi mücadele ama gol lazım bize gol!";
                fanPost.tone = SocialMediaPostTone.Neutral;
                fanPost.likes = Random.Range(10, 80);
            }
            feed.Add(fanPost);
        }

        return feed;
    }
}
