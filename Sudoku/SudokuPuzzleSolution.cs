namespace Sudoku
{
    public class SudokuPuzzleSolution
    {
        public string Value { get; }
        public int Count { get; }

        public SudokuPuzzleSolution(string value, int count)
        {
            Value = value;
            Count = count;
        }
    }
}
