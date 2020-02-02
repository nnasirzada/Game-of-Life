using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLife
{
    class Cell : ICloneable
    {
        /* Cell State Constants*/
        public const int DEAD = 0;
        public const int ALIVE = 1;

        private const int RGB_MAX = 255; 
        private const int RGB_MIN = 0; 
        /* Default constructor. */
        public Cell()
        {
        }
        /* Constructor with state variable. Sets color of the cell according to given state. */
        public Cell(int state)
        {
            this.State = state;
            this.Color = (state == ALIVE) ? Color.Red : Properties.Settings.Default.DeadCellColor;
        }

        public int Age { get; set; } = 0;
        public int State { get; set; } = DEAD;
        public bool WasAlive { get; set; } = false;
        public Color Color { get; set; } = Properties.Settings.Default.DeadCellColor;

        /************** BONUS 3 & 4 *************/
        public void ChangeColor()
        {
            /* Color of dead cells */
            if (State == DEAD)
            {
                /* If the cell was once alive make its color slighly different from default color for a dead cell. */
                if (WasAlive)
                {
                    switch(Properties.Settings.Default.DeadCellColor.Name)
                    {
                        case "Black":
                            this.Color = Color.FromArgb(25, 25, 25);
                            break;
                        case "White":
                            this.Color = Color.FromArgb(229, 229, 229);
                            break;
                    }
                }
                /* If the cell was never alive, set its default color */
                else
                {
                    this.Color = Properties.Settings.Default.DeadCellColor;
                }
            }
            /* Color of alive cells */
            else
            {
                /* Reference: https://stackoverflow.com/a/27901262 */

                var percentage = (Age > 10) ? 100 : Age * 10;
                /* percentage of the range from RGB_MIN to RGB_MAX) */
                var redPercent = Math.Min(200 - (percentage * 2), 100) / 100f;
                var greenPercent = Math.Min(percentage * 2, 100) / 100f;
                /* Convert those percentages to actual RGB values in the range */
                var red = RGB_MIN + ((RGB_MAX - RGB_MIN) * redPercent);
                var green = RGB_MIN + ((RGB_MAX - RGB_MIN) * greenPercent);
                /* No blue color. 
                   The youngest cell will have red color while the oldest will have green */
                this.Color = Color.FromArgb((int)red, (int)green, RGB_MIN);
            }
        }

        /* Creates copy of the current instance.  */
        public object Clone()
        {
            /* Create a new object, copy the nonstatic fields of the current object to the new object and return it. */
            return this.MemberwiseClone();
        }

        /* String representation of the current instance. Used when writing cells to a file. */
        public override string ToString()
        {
            return string.Format("{0}", this.State);
        }
    }
}
