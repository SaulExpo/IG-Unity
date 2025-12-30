using Firebase;
using Firebase.Auth;
using Firebase.Database;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;


public class Login : MonoBehaviour
{
    public TMP_InputField emailInput, passwordInput;
    public TextMeshProUGUI statusText;

    private FirebaseAuth auth;
    private DatabaseReference dbRef;
    public Button menuButton; 
    public Button loginButton;
    public Button RegisterButton;


    async void Start()
    {
        statusText.gameObject.SetActive(false); 
        RegisterButton.onClick.AddListener(GoToRegister);
        menuButton.onClick.AddListener(GoToMenu);

        var status = await FirebaseApp.CheckAndFixDependenciesAsync();

        if (status == DependencyStatus.Available)
        {
            auth = FirebaseAuth.DefaultInstance;

            dbRef = FirebaseDatabase.GetInstance("https://jueguito-68a46-default-rtdb.europe-west1.firebasedatabase.app/").RootReference;

            loginButton.onClick.AddListener(LoginUser);
            Debug.Log("Firebase inicializado con éxito");
        }
        else
        {
            Debug.LogError("Firebase no disponible: " + status);
        }
    }
    
    public void LoginUser()
    {
        string email = emailInput.text;
        string password = passwordInput.text;
        statusText.text = "";

        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsCompleted && !task.IsFaulted)
            {
                UnityMainThreadDispatcher.Enqueue(() =>
                {
                    FirebaseUser user = auth.CurrentUser;

                    if (user != null && user.IsEmailVerified)
                    {
                        SceneManager.LoadScene("MainMenu");
                    }
                    else
                    {
                        statusText.gameObject.SetActive(true);
                        statusText.text = "Por favor, verifica tu correo electrónico antes de iniciar sesión.";
                        auth.SignOut();
                    }
                });
            }
            else
            {
                UnityMainThreadDispatcher.Enqueue(() =>
                {
                    statusText.gameObject.SetActive(true);
                    foreach (var ex in task.Exception.Flatten().InnerExceptions)
                    {
                        Debug.Log(ex);
                        if (ex is FirebaseException firebaseEx)
                        {
                            var authError = (AuthError)firebaseEx.ErrorCode;

                            string mensaje = GetMessageAuthError(authError);
                            Debug.LogError("Código de error: " + authError);
                            statusText.text = mensaje;
                        }
                        else
                        {
                            statusText.text = "Error desconocido: " + ex.Message;
                        }
                    }
                });
            }
        });
    }
    
    private string GetMessageAuthError(AuthError errorCode)
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

    private void GoToRegister()
    {
        SceneManager.LoadScene("Register");
    }
}
