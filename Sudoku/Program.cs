using Sudoku;
using System.Diagnostics;

var stopwatch = new Stopwatch();
stopwatch.Start();

try
{
    var filePath = args.Length > 0 ? args[0] :
        throw new ArgumentNullException("args[0]", "File path of the Sudoku grid is not provided.");
    var sudokuPuzzle = SudokuPuzzle.Create(filePath);

    var solution = sudokuPuzzle.Solve();

    Console.WriteLine(solution.Value);
}
catch (InvalidDataException e)
{
    Console.WriteLine(e.Message);
}
catch (Exception)
{
    throw;
}
finally
{
    stopwatch.Stop();
    var elapsedTime = stopwatch.Elapsed;
    Console.WriteLine("Elapsed Time (ms): " + elapsedTime.Milliseconds);
}