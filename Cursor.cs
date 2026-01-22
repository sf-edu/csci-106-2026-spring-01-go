namespace Go;

public class Cursor
{
	public int X { get; private set; }
	public int Y { get; private set; }

	private readonly int Min = 0;
	private readonly int Max;

	public Cursor(int max)
	{
		X = max / 2;
		Y = max / 2;
		Max = max;
	}

	public void HandleInput()
	{
		switch (Console.ReadKey(true).Key)
		{
			case ConsoleKey.W:
			case ConsoleKey.UpArrow:
			case ConsoleKey.K:
				Y = Clamp(Y - 1);
				break;

			case ConsoleKey.A:
			case ConsoleKey.LeftArrow:
			case ConsoleKey.H:
				X = Clamp(X - 1);
				break;

			case ConsoleKey.S:
			case ConsoleKey.DownArrow:
			case ConsoleKey.J:
				Y = Clamp(Y + 1);
				break;

			case ConsoleKey.D:
			case ConsoleKey.RightArrow:
			case ConsoleKey.L:
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
