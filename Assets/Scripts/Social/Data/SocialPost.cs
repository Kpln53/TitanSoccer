using System;
using UnityEngine;

namespace TitanSoccer.Social.Data
{
    [Serializable]
    public class SocialPost
    {
        public string ID;
        public string AuthorName;
        public string Handle; // @kullaniciadi
        public string Content;
        public Sprite Avatar; // İsteğe bağlı, UI'da set edilecek
        
        public int Likes;
        public int Comments;
        public int Shares;
        
        public DateTime Timestamp;
        public string FormattedTime => GetTimeAgo();

        public PostType Type;
        public PostCategory Category;
        public PostTone Tone;

        public SocialPost(string author, string handle, string content, PostType type, PostCategory category, PostTone tone = PostTone.Neutral)
        {
            ID = Guid.NewGuid().ToString();
            AuthorName = author;
            Handle = handle;
            Content = content;
            Type = type;
            Category = category;
            Tone = tone;
            Timestamp = DateTime.Now; // Oyun içi zamanla değiştirilecek
            
            // Başlangıç etkileşimleri (rastgele başlangıç için)
            Likes = 0;
            Comments = 0;
            Shares = 0;
        }

        private string GetTimeAgo()
        {
            // Basit bir zaman formatlayıcı, ileride oyun saatine bağlanabilir
            TimeSpan diff = DateTime.Now - Timestamp;
            if (diff.TotalMinutes < 1) return "Şimdi";
            if (diff.TotalMinutes < 60) return $"{(int)diff.TotalMinutes}d önce";
            if (diff.TotalHours < 24) return $"{(int)diff.TotalHours}s önce";
            return $"{(int)diff.TotalDays}g önce";
        }
    }
}
