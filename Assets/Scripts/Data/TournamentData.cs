using UnityEngine;
using System.Collections.Generic;

public enum TournamentFormat
{
    League,             // Klasik Lig (Süper Lig vb.)
    Knockout,           // Eleme Usulü (Kupa)
    GroupsAndKnockout,  // Eski Şampiyonlar Ligi (Grup + Eleme)
    SwissLeaguePhase    // Yeni Şampiyonlar Ligi (36 Takımlı Lig + Eleme)
}

[System.Serializable]
public class TournamentData
{
    public string tournamentName;
    public string country = "International";
    public Sprite logo;
    public TournamentFormat format = TournamentFormat.Knockout;
    
    // Takımların sadece isimlerini tutuyoruz (Referans için)
    // Runtime'da DataPackManager.GetTeamByName() ile asıl veriye ulaşacağız.
    public List<string> teamNames = new List<string>(); 
}
