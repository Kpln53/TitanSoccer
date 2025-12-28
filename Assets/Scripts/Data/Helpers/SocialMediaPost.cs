using System;
using System.Collections.Generic;

/// <summary>
/// Sosyal medya postu - Sosyal medya sistemi için
/// </summary>
[Serializable]
public class SocialMediaPost
{
    [Header("Temel Bilgiler")]
    public string content;                 // Post içeriği
    public string author;                  // Yazar (@OyuncuAdı)
    public SocialMediaPostType type;       // Post türü
    
    [Header("Tarih")]
    public DateTime date;                  // Tarih
    public string dateString;              // Tarih (string)
    
    [Header("Etkileşim")]
    public int likes;                      // Beğeni sayısı
    public int comments;                   // Yorum sayısı
    public List<string> commentList;       // Yorum listesi (basit string listesi)
    
    [Header("Ek Bilgiler")]
    public bool isLikedByPlayer;           // Oyuncu tarafından beğenildi mi?
    
    public SocialMediaPost()
    {
        date = DateTime.Now;
        dateString = date.ToString("yyyy-MM-dd HH:mm:ss");
        type = SocialMediaPostType.Normal;
        likes = 0;
        comments = 0;
        commentList = new List<string>();
        isLikedByPlayer = false;
    }
}

