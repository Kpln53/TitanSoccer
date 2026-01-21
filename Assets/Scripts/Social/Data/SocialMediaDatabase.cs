using System.Collections.Generic;
using UnityEngine;

namespace TitanSoccer.Social.Data
{
    [CreateAssetMenu(fileName = "SocialMediaDatabase", menuName = "TitanSoccer/Social/Database")]
    public class SocialMediaDatabase : ScriptableObject
    {
        [Header("Post Templates")]
        public List<string> WinPostsPositive;
        public List<string> WinPostsHumble;
        public List<string> LossPostsSad;
        public List<string> LossPostsMotivated;
        public List<string> GoalPosts;
        public List<string> TrainingPosts;

        [Header("Fan Comments")]
        public List<string> PositiveComments;
        public List<string> NegativeComments;
        public List<string> NeutralComments;

        [Header("Media News")]
        public List<string> TransferRumors;
        public List<string> MatchHeadlines;

        public string GetRandomTemplate(List<string> source)
        {
            if (source == null || source.Count == 0) return "Content not found.";
            return source[Random.Range(0, source.Count)];
        }
    }
}
