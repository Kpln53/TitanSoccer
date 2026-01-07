using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using UnityEditor.SceneManagement;

/// <summary>
/// PostMatch sahnesindeki UI referanslarını otomatik bağlar
/// </summary>
public class PostMatchUISetup : Editor
{
    [MenuItem("TitanSoccer/Setup/Setup PostMatch UI References")]
    public static void SetupPostMatchUI()
    {
        // PostMatch sahnesini aç
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/PostMatch.unity");
        
        // PostMatchController'ı bul
        PostMatchUIController controller = Object.FindObjectOfType<PostMatchUIController>();
        if (controller == null)
        {
            Debug.LogError("[PostMatchUISetup] PostMatchUIController bulunamadı!");
            return;
        }

        // Canvas'ı bul
        Canvas canvas = Object.FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("[PostMatchUISetup] Canvas bulunamadı!");
            return;
        }

        // UI elementlerini bul ve bağla
        Transform canvasT = canvas.transform;

        // Score Text
        Transform scoreT = canvasT.Find("FinalScoreText ");
        if (scoreT != null)
            controller.scoreText = scoreT.GetComponent<TextMeshProUGUI>();

        // Match Result Text
        Transform resultT = canvasT.Find("MatchResultText");
        if (resultT != null)
            controller.matchResultText = resultT.GetComponent<TextMeshProUGUI>();

        // Home Squad Text
        Transform homeSquadT = canvasT.Find("HomeSquadRatingsText");
        if (homeSquadT != null)
            controller.homeSquadText = homeSquadT.GetComponent<TextMeshProUGUI>();

        // Away Squad Text
        Transform awaySquadT = canvasT.Find("AwaySquadRatingsText");
        if (awaySquadT != null)
            controller.awaySquadText = awaySquadT.GetComponent<TextMeshProUGUI>();

        // Player Name Text
        Transform playerNameT = canvasT.Find("PlayerNameText");
        if (playerNameT != null)
            controller.playerNameText = playerNameT.GetComponent<TextMeshProUGUI>();

        // Player Rating Text
        Transform playerRatingT = canvasT.Find("PlayerRatingText");
        if (playerRatingT != null)
            controller.playerRatingText = playerRatingT.GetComponent<TextMeshProUGUI>();

        // Player Stats Text (Goals, Assists, Shots - mevcut yapıya göre)
        Transform playerStatsT = canvasT.Find("PlayerStatsText");
        if (playerStatsT != null)
        {
            // Eğer tek bir text varsa, onu goals için kullan
            controller.playerGoalsText = playerStatsT.GetComponent<TextMeshProUGUI>();
        }

        // Other Matches Panel içindekiler
        Transform otherMatchesPanel = canvasT.Find("OtherMatchesPanel");
        if (otherMatchesPanel != null)
        {
            Transform titleT = otherMatchesPanel.Find("OtherMatchesTitle");
            if (titleT != null)
                controller.otherMatchesTitle = titleT.GetComponent<TextMeshProUGUI>();

            Transform textT = otherMatchesPanel.Find("OtherMatchesText");
            if (textT != null)
                controller.otherMatchesText = textT.GetComponent<TextMeshProUGUI>();
        }

        // Buttons
        Transform continueT = canvasT.Find("ContinueButton");
        if (continueT != null)
            controller.continueButton = continueT.GetComponent<Button>();

        Transform interviewT = canvasT.Find("InterviewButton");
        if (interviewT != null)
            controller.interviewButton = interviewT.GetComponent<Button>();

        // Değişiklikleri kaydet
        EditorUtility.SetDirty(controller);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);

        Debug.Log("[PostMatchUISetup] PostMatch UI referansları başarıyla bağlandı!");
    }
}
