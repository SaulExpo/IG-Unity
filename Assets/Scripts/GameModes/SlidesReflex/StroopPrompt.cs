[System.Serializable]
public class StroopPrompt
{
    public string displayedText;     
    public Direction arrowDirection;
}

public enum Direction
{
    Left,
    Right,
    Up,
    Down
}
