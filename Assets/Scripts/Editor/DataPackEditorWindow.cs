using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class DataPackEditorWindow : EditorWindow
{
    private DataPack activeDataPack;
    private Vector2 scrollPos;
    
    // Selection States
    private int selectedTab = 0; // 0: Info, 1: Leagues, 2: Tournaments, 3: Standalone Teams, 4: Rivalries
    private LeagueData selectedLeague;
    private TournamentData selectedTournament;
    private TeamData selectedTeam;
    private PlayerData selectedPlayer;
    
    // Scroll States
    private Vector2 leagueListScroll;
    private Vector2 teamListScroll;
    private Vector2 playerListScroll;
    
    // Cache
    private string[] allTeamNamesCache;
    
    [MenuItem("TitanSoccer/Data Pack Editor")]
    public static void ShowWindow()
    {
        GetWindow<DataPackEditorWindow>("Data Pack Editor");
    }

    private void OnGUI()
    {
        if (activeDataPack == null)
        {
            EditorGUILayout.HelpBox("Please select a DataPack to edit.", MessageType.Info);
            activeDataPack = (DataPack)EditorGUILayout.ObjectField("Data Pack", activeDataPack, typeof(DataPack), false);
            
            if (GUILayout.Button("Create New DataPack"))
            {
                CreateNewDataPack();
            }
            return;
        }

        // Cache güncelle
        UpdateTeamCache();

        EditorGUILayout.BeginHorizontal();
        
        // --- TOP BAR ---
        activeDataPack = (DataPack)EditorGUILayout.ObjectField("Active Data Pack", activeDataPack, typeof(DataPack), false);
        
        if (GUILayout.Button("Save Changes", GUILayout.Width(100)))
        {
            EditorUtility.SetDirty(activeDataPack);
            AssetDatabase.SaveAssets();
        }
        
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space();
        
        // --- TABS ---
        string[] tabs = { "Pack Info", "Leagues", "Tournaments", "Standalone Teams", "Rivalries" };
        selectedTab = GUILayout.Toolbar(selectedTab, tabs);
        
        EditorGUILayout.Space();
        
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        
        switch (selectedTab)
        {
            case 0: DrawPackInfo(); break;
            case 1: DrawLeaguesTab(); break;
            case 2: DrawTournamentsTab(); break;
            case 3: DrawStandaloneTeamsTab(); break;
            case 4: DrawRivalriesTab(); break;
        }
        
        EditorGUILayout.EndScrollView();
    }

    private void UpdateTeamCache()
    {
        if (activeDataPack == null) return;
        
        List<string> names = new List<string>();
        
        // Liglerdeki takımlar
        if (activeDataPack.leagues != null)
        {
            foreach (var league in activeDataPack.leagues)
            {
                if (league.teams != null)
                {
                    foreach (var team in league.teams)
                    {
                        if (!string.IsNullOrEmpty(team.teamName))
                            names.Add(team.teamName);
                    }
                }
            }
        }
        
        // Standalone takımlar
        if (activeDataPack.standaloneTeams != null)
        {
            foreach (var team in activeDataPack.standaloneTeams)
            {
                if (!string.IsNullOrEmpty(team.teamName))
                    names.Add(team.teamName);
            }
        }
        
        names.Sort();
        allTeamNamesCache = names.ToArray();
    }

    private void CreateNewDataPack()
    {
        DataPack newPack = CreateInstance<DataPack>();
        string path = EditorUtility.SaveFilePanelInProject("Create DataPack", "NewDataPack", "asset", "Create a new DataPack");
        if (!string.IsNullOrEmpty(path))
        {
            AssetDatabase.CreateAsset(newPack, path);
            AssetDatabase.SaveAssets();
            activeDataPack = newPack;
        }
    }

    // --- TAB 0: INFO ---
    private void DrawPackInfo()
    {
        EditorGUILayout.LabelField("Data Pack Information", EditorStyles.boldLabel);
        activeDataPack.packName = EditorGUILayout.TextField("Pack Name", activeDataPack.packName);
        activeDataPack.packId = EditorGUILayout.TextField("Pack ID", activeDataPack.packId);
        activeDataPack.packVersion = EditorGUILayout.TextField("Version", activeDataPack.packVersion);
        activeDataPack.packAuthor = EditorGUILayout.TextField("Author", activeDataPack.packAuthor);
        
        EditorGUILayout.LabelField("Description");
        activeDataPack.packDescription = EditorGUILayout.TextArea(activeDataPack.packDescription, GUILayout.Height(60));
        
        activeDataPack.packLogo = (Sprite)EditorGUILayout.ObjectField("Logo", activeDataPack.packLogo, typeof(Sprite), false);
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Statistics", EditorStyles.boldLabel);
        EditorGUILayout.LabelField($"Total Leagues: {activeDataPack.leagues?.Count ?? 0}");
        EditorGUILayout.LabelField($"Total Teams: {activeDataPack.GetTotalTeamCount()}");
        EditorGUILayout.LabelField($"Total Players: {activeDataPack.GetTotalPlayerCount()}");
    }

    // --- TAB 1: LEAGUES ---
    private void DrawLeaguesTab()
    {
        EditorGUILayout.BeginHorizontal();
        
        // Left: League List
        EditorGUILayout.BeginVertical(GUILayout.Width(200));
        EditorGUILayout.LabelField("Leagues", EditorStyles.boldLabel);
        
        if (GUILayout.Button("+ Add League"))
        {
            activeDataPack.leagues.Add(new LeagueData { leagueName = "New League" });
        }
        
        leagueListScroll = EditorGUILayout.BeginScrollView(leagueListScroll, "box");
        for (int i = 0; i < activeDataPack.leagues.Count; i++)
        {
            LeagueData league = activeDataPack.leagues[i];
            GUI.backgroundColor = selectedLeague == league ? Color.cyan : Color.white;
            if (GUILayout.Button(league.leagueName))
            {
                selectedLeague = league;
                selectedTeam = null;
                selectedPlayer = null;
            }
            GUI.backgroundColor = Color.white;
        }
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
        
        // Right: League Details
        EditorGUILayout.BeginVertical();
        if (selectedLeague != null)
        {
            DrawLeagueDetails(selectedLeague);
        }
        else
        {
            EditorGUILayout.HelpBox("Select a league to edit.", MessageType.Info);
        }
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.EndHorizontal();
    }

    private void DrawLeagueDetails(LeagueData league)
    {
        EditorGUILayout.LabelField("League Details", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Remove League", GUILayout.Width(120)))
        {
            activeDataPack.leagues.Remove(league);
            selectedLeague = null;
            return;
        }
        EditorGUILayout.EndHorizontal();
        
        league.leagueName = EditorGUILayout.TextField("Name", league.leagueName);
        league.leagueCountry = EditorGUILayout.TextField("Country", league.leagueCountry);
        league.leagueTier = EditorGUILayout.IntField("Tier", league.leagueTier);
        league.leaguePower = EditorGUILayout.IntSlider("Power", league.leaguePower, 0, 100);
        league.leagueLogo = (Sprite)EditorGUILayout.ObjectField("Logo", league.leagueLogo, typeof(Sprite), false);
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField($"Teams ({league.teams.Count})", EditorStyles.boldLabel);
        
        DrawTeamList(league.teams);
    }

    // --- TAB 2: TOURNAMENTS ---
    private void DrawTournamentsTab()
    {
        EditorGUILayout.BeginHorizontal();
        
        // Left: List
        EditorGUILayout.BeginVertical(GUILayout.Width(200));
        EditorGUILayout.LabelField("Tournaments", EditorStyles.boldLabel);
        
        if (GUILayout.Button("+ Add Tournament"))
        {
            activeDataPack.tournaments.Add(new TournamentData { tournamentName = "New Tournament" });
        }
        
        leagueListScroll = EditorGUILayout.BeginScrollView(leagueListScroll, "box");
        for (int i = 0; i < activeDataPack.tournaments.Count; i++)
        {
            TournamentData tour = activeDataPack.tournaments[i];
            GUI.backgroundColor = selectedTournament == tour ? Color.cyan : Color.white;
            if (GUILayout.Button(tour.tournamentName))
            {
                selectedTournament = tour;
                selectedTeam = null;
                selectedPlayer = null;
            }
            GUI.backgroundColor = Color.white;
        }
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
        
        // Right: Details
        EditorGUILayout.BeginVertical();
        if (selectedTournament != null)
        {
            DrawTournamentDetails(selectedTournament);
        }
        else
        {
            EditorGUILayout.HelpBox("Select a tournament to edit.", MessageType.Info);
        }
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.EndHorizontal();
    }

    private void DrawTournamentDetails(TournamentData tour)
    {
        EditorGUILayout.LabelField("Tournament Details", EditorStyles.boldLabel);
        
        if (GUILayout.Button("Remove Tournament", GUILayout.Width(150)))
        {
            activeDataPack.tournaments.Remove(tour);
            selectedTournament = null;
            return;
        }
        
        tour.tournamentName = EditorGUILayout.TextField("Name", tour.tournamentName);
        tour.country = EditorGUILayout.TextField("Country", tour.country);
        tour.format = (TournamentFormat)EditorGUILayout.EnumPopup("Format", tour.format);
        tour.logo = (Sprite)EditorGUILayout.ObjectField("Logo", tour.logo, typeof(Sprite), false);
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField($"Participating Teams ({tour.teamNames.Count})", EditorStyles.boldLabel);
        
        // Takım Ekleme (Dropdown ile)
        if (allTeamNamesCache != null && allTeamNamesCache.Length > 0)
        {
            EditorGUILayout.BeginHorizontal();
            int selectedIndex = EditorGUILayout.Popup(0, allTeamNamesCache);
            if (GUILayout.Button("Add Selected Team"))
            {
                string teamToAdd = allTeamNamesCache[selectedIndex];
                if (!tour.teamNames.Contains(teamToAdd))
                {
                    tour.teamNames.Add(teamToAdd);
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        else
        {
            EditorGUILayout.HelpBox("No teams available in DataPack. Add teams to Leagues first.", MessageType.Warning);
        }

        // Takım Listesi
        for (int i = 0; i < tour.teamNames.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(tour.teamNames[i]);
            if (GUILayout.Button("X", GUILayout.Width(25)))
            {
                tour.teamNames.RemoveAt(i);
                return;
            }
            EditorGUILayout.EndHorizontal();
        }
    }

    // --- TAB 3: STANDALONE TEAMS ---
    private void DrawStandaloneTeamsTab()
    {
        EditorGUILayout.LabelField("Standalone Teams (No League)", EditorStyles.boldLabel);
        DrawTeamList(activeDataPack.standaloneTeams);
    }

    // --- TAB 4: RIVALRIES ---
    private void DrawRivalriesTab()
    {
        EditorGUILayout.LabelField("Rivalries (Derbies)", EditorStyles.boldLabel);
        
        if (GUILayout.Button("+ Add Rivalry"))
        {
            activeDataPack.rivalries.Add(new RivalryData { derbyName = "New Derby" });
        }
        
        if (allTeamNamesCache == null || allTeamNamesCache.Length == 0)
        {
            EditorGUILayout.HelpBox("Add teams to Leagues first to create rivalries.", MessageType.Warning);
            return;
        }

        for (int i = 0; i < activeDataPack.rivalries.Count; i++)
        {
            RivalryData r = activeDataPack.rivalries[i];
            EditorGUILayout.BeginVertical("box");
            
            EditorGUILayout.BeginHorizontal();
            r.derbyName = EditorGUILayout.TextField("Derby Name", r.derbyName);
            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                activeDataPack.rivalries.RemoveAt(i);
                return;
            }
            EditorGUILayout.EndHorizontal();
            
            // Team 1 Selection
            int index1 = System.Array.IndexOf(allTeamNamesCache, r.team1);
            if (index1 < 0) index1 = 0;
            index1 = EditorGUILayout.Popup("Team 1", index1, allTeamNamesCache);
            if (allTeamNamesCache.Length > 0) r.team1 = allTeamNamesCache[index1];

            // Team 2 Selection
            int index2 = System.Array.IndexOf(allTeamNamesCache, r.team2);
            if (index2 < 0) index2 = 0;
            index2 = EditorGUILayout.Popup("Team 2", index2, allTeamNamesCache);
            if (allTeamNamesCache.Length > 0) r.team2 = allTeamNamesCache[index2];

            r.intensity = EditorGUILayout.IntSlider("Intensity", r.intensity, 1, 10);
            
            EditorGUILayout.EndVertical();
        }
    }

    // --- SHARED: TEAM LIST & DETAILS ---
    private void DrawTeamList(List<TeamData> teams)
    {
        EditorGUILayout.BeginHorizontal();
        
        // Team List
        EditorGUILayout.BeginVertical(GUILayout.Width(200));
        if (GUILayout.Button("+ Add Team"))
        {
            teams.Add(new TeamData { teamName = "New Team", teamShortName = "NEW" });
        }
        
        teamListScroll = EditorGUILayout.BeginScrollView(teamListScroll, "box", GUILayout.Height(300));
        for (int i = 0; i < teams.Count; i++)
        {
            TeamData team = teams[i];
            GUI.backgroundColor = selectedTeam == team ? Color.cyan : Color.white;
            if (GUILayout.Button(team.teamName))
            {
                selectedTeam = team;
                selectedPlayer = null;
            }
            GUI.backgroundColor = Color.white;
        }
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
        
        // Team Details
        EditorGUILayout.BeginVertical();
        if (selectedTeam != null)
        {
            // Ensure selected team is in the current list (context switch check)
            if (teams.Contains(selectedTeam))
            {
                DrawTeamDetails(selectedTeam, teams);
            }
            else
            {
                selectedTeam = null;
            }
        }
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.EndHorizontal();
    }

    private void DrawTeamDetails(TeamData team, List<TeamData> parentList)
    {
        EditorGUILayout.LabelField("Team Details", EditorStyles.boldLabel);
        
        if (GUILayout.Button("Remove Team", GUILayout.Width(120)))
        {
            parentList.Remove(team);
            selectedTeam = null;
            return;
        }
        
        team.teamName = EditorGUILayout.TextField("Name", team.teamName);
        team.teamShortName = EditorGUILayout.TextField("Short Name", team.teamShortName);
        team.teamCountry = EditorGUILayout.TextField("Country", team.teamCountry);
        team.stadiumName = EditorGUILayout.TextField("Stadium", team.stadiumName);
        team.stadiumCapacity = EditorGUILayout.IntField("Capacity", team.stadiumCapacity);
        
        team.primaryColor = EditorGUILayout.ColorField("Primary Color", team.primaryColor);
        team.secondaryColor = EditorGUILayout.ColorField("Secondary Color", team.secondaryColor);
        
        team.teamLogo = (Sprite)EditorGUILayout.ObjectField("Logo", team.teamLogo, typeof(Sprite), false);
        
        team.transferBudget = EditorGUILayout.LongField("Transfer Budget", team.transferBudget);
        team.wageBudget = EditorGUILayout.LongField("Wage Budget", team.wageBudget);
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField($"Players ({team.players.Count}) - Avg Power: {team.GetTeamPower()}", EditorStyles.boldLabel);
        
        DrawPlayerList(team);
    }

    // --- SHARED: PLAYER LIST & DETAILS ---
    private void DrawPlayerList(TeamData team)
    {
        EditorGUILayout.BeginHorizontal();
        
        // Player List
        EditorGUILayout.BeginVertical(GUILayout.Width(200));
        if (GUILayout.Button("+ Add Player"))
        {
            team.players.Add(new PlayerData { playerName = "New Player", position = PlayerPosition.SF, overall = 60 });
        }
        
        playerListScroll = EditorGUILayout.BeginScrollView(playerListScroll, "box", GUILayout.Height(250));
        for (int i = 0; i < team.players.Count; i++)
        {
            PlayerData player = team.players[i];
            GUI.backgroundColor = selectedPlayer == player ? Color.cyan : Color.white;
            if (GUILayout.Button($"{player.position} - {player.playerName} ({player.overall})"))
            {
                selectedPlayer = player;
            }
            GUI.backgroundColor = Color.white;
        }
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
        
        // Player Details
        EditorGUILayout.BeginVertical();
        if (selectedPlayer != null && team.players.Contains(selectedPlayer))
        {
            DrawPlayerDetails(selectedPlayer, team);
        }
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.EndHorizontal();
    }

    private void DrawPlayerDetails(PlayerData player, TeamData team)
    {
        EditorGUILayout.LabelField("Player Details", EditorStyles.boldLabel);
        
        if (GUILayout.Button("Remove Player", GUILayout.Width(120)))
        {
            team.players.Remove(player);
            selectedPlayer = null;
            return;
        }
        
        player.playerName = EditorGUILayout.TextField("Name", player.playerName);
        player.position = (PlayerPosition)EditorGUILayout.EnumPopup("Position", player.position);
        player.age = EditorGUILayout.IntSlider("Age", player.age, 15, 45);
        player.nationality = EditorGUILayout.TextField("Nationality", player.nationality);
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Stats", EditorStyles.boldLabel);
        
        // Overall Management
        EditorGUILayout.BeginHorizontal();
        player.overall = EditorGUILayout.IntSlider("Overall", player.overall, 1, 99);
        if (GUILayout.Button("Auto Set Stats", GUILayout.Width(100)))
        {
            player.SetOverall(player.overall);
        }
        EditorGUILayout.EndHorizontal();
        
        player.potential = EditorGUILayout.IntSlider("Potential", player.potential, player.overall, 99);
        
        EditorGUILayout.Space();
        
        // Detailed Stats
        if (player.position == PlayerPosition.KL)
        {
            player.saveReflex = EditorGUILayout.IntSlider("Reflex", player.saveReflex, 1, 99);
            player.goalkeeperPositioning = EditorGUILayout.IntSlider("Positioning", player.goalkeeperPositioning, 1, 99);
            player.aerialAbility = EditorGUILayout.IntSlider("Aerial", player.aerialAbility, 1, 99);
            player.oneOnOne = EditorGUILayout.IntSlider("1v1", player.oneOnOne, 1, 99);
            player.handling = EditorGUILayout.IntSlider("Handling", player.handling, 1, 99);
        }
        else
        {
            player.shootingSkill = EditorGUILayout.IntSlider("Shooting", player.shootingSkill, 1, 99);
            player.passingSkill = EditorGUILayout.IntSlider("Passing", player.passingSkill, 1, 99);
            player.dribblingSkill = EditorGUILayout.IntSlider("Dribbling", player.dribblingSkill, 1, 99);
            player.falsoSkill = EditorGUILayout.IntSlider("Curve (Falso)", player.falsoSkill, 1, 99);
            player.speed = EditorGUILayout.IntSlider("Speed", player.speed, 1, 99);
            player.physicalStrength = EditorGUILayout.IntSlider("Physical", player.physicalStrength, 1, 99);
            player.defendingSkill = EditorGUILayout.IntSlider("Defending", player.defendingSkill, 1, 99);
            player.stamina = EditorGUILayout.IntSlider("Stamina", player.stamina, 1, 99);
        }
        
        // Recalculate overall based on manual stats
        if (GUILayout.Button("Recalculate Overall from Stats"))
        {
            player.CalculateOverall();
        }
        
        player.marketValue = EditorGUILayout.LongField("Market Value", player.marketValue);
        if (GUILayout.Button("Auto Calc Value"))
        {
            player.CalculateMarketValue();
        }
    }
}
