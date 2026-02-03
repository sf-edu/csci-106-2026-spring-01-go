namespace Go;

public static class Program
{
    public static void Main()
    {
        var size = 19;
        
        var cursor = new Cursor(size);
        var board = new Board(size, cursor);

        board.Init();

        while (true)
        {
            board.Render();
            cursor.HandleInput();
        }
        
        board.Cleanup();
    }
}

