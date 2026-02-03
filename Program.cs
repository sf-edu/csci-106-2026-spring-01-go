namespace Go;

public static class Program
{
    public static void Main()
    {
        var size = 19;
        
		var inputHandler = new InputHandler();
        var cursor = new Cursor(inputHandler, size);
        var board = new Board(inputHandler, size, cursor);

        board.Init();

        while (true)
        {
			if (inputHandler.CurrentEvent == InputEvent.Quit)
			{
				break;
			}
			
			cursor.Update();
			board.Update();

            board.Render();

            inputHandler.HandleInput();
        }
        
        board.Cleanup();
    }
}

