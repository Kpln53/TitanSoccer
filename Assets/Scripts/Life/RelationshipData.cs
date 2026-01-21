using System;
using System.Collections.Generic;
using UnityEngine;

namespace TitanSoccer.Life
{
    public enum RelationshipType
    {
        Girlfriend, // Kız Arkadaş
        Family,     // Aile
        Team,       // Takım Arkadaşları
        Coach,      // Teknik Direktör
        Fans,       // Taraftarlar
        Sponsor,    // Sponsorlar
        Manager     // Menajer
    }

    public enum InteractionType
    {
        Chat,           // Sohbet Et (Rastgele)
        Gift,           // Hediye Al (Para gider, kesin artış)
        Dinner,         // Yemeğe Git
        Nightclub,      // Gece Kulübü (Sadece Takım)
        TrainTogether,  // Birlikte Çalış (Sadece Takım)
        Request         // Talepte Bulun (Sponsor/Hoca)
    }

    [Serializable]
    public class RelationshipData
    {
        public string id;
        public string name; // Örn: "Elif Kaya" veya "Carlo Ancelotti"
        public RelationshipType type;
        public float value; // 0-100 arası ilişki puanı
        
        public string GetStatusText()
        {
            if (value >= 90) return "Mükemmel";
            if (value >= 75) return "Çok İyi";
            if (value >= 60) return "İyi";
            if (value >= 40) return "Nötr";
            if (value >= 20) return "Kötü";
            return "Berbat";
        }

        public Color GetStatusColor()
        {
            if (value >= 80) return new Color(0.2f, 0.8f, 0.2f); // Yeşil
            if (value >= 50) return new Color(1f, 0.8f, 0.2f);   // Sarı
            return new Color(0.9f, 0.2f, 0.2f);                  // Kırmızı
        }
    }

    [Serializable]
    public class InteractionOption
    {
        public string label;
        public int cost; // Para maliyeti
        public int energyCost; // Enerji maliyeti
        public InteractionType type;
    }
}
