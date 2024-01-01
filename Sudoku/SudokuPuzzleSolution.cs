namespace Sudoku
{
    public class SudokuPuzzleSolution
    {
        public SudokuPuzzleSolution(string value, int count)
        {
            Value = value;
            Count = count;
        }

        public int Count { get; }
        public string Value { get; }
    }
}