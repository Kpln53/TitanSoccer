using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Commentary System - Maç yorumları üretir
/// </summary>
public class CommentarySystem : MonoBehaviour
{
    private static CommentarySystem instance;
    public static CommentarySystem Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = new GameObject("CommentarySystem");
                instance = obj.AddComponent<CommentarySystem>();
                DontDestroyOnLoad(obj);
            }
            return instance;
        }
    }

    private float lastCommentaryTime = 0f;
    private float commentaryCooldown = 2f; // Minimum 2 saniye aralık

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    /// <summary>
    /// Yorum satırı ekle (cooldown ile)
    /// </summary>
    public void AddLine(string line)
    {
        if (Time.time - lastCommentaryTime < commentaryCooldown)
            return;

        if (MatchContext.Instance != null)
        {
            MatchContext.Instance.AddCommentary(line);
        }
        Debug.Log($"[Commentary] {line}");
        lastCommentaryTime = Time.time;
    }

    /// <summary>
    /// Zorunlu yorum ekle (cooldown yok - gol gibi önemli olaylar için)
    /// </summary>
    public void AddImportantLine(string line)
    {
        if (MatchContext.Instance != null)
        {
            MatchContext.Instance.AddCommentary(line);
        }
        Debug.Log($"[Commentary] {line}");
        lastCommentaryTime = Time.time;
    }

    /// <summary>
    /// Genel yorum şablonları
    /// </summary>
    public void AddGeneralCommentary(string playerName, string action)
    {
        string[] templates = {
            $"{playerName} {action}.",
            $"{playerName} topu {action}.",
            $"İyi bir {action} {playerName} tarafından.",
            $"{playerName} mükemmel bir {action} yaptı.",
            $"{playerName} {action} ve pozisyonu ilerletiyor.",
            $"{action} {playerName}!"
        };

        string line = templates[Random.Range(0, templates.Length)];
        AddLine(line);
    }

    /// <summary>
    /// Gol yorumu
    /// </summary>
    public void AddGoalCommentary(string scorerName)
    {
        string[] templates = {
            $"GOL! {scorerName} attı!",
            $"GOL! Harika bir vuruş {scorerName} tarafından!",
            $"GOOOOL! {scorerName} sayıyı buldu!",
            $"GOL! {scorerName} muhteşem bir gol attı!"
        };
        string line = templates[Random.Range(0, templates.Length)];
        AddImportantLine(line);
    }
}
