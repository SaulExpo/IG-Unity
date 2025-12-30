using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using Firebase;
using UnityEngine.SceneManagement;
using Firebase.Auth;
using Firebase.Database;

public class ColorReflexMode : AllModes
{
    public TextMeshProUGUI colorText; 
    public Button[] colorButtons;  
    
    public GameObject colorPanel;

    
    public Image colorSquare1;
    public Image colorSquare2;
    
    public GridLayoutGroup buttonGridLayout;
    float cellWidth = 300f;
    float cellHeight = 300f;



    private List<Color> allColors = new List<Color>
    {
        Color.red,                                 // rojo
        Color.green,                               // verde
        Color.blue,                                // azul
        Color.yellow,                              // amarillo
        Color.magenta,                             // magenta
        Color.cyan,                                // cian
        new Color(1f, 0.5f, 0f),           // naranja
        new Color(0.5f, 0f, 1f),           // púrpura oscuro
        new Color(0.3f, 0.7f, 0.2f),       // verde lima
        Color.gray,                                // gris
        new Color(1f, 1f, 0.5f),           // amarillo pálido
        new Color(0.5f, 0.5f, 1f),         // azul claro
        new Color(1f, 0.6f, 0.8f),         // rosa claro
        new Color(0.8f, 0.3f, 0.3f),       // rojo apagado
        new Color(0.2f, 0.4f, 0.9f),       // azul oscuro
        new Color(0.4f, 0.2f, 0.1f),       // marrón oscuro
        new Color(0.9f, 0.9f, 0.9f),       // casi blanco
        new Color(0.2f, 0.8f, 0.8f),       // turquesa
        new Color(0.6f, 0.4f, 0.8f),       // lila
        new Color(0.1f, 0.1f, 0.1f),       // casi negro
        new Color(1f, 0f, 1f),             // Fucsia
        new Color(0.5f, 0.5f, 0f),         // Oliva
        new Color(0.7f, 0.3f, 0.1f),       // Terracota
        new Color(0.9f, 0.2f, 0.3f),       // Carmesí
        new Color(0.2f, 0.8f, 0f),         // Verde esmeralda
        new Color(0.8f, 0.5f, 0f),         // Dorado
        new Color(0f, 0f, 0.5f),           // Azul marino
        new Color(0.7f, 0.7f, 0.7f),       // Gris claro
        new Color(0.1f, 0.6f, 0.3f),       // Verde bosque
        new Color(0.9f, 0.7f, 0.1f),       // Amarillo oro
        new Color(0.3f, 0.3f, 0.6f),       // Azul petróleo
        new Color(0.8f, 0.1f, 0.7f),       // Rosa fuerte
        new Color(0.6f, 0.9f, 0.6f),       // Verde pastel
        new Color(1f, 0.8f, 0.3f),         // Amarillo mostaza
        new Color(0.4f, 0.1f, 0.3f),       // Púrpura claro
        new Color(0.2f, 0.7f, 1f),         // Celeste
        new Color(0.7f, 0.1f, 0.6f),       // Rosa chicle
        new Color(0.3f, 0.5f, 0.7f),       // Azul hielo
        new Color(0.4f, 0.2f, 0.5f),       // Púrpura
        new Color(0.8f, 0.9f, 0.6f),       // Verde limón
        new Color(1f, 0.5f, 0.31f),        // Coral
        new Color(0.8f, 0.6f, 1f)          // Lavanda
    };

    private List<Color> activeColors = new List<Color>();
    private Color targetColor;


    protected override void Start()
    {
        base.Start();
        colorPanel.gameObject.SetActive(false);
    }

    public override void StartGame()
    {
        base.StartGame();
        colorPanel.gameObject.SetActive(true);
        
        foreach (Button button in colorButtons)
        {
            if (button != null)
            {
                button.onClick.AddListener(() => OnButtonClick(button));
            }
            else
            {
                Debug.LogError("El botón es null, no se puede añadir el listener.");
            }
        }

        activeColors.Add(Color.red);
        activeColors.Add(Color.green);
        activeColors.Add(Color.blue);

        for (int i = 0; i < colorButtons.Length; i++)
        {
            if (colorButtons.Length >= 3)
                colorButtons[i].gameObject.SetActive(false);
        }
        StartNewRound();
        
    }


    protected override void Update()
    {
        if (started == true)
        {
            base.Update();
        }
    }

    public override void StartNewRound()
    {
        if (timer <= 0) return;
        round++;

        if (round % 5 == 0 && activeColors.Count < allColors.Count)
        {
            Color newColor = allColors[activeColors.Count];
            activeColors.Add(newColor);

            if (colorButtons.Length > activeColors.Count - 1)
                colorButtons[activeColors.Count - 1].gameObject.SetActive(true);

            AdjustGridLayout(); 
        }

        targetColor = activeColors[Random.Range(0, activeColors.Count)];

        string colorName = GetColorName(targetColor);
        colorText.text = colorName;

        colorText.color = targetColor;
        colorSquare1.color = targetColor;
        colorSquare2.color = targetColor;

        SetButtonColors();
    }

    private void SetButtonColors()
    {
        List<Color> colorsCopy = new List<Color>(activeColors);

        for (int i = 0; i < activeColors.Count; i++)
        {
            int index = Random.Range(0, colorsCopy.Count);
            colorButtons[i].GetComponent<Image>().color = colorsCopy[index];
            colorButtons[i].interactable = true;
            colorButtons[i].gameObject.SetActive(true); 
            colorsCopy.RemoveAt(index);
        }
    }
    private void AdjustGridLayout()
    {
        if (buttonGridLayout != null)
        {
            int count = activeColors.Count;

            if (count <= 3)
                buttonGridLayout.constraintCount = 3;
            else if (count <= 4)
                buttonGridLayout.constraintCount = 2;
            else if (count <= 9)
                buttonGridLayout.constraintCount = 3;
            else if (count <= 16)
            {
                buttonGridLayout.constraintCount = 4;
                cellWidth = 230f;
                cellHeight = 230f;
            }
            else if (count <= 25)
            {
                buttonGridLayout.constraintCount = 5;
                cellWidth = 180f;
                cellHeight = 180f;
            }
            else if (count <= 36)
            {
                buttonGridLayout.constraintCount = 6;
                cellWidth = 140f;
                cellHeight = 140f;
            }
            else
            {
                buttonGridLayout.constraintCount = 7;
                cellWidth = 120f;
                cellHeight = 120f;
            }

            buttonGridLayout.cellSize = new Vector2(cellWidth, cellHeight);
        }
    }

    private string GetColorName(Color color)
    {
        if (color == Color.red) return "Rojo";
        if (color == Color.green) return "Verde";
        if (color == Color.blue) return "Azul";
        if (color == Color.yellow) return "Amarillo";
        if (color == Color.magenta) return "Magenta";
        if (color == Color.cyan) return "Cyan";
        if (color == new Color(1f, 0.5f, 0f)) return "Naranja";
        if (color == new Color(0.5f, 0f, 1f)) return "Púrpura oscuro";
        if (color == new Color(0.3f, 0.7f, 0.2f)) return "Verde lima";
        if (color == Color.gray) return "Gris";
        if (color == new Color(1f, 1f, 0.5f)) return "Amarillo pálido";
        if (color == new Color(0.5f, 0.5f, 1f)) return "Azul claro";
        if (color == new Color(1f, 0.6f, 0.8f)) return "Rosa";
        if (color == new Color(0.8f, 0.3f, 0.3f)) return "Rojo apagado";
        if (color == new Color(0.2f, 0.4f, 0.9f)) return "Azul oscuro";
        if (color == new Color(0.4f, 0.2f, 0.1f)) return "Marrón";
        if (color == new Color(0.9f, 0.9f, 0.9f)) return "Casi blanco";
        if (color == new Color(0.2f, 0.8f, 0.8f)) return "Turquesa";
        if (color == new Color(0.6f, 0.4f, 0.8f)) return "Lila";
        if (color == new Color(0.1f, 0.1f, 0.1f)) return "Casi negro";
        if (color == new Color(1f, 0f, 1f)) return "Fucsia";           
        if (color == new Color(0.5f, 0.5f, 0f)) return "Oliva";          
        if (color == new Color(0.7f, 0.3f, 0.1f)) return "Terracota";     
        if (color == new Color(0.9f, 0.2f, 0.3f)) return "Carmesí";      
        if (color == new Color(0.2f, 0.8f, 0f)) return "Verde esmeralda"; 
        if (color == new Color(0.8f, 0.5f, 0f)) return "Dorado";         
        if (color == new Color(0f, 0f, 0.5f)) return "Azul marino";      
        if (color == new Color(0.7f, 0.7f, 0.7f)) return "Gris claro";    
        if (color == new Color(0.1f, 0.6f, 0.3f)) return "Verde bosque"; 
        if (color == new Color(0.9f, 0.7f, 0.1f)) return "Amarillo oro"; 
        if (color == new Color(0.3f, 0.3f, 0.6f)) return "Azul petróleo"; 
        if (color == new Color(0.8f, 0.1f, 0.7f)) return "Rosa fuerte";   
        if (color == new Color(0.6f, 0.9f, 0.6f)) return "Verde pastel";  
        if (color == new Color(1f, 0.8f, 0.3f)) return "Amarillo mostaza"; 
        if (color == new Color(0.4f, 0.1f, 0.3f)) return "Púrpura claro"; 
        if (color == new Color(0.2f, 0.7f, 1f)) return "Celeste";       
        if (color == new Color(0.7f, 0.1f, 0.6f)) return "Rosa chicle";  
        if (color == new Color(0.3f, 0.5f, 0.7f)) return "Azul hielo";  
        if (color == new Color(0.4f, 0.2f, 0.5f)) return "Púrpura";       
        if (color == new Color(0.8f, 0.9f, 0.6f)) return "Verde limón";   
        if (color == new Color(1f, 0.5f, 0.31f)) return "Coral";  
        if (color == new Color(0.8f, 0.6f, 1f)) return "Lavanda";  
        return "???";
    }

    private void OnButtonClick(Button clickedButton)
    {
        if (clickedButton.GetComponent<Image>().color == targetColor)
        {
            score++;
            scoreText.text = "Puntos: " + score;

            timer += 2f;
        }
        else
        {
            timer -= 15f;
            score--;
            scoreText.text = "Puntos: " + score;
        }
        StartNewRound();
    }


    public override void RestartGame()
    {
        base.RestartGame();
        activeColors.Clear();
        activeColors.Add(Color.red);
        activeColors.Add(Color.green);
        activeColors.Add(Color.blue);


        for (int i = 0; i < colorButtons.Length; i++)
        {
            if (i < 2)
            {
                colorButtons[i].gameObject.SetActive(true);
                colorButtons[i].interactable = true;
            }
            else
            {
                colorButtons[i].gameObject.SetActive(false);
            }
        }

        if (buttonGridLayout != null)
        {
            buttonGridLayout.constraintCount = 3;
        }
        StartNewRound();
    }
}
