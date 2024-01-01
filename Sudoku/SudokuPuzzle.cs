using System.Text;

namespace Sudoku
{
    public class SudokuPuzzle
    {
        private readonly List<CellList> _boxes;
        private readonly List<CellList> _rows;
        private readonly List<CellList> _columns;
        private readonly List<Cell> _cells;

        private readonly List<int> _numberChoices;

        private SudokuPuzzle()
        {
            _boxes = new List<CellList>(9)
            {
                new CellList(new Coordinate(0, 0)),
                new CellList(new Coordinate(0, 3)),
                new CellList(new Coordinate(0, 6)),
                new CellList(new Coordinate(3, 0)),
                new CellList(new Coordinate(3, 3)),
                new CellList(new Coordinate(3, 6)),
                new CellList(new Coordinate(6, 0)),
                new CellList(new Coordinate(6, 3)),
                new CellList(new Coordinate(6, 6))
            };
            _rows = new List<CellList>(Enumerable.Range(0, 9).Select(x => new CellList(new Coordinate(x, 0))));
            _columns = new List<CellList>(Enumerable.Range(0, 9).Select(x => new CellList(new Coordinate(0, x))));
            _cells = new List<Cell>();
            _numberChoices = new List<int>(9) { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    InitializeCell(new Coordinate(row, col), 0);
                }
            }
        }

        /// <summary>
        /// Create a new Sudoku puzzle.
        /// </summary>
        /// <returns>A new Sudoku puzzle.</returns>
        public static SudokuPuzzle Create()
        {
            var sudokuPuzzle = new SudokuPuzzle();

            var rng = new Random();
            var randomizedNumberChoices = sudokuPuzzle._numberChoices.OrderBy(x => rng.Next()).ToList();

            var solutions = sudokuPuzzle.Solve(randomizedNumberChoices);
            sudokuPuzzle.Load(solutions[0]);

            var attemptCount = 0;
            var randomCellIndex = -1;
            var prevRemovedNumber = 0;

            // Number of attempts to remove a number from Sudoku.
            while (attemptCount < 64)
            {
                attemptCount++;

                randomCellIndex = rng.Next(0, 80);
                prevRemovedNumber = sudokuPuzzle._cells[randomCellIndex].Number;
                sudokuPuzzle._cells[randomCellIndex].Number = 0;

                solutions = sudokuPuzzle.Solve(sudokuPuzzle._numberChoices);

                if (solutions.Count > 1)
                {
                    sudokuPuzzle._cells[randomCellIndex].Number = prevRemovedNumber;
                }
            }

            return sudokuPuzzle;
        }

        /// <summary>
        /// Creates a Sudoku puzzle by loading it from a file.
        /// </summary>
        /// <param name="filePath">File path of the Sudoku puzzle.</param>
        /// <returns>Initialized Sudoku puzzle loaded from a file.</returns>
        /// <exception cref="InvalidDataException"></exception>
        public static SudokuPuzzle Create(string filePath)
        {
            var sudokuPuzzle = new SudokuPuzzle();

            sudokuPuzzle.LoadFromFile(filePath);

            return sudokuPuzzle;
        }

        /// <summary>
        /// Solves the Sudoku puzzle.
        /// </summary>
        /// <returns>Sudoku puzzle's solution.</returns>
        public SudokuPuzzleSolution Solve()
        {
            var solutions = Solve(_numberChoices);

            var solution = new SudokuPuzzleSolution(solutions[0], solutions.Count);
            return solution;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            foreach (var row in _rows)
            {
                sb.AppendLine(row.ToString());
            }

            return sb.ToString();
        }

        /// <summary>
        /// Selects the empty cell that has Minimum Remaining Values (MRV). <br/>
        /// Note: MRV empty cell has the highest number of filled cells in its row, column or box.
        /// </summary>
        /// <param name="emptyCells">Cells with no number.</param>
        /// <returns>Empty cell that has Minimum Remaining Values (MRV).</returns>
        private static Cell SelectMrvEmptyCell(List<Cell> emptyCells)
        {
            var mrvEmptyCell = emptyCells[0];

            for (int i = 1; i < emptyCells.Count; i++)
            {
                if (emptyCells[i].GetMaxFilledCellsCount() > mrvEmptyCell.GetMaxFilledCellsCount())
                {
                    mrvEmptyCell = emptyCells[i];
                }
            }

            return mrvEmptyCell;
        }

        /// <summary>
        /// Checks the validity of filled cells.
        /// </summary>
        /// <exception cref="InvalidDataException">Thrown when a cell causes the Sudoku puzzle to become malformed.</exception>
        private void CheckFilledCellsValidity()
        {
            var filledCells = _cells.Where(c => c.Number != 0);

            foreach (var filledCell in filledCells)
            {
                filledCell.CheckFilledCellValidity();
            }
        }

        private int InferBoxStartingAxis(int cellAxis)
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

        private Coordinate InferBoxStartingCoordinate(Coordinate cellCoordinate)
        {
            return new Coordinate(InferBoxStartingAxis(cellCoordinate.X), InferBoxStartingAxis(cellCoordinate.Y));
        }

        private void InitializeCell(Coordinate cellCoordinate, int number)
        {
            var boxStartingCoordinate = InferBoxStartingCoordinate(cellCoordinate);
            var box = _boxes.Where(box => box.StartingCoordinate.Equals(boxStartingCoordinate)).Single();

            var cellRow = _rows[cellCoordinate.X];
            var cellColumn = _columns[cellCoordinate.Y];

            var cell = new Cell(cellCoordinate, number, box, cellRow, cellColumn);

            box.Cells.Add(cell);
            cellRow.Cells.Add(cell);
            cellColumn.Cells.Add(cell);
            _cells.Add(cell);
        }

        private void Load(string sudokuPuzzle)
        {
            var rows = sudokuPuzzle.Split("\n");

            for (int x = 0; x < 9; x++)
            {
                for (int y = 0; y < 9; y++)
                {
                    var number = int.Parse(rows[x][y].ToString());
                    var cell = _cells.First(c => c.Coordinate.X == x && c.Coordinate.Y == y);
                    cell.Number = number;
                }
            }
        }

        /// <summary>
        /// Loads Sudoku puzzle from a file.
        /// </summary>
        /// <param name="filePath">File path of the Sudoku puzzle.</param>
        /// <exception cref="InvalidDataException">Thrown when Sudoku puzzle is malformed.</exception>
        private void LoadFromFile(string filePath)
        {
            string? line;
            int row = 0;
            var sb = new StringBuilder();

            StreamReader sr = new StreamReader(filePath);
            line = sr.ReadLine();

            while (line != null)
            {
                if (row >= 9)
                {
                    throw new InvalidDataException($"Valid Sudoku must have 9 rows, not {row + 1} row(s). {nameof(filePath)}: {filePath}");
                }

                if (line.Length != 9)
                {
                    throw new InvalidDataException($"Row {row + 1} in file \"{filePath}\" doesn't have exactly 9 numbers.");
                }

                sb.AppendLine(line);

                line = sr.ReadLine();
                row++;
            }

            sr.Close();

            Load(sb.ToString());

            CheckFilledCellsValidity();
        }

        private List<string> Solve(List<int> numberChoices)
        {
            var emptyCells = _cells.Where(c => c.Number == 0).ToList();
            var mrvEmptyCell = SelectMrvEmptyCell(emptyCells);
            var queuedEmtpyCells = new List<Cell>
            {
                mrvEmptyCell
            };

            var solutions = new List<string>();

            int i = 0;
            while (i >= 0 && i < queuedEmtpyCells.Count)
            {
                queuedEmtpyCells[i].Number = queuedEmtpyCells[i]
                    .GetFillableNumber(queuedEmtpyCells[i].Number, numberChoices);

                // Backtrack if the empty cell can't be filled.
                var canBeFilled = queuedEmtpyCells[i].Number != 0;
                if (!canBeFilled)
                {
                    i--;
                    continue;
                }

                // Proceed to fill the next empty cell in the queue.
                var isEndOfQueue = i == queuedEmtpyCells.Count - 1;
                if (!isEndOfQueue)
                {
                    i++;
                    continue;
                }

                // Add the next empty cell to the queue.
                emptyCells = _cells.Where(c => c.Number == 0).ToList();
                if (emptyCells.Any())
                {
                    mrvEmptyCell = SelectMrvEmptyCell(emptyCells);
                    queuedEmtpyCells.Add(mrvEmptyCell);

                    i++;
                    continue;
                }

                solutions.Add(ToString());

                // Backtrack to find more solutions.
                if (solutions.Count < 2)
                {
                    queuedEmtpyCells[i].Number = 0;
                    i--;
                    continue;
                }

                // End the loop once the second solution is found
                foreach (var emptyCell in queuedEmtpyCells)
                {
                    emptyCell.Number = 0;
                }
                break;
            }

            return solutions;
        }
    }
}