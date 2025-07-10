using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    // Nombre exacto de la escena donde quieres que se destruya la música
    public string sceneToStopMusic1 = "EscenaSinMusica";
    public string sceneToStopMusic2 = "EscenaSinMusica";
    public string sceneToStopMusic3 = "EscenaSinMusica";

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            // Suscribirse al evento cuando cambias de escena
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == sceneToStopMusic1 || scene.name == sceneToStopMusic2 || scene.name == sceneToStopMusic3)
        {
            Destroy(gameObject); // Destruye el MusicManager
        }
    }

    private void OnDestroy()
    {
        // Desuscribirse para evitar errores
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}