using UnityEngine;
using UnityEditor;

/// <summary>
/// SimpleNewsTestUI için yardımcı araçlar
/// </summary>
public class SimpleNewsTestHelper
{
    [MenuItem("TitanSoccer/News/🚀 Test Simple News UI")]
    public static void TestSimpleNewsUI()
    {
        var testUI = GameObject.FindObjectOfType<SimpleNewsTestUI>();
        if (testUI != null)
        {
            testUI.CreateTestNews();
            Debug.Log("✅ SimpleNewsTestUI.CreateTestNews() çağrıldı");
        }
        else
        {
            Debug.LogWarning("SimpleNewsTestUI bulunamadı!");
        }
    }
    
    [MenuItem("TitanSoccer/News/🎲 Generate Random News")]
    public static void GenerateRandomNews()
    {
        var testUI = GameObject.FindObjectOfType<SimpleNewsTestUI>();
        if (testUI != null)
        {
            testUI.GenerateRandomNews();
            Debug.Log("✅ SimpleNewsTestUI.GenerateRandomNews() çağrıldı");
        }
        else
        {
            Debug.LogWarning("SimpleNewsTestUI bulunamadı!");
        }
    }
    
    [MenuItem("TitanSoccer/News/🗑️ Clear Test News")]
    public static void ClearTestNews()
    {
        var testUI = GameObject.FindObjectOfType<SimpleNewsTestUI>();
        if (testUI != null)
        {
            testUI.ClearAllNews();
            Debug.Log("✅ SimpleNewsTestUI.ClearAllNews() çağrıldı");
        }
        else
        {
            Debug.LogWarning("SimpleNewsTestUI bulunamadı!");
        }
    }
}