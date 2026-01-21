using System;
using System.Collections.Generic;
using UnityEngine;
using TitanSoccer.Social.Data;

namespace TitanSoccer.Social
{
    public class SocialMediaManager : MonoBehaviour
    {
        public static SocialMediaManager Instance { get; private set; }

        [Header("Settings")]
        public SocialMediaDatabase database;
        public int initialFollowers = 1500;

        [Header("State")]
        public List<SocialPost> Feed = new List<SocialPost>();
        public int CurrentFollowers;
        public MatchData PendingMatchContext { get; private set; } // Son oynanan maçın verisi
        
        // Events
        public event Action<SocialPost> OnPostAdded;
        public event Action<int> OnFollowerCountChanged;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            CurrentFollowers = initialFollowers;
            OnFollowerCountChanged?.Invoke(CurrentFollowers);
            
            // Test için başlangıç postları
            AddPost(new SocialPost("Club Admin", "@TitansOfficial", "Welcome to the new season! We have big goals this year. #TitanSoccer", PostType.Club, PostCategory.General));
            AddPost(new SocialPost("Transfer News", "@SoccerDaily", "BREAKING: Titans FC looking to strengthen their midfield in the upcoming window.", PostType.News, PostCategory.Transfer));
            AddPost(new SocialPost("Fan01", "@SuperFan_99", "Can't wait for the first match! The team looks ready. 🔥", PostType.Fan, PostCategory.General));
            AddPost(new SocialPost("Coach", "@HeadCoach", "Training week has been intense. The lads are sharp.", PostType.Player, PostCategory.Training));
            AddPost(new SocialPost("Sports Analyst", "@TacticalView", "Titans FC's new formation shows promise, but defense needs to hold up.", PostType.News, PostCategory.General));
        }

        public void SetPendingMatchContext(MatchData matchData)
        {
            PendingMatchContext = matchData;
            Debug.Log($"[SocialMedia] Match context set: {matchData.homeTeamName} vs {matchData.awayTeamName}");
        }

        public void ClearPendingMatchContext()
        {
            PendingMatchContext = null;
        }

        public void AddPost(SocialPost post)
        {
            Feed.Insert(0, post); // En başa ekle (yeni post üstte)
            
            // Etkileşim simülasyonunu başlat
            SimulateEngagement(post);

            OnPostAdded?.Invoke(post);
        }

        public void AddFollowers(int amount)
        {
            CurrentFollowers += amount;
            OnFollowerCountChanged?.Invoke(CurrentFollowers);
        }

        private void SimulateEngagement(SocialPost post)
        {
            // Basit bir başlangıç simülasyonu
            // İleride EngagementCalculator sınıfına taşınacak
            int baseLikes = (int)(CurrentFollowers * 0.05f); // %5 etkileşim
            int randomFactor = UnityEngine.Random.Range(0, (int)(baseLikes * 0.5f));
            
            post.Likes = baseLikes + randomFactor;
            post.Comments = (int)(post.Likes * 0.1f);
            post.Shares = (int)(post.Likes * 0.05f);
        }
    }
}
