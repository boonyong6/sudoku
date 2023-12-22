using Sudoku;
using System.Diagnostics;

var stopwatch = new Stopwatch();
stopwatch.Start();

try
{
    //SudokuPuzzle puzzle;

    //for (int i = 0; i < 10; i++)
    //{
    //    puzzle = SudokuPuzzle.Create();
    //    Console.WriteLine(puzzle);
    //    //Console.WriteLine();
    //}

    //return;

    var filePath = args.Length > 0 ? args[0] :
        throw new ArgumentNullException("args[0]", "File path of the Sudoku grid is not provided.");

    var sudokuPuzzle = new SudokuPuzzle(filePath);

    var solution = sudokuPuzzle.Solve();

    Console.WriteLine(solution);
}
catch (InvalidOperationException e)
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
    var ts = stopwatch.Elapsed;

    //Console.WriteLine(String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
    //            ts.Hours, ts.Minutes, ts.Seconds,
    //            ts.Milliseconds / 10));

    Console.WriteLine("Timelapsed (ms): " + ts.Milliseconds);
}
