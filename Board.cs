#pragma warning disable CS8524

namespace Go;

public class Board
{
	private readonly InputHandler InputHandler;
    private readonly Cursor Cursor;
    private readonly int Size;

    private SpaceState[] Spaces;
    private bool IsPlayer1;

    public Board(InputHandler inputHandler, int size, Cursor cursor)
    {
		InputHandler = inputHandler;
        Cursor = cursor;
        Size = size;

        IsPlayer1 = true;

        Spaces = new SpaceState[size * size];
        for (var i = 0; i < Spaces.Length; i++)
        {
            Spaces[i] = SpaceState.Empty;
        }
    }

    public void Update()
    {
        switch (InputHandler.CurrentEvent)
		{
			case InputEvent.PlaceToken:
                Place();
				break;
		}
    }

    private const string ENTER_ALT_BUFFER = "\x1b[?1049h";
    private const string LEAVE_ALT_BUFFER = "\x1b[?1049l";

    private const string HIDE_CURSOR = "\u001b[?25l";
    private const string SHOW_CURSOR = "\u001b[?25h";

    public void Init()
    {
        Console.Write(ENTER_ALT_BUFFER + HIDE_CURSOR);
        Console.Clear();
    }

    public void Cleanup()
    {
        Console.Write(LEAVE_ALT_BUFFER + SHOW_CURSOR);
    }

    private const string BLINK_START = "\u001B[5m";
    private const string BLINK_RESET = "\u001B[0m";

    public void Render()
    {
        Console.SetCursorPosition(0, 0);
        var buffer = string.Empty;

        for (var y = 0; y < Size; y++)
        {
            for (var x = 0; x < Size; x++)
            {
                var cursor = Cursor.IsAt(x, y);
                var token = GetToken(GetState(x, y), cursor).ToString();

                if (cursor) {
                    token = BLINK_START + token + BLINK_RESET;
                }

                buffer += token;

                if (x != Size - 1)
                {
                    buffer += " ";
                }
            }

            buffer += "\n";
        }

        var currentPlayer = IsPlayer1 ? "Black" : "White";
        buffer += $"\nCurrent player: {GetToken(CurrentToken, false)} {currentPlayer}";

        Console.Write(buffer);
    }

    private SpaceState GetState(int x, int y) => Spaces[GetIndex(x, y)];
    private SpaceState CursorState => Spaces[CursorIndex];

    private int GetIndex(int x, int y) => x + Size * y;
    private int CursorIndex => GetIndex(Cursor.X, Cursor.Y);

    public SpaceState CurrentToken => IsPlayer1
        ? SpaceState.BlackStone
        : SpaceState.WhiteStone;

    public char GetToken(SpaceState state, bool cursor)
    {
        return (state, cursor) switch
        {
            (SpaceState.BlackStone, _) => '○',
            (SpaceState.WhiteStone, _) => '●',
            (SpaceState.Empty, false) => '·',
            (SpaceState.Empty, true) => '⊗',
        };
    }

    public void Place()
    {
        if (CursorState == SpaceState.Empty)
        {
            Spaces[Cursor.X + Size * Cursor.Y] = CurrentToken;
            IsPlayer1 = !IsPlayer1;
        }
    }
}

public enum SpaceState
{
    Empty,
    BlackStone,
    WhiteStone,
}
