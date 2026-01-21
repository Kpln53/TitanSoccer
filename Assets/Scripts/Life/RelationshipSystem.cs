using System.Collections.Generic;
using UnityEngine;
using TitanSoccer.Life;

public class RelationshipSystem : MonoBehaviour
{
    public static RelationshipSystem Instance { get; private set; }

    public List<RelationshipData> Relationships = new List<RelationshipData>();

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

    private void Start()
    {
        InitializeRelationships();
    }

    private void InitializeRelationships()
    {
        if (Relationships.Count > 0) return;

        // Varsayılan İlişkiler
        Relationships.Add(new RelationshipData { id = "gf", name = "Elif Kaya", type = RelationshipType.Girlfriend, value = 98 });
        Relationships.Add(new RelationshipData { id = "fam", name = "Yıldız Ailesi", type = RelationshipType.Family, value = 85 });
        Relationships.Add(new RelationshipData { id = "coach", name = "Carlo Ancelotti", type = RelationshipType.Coach, value = 60 });
        Relationships.Add(new RelationshipData { id = "team", name = "Takım Arkadaşları", type = RelationshipType.Team, value = 80 });
        Relationships.Add(new RelationshipData { id = "fans", name = "Taraftarlar", type = RelationshipType.Fans, value = 90 });
        Relationships.Add(new RelationshipData { id = "manager", name = "Ahmet Bulut", type = RelationshipType.Manager, value = 70 });
        Relationships.Add(new RelationshipData { id = "sponsor", name = "Nike", type = RelationshipType.Sponsor, value = 50 });
    }

    public List<InteractionOption> GetInteractions(RelationshipType type)
    {
        List<InteractionOption> options = new List<InteractionOption>();

        // Ortak Seçenekler
        options.Add(new InteractionOption { label = "Sohbet Et", cost = 0, energyCost = 5, type = InteractionType.Chat });
        options.Add(new InteractionOption { label = "Hediye Al ($500)", cost = 500, energyCost = 0, type = InteractionType.Gift });
        options.Add(new InteractionOption { label = "Yemeğe Çık ($200)", cost = 200, energyCost = 10, type = InteractionType.Dinner });

        // Özel Seçenekler
        if (type == RelationshipType.Team)
        {
            options.Add(new InteractionOption { label = "Gece Kulübü ($1000)", cost = 1000, energyCost = 20, type = InteractionType.Nightclub });
            options.Add(new InteractionOption { label = "Birlikte Çalış", cost = 0, energyCost = 15, type = InteractionType.TrainTogether });
        }

        return options;
    }

    public string ExecuteInteraction(RelationshipData rel, InteractionOption interaction)
    {
        // Para ve Enerji kontrolü burada yapılmalı (GameManager üzerinden)
        // Şimdilik sadece ilişki puanını değiştiriyoruz

        float change = 0;
        string message = "";

        switch (interaction.type)
        {
            case InteractionType.Chat:
                // %70 ihtimalle pozitif, %30 negatif
                if (Random.value > 0.3f)
                {
                    change = Random.Range(2, 5);
                    message = "Güzel bir sohbet oldu.";
                }
                else
                {
                    change = Random.Range(-3, -1);
                    message = "Sohbet biraz gergin geçti.";
                }
                break;

            case InteractionType.Gift:
                change = Random.Range(10, 15);
                message = "Hediyeye bayıldı!";
                break;

            case InteractionType.Dinner:
                change = Random.Range(5, 10);
                message = "Keyifli bir yemekti.";
                break;

            case InteractionType.Nightclub:
                change = Random.Range(8, 12);
                message = "Eğlenceli bir geceydi ama yoruldun.";
                break;

            case InteractionType.TrainTogether:
                change = Random.Range(3, 6);
                message = "Takım uyumu arttı.";
                break;
        }

        rel.value = Mathf.Clamp(rel.value + change, 0, 100);
        return message;
    }
}
