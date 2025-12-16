using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// İlişki verileri
/// </summary>
[System.Serializable]
public class RelationsData
{
    [Header("Teknik Direktör")]
    [Range(0f, 1f)] public float coachRelationship = 0.5f;
    
    [Header("Takım Arkadaşları")]
    public Dictionary<string, float> teammateRelationships = new Dictionary<string, float>();
    
    [Header("Sponsorlar")]
    public Dictionary<string, float> sponsorRelationships = new Dictionary<string, float>();
    
    [Header("Aile")]
    [Range(0f, 1f)] public float familyRelationship = 0.8f;
    
    [Header("Sevgili")]
    [Range(0f, 1f)] public float girlfriendRelationship = 0.5f;
    public bool hasGirlfriend = false;
    
    [Header("Menajer")]
    [Range(0f, 1f)] public float managerRelationship = 0.7f;
}

