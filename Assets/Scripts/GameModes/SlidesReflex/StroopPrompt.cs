[System.Serializable]
public class StroopPrompt
{
    public string displayedText;        // Ej: "Izquierda"
    public Direction arrowDirection;
}

public enum Direction
{
    Left,
    Right,
    Up,
    Down
}
