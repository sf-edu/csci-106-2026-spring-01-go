namespace Go;

public static class Program
{
    public static void Main()
    {
        var size = 19;
        
        var board = new Board(size);
        var cursor = new Cursor(size);

        board.Init();

        while (true)
        {
            board.Render();
            cursor.HandleInput();
        }
        
        board.Cleanup();
    }
}

