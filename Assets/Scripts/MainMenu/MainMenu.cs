using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Button accountButton;
    public Button reflexModeButton;
    public Button slidesModeButton;
    public Button memoryModeButton;
    private void Start()
    {
        accountButton.onClick.AddListener(LoadAccount);
        reflexModeButton.onClick.AddListener(LoadColorReflexMode);
        slidesModeButton.onClick.AddListener(LoadSlidesReflexMode);
        memoryModeButton.onClick.AddListener(LoadMemoryReflexMode);
        if (!SessionManager.Instance.IsUserLoggedIn())
        {
            accountButton.GetComponentInChildren<TextMeshProUGUI>().text = "Iniciar Sesion";
        }
        else
        {
            accountButton.GetComponentInChildren<TextMeshProUGUI>().text = "Cuenta";
        }
        
    }

    public void LoadColorReflexMode()
    {
        SceneManager.LoadScene("ColorReflexMode");
    }

    public void LoadSlidesReflexMode()
    {
        SceneManager.LoadScene("SlidesReflexMode");
    }

    public void LoadMemoryReflexMode()
    {
        SceneManager.LoadScene("MemoryReflexMode");
    }

    public void LoadAccount()
    {
        if (!SessionManager.Instance.IsUserLoggedIn())
        {
            SceneManager.LoadScene("Login");
        }
        else
        {
            SceneManager.LoadScene("Account"); 
        }
        
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    
    
}
