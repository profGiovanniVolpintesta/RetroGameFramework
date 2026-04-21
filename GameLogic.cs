using System;
using System.Windows.Forms;

namespace RetroGameFramework
{
    internal class GameLogic : BaseGameLogic
    {
        public GameLogic(GameConfig GameConfig) : base(GameConfig) { }

        // GameConfig is a variable already accessible in methods to retrieve the game configs
        // bool IsPaused() is a function already accessible in methods to check if the game is paused
        // void SetPaused(bool) is a function already accessible in methods to set the game in pause and to resume it

        // GAME DATA
        // Declare here game-specific data that should survive the frame
        float[] ballPosition; // ball position in screen pixels
        float[] ballSpeed; // ball speed in pixels per frame

        // Initialization call, used to customize GameConfig data (used to customize the engine behaviour)
        protected override void OnInitGameConfig(GameConfig GameConfig)
        {
            GameConfig.Title = "Bouncing Ball";

            GameConfig.PixelsMatrixWidth = 64;
            GameConfig.PixelsMatrixHeight = 48;
            GameConfig.PixelSize = 10;

            GameConfig.FrameRate = 12;

            GameConfig.BackgroundColor = System.Drawing.Color.Black;
            //GameForm.Initializer.ForegroundColor = System.Drawing.Color.White;
            GameConfig.ForegroundColor = System.Drawing.Color.FromArgb(255, 255, 255);
            GameConfig.AdditionalColors = new System.Drawing.Color[] { System.Drawing.Color.Yellow, System.Drawing.Color.Green };
        }

        // Called at the start of the first frame of the game.
        // It's main purpose it's to setup the scene.
        protected override void OnStartGame(int[,] pixels)
        {
            // set the ball in the center of the screen
            ballPosition = new float[] { pixels.GetLength(0) / 2, pixels.GetLength(1) / 2 };
            // give the fall a speed
            ballSpeed = new float[] { 2, 2 };
        }

        // Called once per frame, BEFORE the OnLoopGame event.
        protected override void OnClear(int[,] pixels)
        {
            DrawBall(pixels, 0); // set the background color in the former ball location
        }

        // Called once per frame.
        // Here the actual logic happens.
        protected override void OnLoopGame()
        {
            UpdateBallPosition();
        }

        // Called once per frame, AFTER the OnLoopGame event.
        protected override void OnDraw(int[,] pixels)
        {
            DrawBall(pixels, 1); // set the foregorund color in the current ball location
        }

        // Called at the end of the last frame of the game.
        // Its main purpose it's to dispose resources, as the game will end immediately after this call.
        protected override void OnEndGame()
        {

        }

        private void UpdateBallPosition()
        {
            // Update ball's position
            ballPosition[0] += ballSpeed[0];
            ballPosition[1] += ballSpeed[1];

            // Check hits with screen bounds to make the ball bounce
            // The bounce is cheched with a margin to consider the ball dimension
            // In the collision checkings, the radius is always reduced by 0.5 beceuse the center pixel should not be computed.

            float ballRadius = 1.5f;

            if (ballSpeed[0] < 0 && ballPosition[0] - (ballRadius - 0.5f) <= 0) // horizontal check to the left
            {
                // if the ball is going to the left and it went outside the left screen bound,
                ballPosition[0] += (ballRadius - 0.5f) - ballPosition[0]; // correct the position after the bounce
                ballSpeed[0] *= -1; // flip the speed direction
            }
            else if (ballSpeed[0] > 0 && ballPosition[0] + (ballRadius - 0.5) >= GameConfig.PixelsMatrixWidth - 1) // horizontal check to the right
            {
                // if the ball is going to the right and it went outside the right screen bound,
                ballPosition[0] -= ballPosition[0] - (GameConfig.PixelsMatrixWidth - 1 - (ballRadius - 0.5f)); // correct the position after the bounce
                ballSpeed[0] *= -1; // flip the speed direction
            }

            if (ballSpeed[1] < 0 && ballPosition[1] - (ballRadius - 0.5f) <= 0) // vertical check to the top
            {
                // if the ball is going up and it went outside the top screen bound,
                ballPosition[1] += (ballRadius - 0.5f) - ballPosition[1]; // correct the position after the bounce
                ballSpeed[1] *= -1; // flip the speed direction
            }
            else if (ballSpeed[1] > 0 && ballPosition[1] + (ballRadius - 0.5f) >= GameConfig.PixelsMatrixHeight - 1) // vertical check to the bottom
            {
                // if the ball is going down and it went outside the bottom screen bound,
                ballPosition[1] -= ballPosition[1] - (GameConfig.PixelsMatrixHeight - 1 - (ballRadius - 0.5f)); // correct the position after the bounce
                ballSpeed[1] *= -1; // flip the speed direction
            }
        }

        private void DrawBall(int[,] pixels, int color)
        {
            // BALL EXAMPLE:     2
            //                  415
            //                   3

            DrawPixel(pixels, ballPosition[0],      ballPosition[1],        color);  // 1
            DrawPixel(pixels, ballPosition[0] - 1,  ballPosition[1],        color);  // 2
            DrawPixel(pixels, ballPosition[0] + 1,  ballPosition[1],        color);  // 3
            DrawPixel(pixels, ballPosition[0],      ballPosition[1] - 1,    color);  // 4
            DrawPixel(pixels, ballPosition[0],      ballPosition[1] + 1,    color);  // 5
        }

        private void DrawPixel(int[,] pixels, float x, float y, int color)
        {
            int posX = (int)x;
            int posY = (int)y;
            if (posX >= 0 && posX < pixels.GetLength(0)
                && posY >= 0 && posY < pixels.GetLength(1))
            {
                pixels[posX, posY] = color;
            }
        }

        // Called the first frame a key is pressed, and not called anymore unless the key is released
        protected override void OnKeyDown(Keys KeyCode)
        {
            if (!IsPaused())
            {
                float[] ballSpeedAbs = new float[] { Math.Abs(ballSpeed[0]), Math.Abs(ballSpeed[1]) };
                if (KeyCode == Keys.Up || KeyCode == Keys.W)
                {
                    ballSpeed[1] = -ballSpeedAbs[1];
                }
                else if (KeyCode == Keys.Down || KeyCode == Keys.S)
                {
                    ballSpeed[1] = ballSpeedAbs[1];
                }
                else if (KeyCode == Keys.Right || KeyCode == Keys.D)
                {
                    ballSpeed[0] = ballSpeedAbs[0];
                }
                else if (KeyCode == Keys.Left || KeyCode == Keys.A)
                {
                    ballSpeed[0] = -ballSpeedAbs[0];
                }
                else if (KeyCode == Keys.P)
                {
                    SetPaused(true);
                }
            }
            else
            {
                if (KeyCode == Keys.P)
                {
                    SetPaused(false);
                }
            }
        }

        // Called if a key has been released (even in the same frame it has been released)
        protected override void OnKeyUp(Keys KeyCode)
        {
        
        }

        // Called during the frame a key is pressed and in all the following frames until it's released (excluding the frame it's released)
        protected override void OnKeyPress(Keys KeyCode)
        {
        
        }

    }
}
