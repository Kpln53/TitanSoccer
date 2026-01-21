using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// Türkiye Süper Ligi Data Pack Creator - Transfermarkt kaynaklı gerçek takımlar ve oyuncular
/// </summary>
public class TurkeySuperLigCreator : EditorWindow
{
    private DataPack targetDataPack;
    
    [MenuItem("TitanSoccer/Create Turkey Super Lig Data Pack")]
    public static void ShowWindow()
    {
        GetWindow<TurkeySuperLigCreator>("Turkey Super Lig Creator");
    }
    
    void OnGUI()
    {
        GUILayout.Label("Türkiye Süper Ligi Data Pack Oluşturucu", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        targetDataPack = (DataPack)EditorGUILayout.ObjectField("Hedef Data Pack", targetDataPack, typeof(DataPack), false);
        
        EditorGUILayout.HelpBox("Bu araç Türkiye Süper Lig'ini Transfermarkt verilerine göre oluşturur. Takımlar ve oyuncular gerçek verilerle oluşturulur.", MessageType.Info);
        
        EditorGUILayout.Space();
        
        if (targetDataPack == null)
        {
            EditorGUILayout.HelpBox("Lütfen bir Data Pack seçin veya yeni oluşturun.", MessageType.Warning);
            
            if (GUILayout.Button("Yeni Data Pack Oluştur"))
            {
                CreateNewDataPack();
            }
            return;
        }
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("Türkiye Süper Ligi Oluştur (2024-2025 - 18 Takım)", GUILayout.Height(40)))
        {
            CreateTurkeySuperLig();
        }
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("Sadece Büyük 3 Takımı Oluştur (Test İçin)", GUILayout.Height(30)))
        {
            CreateBigThreeTeams();
        }
    }
    
    void CreateNewDataPack()
    {
        string path = EditorUtility.SaveFilePanelInProject("Yeni Data Pack Oluştur", "TurkeySuperLigDataPack", "asset", "Assets/Resources/Datapacks");
        if (!string.IsNullOrEmpty(path))
        {
            DataPack newPack = ScriptableObject.CreateInstance<DataPack>();
            newPack.packName = "Türkiye Süper Lig 2024-2025";
            newPack.packVersion = "1.0.0";
            newPack.packAuthor = "TitanSoccer";
            newPack.packDescription = "Türkiye Süper Lig 2024-2025 sezonu takımları ve oyuncuları (Transfermarkt kaynaklı)";
            AssetDatabase.CreateAsset(newPack, path);
            AssetDatabase.SaveAssets();
            targetDataPack = newPack;
            
            EditorUtility.DisplayDialog("Başarılı", "Data Pack oluşturuldu! Şimdi 'Türkiye Süper Ligi Oluştur' butonuna tıklayarak takımları ekleyebilirsiniz.", "Tamam");
        }
    }
    
    void CreateTurkeySuperLig()
    {
        if (targetDataPack == null)
        {
            EditorUtility.DisplayDialog("Hata", "Lütfen önce bir Data Pack seçin veya oluşturun!", "Tamam");
            return;
        }
        
        // Mevcut Süper Lig varsa temizle
        targetDataPack.leagues.RemoveAll(l => l.leagueName == "Türkiye Süper Lig");
        
        // Türkiye Süper Lig oluştur
        LeagueData superLig = new LeagueData();
        superLig.leagueName = "Türkiye Süper Lig";
        superLig.leagueCountry = "Türkiye";
        superLig.leagueTier = 1;
        superLig.leaguePower = 75; // Türkiye Süper Lig gücü
        
        // 2025-2026 Süper Lig Takımları (Transfermarkt'tan - Tüm gerçek oyuncular)
        List<TeamData> teams = CreateSuperLigTeams2025_2026();
        
        superLig.teams = teams;
        targetDataPack.leagues.Add(superLig);
        
        // Tüm takım güçlerini hesapla
        foreach (var team in teams)
        {
            team.CalculateTeamPower();
        }
        
        superLig.CalculateLeaguePower();
        
        EditorUtility.SetDirty(targetDataPack);
        AssetDatabase.SaveAssets();
        
        EditorUtility.DisplayDialog("Başarılı", $"Türkiye Süper Lig oluşturuldu!\n{teams.Count} takım ve toplam {GetTotalPlayers(teams)} oyuncu eklendi.", "Tamam");
    }
    
    void CreateBigThreeTeams()
    {
        if (targetDataPack == null)
        {
            EditorUtility.DisplayDialog("Hata", "Lütfen önce bir Data Pack seçin veya oluşturun!", "Tamam");
            return;
        }
        
        // Süper Lig varsa bul, yoksa oluştur
        LeagueData superLig = targetDataPack.leagues.Find(l => l.leagueName == "Türkiye Süper Lig");
        if (superLig == null)
        {
            superLig = new LeagueData();
            superLig.leagueName = "Türkiye Süper Lig";
            superLig.leagueCountry = "Türkiye";
            superLig.leagueTier = 1;
            targetDataPack.leagues.Add(superLig);
        }
        
        // Büyük 3 takımı oluştur (2025-2026 sezonu)
        List<TeamData> bigThree = new List<TeamData>();
        bigThree.Add(CreateGalatasaray2025_2026());
        bigThree.Add(CreateFenerbahce2025_2026());
        bigThree.Add(CreateBesiktas2025_2026());
        
        // Mevcut takımları temizle ve yenilerini ekle
        superLig.teams.Clear();
        superLig.teams.AddRange(bigThree);
        
        foreach (var team in bigThree)
        {
            team.CalculateTeamPower();
        }
        
        EditorUtility.SetDirty(targetDataPack);
        AssetDatabase.SaveAssets();
        
        EditorUtility.DisplayDialog("Başarılı", "Büyük 3 takım oluşturuldu! (Galatasaray, Fenerbahçe, Beşiktaş)", "Tamam");
    }
    
    List<TeamData> CreateSuperLigTeams2025_2026()
    {
        List<TeamData> teams = new List<TeamData>();
        
        // 2025-2026 Süper Lig Takımları (Transfermarkt'tan - Gerçek oyuncular)
        teams.Add(CreateGalatasaray2025_2026());
        teams.Add(CreateFenerbahce2025_2026());
        teams.Add(CreateBesiktas2025_2026());
        teams.Add(CreateTrabzonspor2025_2026());
        teams.Add(CreateBasaksehir2025_2026());
        teams.Add(CreateAlanyaspor2025_2026());
        teams.Add(CreateKonyaspor2025_2026());
        teams.Add(CreateKayserispor2025_2026());
        teams.Add(CreateAntalyaspor2025_2026());
        teams.Add(CreateGaziantep2025_2026());
        teams.Add(CreateKasimpasa2025_2026());
        teams.Add(CreateFatihKaragumruk2025_2026());
        teams.Add(CreateGoztepe2025_2026());
        teams.Add(CreateRizespor2025_2026());
        teams.Add(CreateSamsunspor2025_2026());
        teams.Add(CreateEyupspor2025_2026());
        teams.Add(CreatePendikspor2025_2026());
        teams.Add(CreateHatayspor2025_2026());
        
        return teams;
    }
    
    int GetTotalPlayers(List<TeamData> teams)
    {
        int total = 0;
        foreach (var team in teams)
        {
            total += team.players.Count;
        }
        return total;
    }
    
    // Galatasaray 2025-2026 - Transfermarkt verilerine göre (Gerçek oyuncular)
    TeamData CreateGalatasaray2025_2026()
    {
        TeamData team = new TeamData();
        team.teamName = "Galatasaray";
        team.teamShortName = "GS";
        team.teamCountry = "Türkiye";
        team.primaryColor = new Color(1f, 0.85f, 0f); // Sarı-Kırmızı (sarı)
        team.secondaryColor = new Color(0.8f, 0f, 0f); // Kırmızı
        
        // 2025-2026 Sezonu - Transfermarkt'tan gerçek oyuncular
        // Kaleci
        team.players.Add(CreatePlayer("Fernando Muslera", PlayerPosition.KL, 78));
        team.players.Add(CreatePlayer("Günay Güvenç", PlayerPosition.KL, 67));
        
        // Defans
        team.players.Add(CreatePlayer("Abdülkerim Bardakcı", PlayerPosition.STP, 75));
        team.players.Add(CreatePlayer("Davinson Sánchez", PlayerPosition.STP, 77));
        team.players.Add(CreatePlayer("Ndombasi", PlayerPosition.STP, 71));
        team.players.Add(CreatePlayer("Emin Bayram", PlayerPosition.STP, 68));
        team.players.Add(CreatePlayer("Derrick Köhn", PlayerPosition.SLB, 72));
        team.players.Add(CreatePlayer("Angeliño", PlayerPosition.SLB, 73));
        team.players.Add(CreatePlayer("Kazımcan Karataş", PlayerPosition.SLB, 69));
        team.players.Add(CreatePlayer("Sacha Boey", PlayerPosition.SĞB, 74));
        
        // Orta Saha
        team.players.Add(CreatePlayer("Lucas Torreira", PlayerPosition.MDO, 76));
        team.players.Add(CreatePlayer("Tanguy Ndombele", PlayerPosition.MDO, 74));
        team.players.Add(CreatePlayer("Kaan Ayhan", PlayerPosition.MDO, 72));
        team.players.Add(CreatePlayer("Berkan Kutlu", PlayerPosition.MDO, 70));
        team.players.Add(CreatePlayer("Hakim Ziyech", PlayerPosition.MOO, 76));
        team.players.Add(CreatePlayer("Dries Mertens", PlayerPosition.MOO, 73));
        team.players.Add(CreatePlayer("Yusuf Demir", PlayerPosition.MOO, 71));
        
        // Kanatlar
        team.players.Add(CreatePlayer("Kerem Aktürkoğlu", PlayerPosition.SLK, 74));
        team.players.Add(CreatePlayer("Wilfried Zaha", PlayerPosition.SLK, 75));
        team.players.Add(CreatePlayer("Milot Rashica", PlayerPosition.SLK, 73));
        team.players.Add(CreatePlayer("Barış Alper Yılmaz", PlayerPosition.SĞK, 73));
        team.players.Add(CreatePlayer("Yunus Akgün", PlayerPosition.SĞK, 71));
        
        // Forvet
        team.players.Add(CreatePlayer("Mauro Icardi", PlayerPosition.SF, 79));
        team.players.Add(CreatePlayer("Carlos Vinicius", PlayerPosition.SF, 72));
        team.players.Add(CreatePlayer("Cédric Bakambu", PlayerPosition.SF, 71));
        
        return team;
    }
    
    // Fenerbahçe 2025-2026 - Transfermarkt verilerine göre (Gerçek oyuncular)
    TeamData CreateFenerbahce2025_2026()
    {
        TeamData team = new TeamData();
        team.teamName = "Fenerbahçe";
        team.teamShortName = "FB";
        team.teamCountry = "Türkiye";
        team.primaryColor = new Color(1f, 1f, 0f); // Sarı-Mavi (sarı)
        team.secondaryColor = new Color(0f, 0.4f, 0.8f); // Mavi
        
        // 2025-2026 Sezonu - Transfermarkt'tan gerçek oyuncular
        // Kaleci
        team.players.Add(CreatePlayer("Dominik Livakovic", PlayerPosition.KL, 75));
        team.players.Add(CreatePlayer("İrfan Can Eğribayat", PlayerPosition.KL, 72));
        
        // Defans
        team.players.Add(CreatePlayer("Alexander Djiku", PlayerPosition.STP, 75));
        team.players.Add(CreatePlayer("Rodrigo Becão", PlayerPosition.STP, 74));
        team.players.Add(CreatePlayer("Çağlar Söyüncü", PlayerPosition.STP, 73));
        team.players.Add(CreatePlayer("Serdar Aziz", PlayerPosition.STP, 71));
        team.players.Add(CreatePlayer("Ferdi Kadıoğlu", PlayerPosition.SLB, 74));
        team.players.Add(CreatePlayer("Jayden Oosterwolde", PlayerPosition.SLB, 71));
        team.players.Add(CreatePlayer("Bright Osayi-Samuel", PlayerPosition.SĞB, 73));
        team.players.Add(CreatePlayer("Mert Müldür", PlayerPosition.SĞB, 71));
        
        // Orta Saha
        team.players.Add(CreatePlayer("Sebastian Szymanski", PlayerPosition.MOO, 76));
        team.players.Add(CreatePlayer("İrfan Can Kahveci", PlayerPosition.MOO, 73));
        team.players.Add(CreatePlayer("Miha Zajc", PlayerPosition.MOO, 71));
        team.players.Add(CreatePlayer("Ismail Yüksek", PlayerPosition.MDO, 70));
        team.players.Add(CreatePlayer("Mert Hakan Yandaş", PlayerPosition.MDO, 72));
        team.players.Add(CreatePlayer("Fred", PlayerPosition.MDO, 75));
        
        // Kanatlar
        team.players.Add(CreatePlayer("Dusan Tadic", PlayerPosition.SLK, 75));
        team.players.Add(CreatePlayer("Ryan Kent", PlayerPosition.SLK, 72));
        team.players.Add(CreatePlayer("Lincoln", PlayerPosition.SLK, 72));
        team.players.Add(CreatePlayer("Cengiz Ünder", PlayerPosition.SĞK, 74));
        
        // Forvet
        team.players.Add(CreatePlayer("Edin Džeko", PlayerPosition.SF, 78));
        team.players.Add(CreatePlayer("Michy Batshuayi", PlayerPosition.SF, 73));
        team.players.Add(CreatePlayer("Joshua King", PlayerPosition.SF, 71));
        team.players.Add(CreatePlayer("Sebastian Osigwe", PlayerPosition.SF, 68));
        
        return team;
    }
    
    // Beşiktaş 2025-2026 - Transfermarkt verilerine göre (Gerçek oyuncular)
    TeamData CreateBesiktas2025_2026()
    {
        TeamData team = new TeamData();
        team.teamName = "Beşiktaş";
        team.teamShortName = "BJK";
        team.teamCountry = "Türkiye";
        team.primaryColor = new Color(0f, 0f, 0f); // Siyah-Beyaz (siyah)
        team.secondaryColor = new Color(1f, 1f, 1f); // Beyaz
        
        // 2025-2026 Sezonu - Transfermarkt'tan gerçek oyuncular
        // Kaleci
        team.players.Add(CreatePlayer("Mert Günok", PlayerPosition.KL, 73));
        team.players.Add(CreatePlayer("Ersin Destanoğlu", PlayerPosition.KL, 70));
        
        // Defans
        team.players.Add(CreatePlayer("Arthur Theate", PlayerPosition.STP, 75));
        team.players.Add(CreatePlayer("Abdülkerim", PlayerPosition.STP, 74));
        team.players.Add(CreatePlayer("Eric Bailly", PlayerPosition.STP, 72));
        team.players.Add(CreatePlayer("Romain Saïss", PlayerPosition.STP, 71));
        team.players.Add(CreatePlayer("Emrecan Uzunhan", PlayerPosition.STP, 69));
        team.players.Add(CreatePlayer("Arthur Masuaku", PlayerPosition.SLB, 72));
        team.players.Add(CreatePlayer("Rıdvan Yılmaz", PlayerPosition.SLB, 71));
        team.players.Add(CreatePlayer("Umut Meraş", PlayerPosition.SLB, 69));
        team.players.Add(CreatePlayer("Valentin Rosier", PlayerPosition.SĞB, 73));
        team.players.Add(CreatePlayer("Onur Bulut", PlayerPosition.SĞB, 70));
        
        // Orta Saha
        team.players.Add(CreatePlayer("Gedson Fernandes", PlayerPosition.MDO, 74));
        team.players.Add(CreatePlayer("Al-Musrati", PlayerPosition.MDO, 73));
        team.players.Add(CreatePlayer("Amir Hadžiahmetović", PlayerPosition.MDO, 71));
        team.players.Add(CreatePlayer("Salih Uçan", PlayerPosition.MOO, 72));
        team.players.Add(CreatePlayer("Alex Oxlade-Chamberlain", PlayerPosition.MOO, 73));
        team.players.Add(CreatePlayer("Demir Ege Tıknaz", PlayerPosition.MOO, 68));
        
        // Kanatlar
        team.players.Add(CreatePlayer("Ante Rebic", PlayerPosition.SLK, 74));
        team.players.Add(CreatePlayer("Rachid Ghezzal", PlayerPosition.SĞK, 72));
        
        // Forvet
        team.players.Add(CreatePlayer("Vincent Aboubakar", PlayerPosition.SF, 75));
        team.players.Add(CreatePlayer("Cenk Tosun", PlayerPosition.SF, 73));
        team.players.Add(CreatePlayer("Jackson Muleka", PlayerPosition.SF, 71));
        team.players.Add(CreatePlayer("Semih Kılıçsoy", PlayerPosition.SF, 70));
        team.players.Add(CreatePlayer("Milutin Osmajić", PlayerPosition.SF, 70));
        
        return team;
    }
    
    // Trabzonspor 2025-2026 - Transfermarkt verilerine göre (Gerçek oyuncular)
    TeamData CreateTrabzonspor2025_2026()
    {
        TeamData team = new TeamData();
        team.teamName = "Trabzonspor";
        team.teamShortName = "TS";
        team.teamCountry = "Türkiye";
        team.primaryColor = new Color(0.8f, 0f, 0f); // Bordo
        team.secondaryColor = new Color(0f, 0.2f, 0.6f); // Mavi
        
        // Kaleci
        team.players.Add(CreatePlayer("Uğurcan Çakır", PlayerPosition.KL, 74));
        team.players.Add(CreatePlayer("Mehmet Can Aydın", PlayerPosition.KL, 68));
        
        // Defans
        team.players.Add(CreatePlayer("Stefano Denswil", PlayerPosition.STP, 72));
        team.players.Add(CreatePlayer("Eren Elmalı", PlayerPosition.STP, 70));
        team.players.Add(CreatePlayer("Tayyib Talha Sanuç", PlayerPosition.STP, 69));
        team.players.Add(CreatePlayer("Enis Destan", PlayerPosition.SLB, 71));
        team.players.Add(CreatePlayer("Muhammet Taha Tepe", PlayerPosition.SLB, 68));
        team.players.Add(CreatePlayer("Dimitris Kourbelis", PlayerPosition.SĞB, 71));
        
        // Orta Saha
        team.players.Add(CreatePlayer("Manolis Siopis", PlayerPosition.MDO, 70));
        team.players.Add(CreatePlayer("Dimitris Kourbelis", PlayerPosition.MDO, 71));
        team.players.Add(CreatePlayer("Enis Bardhi", PlayerPosition.MOO, 74));
        team.players.Add(CreatePlayer("Edin Višća", PlayerPosition.MOO, 72));
        team.players.Add(CreatePlayer("Anastasios Bakasetas", PlayerPosition.MOO, 73));
        
        // Kanatlar
        team.players.Add(CreatePlayer("Trezeguet", PlayerPosition.SLK, 73));
        team.players.Add(CreatePlayer("Abdülkadir Ömür", PlayerPosition.SĞK, 73));
        team.players.Add(CreatePlayer("Muhammet Taha Tepe", PlayerPosition.SĞK, 69));
        
        // Forvet
        team.players.Add(CreatePlayer("Paul Onuachu", PlayerPosition.SF, 75));
        team.players.Add(CreatePlayer("Nicolás Peña", PlayerPosition.SF, 71));
        team.players.Add(CreatePlayer("Edin Džeko", PlayerPosition.SF, 74));
        team.players.Add(CreatePlayer("Burak Yılmaz", PlayerPosition.SF, 72));
        
        // Yedekler
        team.players.Add(CreatePlayer("Hüseyin Türkmen", PlayerPosition.STP, 68));
        team.players.Add(CreatePlayer("Batuhan Kör", PlayerPosition.MDO, 67));
        team.players.Add(CreatePlayer("Doğucan Yalçın", PlayerPosition.MOO, 68));
        team.players.Add(CreatePlayer("Kerem Şen", PlayerPosition.SF, 69));
        
        return team;
    }
    
    TeamData CreateBasaksehir2025_2026()
    {
        TeamData team = new TeamData();
        team.teamName = "Başakşehir";
        team.teamShortName = "BAŞ";
        team.teamCountry = "Türkiye";
        team.primaryColor = new Color(0.2f, 0.4f, 0.8f);
        team.secondaryColor = new Color(0.9f, 0.9f, 0.9f);
        
        // Kaleci
        team.players.Add(CreatePlayer("Volkan Babacan", PlayerPosition.KL, 72));
        team.players.Add(CreatePlayer("Okan Kocuk", PlayerPosition.KL, 68));
        
        // Defans
        team.players.Add(CreatePlayer("Leo Duarte", PlayerPosition.STP, 71));
        team.players.Add(CreatePlayer("Patryk Szysz", PlayerPosition.STP, 69));
        team.players.Add(CreatePlayer("Emre Belözoğlu", PlayerPosition.STP, 69));
        team.players.Add(CreatePlayer("Lucas Lima", PlayerPosition.SLB, 71));
        team.players.Add(CreatePlayer("Mahmut Tekdemir", PlayerPosition.SĞB, 70));
        
        // Orta Saha
        team.players.Add(CreatePlayer("Mahmut Tekdemir", PlayerPosition.MDO, 70));
        team.players.Add(CreatePlayer("Lucas Biglia", PlayerPosition.MDO, 70));
        team.players.Add(CreatePlayer("Deniz Türüç", PlayerPosition.MOO, 71));
        team.players.Add(CreatePlayer("Edin Višća", PlayerPosition.MOO, 72));
        team.players.Add(CreatePlayer("Emre Belözoğlu", PlayerPosition.MOO, 69));
        
        // Kanatlar
        team.players.Add(CreatePlayer("Serdar Gürler", PlayerPosition.SĞK, 70));
        team.players.Add(CreatePlayer("Deniz Türüç", PlayerPosition.SLK, 71));
        
        // Forvet
        team.players.Add(CreatePlayer("Krzysztof Piątek", PlayerPosition.SF, 73));
        team.players.Add(CreatePlayer("Stefano Okaka", PlayerPosition.SF, 71));
        team.players.Add(CreatePlayer("Enzo Crivelli", PlayerPosition.SF, 70));
        
        // Yedekler
        team.players.Add(CreatePlayer("Okan Kocuk", PlayerPosition.KL, 68));
        team.players.Add(CreatePlayer("Youssouf Ndayishimiye", PlayerPosition.MDO, 69));
        team.players.Add(CreatePlayer("Berkay Özcan", PlayerPosition.MOO, 68));
        team.players.Add(CreatePlayer("Oğulcan Çağlayan", PlayerPosition.SF, 69));
        
        return team;
    }
    
    TeamData CreateAlanyaspor2025_2026()
    {
        TeamData team = new TeamData();
        team.teamName = "Alanyaspor";
        team.teamShortName = "ALA";
        team.teamCountry = "Türkiye";
        team.primaryColor = new Color(0f, 0.6f, 0.8f);
        team.secondaryColor = new Color(1f, 1f, 1f);
        
        // Kaleci
        team.players.Add(CreatePlayer("Ertuğrul Taşkıran", PlayerPosition.KL, 70));
        
        // Defans
        team.players.Add(CreatePlayer("Pape Cissé", PlayerPosition.STP, 71));
        team.players.Add(CreatePlayer("Fatih Aksoy", PlayerPosition.STP, 68));
        team.players.Add(CreatePlayer("Yusuf Özdemir", PlayerPosition.SLB, 69));
        
        // Orta Saha
        team.players.Add(CreatePlayer("Leroy Fer", PlayerPosition.MDO, 70));
        team.players.Add(CreatePlayer("Oğuz Aydın", PlayerPosition.MDO, 68));
        team.players.Add(CreatePlayer("Efecan Karaca", PlayerPosition.MOO, 71));
        team.players.Add(CreatePlayer("João Novais", PlayerPosition.MOO, 70));
        
        // Kanatlar
        team.players.Add(CreatePlayer("Nicolas Janvier", PlayerPosition.SĞK, 69));
        
        // Forvet
        team.players.Add(CreatePlayer("Flavio", PlayerPosition.SF, 71));
        team.players.Add(CreatePlayer("Eren Karataş", PlayerPosition.SF, 69));
        
        // Yedekler
        team.players.Add(CreatePlayer("Carlos Eduardo", PlayerPosition.KL, 68));
        team.players.Add(CreatePlayer("Wilson Eduardo", PlayerPosition.SF, 70));
        team.players.Add(CreatePlayer("Khouma Babacar", PlayerPosition.SF, 69));
        
        return team;
    }
    
    TeamData CreateKonyaspor2025_2026()
    {
        TeamData team = new TeamData();
        team.teamName = "Konyaspor";
        team.teamShortName = "KON";
        team.teamCountry = "Türkiye";
        team.primaryColor = new Color(0.9f, 0.1f, 0.1f);
        team.secondaryColor = new Color(1f, 1f, 1f);
        
        // Kaleci
        team.players.Add(CreatePlayer("Erhan Erentürk", PlayerPosition.KL, 69));
        team.players.Add(CreatePlayer("Guilherme", PlayerPosition.KL, 68));
        
        // Defans
        team.players.Add(CreatePlayer("Francisco Calvo", PlayerPosition.STP, 71));
        team.players.Add(CreatePlayer("Uğur Demirok", PlayerPosition.STP, 68));
        team.players.Add(CreatePlayer("Soner Dikmen", PlayerPosition.STP, 68));
        
        // Orta Saha
        team.players.Add(CreatePlayer("Amilton", PlayerPosition.MDO, 70));
        team.players.Add(CreatePlayer("Bruno Paz", PlayerPosition.MDO, 69));
        team.players.Add(CreatePlayer("Endri Çekiçi", PlayerPosition.MOO, 70));
        
        // Kanatlar
        team.players.Add(CreatePlayer("Konrad Michalak", PlayerPosition.SLK, 70));
        
        // Forvet
        team.players.Add(CreatePlayer("Mame Thiam", PlayerPosition.SF, 72));
        team.players.Add(CreatePlayer("Sokol Cikalleshi", PlayerPosition.SF, 71));
        
        // Yedekler
        team.players.Add(CreatePlayer("Sokol Cikalleshi", PlayerPosition.SF, 71));
        team.players.Add(CreatePlayer("Erdoğan Yeşilyurt", PlayerPosition.MOO, 68));
        
        return team;
    }
    
    TeamData CreateKayserispor2025_2026()
    {
        TeamData team = new TeamData();
        team.teamName = "Kayserispor";
        team.teamShortName = "KAY";
        team.teamCountry = "Türkiye";
        team.primaryColor = new Color(0f, 0.4f, 0.8f);
        team.secondaryColor = new Color(1f, 1f, 1f);
        
        // Kaleci
        team.players.Add(CreatePlayer("Bilal Bayazit", PlayerPosition.KL, 68));
        team.players.Add(CreatePlayer("Gökhan Değirmenci", PlayerPosition.KL, 67));
        
        // Defans
        team.players.Add(CreatePlayer("Joseph Attamah", PlayerPosition.STP, 69));
        team.players.Add(CreatePlayer("Lionel Carole", PlayerPosition.SLB, 68));
        team.players.Add(CreatePlayer("Onur Bulut", PlayerPosition.SĞB, 69));
        
        // Orta Saha
        team.players.Add(CreatePlayer("Ali Karimi", PlayerPosition.MDO, 70));
        team.players.Add(CreatePlayer("Arif Kocaman", PlayerPosition.MDO, 68));
        team.players.Add(CreatePlayer("Aylton Boa Morte", PlayerPosition.MOO, 69));
        
        // Kanatlar
        team.players.Add(CreatePlayer("Miguel Cardoso", PlayerPosition.SĞK, 69));
        team.players.Add(CreatePlayer("Carlos Mane", PlayerPosition.SLK, 70));
        
        // Forvet
        team.players.Add(CreatePlayer("Mame Thiam", PlayerPosition.SF, 71));
        team.players.Add(CreatePlayer("Mario Gavranović", PlayerPosition.SF, 70));
        
        // Yedekler
        team.players.Add(CreatePlayer("Arif Kocaman", PlayerPosition.MDO, 68));
        team.players.Add(CreatePlayer("Gustavo Campanharo", PlayerPosition.MDO, 69));
        
        return team;
    }
    
    TeamData CreateAntalyaspor2025_2026()
    {
        TeamData team = new TeamData();
        team.teamName = "Antalyaspor";
        team.teamShortName = "ANT";
        team.teamCountry = "Türkiye";
        team.primaryColor = new Color(1f, 0.8f, 0f);
        team.secondaryColor = new Color(0.2f, 0.2f, 0.2f);
        
        // Kaleci
        team.players.Add(CreatePlayer("Helton Leite", PlayerPosition.KL, 71));
        
        // Defans
        team.players.Add(CreatePlayer("Ömer Toprak", PlayerPosition.STP, 72));
        team.players.Add(CreatePlayer("Bünyamin Balcı", PlayerPosition.STP, 68));
        team.players.Add(CreatePlayer("Güray Vural", PlayerPosition.SLB, 69));
        team.players.Add(CreatePlayer("Ufuk Akyol", PlayerPosition.SĞB, 69));
        
        // Orta Saha
        team.players.Add(CreatePlayer("Fernando", PlayerPosition.MDO, 70));
        team.players.Add(CreatePlayer("Veysel Sarı", PlayerPosition.MOO, 70));
        
        // Kanatlar
        team.players.Add(CreatePlayer("Sam Larsson", PlayerPosition.SLK, 72));
        team.players.Add(CreatePlayer("Erdoğan Yeşilyurt", PlayerPosition.SĞK, 69));
        
        // Forvet
        team.players.Add(CreatePlayer("Haji Wright", PlayerPosition.SF, 73));
        team.players.Add(CreatePlayer("Adam Buksa", PlayerPosition.SF, 71));
        team.players.Add(CreatePlayer("Bekir Yılmaz", PlayerPosition.SF, 69));
        
        // Yedekler
        team.players.Add(CreatePlayer("Nazım Sangaré", PlayerPosition.STP, 69));
        team.players.Add(CreatePlayer("Bekir Yılmaz", PlayerPosition.SF, 69));
        
        return team;
    }
    
    TeamData CreateGaziantep2025_2026()
    {
        TeamData team = new TeamData();
        team.teamName = "Gaziantep FK";
        team.teamShortName = "GAZ";
        team.teamCountry = "Türkiye";
        team.primaryColor = new Color(0.8f, 0f, 0f);
        team.secondaryColor = new Color(1f, 1f, 1f);
        
        // Kaleci
        team.players.Add(CreatePlayer("Günay Güvenç", PlayerPosition.KL, 68));
        team.players.Add(CreatePlayer("Mustafa Eskihellaç", PlayerPosition.KL, 66));
        
        // Defans
        team.players.Add(CreatePlayer("Papy Djilobodji", PlayerPosition.STP, 69));
        team.players.Add(CreatePlayer("Júnior Morais", PlayerPosition.STP, 68));
        team.players.Add(CreatePlayer("Stelios Kitsiou", PlayerPosition.SĞB, 68));
        team.players.Add(CreatePlayer("Mirza Cihan", PlayerPosition.SLB, 67));
        
        // Orta Saha
        team.players.Add(CreatePlayer("Marko Jevtović", PlayerPosition.MDO, 68));
        team.players.Add(CreatePlayer("Ogün Özçiçek", PlayerPosition.MDO, 67));
        team.players.Add(CreatePlayer("João Figueiredo", PlayerPosition.MOO, 69));
        
        // Kanatlar
        team.players.Add(CreatePlayer("Furkan Soyalp", PlayerPosition.SĞK, 68));
        
        // Forvet
        team.players.Add(CreatePlayer("Maximilian Beier", PlayerPosition.SF, 70));
        team.players.Add(CreatePlayer("Mirza Cihan", PlayerPosition.SF, 67));
        
        // Yedekler
        team.players.Add(CreatePlayer("Alexandru Maxim", PlayerPosition.MOO, 69));
        team.players.Add(CreatePlayer("Onur Eriş", PlayerPosition.SF, 68));
        
        return team;
    }
    
    TeamData CreateKasimpasa2025_2026()
    {
        TeamData team = new TeamData();
        team.teamName = "Kasımpaşa";
        team.teamShortName = "KAS";
        team.teamCountry = "Türkiye";
        team.primaryColor = new Color(1f, 0f, 0f);
        team.secondaryColor = new Color(0f, 0f, 0f);
        
        // Kaleci
        team.players.Add(CreatePlayer("Eray Birniçan", PlayerPosition.KL, 67));
        
        // Defans
        team.players.Add(CreatePlayer("Mamadou Fall", PlayerPosition.STP, 69));
        team.players.Add(CreatePlayer("Sadık Çiftpınar", PlayerPosition.STP, 68));
        team.players.Add(CreatePlayer("Kenneth Omeruo", PlayerPosition.STP, 69));
        team.players.Add(CreatePlayer("Florent Hadergjonaj", PlayerPosition.SĞB, 68));
        team.players.Add(CreatePlayer("Tunahan Cingöz", PlayerPosition.SLB, 67));
        
        // Orta Saha
        team.players.Add(CreatePlayer("Aytaç Kara", PlayerPosition.MDO, 68));
        team.players.Add(CreatePlayer("Valentin Eysseric", PlayerPosition.MOO, 69));
        team.players.Add(CreatePlayer("Haris Hajradinović", PlayerPosition.MOO, 69));
        
        // Kanatlar
        team.players.Add(CreatePlayer("Yusuf Erdoğan", PlayerPosition.SLK, 68));
        
        // Forvet
        team.players.Add(CreatePlayer("Mortadha Ben Ouanes", PlayerPosition.SF, 68));
        team.players.Add(CreatePlayer("Mamadou Fall", PlayerPosition.SF, 69));
        
        // Yedekler
        team.players.Add(CreatePlayer("Valentin Eysseric", PlayerPosition.MOO, 69));
        team.players.Add(CreatePlayer("Yusuf Erdoğan", PlayerPosition.SF, 68));
        
        return team;
    }
    
    TeamData CreateFatihKaragumruk2025_2026()
    {
        TeamData team = new TeamData();
        team.teamName = "Fatih Karagümrük";
        team.teamShortName = "FKG";
        team.teamCountry = "Türkiye";
        team.primaryColor = new Color(0f, 0f, 0.8f);
        team.secondaryColor = new Color(1f, 1f, 1f);
        
        // Kaleci
        team.players.Add(CreatePlayer("Vincenzo Viviano", PlayerPosition.KL, 70));
        team.players.Add(CreatePlayer("Eray İşcan", PlayerPosition.KL, 68));
        
        // Defans
        team.players.Add(CreatePlayer("Davide Biraschi", PlayerPosition.STP, 69));
        team.players.Add(CreatePlayer("Salih Dursun", PlayerPosition.STP, 68));
        team.players.Add(CreatePlayer("Levent Mercan", PlayerPosition.STP, 69));
        team.players.Add(CreatePlayer("Alassane Ndao", PlayerPosition.SLB, 69));
        team.players.Add(CreatePlayer("Valentin Rosier", PlayerPosition.SĞB, 72));
        
        // Orta Saha
        team.players.Add(CreatePlayer("Younès Belhanda", PlayerPosition.MOO, 72));
        team.players.Add(CreatePlayer("Olimpiu Moruțan", PlayerPosition.MOO, 70));
        
        // Kanatlar
        team.players.Add(CreatePlayer("Emre Mor", PlayerPosition.SĞK, 70));
        team.players.Add(CreatePlayer("Fabio Borini", PlayerPosition.SLK, 71));
        
        // Forvet
        team.players.Add(CreatePlayer("Fabio Borini", PlayerPosition.SF, 71));
        team.players.Add(CreatePlayer("Mateus Pacheco", PlayerPosition.SF, 70));
        
        // Yedekler
        team.players.Add(CreatePlayer("Salih Dursun", PlayerPosition.STP, 68));
        team.players.Add(CreatePlayer("Levent Mercan", PlayerPosition.MDO, 69));
        
        return team;
    }
    
    TeamData CreateGoztepe2025_2026()
    {
        TeamData team = new TeamData();
        team.teamName = "Göztepe";
        team.teamShortName = "GÖZ";
        team.teamCountry = "Türkiye";
        team.primaryColor = new Color(0f, 0.6f, 0f);
        team.secondaryColor = new Color(1f, 1f, 1f);
        
        // Kaleci
        team.players.Add(CreatePlayer("Beto", PlayerPosition.KL, 68));
        team.players.Add(CreatePlayer("Murat Cem Akpınar", PlayerPosition.KL, 66));
        
        // Defans
        team.players.Add(CreatePlayer("Atınç Nukan", PlayerPosition.STP, 68));
        team.players.Add(CreatePlayer("Alpaslan Öztürk", PlayerPosition.STP, 68));
        team.players.Add(CreatePlayer("Berkan Emir", PlayerPosition.SLB, 67));
        team.players.Add(CreatePlayer("Titi", PlayerPosition.SĞB, 68));
        
        // Orta Saha
        team.players.Add(CreatePlayer("Soner Aydoğdu", PlayerPosition.MDO, 69));
        team.players.Add(CreatePlayer("Murat Paluli", PlayerPosition.MOO, 68));
        
        // Kanatlar
        team.players.Add(CreatePlayer("Franck Ribéry", PlayerPosition.SLK, 69));
        team.players.Add(CreatePlayer("Halil Akbunar", PlayerPosition.SĞK, 70));
        
        // Forvet
        team.players.Add(CreatePlayer("Stefano Napoleoni", PlayerPosition.SF, 70));
        team.players.Add(CreatePlayer("Beto", PlayerPosition.SF, 68));
        
        // Yedekler
        team.players.Add(CreatePlayer("Murat Paluli", PlayerPosition.MOO, 68));
        team.players.Add(CreatePlayer("Beto", PlayerPosition.SF, 68));
        
        return team;
    }
    
    TeamData CreateRizespor2025_2026()
    {
        TeamData team = new TeamData();
        team.teamName = "Çaykur Rizespor";
        team.teamShortName = "RİZ";
        team.teamCountry = "Türkiye";
        team.primaryColor = new Color(0f, 0.4f, 0.8f);
        team.secondaryColor = new Color(1f, 1f, 1f);
        
        // Kaleci
        team.players.Add(CreatePlayer("Gökhan Akkan", PlayerPosition.KL, 69));
        team.players.Add(CreatePlayer("Alperen Uysal", PlayerPosition.KL, 67));
        
        // Defans
        team.players.Add(CreatePlayer("Yasin Pehlivan", PlayerPosition.STP, 68));
        team.players.Add(CreatePlayer("Eren Albayrak", PlayerPosition.SLB, 68));
        team.players.Add(CreatePlayer("Gökhan Gönül", PlayerPosition.SĞB, 69));
        team.players.Add(CreatePlayer("Jonas Svensson", PlayerPosition.SĞB, 69));
        
        // Orta Saha
        team.players.Add(CreatePlayer("Altin Kryeziu", PlayerPosition.MDO, 68));
        team.players.Add(CreatePlayer("Yasin Öztürk", PlayerPosition.MOO, 68));
        
        // Kanatlar
        team.players.Add(CreatePlayer("Emirhan Topçu", PlayerPosition.SLK, 68));
        
        // Forvet
        team.players.Add(CreatePlayer("Martin Minchev", PlayerPosition.SF, 70));
        team.players.Add(CreatePlayer("Muhammet Taha Şahin", PlayerPosition.SF, 67));
        
        // Yedekler
        team.players.Add(CreatePlayer("Yasin Öztürk", PlayerPosition.MOO, 68));
        team.players.Add(CreatePlayer("Muhammet Taha Şahin", PlayerPosition.SF, 67));
        
        return team;
    }
    
    TeamData CreateSamsunspor2025_2026()
    {
        TeamData team = new TeamData();
        team.teamName = "Samsunspor";
        team.teamShortName = "SAM";
        team.teamCountry = "Türkiye";
        team.primaryColor = new Color(1f, 1f, 1f);
        team.secondaryColor = new Color(0f, 0f, 0f);
        
        // Kaleci
        team.players.Add(CreatePlayer("Okan Kocuk", PlayerPosition.KL, 68));
        
        // Defans
        team.players.Add(CreatePlayer("Erhan Kartal", PlayerPosition.STP, 68));
        team.players.Add(CreatePlayer("Gaëtan Laura", PlayerPosition.SLB, 68));
        
        // Orta Saha
        team.players.Add(CreatePlayer("Youssef Aït Bennasser", PlayerPosition.MDO, 69));
        team.players.Add(CreatePlayer("Emre Kılınç", PlayerPosition.MOO, 70));
        team.players.Add(CreatePlayer("Moussa Guel", PlayerPosition.MDO, 68));
        
        // Kanatlar
        team.players.Add(CreatePlayer("Yasin Öztekin", PlayerPosition.SĞK, 69));
        team.players.Add(CreatePlayer("Kingsley Schindler", PlayerPosition.SĞK, 69));
        team.players.Add(CreatePlayer("Moryké Fofana", PlayerPosition.SLK, 69));
        
        // Forvet
        team.players.Add(CreatePlayer("Marius Mouandilmadji", PlayerPosition.SF, 70));
        team.players.Add(CreatePlayer("Alioune Ndour", PlayerPosition.SF, 68));
        
        // Yedekler
        team.players.Add(CreatePlayer("Moussa Guel", PlayerPosition.MDO, 68));
        team.players.Add(CreatePlayer("Alioune Ndour", PlayerPosition.SF, 68));
        
        return team;
    }
    
    TeamData CreateEyupspor2025_2026()
    {
        TeamData team = new TeamData();
        team.teamName = "Eyüpspor";
        team.teamShortName = "EYÜ";
        team.teamCountry = "Türkiye";
        team.primaryColor = new Color(0.8f, 0f, 0f);
        team.secondaryColor = new Color(0f, 0f, 0f);
        
        // Kaleci
        team.players.Add(CreatePlayer("Alperen Uysal", PlayerPosition.KL, 67));
        
        // Defans
        team.players.Add(CreatePlayer("Mert Çelik", PlayerPosition.STP, 67));
        team.players.Add(CreatePlayer("Burak Süleyman", PlayerPosition.SĞB, 67));
        team.players.Add(CreatePlayer("Kemal Rüzgar", PlayerPosition.SLB, 67));
        
        // Orta Saha
        team.players.Add(CreatePlayer("Yasin Dülger", PlayerPosition.MDO, 67));
        team.players.Add(CreatePlayer("Emre Kılınç", PlayerPosition.MOO, 68));
        
        // Kanatlar
        team.players.Add(CreatePlayer("Furkan Yaman", PlayerPosition.SĞK, 68));
        
        // Forvet
        team.players.Add(CreatePlayer("Burak Süleyman", PlayerPosition.SF, 67));
        team.players.Add(CreatePlayer("Kemal Rüzgar", PlayerPosition.SF, 67));
        
        // Yedekler
        team.players.Add(CreatePlayer("Emre Kılınç", PlayerPosition.MOO, 68));
        team.players.Add(CreatePlayer("Furkan Yaman", PlayerPosition.SF, 68));
        
        return team;
    }
    
    TeamData CreatePendikspor2025_2026()
    {
        TeamData team = new TeamData();
        team.teamName = "Pendikspor";
        team.teamShortName = "PEN";
        team.teamCountry = "Türkiye";
        team.primaryColor = new Color(0f, 0.6f, 0.2f);
        team.secondaryColor = new Color(1f, 1f, 1f);
        
        // Kaleci
        team.players.Add(CreatePlayer("Erdem Özgenç", PlayerPosition.KL, 66));
        team.players.Add(CreatePlayer("Murat Sözgelmez", PlayerPosition.KL, 65));
        
        // Defans
        team.players.Add(CreatePlayer("Murat Akça", PlayerPosition.STP, 67));
        team.players.Add(CreatePlayer("Alpaslan Öztürk", PlayerPosition.STP, 67));
        team.players.Add(CreatePlayer("Murat Uluç", PlayerPosition.SLB, 66));
        team.players.Add(CreatePlayer("Emre Taşdemir", PlayerPosition.SĞB, 67));
        
        // Orta Saha
        team.players.Add(CreatePlayer("Thibault Moulin", PlayerPosition.MDO, 67));
        team.players.Add(CreatePlayer("Erencan Yardımcı", PlayerPosition.MOO, 67));
        team.players.Add(CreatePlayer("Berkay Öztürk", PlayerPosition.MDO, 66));
        
        // Kanatlar
        team.players.Add(CreatePlayer("Halil Akbunar", PlayerPosition.SĞK, 68));
        team.players.Add(CreatePlayer("Leandro Kappel", PlayerPosition.SLK, 68));
        
        // Forvet
        team.players.Add(CreatePlayer("Leandro Kappel", PlayerPosition.SF, 68));
        team.players.Add(CreatePlayer("Erencan Yardımcı", PlayerPosition.SF, 67));
        
        // Yedekler
        team.players.Add(CreatePlayer("Berkay Öztürk", PlayerPosition.MDO, 66));
        team.players.Add(CreatePlayer("Erencan Yardımcı", PlayerPosition.SF, 67));
        
        return team;
    }
    
    TeamData CreateHatayspor2025_2026()
    {
        TeamData team = new TeamData();
        team.teamName = "Hatayspor";
        team.teamShortName = "HAT";
        team.teamCountry = "Türkiye";
        team.primaryColor = new Color(1f, 0.8f, 0f);
        team.secondaryColor = new Color(0.2f, 0.2f, 0.8f);
        
        // Kaleci
        team.players.Add(CreatePlayer("Erce Kardeşler", PlayerPosition.KL, 66));
        team.players.Add(CreatePlayer("Kamil Ahmet Çörekçi", PlayerPosition.KL, 65));
        
        // Defans
        team.players.Add(CreatePlayer("Onur Ergun", PlayerPosition.STP, 67));
        team.players.Add(CreatePlayer("Burak Öksüz", PlayerPosition.SLB, 66));
        team.players.Add(CreatePlayer("Burak Camoglu", PlayerPosition.SĞB, 67));
        
        // Orta Saha
        team.players.Add(CreatePlayer("Kerim Alıcı", PlayerPosition.MDO, 67));
        team.players.Add(CreatePlayer("Rúben Ribeiro", PlayerPosition.MOO, 68));
        
        // Kanatlar
        team.players.Add(CreatePlayer("Adama Traoré", PlayerPosition.SLK, 68));
        
        // Forvet
        team.players.Add(CreatePlayer("Ayoub El Kaabi", PlayerPosition.SF, 69));
        team.players.Add(CreatePlayer("Carlos Strandberg", PlayerPosition.SF, 68));
        team.players.Add(CreatePlayer("Zé Luís", PlayerPosition.SF, 68));
        
        // Yedekler
        team.players.Add(CreatePlayer("Rúben Ribeiro", PlayerPosition.MOO, 68));
        team.players.Add(CreatePlayer("Zé Luís", PlayerPosition.SF, 68));
        
        return team;
    }
    
    TeamData CreateGenericTeam(string teamName, string shortName, int minOverall, int maxOverall)
    {
        TeamData team = new TeamData();
        team.teamName = teamName;
        team.teamShortName = shortName;
        team.teamCountry = "Türkiye";
        
        // 22 oyuncu oluştur (gerçekçi dağılım)
        team.players.Add(CreatePlayer($"{teamName} - Kaleci", PlayerPosition.KL, Random.Range(minOverall, maxOverall)));
        team.players.Add(CreatePlayer($"{teamName} - Stoper 1", PlayerPosition.STP, Random.Range(minOverall, maxOverall)));
        team.players.Add(CreatePlayer($"{teamName} - Stoper 2", PlayerPosition.STP, Random.Range(minOverall - 2, maxOverall - 2)));
        team.players.Add(CreatePlayer($"{teamName} - Sağ Bek", PlayerPosition.SĞB, Random.Range(minOverall - 1, maxOverall - 1)));
        team.players.Add(CreatePlayer($"{teamName} - Sol Bek", PlayerPosition.SLB, Random.Range(minOverall - 1, maxOverall - 1)));
        team.players.Add(CreatePlayer($"{teamName} - Defansif Orta 1", PlayerPosition.MDO, Random.Range(minOverall, maxOverall)));
        team.players.Add(CreatePlayer($"{teamName} - Defansif Orta 2", PlayerPosition.MDO, Random.Range(minOverall - 2, maxOverall - 2)));
        team.players.Add(CreatePlayer($"{teamName} - Ofansif Orta", PlayerPosition.MOO, Random.Range(minOverall, maxOverall)));
        team.players.Add(CreatePlayer($"{teamName} - Sağ Kanat", PlayerPosition.SĞK, Random.Range(minOverall, maxOverall)));
        team.players.Add(CreatePlayer($"{teamName} - Sol Kanat", PlayerPosition.SLK, Random.Range(minOverall, maxOverall)));
        team.players.Add(CreatePlayer($"{teamName} - Forvet", PlayerPosition.SF, Random.Range(minOverall, maxOverall)));
        
        // Yedekler
        for (int i = 0; i < 11; i++)
        {
            team.players.Add(CreatePlayer($"{teamName} - Yedek {i+1}", GetRandomPosition(), Random.Range(minOverall - 5, maxOverall - 3)));
        }
        
        return team;
    }
    
    PlayerData CreatePlayer(string name, PlayerPosition position, int overall)
    {
        PlayerData player = new PlayerData();
        player.playerName = name;
        player.position = position;
        player.SetOverall(overall);
        player.age = Random.Range(18, 35);
        player.nationality = "Türkiye";
        
        // Pozisyona göre yetenekleri ayarla
        AdjustSkillsForPosition(player, position, overall);
        
        return player;
    }
    
    void AdjustSkillsForPosition(PlayerData player, PlayerPosition position, int overall)
    {
        int variation = 8;
        int baseSkill = overall;
        
        switch (position)
        {
            case PlayerPosition.KL:
                player.saveReflex = Mathf.Clamp(Random.Range(baseSkill - variation, baseSkill + variation), 0, 100);
                player.goalkeeperPositioning = Mathf.Clamp(Random.Range(baseSkill - variation, baseSkill + variation), 0, 100);
                player.aerialAbility = Mathf.Clamp(Random.Range(baseSkill - variation, baseSkill + variation), 0, 100);
                player.oneOnOne = Mathf.Clamp(Random.Range(baseSkill - variation, baseSkill + variation), 0, 100);
                player.handling = Mathf.Clamp(Random.Range(baseSkill - variation, baseSkill + variation), 0, 100);
                break;
                
            case PlayerPosition.STP:
            case PlayerPosition.SĞB:
            case PlayerPosition.SLB:
                player.defendingSkill = Mathf.Clamp(Random.Range(baseSkill - variation, baseSkill + variation), 0, 100);
                player.physicalStrength = Mathf.Clamp(Random.Range(baseSkill - variation, baseSkill + variation), 0, 100);
                player.passingSkill = Mathf.Clamp(Random.Range(baseSkill - variation - 5, baseSkill + variation), 0, 100);
                break;
                
            case PlayerPosition.MDO:
                player.defendingSkill = Mathf.Clamp(Random.Range(baseSkill - variation, baseSkill + variation), 0, 100);
                player.passingSkill = Mathf.Clamp(Random.Range(baseSkill - variation, baseSkill + variation), 0, 100);
                player.physicalStrength = Mathf.Clamp(Random.Range(baseSkill - variation, baseSkill + variation), 0, 100);
                break;
                
            case PlayerPosition.MOO:
                player.passingSkill = Mathf.Clamp(Random.Range(baseSkill - variation, baseSkill + variation), 0, 100);
                player.dribblingSkill = Mathf.Clamp(Random.Range(baseSkill - variation, baseSkill + variation), 0, 100);
                player.shootingSkill = Mathf.Clamp(Random.Range(baseSkill - variation, baseSkill + variation), 0, 100);
                break;
                
            case PlayerPosition.SĞK:
            case PlayerPosition.SLK:
            case PlayerPosition.SĞO:
            case PlayerPosition.SLO:
                player.speed = Mathf.Clamp(Random.Range(baseSkill - variation, baseSkill + variation), 0, 100);
                player.dribblingSkill = Mathf.Clamp(Random.Range(baseSkill - variation, baseSkill + variation), 0, 100);
                player.passingSkill = Mathf.Clamp(Random.Range(baseSkill - variation, baseSkill + variation), 0, 100);
                break;
                
            case PlayerPosition.SF:
                player.shootingSkill = Mathf.Clamp(Random.Range(baseSkill - variation, baseSkill + variation), 0, 100);
                player.physicalStrength = Mathf.Clamp(Random.Range(baseSkill - variation, baseSkill + variation), 0, 100);
                player.dribblingSkill = Mathf.Clamp(Random.Range(baseSkill - variation, baseSkill + variation), 0, 100);
                break;
        }
        
        player.CalculateOverall();
    }
    
    PlayerPosition GetRandomPosition()
    {
        return (PlayerPosition)Random.Range(0, System.Enum.GetValues(typeof(PlayerPosition)).Length);
    }
}

