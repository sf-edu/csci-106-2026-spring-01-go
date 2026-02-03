#pragma warning disable CS8524

namespace Go;

public class Board
{
    private readonly Cursor Cursor;
    private readonly int Size;

    private SpaceState[] Spaces;

    public Board(int size, Cursor cursor)
    {
        Cursor = cursor;
        Size = size;

        Spaces = new SpaceState[size * size];
        for (var i = 0; i < Spaces.Length; i++)
        {
            Spaces[i] = SpaceState.Empty;
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

        Console.Write(buffer);
    }

    public SpaceState GetState(int x, int y)
    {
        return Spaces[x + Size * y];
    }

    public char GetToken(SpaceState state, bool cursor)
    {
        return (state, cursor) switch
        {
            (SpaceState.BlackStone, _) => '○',
            (SpaceState.WhiteStone, _) => '⬤',
            (SpaceState.Empty, false) => '·',
            (SpaceState.Empty, true) => '⊗',
        };
    }
}

public enum SpaceState
{
    Empty,
    BlackStone,
    WhiteStone,
}
