using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// İlişkiler verisi - Oyuncunun çevresiyle ilişkileri
/// </summary>
[System.Serializable]
public class RelationsData
{
    [Header("Takım İlişkileri")]
    public List<TeammateRelation> teammateRelations; // Takım arkadaşları listesi
    public int coachRelation = 0;                     // Teknik direktör ilişkisi (-100 ile 100)
    public int managementRelation = 0;                // Yönetim ilişkisi (-100 ile 100)
    
    [Header("Kişisel İlişkiler")]
    public int familyRelation = 50;                   // Aile ilişkisi (0-100)
    public int girlfriendRelation = 0;                // Sevgili ilişkisi (0-100, 0 = sevgili yok)
    public int managerRelation = 50;                  // Menajer ilişkisi (0-100)
    
    public RelationsData()
    {
        teammateRelations = new List<TeammateRelation>();
        coachRelation = 0;
        managementRelation = 0;
        familyRelation = 50;
        girlfriendRelation = 0;
        managerRelation = 50;
    }
    
    /// <summary>
    /// Takım arkadaşı ilişkisini getir (yoksa 0 döner)
    /// </summary>
    public int GetTeammateRelation(string teammateName)
    {
        if (teammateRelations == null)
            return 0;
        
        foreach (var relation in teammateRelations)
        {
            if (relation.teammateName == teammateName)
            {
                return relation.relationLevel;
            }
        }
        return 0;
    }
    
    /// <summary>
    /// Takım arkadaşı ilişkisini ayarla
    /// </summary>
    public void SetTeammateRelation(string teammateName, int relation)
    {
        if (teammateRelations == null)
        {
            teammateRelations = new List<TeammateRelation>();
        }
        
        relation = UnityEngine.Mathf.Clamp(relation, -100, 100);
        
        // Mevcut ilişkiyi bul ve güncelle
        for (int i = 0; i < teammateRelations.Count; i++)
        {
            if (teammateRelations[i].teammateName == teammateName)
            {
                teammateRelations[i].relationLevel = relation;
                return;
            }
        }
        
        // Yeni ilişki ekle
        teammateRelations.Add(new TeammateRelation(teammateName, relation));
    }
    
    /// <summary>
    /// Sevgili var mı kontrol et
    /// </summary>
    public bool HasGirlfriend()
    {
        return girlfriendRelation > 0;
    }
}

/// <summary>
/// Takım arkadaşı ilişkisi (JSON serialization için)
/// </summary>
[System.Serializable]
public class TeammateRelation
{
    public string teammateName;      // Takım arkadaşı adı
    public int relationLevel;        // İlişki seviyesi (-100 ile 100)
    
    public TeammateRelation()
    {
        teammateName = "";
        relationLevel = 0;
    }
    
    public TeammateRelation(string name, int relation)
    {
        teammateName = name;
        relationLevel = relation;
    }
}
