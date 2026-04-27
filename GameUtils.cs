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
using System.Drawing;

namespace RetroGameFramework
{
    
    public static class GameUtils
    {
        public static Point PivotFromAnchorType(Size imageSize, AnchorType pivotPosition)
        {
            switch (pivotPosition)
            {
                case AnchorType.TopLeft: return new Point(0, 0);
                case AnchorType.BottomLeft: return new Point(0, imageSize.Height);
                case AnchorType.TopRight: return new Point(imageSize.Width, 0);
                case AnchorType.BottomRight: return new Point(imageSize.Width, imageSize.Height);
                case AnchorType.Top: return new Point(imageSize.Width / 2, 0);
                case AnchorType.Bottom: return new Point(imageSize.Width / 2, imageSize.Height);
                case AnchorType.Left: return new Point(0, imageSize.Height / 2);
                case AnchorType.Right: return new Point(imageSize.Width, imageSize.Height / 2);
                case AnchorType.Center: return new Point(imageSize.Width / 2, imageSize.Height / 2);
                default: return new Point(0, 0);
            }
        }

        public static void DrawImageOnScreen(int[,] targetMatrix, GameImage image, Point position) { DrawImageOnScreen(targetMatrix, image, position, PaintStyle.Default); }
        public static void DrawImageOnScreen(int[,] targetMatrix, GameImage image, Point position, PaintStyle paintStyle) { DrawImageOnTopOfTarget(targetMatrix, image, position, paintStyle, MatrixOrientation.Transposed); }
        public static void DrawImageOnTopOfTarget(int[,] targetMatrix, GameImage image, Point position, MatrixOrientation targetMatrixOrientation) { DrawImageOnTopOfTarget(targetMatrix, image, position, PaintStyle.Default, targetMatrixOrientation); }
        public static void DrawImageOnTopOfTarget(int[,] targetMatrix, GameImage image, Point position, PaintStyle paintStyle, MatrixOrientation targetMatrixOrientation)
        {
            Point drawStartPosition = new Point(position.X - image.Pivot.X, position.Y - image.Pivot.Y);
            Size targetMatrixSize = GameImage.SizeFromMatrix(targetMatrix, targetMatrixOrientation);

            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    int remappedColor = paintStyle.GetRemappedColor(image.GetPixel(x, y));
                    if (remappedColor != PaintStyle.BACKGROUND_COLOR_INDEX || !paintStyle.transparentBackground)
                    {
                        if (drawStartPosition.X + x >= 0 && drawStartPosition.X + x < targetMatrixSize.Width
                            && drawStartPosition.Y + y >= 0 && drawStartPosition.Y + y < targetMatrixSize.Height)
                        {
                            if (targetMatrixOrientation == MatrixOrientation.AlignedToScreen)
                            {
                                targetMatrix[drawStartPosition.Y + y, drawStartPosition.X + x] = remappedColor;
                            }
                            else
                            {
                                targetMatrix[drawStartPosition.X + x, drawStartPosition.Y + y] = remappedColor;
                            }
                        }
                    }
                }
            }
        }

        public static void ClearScreen (int[,] targetPixelsMatrix)
        {
            for (int x = 0; x < targetPixelsMatrix.GetLength(0); x++)
            {
                for (int y = 0; y < targetPixelsMatrix.GetLength(1); y++)
                {
                    // X coordinate is the column index, while Y coordinate is the row index
                    targetPixelsMatrix[x, y] = 0;
                }
            }
        }

    }
}
