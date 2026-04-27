/*
    Framework to write a simple retro game in pixel art.
    Copyright (C) 2026  Giovanni Volpintesta

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
    
    Contact the author to: john.foxinhead@gmail.com

*/

using System;
using System.Windows.Forms;
using System.Drawing;

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
        float[] ballPosition; // ball position in screen pixels (float to consider also half pixels)
        float[] ballSpeed; // ball speed in pixels per frame (float to consider also half pixels)

        int ballColor = 1;

        GameImage ballImage = new GameImage(new int[,] {
            { 0, 1, 0},
            { 1, 1, 1},
            { 1, 1, 0}
        }, AnchorType.Center);
        PaintStyle ballStyle = PaintStyle.Default;

        GameImage starImage = new GameImage(new int[,] {
            { 0,0,0,0,0,1,0,0,0,0,0 },
            { 0,0,0,0,0,1,0,0,0,0,0 },
            { 0,0,0,0,1,1,1,0,0,0,0 },
            { 1,1,1,1,1,1,1,1,1,1,1 },
            { 0,0,1,1,1,1,1,1,1,0,0 },
            { 0,0,0,1,1,1,1,1,0,0,0 },
            { 0,0,0,1,1,1,1,1,0,0,0 },
            { 0,0,1,1,0,0,0,1,1,0,0 },
            { 0,0,1,0,0,0,0,0,1,0,0 }
        }, AnchorType.Center);
        PaintStyle starStyle = PaintStyle.Default;


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

            GameConfig.AdditionalColors = new System.Drawing.Color[] {
                System.Drawing.Color.Red,
                System.Drawing.Color.Orange,
                System.Drawing.Color.Yellow,
                System.Drawing.Color.Green,
                System.Drawing.Color.Cyan,
                System.Drawing.Color.Blue,
                System.Drawing.Color.Violet,
           };
        }

        // Called at the start of the first frame of the game.
        // It's main purpose it's to setup the scene.
        private void FirstFrameLoop ()
        {
            // set the ball in the center of the screen
            ballPosition = new float[] { GameConfig.PixelsMatrixWidth / 2, GameConfig.PixelsMatrixHeight / 2 };

            // give the fall a speed
            ballSpeed = new float[] { 2, 2 };

            ballStyle.SetColorRemap(1, 2); // start from first additional color;
        }

        // Called once per frame, BEFORE the OnLoopGame event.
        protected override void OnClear(int[,] pixels)
        {
            GameUtils.ClearScreen(pixels);
        }

        // Called once per frame.
        // Here the actual logic happens.
        protected override void OnLoopGame(float deltaTime)
        {
            if (FrameCount == 0)
            {
                FirstFrameLoop();
            }
            else
            {
                UpdateBallPosition();
            }
        }

        // Called once per frame, AFTER the OnLoopGame event.
        protected override void OnDraw(int[,] pixels)
        {
            int screenWidth = pixels.GetLength(0);
            int screenHeight = pixels.GetLength(1);

            // Draw the background star images at the center of the screen
            GameUtils.DrawImageOnScreen(pixels, starImage, new Point((int)(screenWidth * 0.25), (int)(screenHeight * 0.25)), starStyle);
            //GameUtils.DrawImageOnScreen(pixels, starImage, new Point((int)(screenWidth * 0.75), (int)(screenHeight * 0.25)), starStyle);
            GameUtils.DrawImageOnScreen(pixels, starImage, new Point((int)(screenWidth * 0.50), (int)(screenHeight * 0.50)), starStyle);
            GameUtils.DrawImageOnScreen(pixels, starImage, new Point((int)(screenWidth * 0.25), (int)(screenHeight * 0.75)), starStyle);
            //GameUtils.DrawImageOnScreen(pixels, starImage, new Point((int)(screenWidth * 0.75), (int)(screenHeight * 0.75)), starStyle);

            DrawBall(pixels, ballColor); // set the foregorund color in the current ball location
            // GameUtils.DrawImageOnScreen(pixels, ballImage, new Point((int)ballPosition[0], (int)ballPosition[1]), ballStyle);
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
            // BALL EXAMPLE:     1  
            //                  234 
            //                  65  

            DrawPixel(pixels, ballPosition[0] - 1,  ballPosition[1],        color);  // 1
            DrawPixel(pixels, ballPosition[0],      ballPosition[1] - 1,    color);  // 2
            DrawPixel(pixels, ballPosition[0],      ballPosition[1],        color);  // 3
            DrawPixel(pixels, ballPosition[0],      ballPosition[1] + 1,    color);  // 4
            DrawPixel(pixels, ballPosition[0] + 1,  ballPosition[1],        color);  // 5
            DrawPixel(pixels, ballPosition[0] - 1,  ballPosition[1] + 1,    color);  // 6
        }

        private static void DrawPixel(int[,] pixels, float x, float y, int color)
        {
            int posX = (int)x;
            int posY = (int)y;

            if (posX >= 0 && posX < pixels.GetLength(0)
                && posY >= 0 && posY < pixels.GetLength(1))
            {
                // X coordinate is the column index, while Y coordinate is the row index
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
                else if (KeyCode == Keys.C)
                {
                    int tmpColor = ballStyle.GetRemappedColor(PaintStyle.FOREGROUND_COLOR_INDEX);
                    tmpColor++;
                    if (tmpColor >= GameConfig.AdditionalColors.Length + 2)
                        tmpColor = 2;
                    ballStyle.SetColorRemap(PaintStyle.FOREGROUND_COLOR_INDEX, tmpColor);

                    ballColor++;
                    if (ballColor >= GameConfig.AdditionalColors.Length + 2)
                        ballColor = 2;
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
