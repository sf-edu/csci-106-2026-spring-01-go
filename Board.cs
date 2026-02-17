#pragma warning disable CS8524

using System.Collections.Generic;

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
    private SpaceState CursorState {
        get => Spaces[CursorIndex];
        set => Spaces[CursorIndex] = value;
    }

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
        var (canPlace, capturedPositions) = ComputeCapture();
        if (!canPlace) {
            return;
        }

        CursorState = CurrentToken;
        IsPlayer1 = !IsPlayer1;

        foreach (var (x, y) in capturedPositions)
        {
            Spaces[GetIndex(x, y)] = SpaceState.Empty;
        }
    }

    private (bool, List<(int, int)>) ComputeCapture()
    {
        // Can't place on top of other pieces.
        if (CursorState != SpaceState.Empty)
        {
            return (false, new List<(int, int)>());
        }

        var adjacent = new List<(int, int)>
        {
            (Cursor.X - 1, Cursor.Y),
            (Cursor.X + 1, Cursor.Y),
            (Cursor.X, Cursor.Y - 1),
            (Cursor.X, Cursor.Y + 1),
        };

        // group number <> (stone color, liberties)
        var groups = new Dictionary<int, (SpaceState, int)>();
        var groupNumber = 0;

        // board position <> group number
        var positions = new Dictionary<(int, int), int>();

        foreach (var (x, y) in adjacent)
        {
            // Don't check groups in places we've already explored
            if (positions.ContainsKey((x, y)))
            {
                continue;
            }

            var (state, groupPositions, liberties) = GetGroup(x, y);
            if (state == SpaceState.Empty)
            {
                continue;
            }

            groups.Add(groupNumber, (state, liberties));
            foreach (var position in groupPositions)
            {
                positions.Add(position, groupNumber);
            }

            groupNumber += 1;
        }

        var enemyState = CursorState switch {
            SpaceState.WhiteStone => SpaceState.BlackStone,
            _ => SpaceState.WhiteStone,
        };

        // Will placing this stone capture an enemy group?
        if (groups.ContainsValue((enemyState, 1))) {
            var capturedGroups = groups
                .Where(kv => kv.Value == (enemyState, 1))
                .Select(kv => kv.Key);

            var capturedPositions = positions
                .Where(kv => capturedGroups.Contains(kv.Value))
                .Select(kv => kv.Key)
                .ToList();

            return (true, capturedPositions);
        }

        // Placement is fine if any adjacent spaces are empty.
        foreach (var (x, y) in adjacent)
        {
            if (GetState(x, y) == SpaceState.Empty) {
                return (true, new List<(int, int)>());
            }
        }

        // Get the individual liberty counts of adjacent friendly groups.
        var friendlyLiberties = groups
            .Where(kv => kv.Value.Item1 == CursorState)
            .Select(kv => kv.Value.Item2)
            .ToList();

        // Don't allow placement if it results in the capture of a friendly
        // group.
        return (
            1 < (friendlyLiberties.Sum() - friendlyLiberties.Count),
            new List<(int, int)>()
        );
    }

    private bool IsInBounds(int x, int y) =>
        0 <= x && x < Size &&
        0 <= y && y < Size;

    private (SpaceState, HashSet<(int, int)>, int) GetGroup(int x, int y)
    {
        // Positions we've for sure found
        var foundPositions = new HashSet<(int, int)>();
        var positionsToCheck = new Queue<(int, int)>();
        var checkedPositions = new HashSet<(int, int)>();
        var liberties = 0;

        // Make sure our starting position has a stone
        var groupState = GetState(x, y);
        if (groupState == SpaceState.Empty)
        {
            return (SpaceState.Empty, new HashSet<(int, int)>(), 0);
        }

        // Start with the first position, keep going until we run out
        positionsToCheck.Enqueue((x, y));
        while (0 < positionsToCheck.Count)
        {
            // Ignore empty positions and other colors
            var (x1, y1) = positionsToCheck.Dequeue();
            var state = GetState(x1, y1);

            if (state == SpaceState.Empty)
            {
                liberties += 1;
                continue;
            }
            else if (state != groupState)
            {
                continue;
            }

            // Found one! Log it and try its adjacent positions
            foundPositions.Add((x1, y1));
            var adjacent = new List<(int, int)>
            {
                (x1 - 1, y1),
                (x1 + 1, y1),
                (x1, y1 - 1),
                (x1, y1 + 1),
            };

            foreach (var position in adjacent)
            {
                // Make sure we're not double dipping
                var (x2, y2) = position;
                if (!IsInBounds(x2, y2) || checkedPositions.Contains(position))
                {
                    continue;
                }

                checkedPositions.Add(position);
                positionsToCheck.Enqueue(position);
            }
        }

        return (groupState, foundPositions, liberties);
    }
}

public enum SpaceState
{
    Empty,
    BlackStone,
    WhiteStone,
}
