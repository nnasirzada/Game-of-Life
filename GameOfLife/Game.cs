using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLife
{
    static class Game
    {
        public const int CELL_SIZE = 8, COLUMNS = 50, ROWS = 50;
        public static int generationCount = 1;
        private static Random random = new Random();

        static Game() { }
        /* Called once when the game is launched. Returns array of cells generated randomly. */
        public static Cell[,] InitializeGame()
        {
            Cell[,] initialGeneration = new Cell[COLUMNS, ROWS];
            for (int i = 0; i < Game.COLUMNS; i++)
            {
                for (int j = 0; j < Game.ROWS; j++)
                {
                    initialGeneration[i, j] = new Cell(random.Next(0, 2));
                }
            }
            return initialGeneration;
        }
        /* Creates further generations */
        public static void NewGeneration(ref Cell[,] currentGeneration)
        {
            Cell[,] nextGeneration = new Cell[COLUMNS, ROWS];
            for (var i = 0; i < Game.COLUMNS; i++)
            {
                for (var j = 0; j < Game.ROWS; j++)
                {
                    var currentCell = (Cell) currentGeneration[i, j].Clone();
                    var state = currentCell.State;
                    /* Calculate neighbors of the current cell and store the returned value. */
                    var neighbors = Game.CountNeighbors(currentGeneration, i, j);

                    /* Rule #4: Any dead cell with exactly 3 neighbors becomes an alive cell. */
                    if (state == Cell.DEAD && neighbors == 3)
                    {
                        nextGeneration[i, j] = currentCell;
                        nextGeneration[i, j].State = Cell.ALIVE;
                        nextGeneration[i, j].Age = 1;
                    }
                    /* Rule #1 and #3: Any alive cell with fewer than 2 or more than 3 alive neighbors dies. */
                    else if (state == Cell.ALIVE && (neighbors < 2 || neighbors > 3))
                    {
                        nextGeneration[i, j] = currentCell;
                        nextGeneration[i, j].State = Cell.DEAD;
                        nextGeneration[i, j].Age = 0;
                        /* Later WasAlive property will be used to distinguish cells which were once alive from dead ones. */
                        nextGeneration[i, j].WasAlive = true;
                    }
                    /* The rest stays same. Follows Rule #2: Any alive cell with 2 or 3 neighbors lives on the next generation. */
                    else
                    {
                        nextGeneration[i, j] = currentCell;
                        /* Increase age of the alive cell as it'll keep living on the next generation. */
                        if (currentCell.State == Cell.ALIVE)
                        {
                            nextGeneration[i, j].Age++;
                        }
                    }
                    /* Regenerate colors based on changed properties. */
                    nextGeneration[i, j].ChangeColor();
                }
            }
            currentGeneration = nextGeneration.Clone() as Cell[,];
        }
        /* Takes cell array, x and y coordinates (or array indexes). Returns the number of alive neighbors of cell in given coordinates. */
        public static int CountNeighbors(Cell[,] currentGeneration, int x, int y)
        {
            int sum = 0;
            for (var i = -1; i < 2; i++)
            {
                for (var j = -1; j < 2; j++)
                {
                    /* Ignores neighbors outside edges */
                    if (!(x + i < 0 || x + i >= ROWS || y + j < 0 || y + j >= COLUMNS))
                    {
                        var col = x + i;
                        var row = y + j;
                        sum += currentGeneration[col, row].State;
                    }
                }
            }
            /* At the end, subtracts value of given cell because the loop above counts it as a neighbor too. */
            sum -= currentGeneration[x, y].State;
            return sum;
        }
        /* Takes current generation, calculates the number of alive and dead cells and returns as a formatted string. */
        public static string GetCellStats(Cell[,] currentGeneration)
        {
            int alive = 0, dead = 0;
            for(int i = 0; i < Game.COLUMNS; i++)
            {
                for(int j = 0; j < Game.ROWS; j++)
                {
                    if (currentGeneration[i,j].State == Cell.ALIVE)
                    {
                        alive++;
                    } else
                    {
                        dead++;
                    }
                }
            }
            return string.Format("Alive: {0}, Dead: {1}", alive, dead);
        }
    }
}
