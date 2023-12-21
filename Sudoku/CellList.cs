using System.Text;

namespace Sudoku
{
    public class CellList
    {
        public List<Cell> Cells { get; } = new List<Cell>(9);
        public (int x, int y) StartingCoordinate { get; }

        public CellList((int x, int y) startingCoordinate)
        {
            StartingCoordinate = startingCoordinate;
        }

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
