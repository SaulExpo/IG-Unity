using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class MemoryReflexMode : AllModes
{
    public List<Button> botones; 
    public GameObject btnPanel;
    private List<int> sequence = new List<int>();
    private int userIndex = 0;
    private bool waitUserAction = false;
    
    public GridLayoutGroup buttonGridLayout;

    [SerializeField] private float colorTime = 1f;
    [SerializeField] private float colorTime2 = 0.3f;

    protected override void Start()
    {
        base.Start();
        for (int i = 0; i < botones.Count; i++)
        {
            int index = i;
            botones[i].onClick.AddListener(() => PressedButton(index));
            ShutDownButton(botones[i]);

            if (i >= 4)
            {
                botones[i].gameObject.SetActive(false);
            }
        }
        btnPanel.gameObject.SetActive(false);
    }

    protected override void Update()
    {
    }
    
    public override void StartGame()
    {
        base.StartGame();
        btnPanel.gameObject.SetActive(true);
        timerText.gameObject.SetActive(false);
        StartNewRound();
    }
    
    public override void StartNewRound()
    {
        round++;
        score++;
        AdjustGridLayout();
        if (round == 5)
        {
            for (int i = 4; i < 9; i++)
            {
                botones[i].gameObject.SetActive(true);
            }
        }
        if (round == 13)
        {
            for (int i = 9; i < botones.Count; i++)
            {
                botones[i].gameObject.SetActive(true);
            }
        }

        int nuevoPaso = Random.Range(0, GetActiveButtons());
        sequence.Add(nuevoPaso);
        StartCoroutine(ShowSequence());
    }

    public override void RestartGame()
    {
        base.RestartGame();
        for (int i = 4; i < botones.Count; i++)
        {
            botones[i].gameObject.SetActive(false);
        }
        sequence.Clear();
        timerText.gameObject.SetActive(false);
        StartNewRound(); 
    }
    
    IEnumerator ShowSequence()
    {
        waitUserAction = false;
        for (int i = 0; i < sequence.Count; i++)
        {
            int index = sequence[i];
            LitButton(botones[index]);
            yield return new WaitForSeconds(colorTime); 
            ShutDownButton(botones[index]);
            yield return new WaitForSeconds(colorTime2); 
        }
        waitUserAction = true;
        userIndex = 0;
    }
    
    void LitButton(Button boton)
    {
        boton.image.color = Color.yellow;
    }

    void ShutDownButton(Button boton)
    {
        boton.image.color = Color.black;
    }
    

    
    public void PressedButton(int index)
    {
        if (started)
        {
            if (!waitUserAction) return;

            LitButton(botones[index]);
            StartCoroutine(ShutDownAfter(botones[index]));

            if (index == sequence[userIndex])
            {
                userIndex++;
                if (userIndex >= sequence.Count)
                {
                    UpdateScoreText();
                    Invoke(nameof(StartNewRound), 1f); 
                }
            }
            else
            {
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
    }

    IEnumerator ShutDownAfter(Button boton)
    {
        yield return new WaitForSeconds(0.5f);
        ShutDownButton(boton);
    }
    
    int GetActiveButtons()
    {
        int count = 0;
        foreach (var boton in botones)
        {
            if (boton.gameObject.activeSelf)
            {
                count++;
            }
        }
        return count;
    }
    
    private void AdjustGridLayout()
    {
        if (buttonGridLayout != null)
        {
            if (round == 5 || round == 13 || round == 27)
            {
                sequence.Clear();
                colorTime = 1f;
                colorTime = 0.3f;
            }
            if (round < 5)
            {
                colorTime = 1f;
                colorTime = 0.3f;
                buttonGridLayout.cellSize = new Vector2(300f, 300f);
            }
            else if (round < 13)
            {
                colorTime = 0.5f;
                colorTime = 0.2f;
                buttonGridLayout.cellSize = new Vector2(250f, 250f);
            }
            else 
            {
                colorTime = 0.25f;
                colorTime = 0.1f;
                buttonGridLayout.cellSize = new Vector2(200f, 200f);
            }
        }
    }
    void UpdateScoreText()
    {
        scoreText.text = "Puntos: " + score;
    }
}
