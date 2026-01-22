namespace Go;

public static class Program
{
    public static void Main()
    {
        // var board = new Board(19);
        // board.Render();

        var cursor = new Cursor(19);
        while (true)
        {
            cursor.HandleInput();
        }
    }
}

