using System.Text;

namespace Sudoku
{
    class CellList
    {
        public CellList(Coordinate startingCoordinate)
        {
            StartingCoordinate = startingCoordinate;
        }

        public List<Cell> Cells { get; } = new List<Cell>(9);
        public Coordinate StartingCoordinate { get; }

        public override string ToString()
        {
            var sb = new StringBuilder();

            foreach (var cell in Cells)
            {
                sb.Append(cell.Number.ToString());
            }

            return sb.ToString();
        }
    }
}