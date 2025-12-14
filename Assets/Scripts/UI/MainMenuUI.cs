using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public void OnPlayButton()
    {
        Debug.Log("Oyna tu�una bas�ld�"); // test i�in
        SceneManager.LoadScene("SaveSlots");
    }

    public void OnQuitButton()
    {
        Application.Quit();
    }
    
    public void OnDataPackButton()
    {
        Debug.Log("Data Pack butonuna basıldı");
        SceneManager.LoadScene("DataPackMenu");
    }
}
