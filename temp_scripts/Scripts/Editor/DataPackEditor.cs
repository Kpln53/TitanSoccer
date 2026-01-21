using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// Data Pack Editor - Takımlar, ligler ve oyuncular oluşturma aracı
/// </summary>
public class DataPackEditor : EditorWindow
{
    private DataPack currentDataPack;
    private Vector2 scrollPosition;
    private int selectedTab = 0;
    private string[] tabNames = { "Data Pack", "Ligler", "Takımlar", "Oyuncular" };
    
    // Yeni oluşturma için
    private string newLeagueName = "";
    private string newTeamName = "";
    private string newPlayerName = "";
    private PlayerPosition newPlayerPosition = PlayerPosition.SF;
    private int newPlayerOverall = 70;
    
    [MenuItem("TitanSoccer/Data Pack Editor")]
    public static void ShowWindow()
    {
        GetWindow<DataPackEditor>("Data Pack Editor");
    }
    
    void OnGUI()
    {
        GUILayout.Label("Data Pack Editor", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        // Data Pack seçimi
        currentDataPack = (DataPack)EditorGUILayout.ObjectField("Data Pack", currentDataPack, typeof(DataPack), false);
        
        if (currentDataPack == null)
        {
            EditorGUILayout.HelpBox("Lütfen bir Data Pack seçin veya yeni bir tane oluşturun.", MessageType.Warning);
            
            if (GUILayout.Button("Yeni Data Pack Oluştur"))
            {
                CreateNewDataPack();
            }
            return;
        }
        
        EditorGUILayout.Space();
        
        // Tab sistemi
        selectedTab = GUILayout.Toolbar(selectedTab, tabNames);
        EditorGUILayout.Space();
        
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        
        switch (selectedTab)
        {
            case 0:
                DrawDataPackTab();
                break;
            case 1:
                DrawLeaguesTab();
                break;
            case 2:
                DrawTeamsTab();
                break;
            case 3:
                DrawPlayersTab();
                break;
        }
        
        EditorGUILayout.EndScrollView();
        
        // Değişiklikleri kaydet
        if (GUI.changed)
        {
            EditorUtility.SetDirty(currentDataPack);
        }
    }
    
    void DrawDataPackTab()
    {
        EditorGUILayout.LabelField("Data Pack Bilgileri", EditorStyles.boldLabel);
        
        currentDataPack.packName = EditorGUILayout.TextField("Pack Adı", currentDataPack.packName);
        currentDataPack.packVersion = EditorGUILayout.TextField("Versiyon", currentDataPack.packVersion);
        currentDataPack.packAuthor = EditorGUILayout.TextField("Yazar", currentDataPack.packAuthor);
        currentDataPack.packDescription = EditorGUILayout.TextArea(currentDataPack.packDescription, GUILayout.Height(60));
        
        EditorGUILayout.Space();
        
        EditorGUILayout.LabelField("İstatistikler", EditorStyles.boldLabel);
        EditorGUILayout.LabelField($"Lig Sayısı: {currentDataPack.leagues.Count}");
        
        int totalTeams = 0;
        int totalPlayers = 0;
        
        foreach (var league in currentDataPack.leagues)
        {
            totalTeams += league.teams.Count;
            foreach (var team in league.teams)
            {
                totalPlayers += team.players.Count;
            }
        }
        
        foreach (var team in currentDataPack.standaloneTeams)
        {
            totalTeams++;
            totalPlayers += team.players.Count;
        }
        
        EditorGUILayout.LabelField($"Toplam Takım: {totalTeams}");
        EditorGUILayout.LabelField($"Toplam Oyuncu: {totalPlayers}");
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("Tüm Takım Güçlerini Hesapla"))
        {
            currentDataPack.CalculateAllTeamPowers();
            EditorUtility.SetDirty(currentDataPack);
        }
    }
    
    void DrawLeaguesTab()
    {
        EditorGUILayout.LabelField("Ligler", EditorStyles.boldLabel);
        
        // Yeni lig ekle
        EditorGUILayout.BeginHorizontal();
        newLeagueName = EditorGUILayout.TextField("Yeni Lig Adı", newLeagueName);
        if (GUILayout.Button("Lig Ekle", GUILayout.Width(100)))
        {
            if (!string.IsNullOrEmpty(newLeagueName))
            {
                LeagueData newLeague = new LeagueData();
                newLeague.leagueName = newLeagueName;
                currentDataPack.leagues.Add(newLeague);
                newLeagueName = "";
                EditorUtility.SetDirty(currentDataPack);
            }
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space();
        
        // Lig listesi
        for (int i = currentDataPack.leagues.Count - 1; i >= 0; i--)
        {
            var league = currentDataPack.leagues[i];
            
            EditorGUILayout.BeginVertical("box");
            
            EditorGUILayout.BeginHorizontal();
            league.leagueName = EditorGUILayout.TextField("Lig Adı", league.leagueName);
            if (GUILayout.Button("Sil", GUILayout.Width(50)))
            {
                currentDataPack.leagues.RemoveAt(i);
                EditorUtility.SetDirty(currentDataPack);
                continue;
            }
            EditorGUILayout.EndHorizontal();
            
            league.leagueCountry = EditorGUILayout.TextField("Ülke", league.leagueCountry);
            league.leagueTier = EditorGUILayout.IntField("Lig Seviyesi", league.leagueTier);
            EditorGUILayout.LabelField($"Takım Sayısı: {league.teams.Count}");
            
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }
    }
    
    void DrawTeamsTab()
    {
        EditorGUILayout.LabelField("Takımlar", EditorStyles.boldLabel);
        
        // Yeni takım ekle
        EditorGUILayout.BeginHorizontal();
        newTeamName = EditorGUILayout.TextField("Yeni Takım Adı", newTeamName);
        if (GUILayout.Button("Takım Ekle", GUILayout.Width(100)))
        {
            if (!string.IsNullOrEmpty(newTeamName))
            {
                TeamData newTeam = new TeamData();
                newTeam.teamName = newTeamName;
                newTeam.teamShortName = newTeamName.Substring(0, Mathf.Min(3, newTeamName.Length)).ToUpper();
                currentDataPack.standaloneTeams.Add(newTeam);
                newTeamName = "";
                EditorUtility.SetDirty(currentDataPack);
            }
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space();
        
        // Liglerdeki takımlar
        foreach (var league in currentDataPack.leagues)
        {
            EditorGUILayout.LabelField($"{league.leagueName} - Takımları", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginHorizontal();
            string newTeamInLeague = EditorGUILayout.TextField("Yeni Takım", "");
            if (GUILayout.Button("Ekle", GUILayout.Width(60)))
            {
                if (!string.IsNullOrEmpty(newTeamInLeague))
                {
                    TeamData newTeam = new TeamData();
                    newTeam.teamName = newTeamInLeague;
                    newTeam.teamShortName = newTeamInLeague.Substring(0, Mathf.Min(3, newTeamInLeague.Length)).ToUpper();
                    league.teams.Add(newTeam);
                    EditorUtility.SetDirty(currentDataPack);
                }
            }
            EditorGUILayout.EndHorizontal();
            
            for (int i = league.teams.Count - 1; i >= 0; i--)
            {
                var team = league.teams[i];
                DrawTeamEditor(team, league.teams, i);
            }
            
            EditorGUILayout.Space();
        }
        
        // Standalone takımlar
        EditorGUILayout.LabelField("Standalone Takımlar", EditorStyles.boldLabel);
        for (int i = currentDataPack.standaloneTeams.Count - 1; i >= 0; i--)
        {
            var team = currentDataPack.standaloneTeams[i];
            DrawTeamEditor(team, currentDataPack.standaloneTeams, i);
        }
    }
    
    void DrawTeamEditor(TeamData team, List<TeamData> teamList, int index)
    {
        EditorGUILayout.BeginVertical("box");
        
        EditorGUILayout.BeginHorizontal();
        team.teamName = EditorGUILayout.TextField("Takım Adı", team.teamName);
        if (GUILayout.Button("Sil", GUILayout.Width(50)))
        {
            teamList.RemoveAt(index);
            EditorUtility.SetDirty(currentDataPack);
            return;
        }
        EditorGUILayout.EndHorizontal();
        
        team.teamShortName = EditorGUILayout.TextField("Kısa İsim", team.teamShortName);
        team.teamCountry = EditorGUILayout.TextField("Ülke", team.teamCountry);
        team.primaryColor = EditorGUILayout.ColorField("Ana Renk", team.primaryColor);
        team.secondaryColor = EditorGUILayout.ColorField("İkincil Renk", team.secondaryColor);
        
        EditorGUILayout.LabelField($"Takım Gücü: {team.GetTeamPower()}");
        EditorGUILayout.LabelField($"Oyuncu Sayısı: {team.players.Count}");
        
        EditorGUILayout.Space();
        
        // Oyuncu ekle
        EditorGUILayout.BeginHorizontal();
        newPlayerName = EditorGUILayout.TextField("Oyuncu Adı", newPlayerName);
        newPlayerPosition = (PlayerPosition)EditorGUILayout.EnumPopup(newPlayerPosition);
        newPlayerOverall = EditorGUILayout.IntSlider("Overall", newPlayerOverall, 0, 100);
        if (GUILayout.Button("Oyuncu Ekle", GUILayout.Width(100)))
        {
            if (!string.IsNullOrEmpty(newPlayerName))
            {
                PlayerData newPlayer = new PlayerData();
                newPlayer.playerName = newPlayerName;
                newPlayer.position = newPlayerPosition;
                newPlayer.SetOverall(newPlayerOverall);
                team.players.Add(newPlayer);
                team.CalculateTeamPower();
                newPlayerName = "";
                EditorUtility.SetDirty(currentDataPack);
            }
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space();
        
        // Oyuncu listesi
        for (int i = team.players.Count - 1; i >= 0; i--)
        {
            var player = team.players[i];
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"{player.playerName} ({player.position}) - OVR: {player.overall}", GUILayout.Width(300));
            if (GUILayout.Button("Sil", GUILayout.Width(50)))
            {
                team.players.RemoveAt(i);
                team.CalculateTeamPower();
                EditorUtility.SetDirty(currentDataPack);
            }
            EditorGUILayout.EndHorizontal();
        }
        
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space();
    }
    
    void DrawPlayersTab()
    {
        EditorGUILayout.LabelField("Tüm Oyuncular", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("Oyuncuları takımlar sekmesinden ekleyebilirsiniz.", MessageType.Info);
    }
    
    void CreateNewDataPack()
    {
        string path = EditorUtility.SaveFilePanelInProject("Yeni Data Pack Oluştur", "NewDataPack", "asset", "");
        if (!string.IsNullOrEmpty(path))
        {
            DataPack newPack = CreateInstance<DataPack>();
            newPack.packName = "New Data Pack";
            AssetDatabase.CreateAsset(newPack, path);
            AssetDatabase.SaveAssets();
            currentDataPack = newPack;
        }
    }
}

