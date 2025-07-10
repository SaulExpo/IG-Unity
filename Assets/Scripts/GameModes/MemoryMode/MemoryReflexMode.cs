using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class MemoryReflexMode : AllModes
{
    public List<Button> botones; // Referencia a los botones del juego
    public GameObject btnPanel;
    private List<int> secuencia = new List<int>(); // Índices de botones
    private int indiceUsuario = 0;
    private bool esperandoEntradaUsuario = false;
    
    public GridLayoutGroup buttonGridLayout;

    [SerializeField] private float colorTime = 1f;
    [SerializeField] private float colorTime2 = 0.3f;

    protected override void Start()
    {
        base.Start();
        for (int i = 0; i < botones.Count; i++)
        {
            int index = i;
            botones[i].onClick.AddListener(() => BotonPresionado(index));
            ApagarBoton(botones[i]);

            // Oculta los botones extra si son más de 4
            if (i >= 4)
            {
                botones[i].gameObject.SetActive(false);
            }
        }
        btnPanel.gameObject.SetActive(false);
    }

    protected override void Update()
    {
        if (started)
        {
            
        }
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
        // Activar los 5 botones extra cuando se alcanza la ronda 5
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

        int nuevoPaso = Random.Range(0, GetNumeroBotonesActivos());
        secuencia.Add(nuevoPaso);
        StartCoroutine(MostrarSecuencia());
    }

    public override void RestartGame()
    {
        base.RestartGame();
        for (int i = 4; i < botones.Count; i++)
        {
            botones[i].gameObject.SetActive(false);
        }
        secuencia.Clear();
        timerText.gameObject.SetActive(false);
        StartNewRound(); // Empieza una nueva secuencia
    }
    
    IEnumerator MostrarSecuencia()
    {
        esperandoEntradaUsuario = false;
        for (int i = 0; i < secuencia.Count; i++)
        {
            int index = secuencia[i];
            IluminarBoton(botones[index]);
            yield return new WaitForSeconds(colorTime); // espera antes de apagar
            ApagarBoton(botones[index]);
            yield return new WaitForSeconds(colorTime2); // breve pausa antes del siguiente
        }
        esperandoEntradaUsuario = true;
        indiceUsuario = 0;
    }
    
    void IluminarBoton(Button boton)
    {
        boton.image.color = Color.yellow;
    }

    void ApagarBoton(Button boton)
    {
        boton.image.color = Color.black;
    }
    

    
    public void BotonPresionado(int index)
    {
        if (started)
        {
            if (!esperandoEntradaUsuario) return;

            IluminarBoton(botones[index]);
            StartCoroutine(DesiluminarDespues(botones[index]));

            if (index == secuencia[indiceUsuario])
            {
                indiceUsuario++;
                if (indiceUsuario >= secuencia.Count)
                {
                    // Secuencia completa con éxito
                    UpdateScoreText();
                    Invoke(nameof(StartNewRound), 1f); // espera 1s y añade otro paso
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

    IEnumerator DesiluminarDespues(Button boton)
    {
        yield return new WaitForSeconds(0.5f);
        ApagarBoton(boton);
    }
    
    int GetNumeroBotonesActivos()
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
                secuencia.Clear();
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
