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
            //bool[,] matrix = new bool[64, 48];
            bool[,] matrix = new bool[30, 30];

            matrix[matrix.GetLength(0) / 2 - 1, matrix.GetLength(1) / 2 - 1] = true;
            matrix[matrix.GetLength(0) / 2, matrix.GetLength(1) / 2 - 1] = true;
            matrix[matrix.GetLength(0) / 2 - 1, matrix.GetLength(1) / 2] = true;
            matrix[matrix.GetLength(0) / 2, matrix.GetLength(1) / 2] = true;

            matrix[0, 0] = true;
            matrix[matrix.GetLength(0) - 1, 0] = true;
            matrix[0, matrix.GetLength(1) - 1] = true;
            //matrix[matrix.GetLength(0) - 1, matrix.GetLength(1) - 1] = true;

            Application.Run(new GameForm(matrix));
        }
    }
}
