namespace Sudoku
{
    public class Cell
    {
        public Coordinate Coordinate { get; }
        public int Number { get; set; }
        public CellList Box { get; }
        public CellList Row { get; }
        public CellList Column { get; }

        public Cell(Coordinate coordinate, int number, CellList box, CellList row, CellList column)
        {
            Coordinate = coordinate;
            Number = number;
            Box = box;
            Row = row;
            Column = column;
        }
    }
}
