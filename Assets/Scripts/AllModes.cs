using System;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class AllModes : MonoBehaviour
{
        public FirebaseAuth auth;
        public DatabaseReference dbRef;
        public GameObject scoresPanel;
        public GameObject buttonsPanel;
        public Button restartButton;
        public Button scoreButton;
        public Button closeScoreButton;
        public TextMeshProUGUI scoresText;
        public string scoreName;
        public Button startButton;
        public Button menuButton;
        public TextMeshProUGUI startText;
        public TextMeshProUGUI scoreText;
        public TextMeshProUGUI timerText;
        
        public float score;
        public float timer;
        public int round;
        public bool started;
        
        public AudioSource musicSource;

        protected virtual void Start()
        {
            StartDataBase();
            ShowStartElements();
            HideGameElements();
            AddListeners();
            scoresPanel.transform.localScale = Vector3.zero;
            scoreButton.gameObject.SetActive(false);  // Esconde el botón de reiniciar si existe
            SetSceneName();
        }

        protected virtual void Update()
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;
                timerText.text = "Tiempo: " + Mathf.RoundToInt(timer).ToString();
            }
            else
            {
                // Si el temporizador llega a 0, asegúrate de que no baje más
                timer = 0;
                timerText.text = "Tiempo: 0";

                // Deshabilitar los botones cuando se acaba el tiempo
                started = false;

                if (SessionManager.Instance.IsUserLoggedIn()) {
                    SubmitScore(score);
                }
                restartButton.gameObject.SetActive(true);
                if (SessionManager.Instance.IsUserLoggedIn()) {
                    scoreButton.gameObject.SetActive(true);
                }
                menuButton.gameObject.SetActive(true);
            }
        }

        public virtual void StartGame()
        {
            musicSource.Play();
            started = true;
            score = 0;
            timer = 5f;
            round = 0;
            HideStartElements();
            ShowGameElements();
        }
        public virtual void RestartGame()
        {
            // Reiniciar la puntuación y el tiempo
            score = 0;
            timer = 5f;
            round = 0;
            scoreText.text = "Puntos: " + score;
            timerText.text = "Tiempo: " + Mathf.RoundToInt(timer).ToString();
            started = true;
            HideRestartElements();
        }

        public virtual void StartNewRound()
        {
            
        }

        public void SetSceneName()
        {
            if (SceneManager.GetActiveScene().name == "ColorReflexMode")
            {
                scoreName = "highscoreColors";
            } else if (SceneManager.GetActiveScene().name == "SlidesReflexMode")
            {
                scoreName = "highscoreSlides";
            } else if (SceneManager.GetActiveScene().name == "MemoryReflexMode")
            {
                scoreName = "highscoreMemory";
            }else if (SceneManager.GetActiveScene().name == "FastReflexMode")
            {
                scoreName = "highscoreReflex";
            }
        }
        
        public async void StartDataBase(){
            var status = await FirebaseApp.CheckAndFixDependenciesAsync();
            Debug.Log("Firebase dependency status: " + status);

            if (status == DependencyStatus.Available)
            {
                auth = FirebaseAuth.DefaultInstance;

                dbRef = FirebaseDatabase.GetInstance("https://jueguito-68a46-default-rtdb.europe-west1.firebasedatabase.app/").RootReference;

                Debug.Log("Firebase inicializado con éxito");
            }
            else
            {
                Debug.LogError("Firebase no disponible: " + status);
            }
        }
        
        public void SubmitScore(float newScore)
        {
            string uid = auth.CurrentUser.UserId;

            // Obtener la puntuación anterior y comparar
            dbRef.Child("users").Child(uid).Child(scoreName).GetValueAsync().ContinueWith(task =>
            {
                if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    float currentHigh = snapshot.Exists ? float.Parse(snapshot.Value.ToString()) : 0;
                    if (scoreName == "highscoreReflex")
                    {
                        if (newScore < currentHigh)
                        {
                            decimal scoreDecimal = Math.Round((decimal)newScore, 3);
                            dbRef.Child("users").Child(uid).Child(scoreName).SetValueAsync((float)scoreDecimal);
                        }
                    }
                    else
                    {
                        if (newScore > currentHigh)
                        {
                            dbRef.Child("users").Child(uid).Child(scoreName).SetValueAsync(newScore);
                        }
                    }
                }
            });
        }
     
        public void LoadTopScores()
        {
            dbRef.Child("users").OrderByChild(scoreName).LimitToLast(10).GetValueAsync().ContinueWith(task => {

                if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    string ranking = "";

                    foreach (DataSnapshot user in snapshot.Children)
                    {
                        string name = user.Child("username").Value.ToString();
                        string score = user.Child(scoreName).Value.ToString();
                        ranking += name + ": " + score + "\n";
                    }

                    if (scoreName != "highscoreReflex")
                    {
                        // Invertir porque Firebase devuelve en orden ascendente
                        var lines = ranking.Split('\n');
                        System.Array.Reverse(lines);
                        ranking = "";
                        foreach (string line in lines)
                        {
                            ranking += line + "\n";
                        }
                    }
                    UnityMainThreadDispatcher.Enqueue(() => {
                        scoresText.text = ranking;
                        LeanTween.scale(scoresPanel, Vector3.one, 1.5f)
                            .setEase(LeanTweenType.easeOutExpo);
                    });
                }
            });
        }
        
        public void CloseLoadTopScores()
        {
            LeanTween.scale(scoresPanel, Vector3.zero, 1.5f)
                .setEase(LeanTweenType.easeOutExpo);
            restartButton.gameObject.SetActive(true);
            scoreButton.gameObject.SetActive(true);
        }

        public void ShowStartElements()
        {
            startButton.gameObject.SetActive(true);
            startText.gameObject.SetActive(true);
            menuButton.gameObject.SetActive(true);
        }

        public void HideStartElements()
        {
            startButton.gameObject.SetActive(false);
            startText.gameObject.SetActive(false);
            menuButton.gameObject.SetActive(false);
        }

        public void HideGameElements()
        {
            buttonsPanel.gameObject.SetActive(false);
            restartButton.gameObject.SetActive(false);  // Esconde el botón de reiniciar si existe
            scoreText.gameObject.SetActive(false); // Esconde el texto de puntos
            timerText.gameObject.SetActive(false); // Esconde el texto de temporizador
        }

        public void ShowGameElements()
        {
            // Mostrar los elementos del juego
            buttonsPanel.gameObject.SetActive(true);
            scoreText.gameObject.SetActive(true);
            timerText.gameObject.SetActive(true);
        }

        public void HideRestartElements()
        {
            restartButton.gameObject.SetActive(false);
            scoreButton.gameObject.SetActive(false);
            menuButton.gameObject.SetActive(false);
        }

        public void AddListeners()
        {
            scoreButton.onClick.AddListener(LoadTopScores);
            closeScoreButton.onClick.AddListener(CloseLoadTopScores);
            menuButton.onClick.AddListener(GoToMenu);
            startButton.onClick.AddListener(StartGame);
            restartButton.onClick.AddListener(RestartGame);
        }
        
        private void GoToMenu()
        {
            SceneManager.LoadScene("MainMenu");
        }
}