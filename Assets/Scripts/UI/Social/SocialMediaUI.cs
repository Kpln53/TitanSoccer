using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TitanSoccer.Social;
using TitanSoccer.Social.Data;

namespace TitanSoccer.UI.Social
{
    public class SocialMediaUI : MonoBehaviour
    {
        [Header("References")]
        public Transform feedContentParent;
        public SocialPostUI postPrefab;
        
        [Header("Header Stats")]
        public TMPro.TextMeshProUGUI followerCountText;

        [Header("Controls")]
        public Button createPostButton;
        public CreatePostPopupUI createPostPopup;

        private List<SocialPostUI> _activePosts = new List<SocialPostUI>();

        private void Start()
        {
            if (createPostButton)
                createPostButton.onClick.AddListener(OnCreatePostClicked);
        }

        private void Update()
        {
            // Buton durumunu context'e göre güncelle (performans için event driven yapılabilir ama şimdilik Update'de check)
            if (createPostButton)
            {
                bool hasContext = SocialMediaManager.Instance != null && SocialMediaManager.Instance.PendingMatchContext != null;
                createPostButton.interactable = hasContext;
            }
        }

        private void OnEnable()
        {
            if (SocialMediaManager.Instance != null)
            {
                SocialMediaManager.Instance.OnPostAdded += HandleNewPost;
                SocialMediaManager.Instance.OnFollowerCountChanged += UpdateFollowerCount;
                
                RefreshFeed();
                UpdateFollowerCount(SocialMediaManager.Instance.CurrentFollowers);
            }
        }

        private void OnCreatePostClicked()
        {
            if (createPostPopup != null)
            {
                createPostPopup.ShowPostOptions();
            }
        }

        private void OnDisable()
        {
            if (SocialMediaManager.Instance != null)
            {
                SocialMediaManager.Instance.OnPostAdded -= HandleNewPost;
                SocialMediaManager.Instance.OnFollowerCountChanged -= UpdateFollowerCount;
            }
        }

        private void RefreshFeed()
        {
            // Temizle
            foreach (Transform child in feedContentParent)
            {
                Destroy(child.gameObject);
            }
            _activePosts.Clear();

            // Yeniden Doldur
            var feed = SocialMediaManager.Instance.Feed;
            foreach (var post in feed)
            {
                CreatePostObject(post);
            }
        }

        private void HandleNewPost(SocialPost post)
        {
            // Listenin başına eklemek için SetSiblingIndex kullanılabilir veya tamamen yenilenebilir
            // Performans için şimdilik simple instantiate
            var ui = CreatePostObject(post);
            ui.transform.SetAsFirstSibling();
        }

        private SocialPostUI CreatePostObject(SocialPost post)
        {
            SocialPostUI uiInfo = Instantiate(postPrefab, feedContentParent);
            uiInfo.Setup(post);
            _activePosts.Add(uiInfo);
            return uiInfo;
        }

        private void UpdateFollowerCount(int count)
        {
            if (followerCountText)
            {
                followerCountText.text = count.ToString("N0") + " Followers";
            }
        }
    }
}
