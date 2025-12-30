using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    public string sceneToStopMusic1 = "EscenaSinMusica";
    public string sceneToStopMusic2 = "EscenaSinMusica";
    public string sceneToStopMusic3 = "EscenaSinMusica";
    public string sceneToStopMusic4 = "EscenaSinMusica";

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == sceneToStopMusic1 || scene.name == sceneToStopMusic2 || scene.name == sceneToStopMusic3 || scene.name == sceneToStopMusic4)
        {
            Destroy(gameObject); 
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}