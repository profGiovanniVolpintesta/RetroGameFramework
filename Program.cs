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
        // CREATE GAME MATRIX
        static int[,] matrix = new int[64, 48];

        static void Main(string[] args)
        {
            InitGameConfigs();
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Form gameForm = new GameForm(matrix);

            StartGame();

            bool continueGame = true;
            System.Windows.Forms.Timer gameTimer = new System.Windows.Forms.Timer();
            gameTimer.Interval = 33;
            gameTimer.Tick += (s, e) =>
            {
                if (continueGame)
                {
                    gameForm.Invoke(new Action(() =>
                    {
                        // This is done in the gameForm owner thread (the main thread)
                        // through a delegated call. Multithread logic cannot be called
                        // https://visualstudiomagazine.com/articles/2010/11/18/multithreading-in-winforms.aspx
                        gameForm.Invalidate();
                        gameForm.Update();
                    }));
                    GameLoop();
                }
                else
                {
                    EndGame();
                    gameTimer.Stop();
                }
            };

            Application.Run(gameForm); // This runs the form with the main thread as owner
        }

        static void InitGameConfigs()
        {
            // INITIALIZE GAME CONFIGS
            GameForm.Initializer.Title = "Retro Game";
            GameForm.Initializer.PixelSize = 8;
            GameForm.Initializer.BackgroundColor = System.Drawing.Color.Black;
            //GameForm.Initializer.ForegroundColor = System.Drawing.Color.White;
            GameForm.Initializer.ForegroundColor = System.Drawing.Color.FromArgb(255, 255, 255);
            GameForm.Initializer.AdditionalColors = new System.Drawing.Color[] { System.Drawing.Color.Yellow, System.Drawing.Color.Green };
        }

        static void StartGame()
        {
            // SET COLORS IN THE MATRIX
            matrix[matrix.GetLength(0) / 2 - 1, matrix.GetLength(1) / 2 - 1] = 1;
            matrix[matrix.GetLength(0) / 2, matrix.GetLength(1) / 2 - 1] = 1;
            matrix[matrix.GetLength(0) / 2 - 1, matrix.GetLength(1) / 2] = 1;
            matrix[matrix.GetLength(0) / 2, matrix.GetLength(1) / 2] = 1;

            matrix[0, 0] = 2;
            matrix[matrix.GetLength(0) - 1, 0] = 3;
            matrix[0, matrix.GetLength(1) - 1] = 4;
            //matrix[matrix.GetLength(0) - 1, matrix.GetLength(1) - 1] = true;
        }

        static void GameLoop()
        {
            
        }

        static void EndGame()
        {

        }
    }
}
