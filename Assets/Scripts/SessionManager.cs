using Firebase.Auth;
using UnityEngine;

public class SessionManager : MonoBehaviour
{
    public static SessionManager Instance;
    public FirebaseUser CurrentUser { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            CurrentUser = FirebaseAuth.DefaultInstance.CurrentUser;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool IsUserLoggedIn()
    {
        return FirebaseAuth.DefaultInstance.CurrentUser != null;
    }
}