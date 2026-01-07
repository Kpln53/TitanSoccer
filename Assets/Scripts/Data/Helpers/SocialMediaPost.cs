using System;
using System.Collections.Generic;
using UnityEngine;

public enum SocialMediaPostTone
{
    Neutral,
    Positive,    // Övgü dolu / Mutlu
    Negative,    // Eleştirel / Kızgın
    Motivational // Geleceğe odaklı / Hırslı
}

/// <summary>
/// Sosyal medya postu - Sosyal medya sistemi için
/// </summary>
[System.Serializable]
public class SocialMediaPost
{
    [Header("Temel Bilgiler")]
    public string content;                 // Post içeriği
    public string author;                  // Yazar (@OyuncuAdı)
    public SocialMediaPostType type;       // Post türü
    
    [Header("Bağlam ve Ton")]
    public SocialMediaPostTone tone = SocialMediaPostTone.Neutral; // Postun tonu
    public string context;                 // Hangi olayla ilgili? (Örn: "Real Madrid Maçı")
    public int potentialFeedbackScore;     // Bu post ne kadar etkileşim alabilir? (0-100)

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
        tone = SocialMediaPostTone.Neutral;
        likes = 0;
        comments = 0;
        commentList = new List<string>();
        isLikedByPlayer = false;
        context = "";
        potentialFeedbackScore = 50;
    }
}

