using System.Collections.Generic;
using UnityEngine;
using TitanSoccer.Social.Data;

namespace TitanSoccer.Social.Systems
{
    public static class PostGenerator
    {
        public static List<SocialPost> GeneratePostOptions(MatchData matchData)
        {
            List<SocialPost> options = new List<SocialPost>();
            SocialMediaDatabase db = SocialMediaManager.Instance.database;

            if (db == null)
            {
                Debug.LogError("SocialMediaDatabase is not assigned in Manager!");
                return options;
            }

            if (matchData == null) return options;

            // Maç Verilerini Çözümle
            bool isHome = matchData.isHomeTeam;
            int myScore = isHome ? matchData.homeScore : matchData.awayScore;
            int opScore = isHome ? matchData.awayScore : matchData.homeScore;
            bool isWin = myScore > opScore;
            bool isDraw = myScore == opScore;
            bool scoredGoal = matchData.playerGoals > 0;

            // 1. Seçenek: Pozitif / Klasik (Galibiyet veya Mağlubiyet durumuna göre)
            if (isWin)
            {
                options.Add(CreatePlayerPost(db.GetRandomTemplate(db.WinPostsPositive), PostCategory.MatchWin, PostTone.Positive));
                options.Add(CreatePlayerPost(db.GetRandomTemplate(db.WinPostsHumble), PostCategory.MatchWin, PostTone.Professional));
            }
            else if (isDraw)
            {
                // Beraberlik şablonları eklenebilir, şimdilik nötr
                options.Add(CreatePlayerPost("Tough game, we settle for a point. ⚽", PostCategory.MatchDraw, PostTone.Neutral));
            }
            else
            {
                options.Add(CreatePlayerPost(db.GetRandomTemplate(db.LossPostsSad), PostCategory.MatchLoss, PostTone.Negative));
                options.Add(CreatePlayerPost(db.GetRandomTemplate(db.LossPostsMotivated), PostCategory.MatchLoss, PostTone.Motivational));
            }

            // 2. Seçenek: Kişisel Performans (Gol attıysa)
            if (scoredGoal)
            {
                // Basit string birleştirme, ileride Database'den çekilebilir
                string goalText = isWin ? "Great feeling to verify the net and get the win! ⚽🔥" : "Happy to score, but the result is what matters. We go again. ⚽";
                options.Add(CreatePlayerPost(goalText, PostCategory.GoalScored, PostTone.Positive));
            }

            return options;
        }

        public static SocialPost GenerateNewsPost()
        {
            SocialMediaDatabase db = SocialMediaManager.Instance.database;
            string content = db.GetRandomTemplate(db.MatchHeadlines);
            
            return new SocialPost(
                "Global Sports", 
                "@GlobalSportsNews", 
                content, 
                PostType.News, 
                PostCategory.General
            );
        }

        private static SocialPost CreatePlayerPost(string content, PostCategory category, PostTone tone)
        {
            // Player ismi ileride dinamik alınacak (şimdilik "Me")
            return new SocialPost("My Player", "@MyHandle", content, PostType.Player, category, tone);
        }
    }
}
