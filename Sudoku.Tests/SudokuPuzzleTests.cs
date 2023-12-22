namespace Sudoku.Tests
{
    public class SudokuPuzzleTests
    {
        [Theory]
        [InlineData("sudoku-puzzle-easy.txt", "581763924\r\n269415873\r\n473928165\r\n694157238\r\n812396547\r\n357284691\r\n135672489\r\n728549316\r\n946831752\r\n")]
        [InlineData("sudoku-puzzle-medium.txt", "586913472\r\n324687951\r\n179254638\r\n743162589\r\n658479123\r\n291538746\r\n962345817\r\n815726394\r\n437891265\r\n")]
        [InlineData("sudoku-puzzle-hard.txt", "359764182\r\n716582493\r\n824913756\r\n691278534\r\n483159267\r\n275436918\r\n937825641\r\n562341879\r\n148697325\r\n")]
        public void SolveSudoku(string filePath, string expected)
        {
            // arange
            var sudokuPuzzle = new SudokuPuzzle(filePath);

            // act
            var actual = sudokuPuzzle.Solve();

            // assert
            Assert.Equal(expected, actual);
        }

        // Test puzzle that has more than one solutions.
    }
}