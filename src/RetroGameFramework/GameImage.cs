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

using RetroGameFramework.src.RetroGameFramework;
using System;
using System.Drawing;

namespace RetroGameFramework
{
    public struct GameImage
    {
        private static readonly AnchorType DEFAULT_ANCHOR_TYPE = AnchorType.TopLeft;

        public static int WidthFromMatrix(int[,] matrix, MatrixOrientation matrixOrientation) { return matrix.GetLength(matrixOrientation == MatrixOrientation.AlignedToScreen ? 1 : 0); } // If the matrix is aligned to the screen, the image width is the number of columns
        public static int HeightFromMatrix(int[,] matrix, MatrixOrientation matrixOrientation) { return matrix.GetLength(matrixOrientation == MatrixOrientation.AlignedToScreen ? 0 : 1); } // If the matrix is aligned to the screen, the image height is the number of rows
        public static Size SizeFromMatrix(int[,] matrix, MatrixOrientation matrixOrientation) { return new Size(WidthFromMatrix(matrix, matrixOrientation), HeightFromMatrix(matrix, matrixOrientation)); }

        // the image is always stored as transposed
        private int[,] _matrix;

        private Point _pivot;
        public Point Pivot { get { return _pivot; } }
        public int Width { get { return _matrix.GetLength(0); } } // Regardless than how the matrix is passed to the constructor, it is stored transposed, to the width is the number of rows (not columns)
        public int Height { get { return _matrix.GetLength(1); } } // Regardless than how the matrix is passed to the constructor, it is stored transposed, to the height is the number of columns (not rows)
        public Size Size { get { return new Size(Width, Height); } }

        // Returns the color of the image at X and Y coordinates (according to screen coordinates system)
        public int GetPixel(int xCoord, int yCoord)
        {
            // The matrix is transposed, so the X screen coordinate is aligned to the matrix row index,
            // and the Y screen coordinate is aligned to the matrix column index
            return _matrix[xCoord, yCoord];
        }

        // Returns a copy of the matrix, stored with rows transposed as columns
        public int[,] GetMatrixCopy(MatrixOrientation desiredMatrixOrientation = MatrixOrientation.AlignedToScreen)
        {
            int[,] copy = null;

            // _matrix is always transposed so if the desired copy is trasposed too, the matrices are aligned
            // otherwise their coordinates have to be swapped to transpose the copy

            if (_matrix != null)
            {
                copy = desiredMatrixOrientation == MatrixOrientation.Transposed
                    ? new int[_matrix.GetLength(0), _matrix.GetLength(1)]
                    : new int[_matrix.GetLength(1), _matrix.GetLength(0)];
            }

            for (int i = 0; i < copy.GetLength(0); i++)
            {
                for (int j = 0; j < copy.GetLength(1); j++)
                {
                    copy[i, j] = desiredMatrixOrientation == MatrixOrientation.Transposed
                        ? _matrix[i, j]
                        : _matrix[j, i];
                }
            }

            return copy;
        }

        public GameImage(int[,] matrix, Point pivot, MatrixOrientation matrixOrientation = MatrixOrientation.AlignedToScreen)
        {
            _pivot = pivot;

            if (matrix != null)
            {
                // _matrix is always transposed so if the matrix received as argument is trasposed too,
                // the matrices are aligned otherwise their coordinates have to be swapped to transpose _matrix

                _matrix = (matrixOrientation == MatrixOrientation.Transposed)
                    ? new int[matrix.GetLength(0), matrix.GetLength(1)]
                    : new int[matrix.GetLength(1), matrix.GetLength(0)];

                for (int i = 0; i < _matrix.GetLength(0); i++)
                    for (int j = 0; j < _matrix.GetLength(1); j++)
                        _matrix[i, j] = (matrixOrientation == MatrixOrientation.Transposed)
                            ? matrix[i, j]
                            : matrix[j, i];
            }
            else
            {
                _matrix = new int[0, 0];
            }
        }

        public GameImage(int[,] matrix, AnchorType pivotPosition, MatrixOrientation matrixOrientation = MatrixOrientation.AlignedToScreen)
            : this(matrix
                  , GameUtils.PivotFromAnchorType(SizeFromMatrix(matrix, matrixOrientation), pivotPosition)
                  , matrixOrientation)
        { }

        public GameImage(int[,] matrix, MatrixOrientation matrixOrientation = MatrixOrientation.AlignedToScreen)
            : this(matrix
                  , DEFAULT_ANCHOR_TYPE
                  , matrixOrientation)
        { }



        public static GameImage CreateFromRows(string[] rows, Point pivot) { return CreateFromRows(rows, null, pivot); }
        public static GameImage CreateFromRows(string[] rows, char[] colorChars, Point pivot)
        {
            GameImage tmp = new GameImage();
            tmp._matrix = ImageLoader.ReadTextImageFromRows(rows, colorChars, MatrixOrientation.Transposed);
            tmp._pivot = pivot;
            return tmp;
        }

        public static GameImage CreateFromRows(string[] rows, AnchorType pivotPosition) { return CreateFromRows(rows, null, pivotPosition); }
        public static GameImage CreateFromRows(string[] rows, char[] colorChars, AnchorType pivotPosition)
        {
            GameImage tmp = new GameImage();
            tmp._matrix = ImageLoader.ReadTextImageFromRows(rows, colorChars, MatrixOrientation.Transposed);
            tmp._pivot = GameUtils.PivotFromAnchorType(SizeFromMatrix(tmp._matrix, MatrixOrientation.Transposed), pivotPosition);
            return tmp;
        }

        public static GameImage CreateFromRows(string[] rows) { return CreateFromRows(rows, null); }
        public static GameImage CreateFromRows(string[] rows, char[] colorChars)
        {
            return CreateFromRows(rows, colorChars, DEFAULT_ANCHOR_TYPE);
        }



        public static GameImage CreateFromString(string imageString, Point pivot) { return CreateFromString(imageString, null, pivot); }
        public static GameImage CreateFromString(string imageString, char[] colorChars, Point pivot)
        {
            GameImage tmp = new GameImage();
            tmp._matrix = ImageLoader.ReadTextImageFromString(imageString, colorChars, MatrixOrientation.Transposed);
            tmp._pivot = pivot;
            return tmp;
        }

        public static GameImage CreateFromString(string imageString, AnchorType pivotPosition) { return CreateFromString(imageString, null, pivotPosition); }
        public static GameImage CreateFromString(string imageString, char[] colorChars, AnchorType pivotPosition)
        {
            GameImage tmp = new GameImage();
            tmp._matrix = ImageLoader.ReadTextImageFromString(imageString, colorChars, MatrixOrientation.Transposed);
            tmp._pivot = GameUtils.PivotFromAnchorType(SizeFromMatrix(tmp._matrix, MatrixOrientation.Transposed), pivotPosition);
            return tmp;
        }

        public static GameImage CreateFromString(string imageString) { return CreateFromString(imageString, null); }
        public static GameImage CreateFromString(string imageString, char[] colorChars)
        {
            return CreateFromString(imageString, colorChars, DEFAULT_ANCHOR_TYPE);
        }



        public static GameImage CreateFromFile(string filename, Point pivot)
        {
            GameImage tmp = new GameImage();
            tmp._matrix = ImageLoader.ReadTextImageFromFile(filename, MatrixOrientation.Transposed);
            tmp._pivot = pivot;
            return tmp;
        }

        public static GameImage CreateFromFile(string filename, AnchorType pivotPosition)
        {
            GameImage tmp = new GameImage();
            tmp._matrix = ImageLoader.ReadTextImageFromFile(filename, MatrixOrientation.Transposed);
            tmp._pivot = GameUtils.PivotFromAnchorType(SizeFromMatrix(tmp._matrix, MatrixOrientation.Transposed), pivotPosition);
            return tmp;
        }

        public static GameImage CreateFromFile(string filename)
        {
            return CreateFromFile(filename, DEFAULT_ANCHOR_TYPE);
        }



        public static GameImage CreateFromResource(string resourceName, Point pivot)
        {
            GameImage tmp = new GameImage();
            tmp._matrix = ImageLoader.ReadTextImageFromResource(resourceName, MatrixOrientation.Transposed);
            tmp._pivot = pivot;
            return tmp;
        }

        public static GameImage CreateFromResource(string resourceName, AnchorType pivotPosition)
        {
            GameImage tmp = new GameImage();
            tmp._matrix = ImageLoader.ReadTextImageFromResource(resourceName, MatrixOrientation.Transposed);
            tmp._pivot = GameUtils.PivotFromAnchorType(SizeFromMatrix(tmp._matrix, MatrixOrientation.Transposed), pivotPosition);
            return tmp;
        }

        public static GameImage CreateFromResource(string resourceName)
        {
            return CreateFromResource(resourceName, DEFAULT_ANCHOR_TYPE);
        }
    }
}
