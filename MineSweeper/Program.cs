//    Empty = 0,
//    One = 1,
//    Two = 2,
//    Three = 3,
//    Four = 4,
//    Five = 5,
//    Six = 6,
//    Seven = 7,
//    Eigth = 8,
//    Mine = 9,

//    EmptyMarked = 10,
//    OneMarked = 11,
//    TwoMarked = 12,
//    ThreeMarked = 13,
//    FourMarked = 14,
//    FiveMarked = 15,
//    SixMarked = 16,
//    SevenMarked = 17,
//    EigthMarked = 18,
//    MineMarked = 19,

//    EmptyUnknown = 20,
//    OneUnknown = 21,
//    TwoUnknown = 22,
//    ThreeUnknown = 23,
//    FourUnknown = 24,
//    FiveUnknown = 25,
//    SixUnknown = 26,
//    SevenUnknown = 27,
//    EigthUnknown = 28,
//    MineUnknown = 29,

//    EmptyReveal = 30,
//    OneReveal = 31,
//    TwoReveal = 32,
//    ThreeReveal = 33,
//    FourReveal = 34,
//    FiveReveal = 35,
//    SixReveal = 36,
//    SevenReveal = 37,
//    EigthReveal = 38,
//    MineReveal = 39

// r x y = reveal
// m x y = mark
// u x y = unknown

int width = GetInt("Enter board width:", i => i > 0 && i < 80, "width must be between 0 and 80.");
int height = GetInt("Enter board height:", i => i > 0 && i < 25, "height must be between 0 and 25.");
int mines = GetInt("Enter number of mines:", i => i > 0 && i <= (width * height), "mines must be between 1 and (width x height).");
int marks = -mines;
bool won = false;
int[][] board = new int[height + 2][];
for (int y = 0; y < height + 2; y++) board[y] = new int[width + 2];
PlaceMines(board, mines);
Scramble(board, mines);
Propagate(board);
while (won = Place(board, ref mines, ref marks))
{
    Console.Clear();
    PrintBoard(board);
    if (mines == 0 && marks == 0) break;
}
Console.Clear();
PrintBoard(board);
Console.WriteLine(won ? "You Won!!!" : "BOOM!!!");

static void Chord(int[][] board, int x, int y)
{
    int mines = board[y][x] - 30;
    int found = 0;
    int ym = Math.Max(y - 1, 1);
    int xm = Math.Max(x - 1, 1);
    int ya = Math.Min(y + 1, board.Length - 2);
    int xa = Math.Min(x + 1, board[0].Length - 2);
    if (mines > 0)
    {
        for (int xx = xm; xx <= xa; xx++)
        {
            if (board[ym][xx] > 9 && board[ym][xx] < 19 || board[ym][xx] > 19 && board[ym][xx] < 29 ||
                board[ya][xx] > 9 && board[ya][xx] < 19 || board[ya][xx] > 19 && board[ya][xx] < 29) return;
            if (board[ym][xx] == 19) found++;
            if (board[ya][xx] == 19) found++;
        }
        if (board[y][xm] > 9 && board[y][xm] < 19 || board[y][xm] > 19 && board[y][xm] < 29 ||
            board[y][xa] > 9 && board[y][xa] < 19 || board[y][xa] > 19 && board[y][xa] < 29) return;
        if (board[y][xm] == 19) found++;
        if (board[y][xa] == 19) found++;
    }

    if (mines == found)
        for (int yy = ym; yy <= ya; yy++)
            for (int xx = xm; xx <= xa; xx++)
            {
                if (board[yy][xx] == 0)
                {
                    board[yy][xx] = 30;
                    Chord(board, xx, yy);
                }
                else if (board[yy][xx] < 30)
                {
                    board[yy][xx] = (board[yy][xx] % 10) + 30;
                }
            }
}

static bool Place(int[][] board, ref int mines, ref int marks)
{
    do
    {
        string[] parts = (Console.ReadLine() ?? "").Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (parts.Length != 3) continue;
        if (int.TryParse(parts[1], out int x) && int.TryParse(parts[2], out int y) && y < board.Length && x < board[0].Length)
        {
            x++;
            y++;
            switch (parts[0].ToLower())
            {
                case "r":
                    {
                        board[y][x] = (board[y][x] % 10) + 30;
                        if (board[y][x] == 39) return false;
                        Chord(board, x, y);
                        return true;
                    }
                case "m":
                    {
                        if (board[y][x] < 9)
                        {
                            board[y][x] += 10;
                            marks++;
                        }
                        else if (board[y][x] == 9)
                        {
                            mines--;
                            marks++;
                            board[y][x] += 10;
                        }
                        else if (board[y][x] < 19)
                        {
                            marks--;
                            board[y][x] -= 10;
                        }
                        else if (board[y][x] == 19)
                        {
                            mines++;
                            marks--;
                            board[y][x] -= 10;
                        }
                        else if (board[y][x] < 29)
                        {
                            marks++;
                            board[y][x] -= 10;
                        }
                        else if (board[y][x] == 29)
                        {
                            mines++;
                            marks++;
                            board[y][x] -= 10;
                        }
                        return true;
                    }
                case "u":
                    {
                        if (board[y][x] < 10)
                        {
                            board[y][x] += 20;
                        }
                        else if (board[y][x] < 20)
                        {
                            marks--;
                            board[y][x] += 10;
                        }
                        else if (board[y][x] < 30)
                        {
                            board[y][x] -= 20;
                        }
                        return true;
                    }
                default:
                    {
                        Console.WriteLine("Invalid operation, try again...");
                        continue;
                    }
            }
        }
    } while (true);
}

static void Propagate(int[][] board)
{
    for (int y = 1; y < board.Length - 1; y++)
        for (int x = 1; x < board[y].Length - 1; x++)
            if (board[y][x] == 9)
                for (int y2 = y - 1; y2 <= y + 1; y2++)
                    for (int x2 = x - 1; x2 <= x + 1; x2++)
                        if (board[y2][x2] != 9)
                            board[y2][x2]++;
}

static void PlaceMines(int[][] board, int count)
{
    for (int y = 1; y < board.Length - 1; y++)
        for (int x = 1; x < board[y].Length - 1; x++)
        {
            board[y][x] = 9;
            if (--count == 0) return;
        }
}

static void Scramble(int[][] board, int mines)
{
    int h = board.Length - 1;
    for (int y = 1; y < h; y++)
    {
        int w = board[y].Length - 1;
        for (int x = 1; x < w; x++)
        {
            int randY = Random.Shared.Next(1, h);
            int randX = Random.Shared.Next(1, w);
            (board[y][x], board[randY][randX]) = (board[randY][randX], board[y][x]);
            if (--mines == 0) return;
        }
    }
}

static void PrintBoard(int[][] board, bool debug = false)
{
    for (int y = 1; y < board.Length - 1; y++)
    {
        for (int x = 1; x < board[y].Length - 1; x++)
            Console.Write(board[y][x] switch
            {
                < 10 => " ■",
                < 20 => " ƒ",
                < 30 => " ?",
                30 => " ≡",
                31 => " 1",
                32 => " 2",
                33 => " 3",
                34 => " 4",
                35 => " 5",
                36 => " 6",
                37 => " 7",
                38 => " 8",
                39 => " ╬",
                _ => throw new Exception("Unexpected state!")
            });
        Console.WriteLine();
    }
}

static int GetInt(string message, Func<int, bool>? condition = null, string error = "Invalid input")
{
    condition ??= _ => true;
    int result;
    Console.Write(message);
    while (!int.TryParse(Console.ReadLine(), out result) || !condition(result)) Console.WriteLine(error);
    return result;
}