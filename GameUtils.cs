using System;
using System.Drawing;

namespace RetroGameFramework
{
    
    public enum AnchorType
    {
        TopLeft,
        BottomLeft,
        Center
    }

    public enum MatrixOrientation
    {
        Transposed = 0, // default
        AlignedToScreen
    }

    public struct GameImage
    {
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
                  , AnchorType.TopLeft
                  , matrixOrientation)
        { }

    }

    public struct PaintStyle
    {
        public static readonly int BACKGROUND_COLOR_INDEX = 0;
        public static readonly int FOREGROUND_COLOR_INDEX = 1;

        public bool _transparentBackground;
        public bool transparentBackground { get { return _transparentBackground; } set { _transparentBackground = value; } }

        private int[] _colorsRemap;
        public int[] colorsRemap { get { return _colorsRemap; } }

        public PaintStyle(bool transparentBackground, int[] colorsRemap)
        {
            _transparentBackground = transparentBackground;

            int colorsCount = colorsRemap != null ? colorsRemap.Length : 0;

            if (colorsCount > 255)
                throw new IndexOutOfRangeException("Cannot remap more than 256 colors");

            _colorsRemap = new int[colorsCount];
            for (int i = 0; i < _colorsRemap.Length; i++)
            {
                if (colorsRemap[i] >= 0)
                    _colorsRemap[i] = colorsRemap[i];
                else
                    _colorsRemap[i] = i;
            }
        }

        public PaintStyle(bool transparentBackground) : this(transparentBackground, null) { }
        public PaintStyle(int[] colorsRemap) : this(true, colorsRemap) { }
        public static PaintStyle Default { get { return new PaintStyle(true); } }

        public bool ExistsColorRemap (int color)
        {
            return colorsRemap != null && color >= 0 && color < colorsRemap.Length && colorsRemap[color] != color;
        }

        // Retrieves the remapped color, or the original one if it has been remapped
        public int GetRemappedColor (int fromColor)
        {
            return (colorsRemap != null && fromColor >= 0 && fromColor < colorsRemap.Length) ? colorsRemap[fromColor]
                : ((fromColor == 0 || fromColor == 1) ? fromColor : - 1);
        }

        public void EnsureColorRemapSize(int colorsCount)
        {
            if (colorsRemap == null || colorsRemap.Length < colorsCount)
            {
                if (colorsCount > 255) throw new IndexOutOfRangeException("Cannot remap more than 256 colors");

                int[] oldColors = colorsRemap;
                _colorsRemap = new int[colorsCount];
                if (oldColors != null)
                {
                    for (int i = 0; i < oldColors.Length; i++) colorsRemap[i] = oldColors[i];
                }
                for (int i = (oldColors != null) ? oldColors.Length : 0; i < colorsRemap.Length; i++) colorsRemap[i] = i;
            }
        }

        public void SetColorRemap (int fromColor, int toColor)
        {
            if (fromColor < 0) throw new IndexOutOfRangeException("Colors cannot be negative!");

            // if EnsureColorRemapSize method has not been called in advance,
            // a bigger space is allocated to prevent memory allocation ad each call
            if (colorsRemap == null || colorsRemap.Length < fromColor)
                EnsureColorRemapSize(fromColor * 2 + 1);

            colorsRemap[fromColor] = toColor;
        }
    }

    public static class GameUtils
    {
        public static Point PivotFromAnchorType(Size imageSize, AnchorType pivotPosition)
        {
            switch (pivotPosition)
            {
                case AnchorType.TopLeft: return new Point(0, 0);
                case AnchorType.BottomLeft: return new Point(0, imageSize.Height);
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
