namespace Minesweeper
{
    public class Item 
    {
        public bool HasFlag { get; set; } = false;
        public bool IsMine { get; set; } = false;
        public bool IsRevealed { get; set; } = false;
        public int MineCount { get; set; } = 0;

        public string GetDisplay()
        {
            if (IsRevealed)
            {
                if (IsMine)
                {
                    return "M";
                }
                return $"{MineCount}";
            }
            else
            {
                if (HasFlag)
                {
                    return "F";
                }
                return "X";
            }
        }
    }

    public class Game
    {
        public int MineCount { get; private set; } = 10;
        public int RowCount { get; private set; } = 8;
        public int ColumnCount { get; private set; } = 8;
        public int GridSize { get; private set; } = 0;
        public Item[,] Grid { get; private set; }

        public Game(int mineCount, int rowCount, int columnCount)
        {
            MineCount = mineCount;
            RowCount = rowCount;
            ColumnCount = columnCount;
            GridSize = RowCount * ColumnCount;
            Grid = CreateGrid();
        }

        private Item[,] CreateGrid()
        {
            var CreatedGrid = new Item[RowCount, ColumnCount];
            for (int i = 0; i < RowCount; i++)
            {
                for (int j = 0; j < ColumnCount; j++)
                {
                    CreatedGrid[i, j] = new Item();
                }
            }
            return CreatedGrid;
        }

        public void PlaceMines()
        {
            var random = new Random();
            int mineCounter = 0;
            while (mineCounter < MineCount)
            {
                int randomRowIndex = random.Next(0, RowCount);
                int randomColumnIndex = random.Next(0, ColumnCount);
                if (!Grid[randomRowIndex, randomColumnIndex].IsMine)
                {
                    Grid[randomRowIndex, randomColumnIndex].IsMine = true;
                    mineCounter++;
                }
            }
        }

        public void PrintGrid()
        {
            for (int i = 0; i < RowCount; i++)
            {
                for (int j = 0; j < ColumnCount; j++)
                {
                    Console.Write($"{Grid[i, j].GetDisplay()} ");
                }
                Console.WriteLine();
            }
        }

        public void Reveal(int row, int column)
        {
            if (row >= 0 && row < RowCount && column >= 0 && column < ColumnCount)
            {
                Grid[row, column].IsRevealed = true;
            }
            else
            {
                Console.WriteLine("You cannot reveal an item out of range!");
            }
        }

        public void PutFlag(int row, int column)
        {
            if (row >= 0 && row < RowCount && column >= 0 && column < ColumnCount)
            {
                Grid[row, column].HasFlag = !Grid[row, column].HasFlag;
            }
            else
            {
                Console.WriteLine("You cannot put a flag in an item out of range!");
            }
        }

        private List<Tuple<int, int>> GetNeighbors(int row, int column, int[,] directions, int rowCount, int columnCount)
        {
            List<Tuple<int, int>> neighbors = new List<Tuple<int, int>>();
            for (int i = 0; i < directions.GetLength(0); i++)
            {
                int newRow = row + directions[i, 0];
                int newCol = column + directions[i, 1];
                if (newRow >= 0 && newRow < rowCount && newCol >= 0 && newCol < columnCount)
                {
                    neighbors.Add(Tuple.Create(newRow, newCol));
                }
            }

            return neighbors;
        }

        public void RevealAdjacents(int rowIndex, int columnIndex)
        {
            int[,] directions = new int[,]
            {
                {-1, -1}, {-1, 0}, {-1, 1},
                { 0, -1},          { 0, 1},
                { 1, -1}, { 1, 0}, { 1, 1}
            };

            List<Tuple<int, int>> neighbors = GetNeighbors(rowIndex, columnIndex, directions, RowCount, ColumnCount);

            int counter = 0;
            foreach (var neighbor in neighbors)
            {
                if (Grid[neighbor.Item1, neighbor.Item2].IsMine)
                {
                    counter++;
                }
            }
            Grid[rowIndex, columnIndex].MineCount = counter;
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            int mineCount;
            int rowCount;
            int columnCount;
            int gridSize;
            int rowIndex;
            int columnIndex;
            int option;
            string input = string.Empty;
            string[] parts = null;
            bool gameOver = false;

            do
            {
                Console.Write("Enter row count: ");
                rowCount = Convert.ToInt32(Console.ReadLine());
                if (rowCount <= 0)
                {
                    Console.WriteLine("You need to enter a valid number, greater than zero!");
                }
            } while (rowCount <= 0);

            do
            {
                Console.Write("Enter column count: ");
                columnCount = Convert.ToInt32(Console.ReadLine());
                if (columnCount <= 0)
                {
                    Console.WriteLine("You need to enter a valid number, greater than zero!");
                }
            } while (columnCount <= 0);

            gridSize = rowCount * columnCount;
            do
            {
                Console.Write($"Enter the mine count [not greater than {gridSize}]: ");
                mineCount = Convert.ToInt32(Console.ReadLine());
            } while (mineCount > gridSize);
            var game = new Game(mineCount, rowCount, columnCount);
            game.PlaceMines();

            while (!gameOver)
            {
                game.PrintGrid();
                Console.Write("Enter row and column: ");
                input = Console.ReadLine();
                parts = input.Split(' ');
                if (!(parts.Length == 2 && int.TryParse(parts[0], out rowIndex) && int.TryParse(parts[1], out columnIndex)))
                {
                    Console.WriteLine("Invalid input. Please enter two integers separated by a space.");
                    continue;
                }

                do
                {
                    Console.Write("Enter 1 for revealing, enter 2 for putting/removing a flag: ");
                    option = Convert.ToInt32(Console.ReadLine());
                } while (option < 1 || option > 2);
                if (option == 1)
                {
                    game.Reveal(rowIndex, columnIndex);
                    if (game.Grid[rowIndex, columnIndex].IsMine)
                    {
                        game.PrintGrid();
                        Console.WriteLine("GAME OVER!");
                        gameOver = true;
                    }
                    else
                    {
                        game.RevealAdjacents(rowIndex, columnIndex);
                    }
                }
                else if (option == 2)
                {
                    game.PutFlag(rowIndex, columnIndex);
                }



                
                Console.WriteLine();
            }
        }
    }
}