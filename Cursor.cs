namespace Go;

public class Cursor
{
	public int X { get; private set; }
	public int Y { get; private set; }

	private readonly InputHandler InputHandler;

	private readonly int Min = 0;
	private readonly int Max;

	public Cursor(InputHandler inputHandler, int max)
	{
		InputHandler = inputHandler;

		X = max / 2;
		Y = max / 2;
		Max = max;
	}

	public bool IsAt(int x, int y)
	{
		return X == x && Y == y;
	}

	public void Update()
	{
		switch (InputHandler.CurrentEvent)
		{
			case InputEvent.MoveUp:
				Y = Clamp(Y - 1);
				break;
			
			case InputEvent.MoveLeft:
				X = Clamp(X - 1);
				break;

			case InputEvent.MoveDown:
				Y = Clamp(Y + 1);
				break;

			case InputEvent.MoveRight:
				X = Clamp(X + 1);
				break;
		}
	}

	public int Clamp(int value)
	{
		if (value <= Min)
		{
			return Min;
		}

		if (Max <= value)
		{
			return Max - 1;
		}

		return value;
	}
}
