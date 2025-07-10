using TMPro;
using UnityEngine;
using Image = UnityEngine.UI.Image;

public class SlidesReflexMode : AllModes
{
    public TextMeshProUGUI promptText;         // Texto en pantalla (ej. "Izquierda")
    public Image arrowImage;        // Imagen de la flecha
    public Sprite arrowLeft;
    public Sprite arrowRight;
    public Sprite arrowUp;
    public Sprite arrowDown;

    public GameObject backgroundPanel;

    
   // Referencia al botón de iniciar
    private Direction expectedDirection;
    private Vector2 swipeStart;
    private bool isSwiping;

    private StroopPrompt currentPrompt;
    private bool useTextAsCorrect = false;

    


    protected override void Start()
    {
        base.Start();
        arrowImage.gameObject.SetActive(false);
        promptText.gameObject.SetActive(false);
    }

    protected override void Update()
    {
        if (started)
        {
            DetectSwipe();
            // Si el temporizador es mayor que 0, continúa descontando
            base.Update();
        }
    }
    
    public override void StartGame()
    {
        base.StartGame();
        // Mostrar los elementos del juego
        promptText.gameObject.SetActive(true);
        arrowImage.gameObject.SetActive(true);
        // Iniciar el juego normalmente
        UpdateScoreText();
        GeneratePrompt();
        StartNewRound();
    }
    
    public override void StartNewRound()
    {
        Debug.Log("INICIADA RONDA: " + round);
        // Si el tiempo se acaba, no hacer nada hasta reiniciar
        if (timer <= 0) return;
        round++;
        if (round < 10)
        {
            timer = 5f;
        } else if (round < 20)
        {
            timer = 4f;
        } else if (round < 30)
        {
            timer = 3f;
        } else if (round < 40)
        {
            timer = 2f;
        } else
        {
            timer = 1f;
        }
    }

    public override void RestartGame()
    {
        base.RestartGame();
        StartNewRound();
    }

    void GeneratePrompt()
    {
        currentPrompt = new StroopPrompt();

        // 1. Elegir dirección de texto (random)
        Direction textDirection = (Direction)Random.Range(0, 4); // 0 = Left, 1 = Right, 2 = Up, 3 = Down
        currentPrompt.displayedText = textDirection == Direction.Left ? "Izquierda" :
            (textDirection == Direction.Right ? "Derecha" :
                (textDirection == Direction.Up ? "Arriba" : "Abajo"));

        // 2. Elegir aleatoriamente si la flecha coincide con el texto
        bool contradiction = Random.value > 0.5f;
        currentPrompt.arrowDirection = contradiction
            ? GetContradictoryDirection(textDirection)
            : textDirection;

        ChooseMode();
        
        // 3. Asignar dirección correcta según la variable useTextAsCorrect
        expectedDirection = useTextAsCorrect ? textDirection : currentPrompt.arrowDirection;

        // 4. Mostrar en UI
        promptText.text = currentPrompt.displayedText;
        switch (currentPrompt.arrowDirection)
        {
            case Direction.Left:
                arrowImage.sprite = arrowLeft;
                break;
            case Direction.Right:
                arrowImage.sprite = arrowRight;
                break;
            case Direction.Up:
                arrowImage.sprite = arrowUp;
                break;
            case Direction.Down:
                arrowImage.sprite = arrowDown;
                break;
        }
    }
    
    Direction GetContradictoryDirection(Direction dir)
    {
        // Elige una dirección distinta aleatoriamente
        Direction newDir;
        do
        {
            newDir = (Direction)Random.Range(0, 4);
        } while (newDir == dir);

        return newDir;
    }

    void ChooseMode()
    {
        if (round < 0) return;
        if (round < 10)
        {
            useTextAsCorrect = true;
        }else if (round < 20)
        {
            useTextAsCorrect = false;
        }
        else
        {
            useTextAsCorrect = Random.value > 0.5f;
        }

        if (useTextAsCorrect == true)
        {
            backgroundPanel.GetComponent<Image>().color = new Color(0, 255, 0, 175);
        }
        else
        {
            backgroundPanel.GetComponent<Image>().color = new Color(255, 255, 0, 175);
        }
    }

    void DetectSwipe()
    {
        if (Input.GetMouseButtonDown(0))
        {
            swipeStart = Input.mousePosition;
            isSwiping = true;
        }

        if (Input.GetMouseButtonUp(0) && isSwiping)
        {
            Vector2 swipeEnd = Input.mousePosition;
            Vector2 swipe = swipeEnd - swipeStart;

            Direction swipeDirection = Direction.Left;  // Valor por defecto

            // Detectar la dirección del swipe
            if (Mathf.Abs(swipe.x) > Mathf.Abs(swipe.y)) // Horizontal
            {
                swipeDirection = swipe.x > 0 ? Direction.Right : Direction.Left;
            }
            else // Vertical
            {
                swipeDirection = swipe.y > 0 ? Direction.Up : Direction.Down;
            }

            CheckAnswer(swipeDirection);
            isSwiping = false;
        }
    }

    void CheckAnswer(Direction swipeDirection)
    {
        if (swipeDirection == expectedDirection)
        {
            score += 1;
            StartNewRound();
        }
        else
        {
            timer = 0f;
        }

        UpdateScoreText();
        GeneratePrompt();
    }

    void UpdateScoreText()
    {
        scoreText.text = "Puntos: " + score;
    }
    
    
}
