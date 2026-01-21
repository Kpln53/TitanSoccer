using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TitanSoccer.Social;
using System;

public class SocialPostUI : MonoBehaviour
{
    [Header("References")]
    public Image avatarImage;
    public TextMeshProUGUI authorNameText;
    public TextMeshProUGUI handleText; // @username
    public TextMeshProUGUI contentText;
    public Image postImage; // Opsiyonel
    public TextMeshProUGUI likesText;
    public TextMeshProUGUI commentsCountText;
    public TextMeshProUGUI timeText;
    
    public Button likeButton;
    public Button commentButton;

    private SocialPostData currentData;
    private Action<SocialPostData> onCommentClick;

    public void Setup(SocialPostData data, Action<SocialPostData> commentCallback)
    {
        currentData = data;
        onCommentClick = commentCallback;

        if (authorNameText) authorNameText.text = data.authorName;
        if (handleText) handleText.text = data.handle;
        if (contentText) contentText.text = data.content;
        if (timeText) timeText.text = data.timeAgo;
        
        if (likesText) likesText.text = $"{data.likes} Like";
        if (commentsCountText) commentsCountText.text = $"{data.commentsCount} Comment";

        if (data.image != null && postImage != null)
        {
            postImage.gameObject.SetActive(true);
            postImage.sprite = data.image;
        }
        else if (postImage != null)
        {
            postImage.gameObject.SetActive(false);
        }

        // Avatar (Placeholder logic)
        if (avatarImage)
        {
            // Gerçekte data.avatar kullanılır
            // avatarImage.sprite = data.avatar; 
        }

        if (commentButton)
        {
            commentButton.onClick.RemoveAllListeners();
            commentButton.onClick.AddListener(() => onCommentClick?.Invoke(currentData));
        }
    }
}
