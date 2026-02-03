namespace Go;

public enum InputEvent
{
	MoveUp,
	MoveLeft,
	MoveRight,
	MoveDown,
	PlaceToken,
	Quit,
	Noop,
}

public class InputHandler
{
	public InputEvent CurrentEvent { get; private set; }

	public InputHandler()
	{
		CurrentEvent = InputEvent.Noop;
	}

	public void HandleInput()
	{
		switch (Console.ReadKey(true).Key)
		{
			case ConsoleKey.W:
			case ConsoleKey.UpArrow:
			case ConsoleKey.K:
				CurrentEvent = InputEvent.MoveUp;
				break;

			case ConsoleKey.A:
			case ConsoleKey.LeftArrow:
			case ConsoleKey.H:
				CurrentEvent = InputEvent.MoveLeft;
				break;

			case ConsoleKey.S:
			case ConsoleKey.DownArrow:
			case ConsoleKey.J:
				CurrentEvent = InputEvent.MoveDown;
				break;

			case ConsoleKey.D:
			case ConsoleKey.RightArrow:
			case ConsoleKey.L:
				CurrentEvent = InputEvent.MoveRight;
				break;

			case ConsoleKey.Spacebar:
			case ConsoleKey.Enter:
				CurrentEvent = InputEvent.PlaceToken;
				break;

			case ConsoleKey.Escape:
			case ConsoleKey.Q:
				CurrentEvent = InputEvent.Quit;
				break;

			default:
				CurrentEvent = InputEvent.Noop;
				break;
		}
	}

}
