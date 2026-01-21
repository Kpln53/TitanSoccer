using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TitanSoccer.Social.Systems;
using TitanSoccer.Social.Data;
using TitanSoccer.Social;

namespace TitanSoccer.UI.Social
{
    public class CreatePostPopupUI : MonoBehaviour
    {
        [Header("UI Elements")]
        public Transform optionsContainer;
        public Button optionButtonPrefab; // TextMeshPro içeren buton
        public Button closeButton;

        private void Start()
        {
            if (closeButton) closeButton.onClick.AddListener(Close);
        }

        public void ShowPostOptions()
        {
            MatchData context = SocialMediaManager.Instance.PendingMatchContext;
            if (context == null)
            {
                Debug.LogWarning("No pending match context found for post creation.");
                return;
            }

            gameObject.SetActive(true);
            
            // Önceki seçenekleri temizle
            foreach (Transform child in optionsContainer)
            {
                Destroy(child.gameObject);
            }

            // Yeni seçenekleri al
            List<SocialPost> options = PostGenerator.GeneratePostOptions(context);

            // Butonları oluştur
            foreach (var post in options)
            {
                Button btn = Instantiate(optionButtonPrefab, optionsContainer);
                var textComp = btn.GetComponentInChildren<TextMeshProUGUI>();
                if (textComp) textComp.text = post.Content;

                btn.onClick.AddListener(() => OnOptionSelected(post));
            }
        }

        private void OnOptionSelected(SocialPost post)
        {
            SocialMediaManager.Instance.AddPost(post);
            SocialMediaManager.Instance.AddFollowers(Random.Range(50, 200)); // Basit ödül
            SocialMediaManager.Instance.ClearPendingMatchContext(); // Post atıldı, context'i temizle
            
            Close();
            
            // Feed'i yenilemesini tetikleyebiliriz gerekirse
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }
    }
}
