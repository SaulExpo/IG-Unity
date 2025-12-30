using Firebase;
using Firebase.Auth;
using Firebase.Database;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;


public class Account : MonoBehaviour
{
    public TMP_InputField emailInput, usernameInput;

    private FirebaseAuth auth;
    private DatabaseReference dbRef;
    public Button menuButton; 
    public Button logOutButton;
    public FirebaseUser user;


    async void Start()
    {
        logOutButton.onClick.AddListener(LogOut);
        menuButton.onClick.AddListener(GoToMenu);

        var status = await FirebaseApp.CheckAndFixDependenciesAsync();
        user = FirebaseAuth.DefaultInstance.CurrentUser;

        if (status == DependencyStatus.Available)
        {
            auth = FirebaseAuth.DefaultInstance;

            dbRef = FirebaseDatabase.GetInstance("https://jueguito-68a46-default-rtdb.europe-west1.firebasedatabase.app/").RootReference;
            
            Debug.Log("Firebase inicializado");
            
            GetUsername();
        }
        else
        {
            Debug.LogError("Firebase no disponible: " + status);
        }
    }
    
    void GetUsername()
    {
        dbRef.Child("users").Child(user.UserId).Child("username").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted && !task.IsFaulted)
            {
                DataSnapshot snapshot = task.Result;
                string username = snapshot.Value.ToString();
                UnityMainThreadDispatcher.Enqueue(() =>
                {
                    usernameInput.text = username;
                    usernameInput.interactable = false;
                    emailInput.text = user.Email;
                    emailInput.interactable = false;
                });
            }
            else
            {
                Debug.LogError("Error al obtener el nombre de usuario: " + task.Exception);
            }
        });
    }
    
    private void GoToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void LogOut()
    {
        FirebaseAuth.DefaultInstance.SignOut();
        SceneManager.LoadScene("MainMenu");
    }
}
