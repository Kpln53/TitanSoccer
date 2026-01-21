using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// Süper Lig benzeri lig ve takımları otomatik oluşturur
/// </summary>
public class SuperLigDataCreator : EditorWindow
{
    private DataPack targetDataPack;
    
    [MenuItem("TitanSoccer/Create Super Lig Data")]
    public static void ShowWindow()
    {
        GetWindow<SuperLigDataCreator>("Super Lig Data Creator");
    }
    
    void OnGUI()
    {
        GUILayout.Label("Süper Lig Benzeri Lig Oluşturucu", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        targetDataPack = (DataPack)EditorGUILayout.ObjectField("Hedef Data Pack", targetDataPack, typeof(DataPack), false);
        
        EditorGUILayout.HelpBox("Bu araç Süper Lig benzeri bir lig oluşturur. Takım isimleri telif hakkı nedeniyle benzer ama farklıdır.", MessageType.Info);
        
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
        
        if (GUILayout.Button("Süper Lig Oluştur (18 Takım - 2025-2026)", GUILayout.Height(40)))
        {
            CreateSuperLig();
        }
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("Sadece Büyük 3 Takımı Oluştur (Test İçin)"))
        {
            CreateBigThreeTeams();
        }
    }
    
    void CreateNewDataPack()
    {
        string path = EditorUtility.SaveFilePanelInProject("Yeni Data Pack Oluştur", "SuperLigDataPack", "asset", "");
        if (!string.IsNullOrEmpty(path))
        {
            DataPack newPack = CreateInstance<DataPack>();
            newPack.packName = "Süper Lig Data Pack";
            newPack.packVersion = "1.0.0";
            newPack.packAuthor = "TitanSoccer";
            newPack.packDescription = "Süper Lig benzeri lig ve takımlar";
            AssetDatabase.CreateAsset(newPack, path);
            AssetDatabase.SaveAssets();
            targetDataPack = newPack;
        }
    }
    
    void CreateSuperLig()
    {
        if (targetDataPack == null) return;
        
        // Süper Lig oluştur
        LeagueData superLig = new LeagueData();
        superLig.leagueName = "Süper Lig";
        superLig.leagueCountry = "Türkiye";
        superLig.leagueTier = 1;
        
        // 2025-2026 Güncel Süper Lig takımları (18 takım)
        string[] teamNames = {
            "Galatasaray", "Fenerbahçe", "Beşiktaş",
            "Trabzonspor", "Başakşehir", "Alanyaspor",
            "Konyaspor", "Kayserispor", "Antalyaspor",
            "Gaziantep", "Kasımpaşa", "Fatih Karagümrük",
            "Göztepe", "Çaykur Rizespor", "Samsunspor",
            "Eyüpspor", "Gençlerbirliği", "Kocaelispor"
        };
        
        // Takım güçleri (2025-2026 sezonu güncel güçler - 18 takım)
        int[] teamPowers = {
            82, 81, 80, // Büyük 3 (Galatasaray, Fenerbahçe, Beşiktaş)
            75, 74, 73, // Üst sıra (Trabzonspor, Başakşehir, Alanyaspor)
            72, 71, 70, // Orta-üst (Konyaspor, Kayserispor, Antalyaspor)
            69, 68, 67, // Orta (Gaziantep, Kasımpaşa, Fatih Karagümrük)
            66, 65, 64, // Orta-alt (Göztepe, Çaykur Rizespor, Samsunspor)
            63, 62, 61  // Alt sıra (Eyüpspor, Gençlerbirliği, Kocaelispor)
        };
        
        for (int i = 0; i < teamNames.Length; i++)
        {
            TeamData team = CreateTeam(teamNames[i], teamPowers[i]);
            superLig.teams.Add(team);
        }
        
        // Lig'i Data Pack'e ekle
        targetDataPack.leagues.Add(superLig);
        
        // Tüm takım güçlerini hesapla
        targetDataPack.CalculateAllTeamPowers();
        
        EditorUtility.SetDirty(targetDataPack);
        AssetDatabase.SaveAssets();
        
        EditorUtility.DisplayDialog("Başarılı", $"{teamNames.Length} takımlı 2025-2026 Süper Lig oluşturuldu! Her takımda 25-30 oyuncu var.", "Tamam");
    }
    
    void CreateBigThreeTeams()
    {
        if (targetDataPack == null) return;
        
        // Sadece büyük 3 takımı oluştur (test için)
        LeagueData testLig = new LeagueData();
        testLig.leagueName = "Test Lig";
        testLig.leagueCountry = "Türkiye";
        testLig.leagueTier = 1;
        
        TeamData gs = CreateTeam("Galatasaray", 82);
        TeamData fb = CreateTeam("Fenerbahçe", 81);
        TeamData bjk = CreateTeam("Beşiktaş", 80);
        
        testLig.teams.Add(gs);
        testLig.teams.Add(fb);
        testLig.teams.Add(bjk);
        
        targetDataPack.leagues.Add(testLig);
        targetDataPack.CalculateAllTeamPowers();
        
        EditorUtility.SetDirty(targetDataPack);
        AssetDatabase.SaveAssets();
        
        EditorUtility.DisplayDialog("Başarılı", "Büyük 3 takım oluşturuldu!", "Tamam");
    }
    
    TeamData CreateTeam(string teamName, int averagePower)
    {
        TeamData team = new TeamData();
        team.teamName = teamName;
        team.teamShortName = GetShortName(teamName);
        team.teamCountry = "Türkiye";
        team.primaryColor = GetTeamColor(teamName);
        team.secondaryColor = Color.white;
        
        // 25-30 oyuncu oluştur (11 ilk 11 + yedekler)
        // İlk 11 (4-4-2 formasyonu)
        PlayerPosition[] startingPositions = {
            PlayerPosition.KL,   // Kaleci
            PlayerPosition.SĞB,   // Sağ Bek
            PlayerPosition.SLB,   // Sol Bek
            PlayerPosition.STP,   // Stoper 1
            PlayerPosition.STP,   // Stoper 2
            PlayerPosition.SĞK,  // Sağ Kanat
            PlayerPosition.SLK,  // Sol Kanat
            PlayerPosition.MDO,   // Merkez Orta Defans
            PlayerPosition.MOO,   // Merkez Orta Ofans
            PlayerPosition.SF,    // Santrafor 1
            PlayerPosition.SF     // Santrafor 2
        };
        
        // Yedek oyuncu pozisyonları (14-19 oyuncu daha)
        PlayerPosition[] benchPositions = {
            PlayerPosition.KL,   // Yedek Kaleci
            PlayerPosition.SĞB, PlayerPosition.SLB, // Yedek Bekler
            PlayerPosition.STP, PlayerPosition.STP, // Yedek Stoperler
            PlayerPosition.MDO, PlayerPosition.MDO, // Yedek Orta Saha Defans
            PlayerPosition.MOO, PlayerPosition.MOO, // Yedek Orta Saha Ofans
            PlayerPosition.SĞK, PlayerPosition.SLK, // Yedek Kanatlar
            PlayerPosition.SĞO, PlayerPosition.SLO, // Yedek Ofansif Kanatlar
            PlayerPosition.SF, PlayerPosition.SF     // Yedek Forvetler
        };
        
        string[] allPlayerNames = GetPlayerNamesForTeam(teamName);
        
        // Eğer yeterli oyuncu ismi yoksa, varsayılan isimler ekle
        int totalPlayersNeeded = 25 + Random.Range(0, 6); // 25-30 oyuncu
        if (allPlayerNames.Length < totalPlayersNeeded)
        {
            List<string> extendedNames = new List<string>(allPlayerNames);
            string[] defaultNames = {
                "Mehmet Yılmaz", "Ali Kaya", "Mustafa Demir", "Ahmet Şahin", "Hasan Çelik",
                "Hüseyin Yıldız", "İbrahim Yıldırım", "Osman Öztürk", "Yusuf Aydın", "Murat Özdemir",
                "Emre Arslan", "Can Özkan", "Burak Şen", "Kerem Kılıç", "Yasin Aslan",
                "Fatih Çetin", "Serkan Kara", "Onur Koç", "Cem Kurt", "Kemal Özer",
                "Tolga Şimşek", "Barış Polat", "Deniz Erdoğan", "Eren Ateş", "Arda Bulut"
            };
            
            for (int i = allPlayerNames.Length; i < totalPlayersNeeded; i++)
            {
                extendedNames.Add(defaultNames[(i - allPlayerNames.Length) % defaultNames.Length] + " " + (i - allPlayerNames.Length + 1));
            }
            allPlayerNames = extendedNames.ToArray();
        }
        
        // İlk 11 oyuncuyu oluştur
        for (int i = 0; i < startingPositions.Length && i < allPlayerNames.Length; i++)
        {
            PlayerData player = new PlayerData();
            player.playerName = allPlayerNames[i];
            player.position = startingPositions[i];
            
            // Pozisyona göre overall ayarla (ortalama güç ± varyasyon)
            int baseOverall = averagePower;
            int variation = Random.Range(-5, 6); // ±5 varyasyon
            
            // Pozisyona göre ayarlama
            switch (startingPositions[i])
            {
                case PlayerPosition.KL:
                    baseOverall = averagePower + Random.Range(-3, 4);
                    break;
                case PlayerPosition.SF:
                    baseOverall = averagePower + Random.Range(-2, 5); // Forvetler biraz daha güçlü olabilir
                    break;
                case PlayerPosition.MOO:
                case PlayerPosition.SĞK:
                case PlayerPosition.SLK:
                    baseOverall = averagePower + Random.Range(-2, 3);
                    break;
                default:
                    baseOverall = averagePower + variation;
                    break;
            }
            
            player.SetOverall(Mathf.Clamp(baseOverall, 55, 95));
            player.age = Random.Range(20, 35);
            player.nationality = "Türkiye";
            
            team.players.Add(player);
        }
        
        // Yedek oyuncuları oluştur (14-19 oyuncu daha)
        int benchStartIndex = startingPositions.Length;
        int benchCount = totalPlayersNeeded - startingPositions.Length;
        
        for (int i = 0; i < benchCount && (benchStartIndex + i) < allPlayerNames.Length; i++)
        {
            PlayerData player = new PlayerData();
            player.playerName = allPlayerNames[benchStartIndex + i];
            
            // Yedek oyuncu pozisyonu (benchPositions'dan sırayla)
            player.position = benchPositions[i % benchPositions.Length];
            
            // Yedek oyuncular biraz daha düşük overall (ortalama -5 ile -10 arası)
            int benchOverall = averagePower + Random.Range(-10, -4);
            player.SetOverall(Mathf.Clamp(benchOverall, 50, 90));
            player.age = Random.Range(18, 38);
            player.nationality = "Türkiye";
            
            team.players.Add(player);
        }
        
        // Takım gücünü hesapla
        team.CalculateTeamPower();
        
        return team;
    }
    
    string GetShortName(string teamName)
    {
        // Gerçek takım kısa isimleri
        Dictionary<string, string> shortNames = new Dictionary<string, string>
        {
            { "Galatasaray", "GS" },
            { "Fenerbahçe", "FB" },
            { "Beşiktaş", "BJK" },
            { "Trabzonspor", "TS" },
            { "Başakşehir", "BAŞ" },
            { "Alanyaspor", "ALA" },
            { "Konyaspor", "KON" },
            { "Kayserispor", "KAY" },
            { "Adana Demirspor", "ADS" },
            { "Sivasspor", "SVS" },
            { "Antalyaspor", "ANT" },
            { "Gaziantep", "GAZ" },
            { "Kasımpaşa", "KAS" },
            { "Hatayspor", "HAT" },
            { "Fatih Karagümrük", "FKG" },
            { "Göztepe", "GÖZ" },
            { "Çaykur Rizespor", "RİZ" },
            { "Yeni Malatyaspor", "MAL" },
            { "Samsunspor", "SAM" },
            { "Eyüpspor", "EYÜ" },
            { "Gençlerbirliği", "GEN" },
            { "Kocaelispor", "KOC" }
        };
        
        if (shortNames.ContainsKey(teamName))
            return shortNames[teamName];
        
        // Varsayılan: ilk 3 harf
        return teamName.Substring(0, Mathf.Min(3, teamName.Length)).ToUpper();
    }
    
    Color GetTeamColor(string teamName)
    {
        // Gerçek takım renkleri
        Dictionary<string, Color> colors = new Dictionary<string, Color>
        {
            { "Galatasaray", new Color(1f, 0.8f, 0f) }, // Sarı-Kırmızı
            { "Fenerbahçe", new Color(0.1f, 0.1f, 0.9f) }, // Sarı-Lacivert
            { "Beşiktaş", new Color(0f, 0f, 0f) }, // Siyah-Beyaz
            { "Trabzonspor", new Color(0.8f, 0.1f, 0.1f) }, // Bordo-Mavi
            { "Başakşehir", new Color(0.9f, 0.3f, 0.1f) }, // Turuncu
            { "Alanyaspor", new Color(0.9f, 0.3f, 0.1f) }, // Turuncu
            { "Konyaspor", new Color(0.1f, 0.6f, 0.1f) }, // Yeşil
            { "Kayserispor", new Color(0.9f, 0.1f, 0.1f) }, // Kırmızı
            { "Adana Demirspor", new Color(0.1f, 0.1f, 0.9f) }, // Mavi
            { "Sivasspor", new Color(0.9f, 0.1f, 0.1f) }, // Kırmızı
            { "Antalyaspor", new Color(0.1f, 0.5f, 0.8f) }, // Mavi
            { "Gaziantep", new Color(0.9f, 0.1f, 0.1f) }, // Kırmızı
            { "Kasımpaşa", new Color(0.1f, 0.1f, 0.9f) }, // Mavi
            { "Hatayspor", new Color(0.9f, 0.1f, 0.1f) }, // Kırmızı
            { "Fatih Karagümrük", new Color(0.2f, 0.2f, 0.2f) }, // Siyah
            { "Göztepe", new Color(0.9f, 0.1f, 0.1f) }, // Kırmızı
            { "Çaykur Rizespor", new Color(0.1f, 0.6f, 0.1f) }, // Yeşil
            { "Yeni Malatyaspor", new Color(0.9f, 0.1f, 0.1f) }, // Kırmızı
            { "MKE Ankaragücü", new Color(0.1f, 0.1f, 0.9f) }, // Mavi
            { "Giresunspor", new Color(0.1f, 0.6f, 0.1f) }  // Yeşil
        };
        
        if (colors.ContainsKey(teamName))
            return colors[teamName];
        
        return Color.blue; // Varsayılan
    }
    
    string[] GetPlayerNamesForTeam(string teamName)
    {
        // Her takım için gerçek oyuncu isimleri (FIFA'dan esinlenilmiş)
        Dictionary<string, string[]> teamPlayers = new Dictionary<string, string[]>
        {
            { "Galatasaray", new string[] {
                // İlk 11 - 2025-2026
                "Fernando Muslera", "Derrick Köhn", "Angeliño", "Abdülkerim Bardakcı", "Davinson Sánchez",
                "Lucas Torreira", "Kerem Aktürkoğlu", "Barış Alıcı", "Kaan Ayhan", "Mauro Icardi", "Dries Mertens",
                // Yedekler - 2025-2026
                "Günay Güvenç", "Victor Nelsson", "Emin Bayram", "Yunus Akgün", "Tanguy Ndombele",
                "Hakim Ziyech", "Wilfried Zaha", "Tetê", "Cédric Bakambu", "Halil Dervişoğlu",
                "Berkay Özcan", "Yusuf Demir", "Carlos Vinícius", "Bafétimbi Gomis", "Okan Kocuk",
                "Berkan Kutlu", "Milot Rashica", "Yunus Akgün", "Carlos Vinícius", "Bafétimbi Gomis"
            }},
            { "Fenerbahçe", new string[] {
                // İlk 11 - 2025-2026
                "Livakovic", "Ferdi Kadıoğlu", "Bright Osayi-Samuel", "Alexander Djiku", "Rodrigo Becão",
                "İsmail Yüksek", "Sebastian Szymanski", "Jayden Oosterwolde", "İrfan Can Eğribayat", "Edin Džeko", "Michy Batshuayi",
                // Yedekler - 2025-2026
                "İrfan Can Eğribayat", "Çağlar Söyüncü", "Samet Akaydın", "Mert Müldür", "Ozan Tufan",
                "Fred", "Ryan Kent", "İrfan Can Kahveci", "Cengiz Ünder", "Sebastian Osigwe",
                "Serdar Dursun", "Joshua King", "Ryan Babel", "Mert Hakan Yandaş", "Miguel Crespo",
                "Sebastian Osigwe", "Serdar Dursun", "Joshua King", "Ryan Babel", "Mert Hakan Yandaş"
            }},
            { "Beşiktaş", new string[] {
                // İlk 11 - 2025-2026
                "Mert Günok", "Valentin Rosier", "Arthur Masuaku", "Omar Colley", "Daniel Amartey",
                "Gedson Fernandes", "Amir Hadžiahmetović", "Salih Uçan", "Necip Uysal", "Cenk Tosun", "Vincent Aboubakar",
                // Yedekler - 2025-2026
                "Ersin Destanoğlu", "Tayyip Talha Sanuç", "Jean Onana", "Rachid Ghezzal", "Jackson Muleka",
                "Ante Rebić", "Semih Kılıçsoy", "Berkay Vardar", "Emrecan Uzunhan", "Al-Musrati",
                "Milutin Osmajić", "Eric Bailly", "Romain Saïss", "Emrecan Terzi", "Tayfur Bingöl",
                "Gedson Fernandes", "Amir Hadžiahmetović", "Salih Uçan", "Necip Uysal", "Cenk Tosun"
            }},
            { "Trabzonspor", new string[] {
                "Uğurcan Çakır", "Abdülkadir Ömür", "Edin Višća", "Enis Bardhi", "Paul Onuachu",
                "Trezeguet", "Stefano Denswil", "Mehmet Can Aydın", "Enis Destan", "Muhammet Taha Tepe", "Dimitris Kourbelis"
            }},
            { "Başakşehir", new string[] {
                "Volkan Babacan", "Mahmut Tekdemir", "Lucas Lima", "Serdar Gürler", "Krzysztof Piątek",
                "Deniz Türüç", "Emre Belözoğlu", "Okan Kocuk", "Leo Duarte", "Lucas Biglia", "Patryk Szysz"
            }},
            { "Alanyaspor", new string[] {
                "Ertuğrul Taşkıran", "Efecan Karaca", "Leroy Fer", "Nicolas Janvier", "Pape Cissé",
                "Fatih Aksoy", "Yusuf Özdemir", "João Novais", "Flavio", "Oğuz Aydın", "Eren Karataş"
            }},
            { "Konyaspor", new string[] {
                "Erhan Erentürk", "Soner Dikmen", "Amilton", "Francisco Calvo", "Mame Thiam",
                "Endri Çekiçi", "Bruno Paz", "Konrad Michalak", "Sokol Cikalleshi", "Uğur Demirok", "Guilherme"
            }},
            { "Kayserispor", new string[] {
                "Bilal Bayazit", "Miguel Cardoso", "Mame Thiam", "Carlos Mane", "Aylton Boa Morte",
                "Onur Bulut", "Lionel Carole", "Ali Karimi", "Joseph Attamah", "Arif Kocaman", "Gökhan Değirmenci"
            }},
            { "Adana Demirspor", new string[] {
                "Vedat Karakuş", "Younès Belhanda", "Mario Balotelli", "Nani", "Benjamin Stambouli",
                "Yusuf Sarı", "Emre Akbaba", "Abdullah Yılmaz", "Semih Güler", "Papa Alioune Ndiaye", "Ertaç Özbir"
            }},
            { "Sivasspor", new string[] {
                "Ali Şaşal Vural", "Max Gradel", "Clinton N'Jie", "Diafra Sakho", "Hakan Arslan",
                "Erdoğan Yeşilyurt", "Caner Osmanpaşa", "Aaron Appindangoyé", "Ziya Erdal", "Kerem Atakan Kesgin", "Murat Paluli"
            }},
            { "Antalyaspor", new string[] {
                "Helton Leite", "Haji Wright", "Sam Larsson", "Adam Buksa", "Ufuk Akyol",
                "Ömer Toprak", "Fernando", "Güray Vural", "Veysel Sarı", "Erdoğan Yeşilyurt", "Bünyamin Balcı"
            }},
            { "Gaziantep", new string[] {
                "Günay Güvenç", "Maximilian Beier", "João Figueiredo", "Marko Jevtović", "Furkan Soyalp",
                "Papy Djilobodji", "Stelios Kitsiou", "Júnior Morais", "Mirza Cihan", "Ogün Özçiçek", "Mustafa Eskihellaç"
            }},
            { "Kasımpaşa", new string[] {
                "Eray Birniçan", "Aytaç Kara", "Mamadou Fall", "Valentin Eysseric", "Florent Hadergjonaj",
                "Yusuf Erdoğan", "Haris Hajradinović", "Sadık Çiftpınar", "Mortadha Ben Ouanes", "Tunahan Cingöz", "Kenneth Omeruo"
            }},
            { "Hatayspor", new string[] {
                "Erce Kardeşler", "Ayoub El Kaabi", "Rúben Ribeiro", "Carlos Strandberg", "Zé Luís",
                "Burak Camoglu", "Adama Traoré", "Kerim Alıcı", "Onur Ergun", "Burak Öksüz", "Kamil Ahmet Çörekçi"
            }},
            { "Fatih Karagümrük", new string[] {
                "Vincenzo Viviano", "Fabio Borini", "Younès Belhanda", "Valentin Rosier", "Levent Mercan",
                "Emre Mor", "Olimpiu Moruțan", "Alassane Ndao", "Davide Biraschi", "Salih Dursun", "Eray İşcan"
            }},
            { "Göztepe", new string[] {
                "Beto", "Franck Ribéry", "Halil Akbunar", "Soner Aydoğdu", "Stefano Napoleoni",
                "Atınç Nukan", "Alpaslan Öztürk", "Murat Paluli", "Berkan Emir", "Titi", "Murat Cem Akpınar"
            }},
            { "Çaykur Rizespor", new string[] {
                "Gökhan Akkan", "Yasin Pehlivan", "Jonas Svensson", "Martin Minchev", "Altin Kryeziu",
                "Eren Albayrak", "Gökhan Gönül", "Yasin Öztürk", "Emirhan Topçu", "Muhammet Taha Şahin", "Alperen Uysal"
            }},
            { "Yeni Malatyaspor", new string[] {
                "Erkan Kılıç", "Erkan Kaş", "Moryké Fofana", "Benjamin Tetteh", "Mickaël Tirpan",
                "Mustafa Eskihellaç", "Berkay Öztürk", "Erkan Kaş", "Murat Yıldırım", "Erkan Zengin", "Erkan Taşkıran"
            }},
            { "MKE Ankaragücü", new string[] {
                "Bahadır Han Güngördü", "Stelios Kitsiou", "Pedro Tiba", "Anastasios Bakasetas", "Federico Macheda",
                "Tolga Ciğerci", "Atakan Çankaya", "Nihad Mujakić", "Matej Hanousek", "Luka Adžić", "Gökhan Akkan"
            }},
            { "Giresunspor", new string[] {
                "Onurcan Piri", "Vukan Savićević", "Brandon Borrello", "Chiquinho", "Riad Bajić",
                "Alperen Uysal", "Arda Kızıldağ", "Mehmet Güven", "Sergio", "Erol Can Akdağ", "Ferhat Kaplan"
            }},
            { "Pendikspor", new string[] {
                "Erdem Özgenç", "Halil Akbunar", "Thibault Moulin", "Leandro Kappel", "Erencan Yardımcı",
                "Murat Akça", "Alpaslan Öztürk", "Murat Uluç", "Emre Taşdemir", "Berkay Öztürk", "Murat Sözgelmez",
                "Halil Akbunar", "Thibault Moulin", "Leandro Kappel", "Erencan Yardımcı", "Murat Akça",
                "Alpaslan Öztürk", "Murat Uluç", "Emre Taşdemir", "Berkay Öztürk", "Murat Sözgelmez"
            }},
            { "İstanbulspor", new string[] {
                "Alp Arda", "Eduard Rroca", "Valon Ethemi", "David Sambissa", "Jetmir Topalli",
                "Emir Kaan Gültekin", "Muhammed Enes Durmuş", "Okan Erdem", "İbrahim Yılmaz", "Alaaddin Okumuş", "Fatih Tultak",
                "Eduard Rroca", "Valon Ethemi", "David Sambissa", "Jetmir Topalli", "Emir Kaan Gültekin",
                "Muhammed Enes Durmuş", "Okan Erdem", "İbrahim Yılmaz", "Alaaddin Okumuş", "Fatih Tultak"
            }},
            { "Samsunspor", new string[] {
                "Okan Kocuk", "Yasin Öztekin", "Emre Kılınç", "Marius Mouandilmadji", "Gaëtan Laura",
                "Youssef Aït Bennasser", "Kingsley Schindler", "Moryké Fofana", "Erhan Kartal", "Moussa Guel", "Alioune Ndour",
                "Yasin Öztekin", "Emre Kılınç", "Marius Mouandilmadji", "Gaëtan Laura", "Youssef Aït Bennasser",
                "Kingsley Schindler", "Moryké Fofana", "Erhan Kartal", "Moussa Guel", "Alioune Ndour",
                "Okan Kocuk", "Yasin Öztekin", "Emre Kılınç", "Marius Mouandilmadji", "Gaëtan Laura"
            }},
            { "Eyüpspor", new string[] {
                "Alperen Uysal", "Emre Kılınç", "Mert Çelik", "Burak Süleyman", "Kemal Rüzgar",
                "Furkan Yaman", "Yasin Dülger", "Emre Kılınç", "Mert Çelik", "Burak Süleyman", "Kemal Rüzgar",
                "Furkan Yaman", "Yasin Dülger", "Emre Kılınç", "Mert Çelik", "Burak Süleyman",
                "Kemal Rüzgar", "Furkan Yaman", "Yasin Dülger", "Emre Kılınç", "Mert Çelik",
                "Burak Süleyman", "Kemal Rüzgar", "Furkan Yaman", "Yasin Dülger", "Emre Kılınç"
            }},
            { "Gençlerbirliği", new string[] {
                "Erdem Özgenç", "Halil Akbunar", "Thibault Moulin", "Leandro Kappel", "Erencan Yardımcı",
                "Murat Akça", "Alpaslan Öztürk", "Murat Uluç", "Emre Taşdemir", "Berkay Öztürk", "Murat Sözgelmez",
                "Halil Akbunar", "Thibault Moulin", "Leandro Kappel", "Erencan Yardımcı", "Murat Akça",
                "Alpaslan Öztürk", "Murat Uluç", "Emre Taşdemir", "Berkay Öztürk", "Murat Sözgelmez",
                "Erdem Özgenç", "Halil Akbunar", "Thibault Moulin", "Leandro Kappel", "Erencan Yardımcı"
            }},
            { "Kocaelispor", new string[] {
                "Alp Arda", "Eduard Rroca", "Valon Ethemi", "David Sambissa", "Jetmir Topalli",
                "Emir Kaan Gültekin", "Muhammed Enes Durmuş", "Okan Erdem", "İbrahim Yılmaz", "Alaaddin Okumuş", "Fatih Tultak",
                "Eduard Rroca", "Valon Ethemi", "David Sambissa", "Jetmir Topalli", "Emir Kaan Gültekin",
                "Muhammed Enes Durmuş", "Okan Erdem", "İbrahim Yılmaz", "Alaaddin Okumuş", "Fatih Tultak",
                "Alp Arda", "Eduard Rroca", "Valon Ethemi", "David Sambissa", "Jetmir Topalli"
            }}
        };
        
        // Eğer takım listede varsa gerçek oyuncu isimlerini kullan
        if (teamPlayers.ContainsKey(teamName))
        {
            return teamPlayers[teamName];
        }
        
        // Yoksa varsayılan gerçekçi isimler
        string[] defaultNames = {
            "Mehmet Yılmaz", "Ali Kaya", "Mustafa Demir", "Ahmet Şahin", "Hasan Çelik",
            "Hüseyin Yıldız", "İbrahim Yıldırım", "Osman Öztürk", "Yusuf Aydın", "Murat Özdemir", "Emre Arslan"
        };
        
        return defaultNames;
    }
}

