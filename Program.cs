using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RetroGameFramework
{
    internal class Program
    {
        static void Main(string[] args)
        {

            // INITIALIZE GAME CONFIGS

            GameForm.Initializer.Title = "Retro Game";
            GameForm.Initializer.PixelSize = 8;
            GameForm.Initializer.BackgroundColor = System.Drawing.Color.Black;
            //GameForm.Initializer.ForegroundColor = System.Drawing.Color.White;
            GameForm.Initializer.ForegroundColor = System.Drawing.Color.FromArgb(255, 255, 255);
            GameForm.Initializer.AdditionalColors = new System.Drawing.Color[] { System.Drawing.Color.Yellow, System.Drawing.Color.Green};

            // CREATE GAME MATRIX
            int[,] matrix = new int[64, 48];

            // SET COLORS IN THE MATRIX
            matrix[matrix.GetLength(0) / 2 - 1, matrix.GetLength(1) / 2 - 1] = 1;
            matrix[matrix.GetLength(0) / 2, matrix.GetLength(1) / 2 - 1] = 1;
            matrix[matrix.GetLength(0) / 2 - 1, matrix.GetLength(1) / 2] = 1;
            matrix[matrix.GetLength(0) / 2, matrix.GetLength(1) / 2] = 1;

            matrix[0, 0] = 2;
            matrix[matrix.GetLength(0) - 1, 0] = 3;
            matrix[0, matrix.GetLength(1) - 1] = 4;
            //matrix[matrix.GetLength(0) - 1, matrix.GetLength(1) - 1] = true;

            Form gameForm = new GameForm(matrix);
            Application.Run(gameForm);
        }
    }
}
