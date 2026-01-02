using UnityEngine.SceneManagement;

/// <summary>
/// Scene Flow - Scene geçişlerini yönetir
/// </summary>
public static class SceneFlow
{
    public static void LoadPreMatch()
    {
        SceneManager.LoadScene("PreMatch");
    }

    public static void LoadMatchSim()
    {
        SceneManager.LoadScene("MatchSim");
    }

    public static void LoadChanceGameplay()
    {
        SceneManager.LoadScene("MatchChanceGameplay");
    }

    public static void LoadPostMatch()
    {
        SceneManager.LoadScene("PostMatch");
    }

    public static void LoadMainMenu()
    {
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.ChangeState(GameState.MainMenu);
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    public static void LoadCareerHub()
    {
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.ChangeState(GameState.CareerHub);
        }
        else
        {
            SceneManager.LoadScene("CareerHub");
        }
    }
}

