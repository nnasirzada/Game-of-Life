using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameOfLife
{
    /********************** DEMO VIDEO LINK ***********************
     * https://www.facebook.com/nzrnsrzd/videos/2036753939922410/ *
     **************************************************************/
    public partial class Form1 : Form
    {
        Cell[,] currentGeneration;
        List<byte[]> frames;

        public Form1()
        {
            InitializeComponent();
            currentGeneration = Game.InitializeGame();
            frames = new List<byte[]>();
            timer1.Interval = Properties.Settings.Default.TickRate;
            timer1.Tick += new EventHandler(timerTick);
            textBox1.Text = string.Format("{0}, Generation: {1}", Game.GetCellStats(currentGeneration), Game.generationCount++);
        }

        /************** BONUS 1 *************/
        
        /* Load button click event handler. */
        private void LoadGeneration(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Text Files (.txt)|*.txt";
            openFileDialog1.InitialDirectory = "C:\\Users\\Public\\Documents";
            openFileDialog1.FileName = "seed.txt";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                var fileName = openFileDialog1.FileName;
                string[] lines = File.ReadAllLines(fileName);
                int i = 0, j = 0;
                
                foreach(var line in lines)
                {
                    var row = line.Split(' ');

                    foreach(var value in row)
                    {
                        currentGeneration[i, j++] = new Cell(Convert.ToInt32(value));
                    }
                    j = 0;
                    i++;
                }
            }
            Game.generationCount = 1;
            textBox1.Text = string.Format("{0}, Generation: {1}", Game.GetCellStats(currentGeneration), Game.generationCount);
            pictureBox1.Invalidate();
        }
        /* Save button click event handler. */
        private void SaveCurrentGeneration(object sender, EventArgs e)
        {
            saveFileDialog1.FileName = "seed.txt";
            saveFileDialog1.InitialDirectory = "C:\\Users\\Public\\Documents";
            saveFileDialog1.Filter = "Text files (*.txt)|*.txt";

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                using (var stream = new StreamWriter(saveFileDialog1.FileName))
                {
                    for (int i = 0; i < Game.COLUMNS; i++)
                    {
                        for (int j = 0; j < Game.ROWS; j++)
                        {
                            /* Writes current state of each cell to a text file using StreamWriter. */
                            stream.Write(currentGeneration[i, j]);
                            /* If end of row is reached and current cell is not last one then put new line */
                            if (j == Game.ROWS - 1)
                            {
                                if (i != Game.COLUMNS - 1)
                                {
                                    stream.Write("\n");
                                }
                            }
                            /* If it's not end of row then put a space between values */
                            else
                            {
                                stream.Write(" ");
                            }
                        }
                    }
                }
                textBox1.Text = "File Saved! " + saveFileDialog1.FileName;
            }
        }

        /************** BONUS 5 *************/

        /* Open button click event handler. */
        private void OpenGif(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "GIF Files (.GIF)|*.GIF";
            openFileDialog1.InitialDirectory = "C:\\Users\\Public\\Pictures";
            openFileDialog1.FileName = "";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string fileName = openFileDialog1.FileName;
                string extension = Path.GetExtension(fileName);
                /* If selected file is GIF then open it with default application. Otherwise, display error message. */
                if(extension.ToLower() == ".gif")
                {
                    System.Diagnostics.Process.Start(fileName);
                }
                else
                {
                    textBox1.Text = "Unsupported file format.";
                }
            }
        }
        /* Export button click event handler. */
        private void ExportAsGif(object sender, EventArgs e)
        {
            saveFileDialog1.FileName = DateTime.Now.ToString("HHmmss") + ".GIF";
            saveFileDialog1.InitialDirectory = "C:\\Users\\Public\\Pictures";
            saveFileDialog1.Filter = "GIF files (*.GIF)|*.GIF";

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                /* Save last frame before exporting. */
                GifUtils.SaveFrame(ref frames, pictureBox1);
                GifUtils.ExportGif(frames, saveFileDialog1.FileName);
                textBox1.Text = "GIF Exported! " + saveFileDialog1.FileName;
            }  
        }
        /* Closes application when exit button is clicked. */
        private void ExitApplication(object sender, EventArgs e)
        {
            Close();
        }
        /* Start button click event handler. Starts timer. */
        private void LifeStart(object sender, EventArgs e)
        {
            timer1.Start();
            startToolStripMenuItem.Enabled = false;
            stopToolStripMenuItem.Enabled = true;
            stepToolStripMenuItem.Enabled = false;
        }
        /* Stop button click event handler. Stops timer. */
        private void LifeStop(object sender, EventArgs e)
        {
            timer1.Stop();
            startToolStripMenuItem.Enabled = true;
            stopToolStripMenuItem.Enabled = false;
            stepToolStripMenuItem.Enabled = true;
        }
        /* Step button click event handler. Helps to run game one step a time. */
        private void LifeStep(object sender, EventArgs e)
        {
            timerTick(sender, e);
        }
        /* Random button click event handler. */
        private void LifeRandom(object sender, EventArgs e)
        {
            currentGeneration = Game.InitializeGame();
            Game.generationCount = 1;
            frames.Clear();
            textBox1.Text = string.Format("{0}, Generation: {1}", Game.GetCellStats(currentGeneration), Game.generationCount);
            pictureBox1.Invalidate();
        }
        /* Clear button click event handler. */
        private void LifeClear(object sender, EventArgs e)
        {
            for(int i = 0; i < Game.COLUMNS; i++)
            {
                for(int j = 0; j < Game.COLUMNS; j++)
                {
                    currentGeneration[i, j] = new Cell();
                }
            }
            Game.generationCount = 1;
            frames.Clear();
            textBox1.Text = string.Format("{0}, Generation: {1}", Game.GetCellStats(currentGeneration), Game.generationCount);
            pictureBox1.Invalidate();
        }
        /* PictureBox Click event handler */
        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            /* Get X and Y coordinates from event, and then map these numbers to array indexes relatively. */
            Point p = e.Location;
            var x = (int)Math.Floor(p.X / (double)Game.CELL_SIZE);
            var y = (int)Math.Floor(p.Y / (double)Game.CELL_SIZE);

            /* Set state of the cell ALIVE when left button of mouse is clicked. */
            if (e.Button == MouseButtons.Left)
            {
                currentGeneration[x, y] = new Cell(Cell.ALIVE);
            }
            /* Set state of the cell DEAD when right button of mouse is clicked. */
            else if (e.Button == MouseButtons.Right)
            {
                currentGeneration[x, y] = new Cell(Cell.DEAD);
            }
            textBox1.Text = string.Format("{0}, Generation: {1}", Game.GetCellStats(currentGeneration), Game.generationCount);
            /* Redraw everything on PictureBox */
            pictureBox1.Invalidate();
        }

        /* When PictureBox is redrawn, this method handles Paint event */
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            draw(e);
        }

        private void draw(PaintEventArgs e)
        {
            /* Clear surface */
            e.Graphics.Clear(pictureBox1.BackColor);

            for (int i = 0; i < Game.COLUMNS; i++)
            {
                for (int j = 0; j < Game.ROWS; j++)
                {
                    var x = i * Game.CELL_SIZE;
                    var y = j * Game.CELL_SIZE;
                    /* Subtract 1 from width and height of cells to simulate drawing grid lines */
                    Rectangle r = new Rectangle(x, y, Game.CELL_SIZE - 1, Game.CELL_SIZE - 1);
                    using (Pen p = new Pen(currentGeneration[i, j].Color, 1))
                    {
                        e.Graphics.FillRectangle(p.Brush, r);
                    }
                }
            }
        }
        /* Called by timer during every step of simulation of game. */
        private void timerTick(object sender, EventArgs e)
        {
            /* Save current frame before generating next generation. */
            GifUtils.SaveFrame(ref frames, pictureBox1);
            /* Generate next generation */
            Game.NewGeneration(ref currentGeneration);
            textBox1.Text = string.Format("{0}, Generation: {1}", Game.GetCellStats(currentGeneration), Game.generationCount++);
            pictureBox1.Invalidate();
        }

        /************** BONUS 2 *************/
        
        /* Let user choose timer interval from available 3 choices: 250, 500, 1000 ms. */
        private void changeTimerInterval(object sender, EventArgs e)
        {
            var item = sender as ToolStripMenuItem;
            int ms = Convert.ToInt32(item.Text);

            Properties.Settings.Default.TickRate = ms;
            Properties.Settings.Default.Save();

            /* If timer is running, restarts it with a new interval. */
            if(timer1.Enabled)
            {
                timer1.Stop();
                timer1.Interval = ms;
                timer1.Start();
            } else
            {
                timer1.Interval = ms;
            }
        }
        /* Let user choose dead cell color from available 2 choices: Black or White. */
        private void changeDeadCellColor(object sender, EventArgs e)
        {
            var item = sender as ToolStripMenuItem;
            switch(item.Text)
            {
                case "Black":
                    Properties.Settings.Default.DeadCellColor = Color.Black;
                    break;
                case "White":
                    Properties.Settings.Default.DeadCellColor = Color.White;
                    break;
            }
            Properties.Settings.Default.Save();
            /* Loop through cells and redraw them to reflect changes i.e. new colors immediately. */
            foreach (var cell in currentGeneration)
            {
                cell.ChangeColor();
            }
            pictureBox1.Invalidate();
        }
    }
}
