using UnityEngine;
using UnityEngine.UI;

namespace GameModes.ReflexMode
{
    public class FastReflexMode : AllModes
    {
        public float minTime = 1f;
        public float maxTime = 10f;
        public GameObject backgroundPanel;
        
        private bool touchNow;
        private float elapsed = 0f;    

        protected override void Start()
        {
            base.Start();
            touchNow = false;
        }

        protected override void Update()
        {
            if (started)
            {
                if (touchNow)
                {
                    elapsed += Time.deltaTime;
                    UpdateText();
                    if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
                    {
                        touchNow = false;
                        score = Mathf.Floor(elapsed * 1000f) / 1000f;
                        scoreText.gameObject.SetActive(true);
                        scoreText.text = "Tiempo de reacción: " + score;
                        started = false;
                        GameOver();
                    }
                }
                else
                {
                    if (Input.GetMouseButtonDown(0) ||
                        (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
                    {
                        CancelInvoke(nameof(PressNow));
                        score = 999f;
                        scoreText.gameObject.SetActive(true);
                        scoreText.text = "Pulsaste demasiado pronto :(";
                        GameOver();
                    }
                }
            }
        }

        public override void StartGame()
        {
            base.StartGame();
            scoreText.gameObject.SetActive(false);
            timerText.gameObject.SetActive(false);
            float randomTime = Random.Range(minTime, maxTime);
            Invoke(nameof(PressNow), randomTime);
            
        }

        public override void RestartGame()
        {
            base.RestartGame();
            elapsed = 0f;
            backgroundPanel.GetComponent<Image>().color = new Color(0, 255, 0, 145);
            scoreText.gameObject.SetActive(false);
            float randomTime = Random.Range(minTime, maxTime);
            Invoke(nameof(PressNow), randomTime);
        }

        public override void StartNewRound()
        {
            base.StartNewRound();
        }

        private void PressNow()
        {
            timerText.gameObject.SetActive(true);
            touchNow = true;
            ChangeColor();
        }
        
        private void ChangeColor()
        {
            backgroundPanel.GetComponent<Image>().color = new Color(255, 0, 0, 255);
        }
        
        private void UpdateText()
        {
            int minutes      = (int)(elapsed / 60f);
            int seconds      = (int)(elapsed % 60f);
            int milliseconds = (int)((elapsed * 1000f) % 1000f);

            timerText.text = $"{minutes:00}:{seconds:00}:{milliseconds:000}";
        }

        private void GameOver()
        {
            timerText.gameObject.SetActive(false);
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
    
}