using System;
using System.Windows.Forms;

namespace RetroGameFramework
{
    internal class BaseGameLogic
    {
        // CREATE GAME MATRIX
        static void Main(string[] args)
        {
            GameConfig GameConfig = new GameConfig();

            Type GameLogicClass = GameConfig.GameLogicClass;
            if (GameLogicClass == null
                || (GameLogicClass != typeof(BaseGameLogic) && !GameLogicClass.IsSubclassOf(typeof(BaseGameLogic))))
            {
                GameLogicClass = typeof(BaseGameLogic);
            }
            BaseGameLogic GameLogic = (BaseGameLogic) Activator.CreateInstance(GameLogicClass);

            GameLogic.OnInitGameConfig(GameConfig);

            int[,] pixels = new int[GameConfig.PixelsMatrixWidth, GameConfig.PixelsMatrixHeight];

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Form gameForm = new GameForm(GameConfig, pixels);

            bool continueGame = true;

            GameLogic.OnStartGame(pixels);
            GameLogic.OnLoopGame(pixels);

            System.Windows.Forms.Timer gameTimer = new System.Windows.Forms.Timer();
            gameTimer.Interval = 1000 / GameConfig.FrameRate;
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
                    GameLogic.OnLoopGame(pixels);
                }
                else
                {
                    GameLogic.OnEndGame(pixels);
                    gameTimer.Stop();
                }
            };
            gameTimer.Start();

            Application.Run(gameForm); // This runs the form with the main thread as owner
        }

        public BaseGameLogic()
        {

        }

        protected virtual void OnInitGameConfig(GameConfig GameConfig) { }

        protected virtual void OnStartGame(int[,] pixels) { }

        protected virtual void OnLoopGame(int[,] pixels) { }

        protected virtual void OnEndGame(int[,] pixels) { }

    }
}
