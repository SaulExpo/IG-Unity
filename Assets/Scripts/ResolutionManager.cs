using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResolutionManager : MonoBehaviour
{

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        //SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        foreach (CanvasScaler scaler in FindObjectsOfType<CanvasScaler>())
        {
            if (!Application.isMobilePlatform)
            {
                Screen.SetResolution(1280, 720, false); 
                if (scaler != null)
                {
                    scaler.referenceResolution = new Vector2(2000, 3000);
                }
            }
        }
    }
}