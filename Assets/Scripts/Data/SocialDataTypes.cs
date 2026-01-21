using System.Collections.Generic;
using UnityEngine;
using TitanSoccer.Social.Data;

namespace TitanSoccer.Data
{
    [System.Serializable]
    public class SocialMediaProfile
    {
         public int followers;
         public string handle;
         public string bio;

         public SocialMediaProfile()
         {
             followers = 1500;
             handle = "@Player";
             bio = "Pro Player";
         }
    }

    [System.Serializable]
    public class SocialMediaFeed
    {
        public List<SocialPost> posts;
        public SocialMediaFeed()
        {
            posts = new List<SocialPost>();
        }
    }
}
