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
                Console.Write('.');

                if (x != Size - 1)
                {
                    Console.Write(' ');
                }
            }

            Console.WriteLine();
        }
    }
}

public enum SpaceState
{
    Empty,
    BlackStone,
    WhiteStone,
}
