using UnityEngine;

namespace TitanSoccer.Social.Data
{
    public enum PostType
    {
        Player, // Oyuncunun attığı post
        AI,     // Diğer futbolcular veya AI karakterler
        News,   // Haberler
        Fan,    // Taraftar yorumları/postları
        Club    // Kulüp resmi hesabı
    }

    public enum PostCategory
    {
        General,
        MatchWin,
        MatchLoss,
        MatchDraw,
        GoalScored,
        AssistMade,
        CleanSheet,
        Training,
        Transfer,
        Injury,
        Promotion,
        Relegation
    }

    public enum PostTone
    {
        Neutral,
        Positive,
        Negative,
        Motivational,
        Controversial,
        Professional
    }
}
