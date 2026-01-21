using UnityEngine;
using TMPro;
using UnityEngine.UI;
using TitanSoccer.Social.Data;

namespace TitanSoccer.UI.Social
{
    public class SocialPostUI : MonoBehaviour
    {
        [Header("UI Elements")]
        public Image avatarImage;
        public TextMeshProUGUI authorNameText;
        public TextMeshProUGUI handleText;
        public TextMeshProUGUI contentText;
        public TextMeshProUGUI timeText;
        
        [Header("Stats")]
        public TextMeshProUGUI likesText;
        public TextMeshProUGUI commentsText;
        public Button likeButton;

        private SocialPost _post;

        public void Setup(SocialPost post)
        {
            _post = post;
            
            if (authorNameText) authorNameText.text = post.AuthorName;
            if (handleText) handleText.text = post.Handle;
            if (contentText) contentText.text = post.Content;
            if (timeText) timeText.text = post.FormattedTime;
            
            UpdateStats();
            
            // Avatar atama mantığı ileride eklenebilir
            // if (post.Avatar != null) avatarImage.sprite = post.Avatar;
        }

        public void UpdateStats()
        {
            if (_post == null) return;
            
            if (likesText) likesText.text = FormatNumber(_post.Likes);
            if (commentsText) commentsText.text = FormatNumber(_post.Comments);
        }

        private string FormatNumber(int num)
        {
            if (num >= 1000000) return (num / 1000000f).ToString("0.0") + "M";
            if (num >= 1000) return (num / 1000f).ToString("0.0") + "K";
            return num.ToString();
        }
    }
}
