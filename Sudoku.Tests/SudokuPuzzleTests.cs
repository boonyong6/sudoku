namespace Sudoku.Tests
{
    public class SudokuPuzzleTests
    {
        [Theory]
        [InlineData(@"TestData\sudoku-puzzle-easy.txt", "581763924\r\n269415873\r\n473928165\r\n694157238\r\n812396547\r\n357284691\r\n135672489\r\n728549316\r\n946831752\r\n")]
        [InlineData(@"TestData\sudoku-puzzle-medium.txt", "586913472\r\n324687951\r\n179254638\r\n743162589\r\n658479123\r\n291538746\r\n962345817\r\n815726394\r\n437891265\r\n")]
        [InlineData(@"TestData\sudoku-puzzle-hard.txt", "359764182\r\n716582493\r\n824913756\r\n691278534\r\n483159267\r\n275436918\r\n937825641\r\n562341879\r\n148697325\r\n")]
        public void Solve_ValidSudoku_UniqueSolutionReturned(string filePath, string answer)
        {
            var sudokuPuzzle = SudokuPuzzle.Create(filePath.Replace('\\', Path.DirectorySeparatorChar));

            var solution = sudokuPuzzle.Solve();

            Assert.Equal(answer.Replace("\r\n", Environment.NewLine), solution.Value);
            Assert.Equal(1, solution.Count);
        }

        [Theory]
        [InlineData(@"TestData\sudoku-puzzle-not-well-posed.txt")]
        public void Solve_NotWellPosedSudoku_HasMoreThanOneSolution(string filePath)
        {
            var sudokuPuzzle = SudokuPuzzle.Create(filePath.Replace('\\', Path.DirectorySeparatorChar));

            var solution = sudokuPuzzle.Solve();

            Assert.True(solution.Count > 1);
        }

        [Theory]
        [InlineData(@"TestData\sudoku-puzzle-malformed-extra-row.txt")]
        [InlineData(@"TestData\sudoku-puzzle-malformed-extra-row-items.txt")]
        [InlineData(@"TestData\sudoku-puzzle-malformed-repeated-number.txt")]
        public void Instantiate_MalformedSudoku_ThrowInvalidOperationException(string filePath)
        {
            Assert.Throws<InvalidDataException>(() => SudokuPuzzle.Create(filePath.Replace('\\', Path.DirectorySeparatorChar)));
        }

        [Fact]
        public void Create_ValidSudoku_HasUniqueSolution()
        {
            var sudokuPuzzle = SudokuPuzzle.Create();

            var solution = sudokuPuzzle.Solve();

            Assert.Equal(1, solution.Count);
        }

        /* Commands to generate code coverage report (web pages):
         * 
         * coverlet .\bin\Debug\net6.0\Sudoku.Tests.dll --target "dotnet" --targetargs "test --no-build"
         * 
         * dotnet test --collect:"XPlat Code Coverage"
         * 
         * reportgenerator -reports:"Path\To\TestProject\TestResults\{guid}\coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:Html
        */
    }
}