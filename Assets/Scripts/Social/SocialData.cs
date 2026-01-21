using System;
using System.Collections.Generic;
using UnityEngine;

namespace TitanSoccer.Social
{
    public enum PostType
    {
        PlayerPost, // Oyuncunun paylaştığı
        FanPost,    // Taraftar yorumu/postu
        ClubPost,   // Kulüp resmi hesabı
        NewsPost    // Haber hesabı
    }

    public enum Sentiment
    {
        Positive,
        Negative,
        Neutral
    }

    [Serializable]
    public class SocialPostData
    {
        public string id;
        public string authorName;
        public string handle; // @ornek
        public string content;
        public Sprite image; // Opsiyonel resim
        public PostType type;
        
        public int likes;
        public int commentsCount;
        public string timeAgo;

        public List<SocialCommentData> comments;

        public SocialPostData()
        {
            id = Guid.NewGuid().ToString();
            comments = new List<SocialCommentData>();
            timeAgo = "Şimdi";
        }
    }

    [Serializable]
    public class SocialCommentData
    {
        public string authorName;
        public string handle;
        public string content;
        public int likes;
        public Sentiment sentiment; // Yorumun hissiyatı
    }

    [Serializable]
    public class PostOption
    {
        public string buttonText; // Butonda yazacak metin (Örn: "Övgü Paylaş")
        public string postContent; // Paylaşılacak asıl metin
        public Sentiment predictedOutcome; // Beklenen sonuç (Mantıklı/Mantıksız)
        public string description; // Oyuncuya ipucu (Örn: "Takım arkadaşlarını motive et")
    }
}
