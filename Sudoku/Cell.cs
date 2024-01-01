namespace Sudoku
{
    class Cell
    {
        public Cell(Coordinate coordinate, int number, CellList box, CellList row, CellList column)
        {
            Coordinate = coordinate;
            Number = number;
            Box = box;
            Row = row;
            Column = column;
        }

        public CellList Box { get; }
        public CellList Column { get; }
        public Coordinate Coordinate { get; }
        public int Number { get; set; }
        public CellList Row { get; }

        public void CheckFilledCellValidity()
        {
            var isEmptyCell = Number == 0;
            if (isEmptyCell)
            {
                return;
            }

            var rowNumberCount = Row.Cells.Count(c => c.Number == Number);
            var columnNumberCount = Column.Cells.Count(c => c.Number == Number);
            var boxNumberCount = Box.Cells.Count(c => c.Number == Number);

            if (rowNumberCount > 1 || columnNumberCount > 1 || boxNumberCount > 1)
            {
                throw new InvalidDataException($"Cell ({Coordinate.X},{Coordinate.Y}) doesn't comply with the Sudoku rules.");
            }
        }

        public int GetFillableNumber(int currentNumber, IList<int> numberChoices)
        {
            var currentNumberIndex = numberChoices.IndexOf(currentNumber);

            if (currentNumberIndex == 8)
            {
                return 0;
            }

            var nextNumber = numberChoices[currentNumberIndex + 1];

            var isExisted = Box.Cells.Where(cell => cell.Number == nextNumber).Any() ||
                Row.Cells.Where(cell => cell.Number == nextNumber).Any() ||
                Column.Cells.Where(cell => cell.Number == nextNumber).Any();

            if (!isExisted)
            {
                return nextNumber;
            }

            return GetFillableNumber(nextNumber, numberChoices);
        }

        public int GetMaxFilledCellsCount()
        {
            var rowFilledCellsCount = Row.Cells.Count(c => c.Number != 0);
            var columnFilledCellsCount = Column.Cells.Count(c => c.Number != 0);
            var boxFilledCellsCount = Box.Cells.Count(c => c.Number != 0);
            var maxFilledCellsCount = Math.Max(Math.Max(rowFilledCellsCount, columnFilledCellsCount), boxFilledCellsCount);

            return maxFilledCellsCount;
        }
    }
}