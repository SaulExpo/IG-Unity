using System;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;


public class Register : MonoBehaviour
{
    public TMP_InputField emailInput, passwordInput, usernameInput;
    public TextMeshProUGUI statusText;

    private FirebaseAuth auth;
    private DatabaseReference dbRef;
    public Button menuButton;  // Referencia al botón de menu
    public Button loginButton;
    public Button RegisterButton;
    public Button CloseButton;
    public GameObject panel; // Asigna este en el Inspector
    public Vector3 targetScale = new Vector3(0, 0, 0); // La escala final del panel (se reducirá)
    public float animationTime = 1f; // Duración de la animación


    async void Start()
    {
        panel.transform.localScale = Vector3.zero;
        statusText.gameObject.SetActive(false); // Oculto al arrancar
        loginButton.onClick.AddListener(GoToLogIn);
        CloseButton.onClick.AddListener(ClosePanel);
        menuButton.onClick.AddListener(GoToMenu);

        var status = await FirebaseApp.CheckAndFixDependenciesAsync();
        Debug.Log("Firebase dependency status: " + status);
        if (status == DependencyStatus.Available)
        {
            auth = FirebaseAuth.DefaultInstance;

            dbRef = FirebaseDatabase.GetInstance("https://jueguito-68a46-default-rtdb.europe-west1.firebasedatabase.app/").RootReference;

            RegisterButton.onClick.AddListener(RegisterUser);

            Debug.Log("Firebase inicializado con éxito");
        }
        else
        {
            Debug.LogError("Firebase no disponible: " + status);
        }

    }

    public void RegisterUser()
    {
        string email = emailInput.text;
        string password = passwordInput.text;
        string username = usernameInput.text;
        statusText.text = "";
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsCompleted && !task.IsFaulted && username != "")
            {
                // Esto se ejecuta fuera del hilo principal, por lo que usamos el dispatcher
                UnityMainThreadDispatcher.Enqueue(() =>
                {
                    AuthResult result = task.Result;
                    FirebaseUser user = result.User;

                    dbRef.Child("users").Child(user.UserId).Child("username").SetValueAsync(username);
                    dbRef.Child("users").Child(user.UserId).Child("highscoreColors").SetValueAsync(0);
                    dbRef.Child("users").Child(user.UserId).Child("highscoreSlides").SetValueAsync(0);
                    dbRef.Child("users").Child(user.UserId).Child("highscoreMemory").SetValueAsync(0);
                
                    user.SendEmailVerificationAsync();
                    
                    OpenPanel();
                });
            }
            else
            {
                UnityMainThreadDispatcher.Enqueue(() =>
                {
                    statusText.gameObject.SetActive(true);
                    if (username == "") {
                        statusText.text = "Ingresa un nombre de usuario";
                    }
                    else
                    {
                        foreach (var ex in task.Exception.Flatten().InnerExceptions)
                        {
                            Debug.Log(ex);
                            if (ex is FirebaseException firebaseEx)
                            {
                                // Intenta obtener el código de error
                                var authError = (AuthError)firebaseEx.ErrorCode;

                                string mensaje = ObtenerMensajeDeAuthError(authError);
                                Debug.LogError("Código de error: " + authError);
                                statusText.text = mensaje;
                            }
                            else
                            {
                                // Otros tipos de errores
                                statusText.text = "Error desconocido: " + ex.Message;
                            }
                        }
                    }
                });
            }
        });
    }
    
    private string ObtenerMensajeDeAuthError(AuthError errorCode)
    {
        switch (errorCode)
        {
            case AuthError.InvalidEmail:
                return "Correo electrónico inválido.";
            case AuthError.MissingEmail:
                return "Ingresa un correo.";
            case AuthError.WrongPassword:
                return "Contraseña incorrecta.";
            case AuthError.MissingPassword:
                return "Ingresa una contraseña.";
            case AuthError.EmailAlreadyInUse:
                return "Este correo ya está en uso.";
            case AuthError.WeakPassword:
                return "La contraseña es demasiado débil (mínimo 6 caracteres).";
            case AuthError.UserNotFound:
                return "Usuario no encontrado.";
            default:
                return "Error de autenticación.";
        }
    }

    private void GoToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void GoToLogIn()
    {
        SceneManager.LoadScene("Login");
    }

    private void OpenPanel()
    {
        LeanTween.scale(panel, Vector3.one, animationTime)
            .setEase(LeanTweenType.easeOutCubic);
    }

    private void ClosePanel()
    {
        LeanTween.scale(panel, Vector3.zero, animationTime)
            .setEase(LeanTweenType.easeInBack).setOnComplete(() => {
                SceneManager.LoadScene("Login");
            });
    }


}
