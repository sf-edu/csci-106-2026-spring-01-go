#pragma warning disable CS8524

namespace Go;

public class Board
{
    private int Size;
    private SpaceState[] Spaces;

    public Board(int size)
    {
        Size = size;
        Spaces = new SpaceState[size * size];

        for (var i = 0; i < Spaces.Length; i++)
        {
            Spaces[i] = SpaceState.Empty;
        }
    }

    public void Render()
    {
        for (var y = 0; y < Size; y++)
        {
            for (var x = 0; x < Size; x++)
            {
                Console.Write(GetToken(GetState(x, y)));

                if (x != Size - 1)
                {
                    Console.Write(' ');
                }
            }

            Console.WriteLine();
        }
    }

    public SpaceState GetState(int x, int y)
    {
        return Spaces[x + Size * y];
    }

    public char GetToken(SpaceState state)
    {
        return state switch
        {
            SpaceState.BlackStone => '○',
            SpaceState.WhiteStone => '⬤',
            SpaceState.Empty => '·',
        };
    }
}

public enum SpaceState
{
    Empty,
    BlackStone,
    WhiteStone,
}
