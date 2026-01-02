using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Boot Manager - Oyun başlangıç ekranı yöneticisi
/// </summary>
public class BootManager : MonoBehaviour
{
    [Header("Ayarlar")]
    [Tooltip("Boot ekranında beklenecek süre (saniye). 0 ise otomatik geçiş yapılmaz.")]
    public float delayBeforeMainMenu = 3f;
    
    [Tooltip("Herhangi bir tuşa basınca geçiş yapılsın mı?")]
    public bool skipOnAnyKey = true;
    
    [Header("Geçiş")]
    [Tooltip("Boot sonrası yüklenecek scene adı")]
    public string mainMenuSceneName = "MainMenu";

    private bool hasSkipped = false;

    private void Start()
    {
        Debug.Log("[BootManager] Boot screen started");
        
        // Eğer süre belirtilmişse, otomatik geçiş başlat
        if (delayBeforeMainMenu > 0f)
        {
            StartCoroutine(AutoTransition());
        }
    }

    private void Update()
    {
        // Herhangi bir tuşa basıldığında geçiş yap
        if (skipOnAnyKey && !hasSkipped)
        {
            if (Input.anyKeyDown)
            {
                SkipBoot();
            }
        }
    }

    /// <summary>
    /// Otomatik geçiş coroutine'i
    /// </summary>
    private IEnumerator AutoTransition()
    {
        yield return new WaitForSeconds(delayBeforeMainMenu);
        
        if (!hasSkipped)
        {
            TransitionToNextScene();
        }
    }

    /// <summary>
    /// Boot ekranını atla
    /// </summary>
    public void SkipBoot()
    {
        if (hasSkipped) return;
        
        hasSkipped = true;
        Debug.Log("[BootManager] Boot skipped by user");
        TransitionToNextScene();
    }

    /// <summary>
    /// Sonraki scene'e geçiş yap
    /// </summary>
    private void TransitionToNextScene()
    {
        Debug.Log($"[BootManager] Transitioning to {mainMenuSceneName}");
        
        // GameStateManager varsa kullan
        if (GameStateManager.Instance != null)
        {
            // Scene adına göre state belirle
            if (mainMenuSceneName == "MainMenu")
            {
                GameStateManager.Instance.ChangeState(GameState.MainMenu);
            }
            else
            {
                // Direkt scene yükle
                SceneManager.LoadScene(mainMenuSceneName);
            }
        }
        else
        {
            // GameStateManager yoksa direkt scene yükle
            SceneManager.LoadScene(mainMenuSceneName);
        }
    }
}
