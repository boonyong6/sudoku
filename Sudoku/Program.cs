using System.Diagnostics;

var stopwatch = new Stopwatch();
stopwatch.Start();

try
{
    var filePath = args.Length > 0 ? args[0] : 
        throw new ArgumentNullException("args[0]", "File path of the Sudoku grid is not provided.");

    var grid = LoadGridFromFile(filePath);

    // Determine empty cells
    var emptyCellsCoordinates = new List<(int x, int y)>();
    for (int row = 0; row < 9; row++)
    {
        for (int col = 0; col < 9; col++)
        {
            if (grid[row, col] == 0)
            {
                emptyCellsCoordinates.Add((row, col));
            }
        }
    }

    int i = 0;
    while (i < emptyCellsCoordinates.Count)
    {
        var isFilled = FillEmptyCell(grid, emptyCellsCoordinates[i]);

        if (isFilled)
        {
            // Proceed to the next empty cell
            i++;
        }
        else
        {
            // No possible numbers for the current empty cell, backtrack and try again.
            i--;
        }

        if (i < 0)
        {
            Console.WriteLine("Malformed Sudoku - There is no solution for the given Sudoku puzzle.");
            break;
        }
    }

    // TODO: Revalidate solved Sudoku

    PrintGrid(grid);

    stopwatch.Stop();
    var ts = stopwatch.Elapsed;

    Console.WriteLine(String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10));
}
catch (Exception e)
{
    Console.WriteLine(e.Message);
}

int[,] LoadGridFromFile(string filePath)
{
    string? line;
    var grid = new int[9, 9];
    int row = 0;

    try
    {
        StreamReader sr = new StreamReader(filePath);
        line = sr.ReadLine();
        while (line != null)
        {
            for (int i = 0; i < line.Length; i++)
            {
                grid[row, i] = int.Parse(line[i].ToString());
            }

            line = sr.ReadLine();
            row++;
        }

        sr.Close();

        return grid;
    }
    catch (Exception)
    {
        throw;
    }
}

bool FillEmptyCell(int[,] grid, (int x, int y) emptyCellCoordinate)
{
    (int x, int y) subgridCoordinate = GetSubgridCoordinate(emptyCellCoordinate);

    var candidateNumber = grid[emptyCellCoordinate.x, emptyCellCoordinate.y];

    // TODO: Use Minimum Remaining Values (MRV) algorithm to select the next candidate number

    while (candidateNumber < 9)
    {
        candidateNumber++;

        if (IsCandidateNumberFillable(grid, subgridCoordinate, emptyCellCoordinate, candidateNumber))
        {
            grid[emptyCellCoordinate.x, emptyCellCoordinate.y] = candidateNumber;
            return true;
        }
    }

    // Reset empty cell (No possible numbers to fill in)
    grid[emptyCellCoordinate.x, emptyCellCoordinate.y] = 0;

    return false;
}

(int x, int y) GetSubgridCoordinate((int x, int y) cellCoordinate)
{
    return (GetSubgridAxis(cellCoordinate.x), GetSubgridAxis(cellCoordinate.y));
}

int GetSubgridAxis(int cellAxis)
{
    if (cellAxis < 3)
    {
        return 0;
    }

    if (cellAxis < 6)
    {
        return 3;
    }

    return 6;
}

bool IsCandidateNumberFillable(int[,] grid, (int x, int y) subgridCoordinate, (int x, int y) emptyCellCoordinate, int candidateNumber)
{
    // Search the candidate number in the subgrid.
    for (int row = subgridCoordinate.x; row < subgridCoordinate.x + 3; row++)
    {
        for (int col = subgridCoordinate.y; col < subgridCoordinate.y + 3; col++)
        {
            var currentCellNumber = grid[row, col];

            if (currentCellNumber == 0)
            {
                continue;
            }

            if (candidateNumber == currentCellNumber)
            {
                return false;
            }
        }
    }

    // Search the candidate number in the row and column
    for (int i = 0; i < 9; i++)
    {
        if (candidateNumber == grid[emptyCellCoordinate.x, i] || 
            candidateNumber == grid[i, emptyCellCoordinate.y])
        {
            return false;
        }
    }

    return true;
}

void PrintGrid(int[,] grid)
{
    for (int i = 0; i < 9; i++)
    {
        Console.WriteLine("+---+---+---+---+---+---+---+---+---+");
        Console.Write("|");

        for (int j = 0; j < 9; j++)
        {
            if (grid[i, j] == 0)
            {
                Console.Write("   |");
                continue;
            }

            Console.Write($" {grid[i, j]} |");
        }

        Console.WriteLine();
    }

    Console.WriteLine("+---+---+---+---+---+---+---+---+---+");
}
