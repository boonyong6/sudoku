using System.Text;

namespace Sudoku
{
    public class SudokuPuzzle
    {
        public List<CellList> Boxes { get; }
        public List<CellList> Rows { get; }
        public List<CellList> Columns { get; }

        private readonly List<Cell> _cells;
        private readonly List<int> _numberChoices;
        
        private int _recursionCount = 0;

        public SudokuPuzzle(string filePath) : this()
        {
            Load(filePath);
        }

        private SudokuPuzzle()
        {
            Boxes = new List<CellList>(9)
            {
                new CellList((0, 0)),
                new CellList((0, 3)),
                new CellList((0, 6)),
                new CellList((3, 0)),
                new CellList((3, 3)),
                new CellList((3, 6)),
                new CellList((6, 0)),
                new CellList((6, 3)),
                new CellList((6, 6))
            };
            Rows = new List<CellList>(Enumerable.Range(0, 9).Select(x => new CellList((x, 0))));
            Columns = new List<CellList>(Enumerable.Range(0, 9).Select(x => new CellList((0, x))));
            _cells = new List<Cell>();
            _numberChoices = new List<int>(9) { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        }

        public static SudokuPuzzle Create()
        {
            var sudokuPuzzle = new SudokuPuzzle();

            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    sudokuPuzzle.LoadCell(new Coordinate(row, col), 0);
                }
            }

            var rng = new Random();
            var numberChoices = sudokuPuzzle._numberChoices.OrderBy(x => rng.Next()).ToList();

            var solutions = sudokuPuzzle.Solve(numberChoices);
            sudokuPuzzle.ApplySolution(solutions[0]);

            var attemptCount = 0;
            var randomCellIndex = -1;
            var prevRemovedNumber = 0;

            while (attemptCount < 64)
            {
                attemptCount++;

                randomCellIndex = rng.Next(0, 80);
                prevRemovedNumber = sudokuPuzzle._cells[randomCellIndex].Number;
                sudokuPuzzle._cells[randomCellIndex].Number = 0;

                solutions = sudokuPuzzle.Solve(sudokuPuzzle._numberChoices);

                if (solutions.Count == 0 || solutions.Count > 1)
                {
                    sudokuPuzzle._cells[randomCellIndex].Number = prevRemovedNumber;
                }
            }

            return sudokuPuzzle;
        }

        public string Solve()
        {
            var solutions = Solve(_numberChoices);

            if (solutions.Count == 0)
            {
                throw new InvalidOperationException("Malformed Sudoku puzzle. No solution found.");
            }

            if (solutions.Count > 1)
            {
                Console.WriteLine("[Warning] More than one solution was found.");
            }

            return solutions[0];
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            foreach (var row in Rows)
            {
                sb.AppendLine(row.ToString());
            }

            return sb.ToString();
        }

        private void Load(string filePath)
        {
            string? line;
            int row = 0;

            try
            {
                StreamReader sr = new StreamReader(filePath);
                line = sr.ReadLine();

                while (line != null)
                {
                    for (int col = 0; col < line.Length; col++)
                    {
                        var cellCoordinate = new Coordinate(row, col);
                        var number = int.Parse(line[col].ToString());
                        LoadCell(cellCoordinate, number);
                    }

                    line = sr.ReadLine();
                    row++;
                }

                sr.Close();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void LoadCell(Coordinate cellCoordinate, int number)
        {
            var boxInitialCoordinate = InferBoxStartingCoordinate(cellCoordinate);
            var box = Boxes.Where(box => box.StartingCoordinate == boxInitialCoordinate).Single();

            var cellRow = Rows[cellCoordinate.X];
            var cellColumn = Columns[cellCoordinate.Y];

            var cell = new Cell(cellCoordinate, number, box, cellRow, cellColumn);

            box.Cells.Add(cell);
            cellRow.Cells.Add(cell);
            cellColumn.Cells.Add(cell);
            _cells.Add(cell);
        }

        private (int x, int y) InferBoxStartingCoordinate(Coordinate cellCoordinate)
        {
            return (InferBoxInitialAxis(cellCoordinate.X), InferBoxInitialAxis(cellCoordinate.Y));
        }

        private int InferBoxInitialAxis(int cellAxis)
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
                queuedEmtpyCells[i].Number = GetFillableNumber(
                    queuedEmtpyCells[i], queuedEmtpyCells[i].Number, numberChoices);

                //Console.WriteLine(ToString());
                //Thread.Sleep(100);
                //Console.SetCursorPosition(0, 0);

                // TODO: Refactor nested if
                var isEmpty = queuedEmtpyCells[i].Number == 0;
                if (!isEmpty)
                {
                    var isEndOfQueue = i == queuedEmtpyCells.Count - 1;
                    if (isEndOfQueue)
                    {
                        emptyCells = _cells.Where(c => c.Number == 0).ToList();
                        if (emptyCells.Count == 0)
                        {
                            solutions.Add(ToString());

                            // End the loop once the second solution is found
                            if (solutions.Count > 1)
                            {
                                foreach (var emptyCell in queuedEmtpyCells)
                                {
                                    emptyCell.Number = 0;
                                }

                                break;
                            }

                            // Backtrack for more solutions
                            queuedEmtpyCells[i].Number = 0;
                            i--;
                            continue;
                        }

                        mrvEmptyCell = SelectMrvEmptyCell(emptyCells);
                        queuedEmtpyCells.Add(mrvEmptyCell);
                    }

                    i++;
                    continue;
                }

                i--;

                _recursionCount++;
            }

            //ApplySolution(solutions);

            //Console.WriteLine($"Recursion: {_recursionCount}");

            return solutions;
        }

        private void ApplySolution(string solution)
        {
            var rows = solution.Split("\n");

            for (int x = 0; x < 9; x++)
            {
                for (int y = 0; y < 9; y++)
                {
                    var number = int.Parse(rows[x][y].ToString());
                    var cell = _cells.Single(c => c.Coordinate.X == x && c.Coordinate.Y == y);
                    cell.Number = number;
                }
            }
        }

        /// <summary>
        /// MRV empty cell has the highest number of filled cells in its row, column or box.
        /// </summary>
        /// <param name="emptyCells"></param>
        /// <returns></returns>
        private static Cell SelectMrvEmptyCell(List<Cell> emptyCells)
        {
            var mrvEmptyCell = emptyCells[0];

            for (int i = 1; i < emptyCells.Count; i++)
            {
                if (GetMaxFilledCellsCount(emptyCells[i]) > GetMaxFilledCellsCount(mrvEmptyCell))
                {
                    mrvEmptyCell = emptyCells[i];
                }
            }

            return mrvEmptyCell;
        }

        private static int GetMaxFilledCellsCount(Cell cell)
        {
            var rowFilledCellsCount = cell.Row.Cells.Where(c => c.Number != 0).Count();
            var columnFilledCellsCount = cell.Column.Cells.Where(c => c.Number != 0).Count();
            var boxFilledCellsCount = cell.Box.Cells.Where(c => c.Number != 0).Count();
            var maxFilledCellsCount = Math.Max(Math.Max(rowFilledCellsCount, columnFilledCellsCount), boxFilledCellsCount);

            return maxFilledCellsCount;
        }

        private int GetFillableNumber(Cell emptyCell, int currentNumber, IList<int> numberChoices)
        {
            var currentNumberIndex = numberChoices.IndexOf(currentNumber);

            if (currentNumberIndex == 8)
            {
                return 0;
            }

            var nextNumber = numberChoices[currentNumberIndex + 1];

            var isExisted = emptyCell.Box.Cells.Where(cell => cell.Number == nextNumber).Any() ||
                emptyCell.Row.Cells.Where(cell => cell.Number == nextNumber).Any() ||
                emptyCell.Column.Cells.Where(cell => cell.Number == nextNumber).Any();

            if (!isExisted)
            {
                return nextNumber;
            }

            return GetFillableNumber(emptyCell, nextNumber, numberChoices);
        }

    }
}
