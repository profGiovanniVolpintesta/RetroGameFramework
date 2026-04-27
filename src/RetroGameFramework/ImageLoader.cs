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
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace RetroGameFramework.src.RetroGameFramework
{
    public static class ImageLoader
    {
        static readonly string CHARS_LINE_BEGIN = "chars=";
        static readonly char[] DEFAULT_CHARS = { ' ', '*' };

        public static int[,] ReadTextImageFromString(string imageString, MatrixOrientation matrixOrientation = MatrixOrientation.AlignedToScreen)
        {
            return ReadTextImageFromString(imageString, null, matrixOrientation);
        }
        public static int[,] ReadTextImageFromString(string imageString, char[] colorChars, MatrixOrientation matrixOrientation = MatrixOrientation.AlignedToScreen)
        {
            if (imageString == null) imageString = "";

            while (imageString.Contains("\r\r"))
                imageString = imageString.Replace("\r\r", "\r");

            imageString = imageString.Replace("\r\n", "\n");
            imageString = imageString.Replace("\r", "\n");

            string[] lines = imageString.Split(new char[] { '\n' }, StringSplitOptions.None); // split by '\n' keeping empty lines

            return ReadTextImageFromRows(lines, colorChars, matrixOrientation);
        }

        public static int[,] ReadTextImageFromResource(string resourceName, MatrixOrientation matrixOrientation = MatrixOrientation.AlignedToScreen)
        {
            List<string> lines = new List<string>();
            char[] colorChars = DEFAULT_CHARS;

            Assembly assembly = Assembly.GetExecutingAssembly();

            //foreach (string s in assembly.GetManifestResourceNames())
            //    Console.WriteLine(s);

            if (assembly != null)
            {
                string resourceFullPath = "RetroGameFramework.resources.images." + resourceName + ".txtimg";
                using (Stream stream = assembly.GetManifestResourceStream(resourceFullPath))
                {
                    if (stream == null)
                    {
                        throw new FileNotFoundException($"\"{resourceFullPath}\" resource not found!");
                    }

                    using (StreamReader reader = new StreamReader(stream))
                    {
                        if (reader != null)
                        {
                            string line;
                            while ((line = reader.ReadLine()) != null)
                            {
                                if (!string.IsNullOrEmpty(line))
                                {
                                    if (line.StartsWith(CHARS_LINE_BEGIN))
                                    {
                                        line = line.Substring(CHARS_LINE_BEGIN.Length);
                                        colorChars = line.ToCharArray();
                                        continue;
                                    }
                                }
                                lines.Add(line);
                            }
                        }
                    }
                }
            }

            return ReadTextImageFromRows(lines.ToArray(), colorChars, matrixOrientation);
        }

        public static int[,] ReadTextImageFromFile(string filename, MatrixOrientation matrixOrientation = MatrixOrientation.AlignedToScreen)
        {
            List<string> lines = new List<string>();
            char[] colorChars = DEFAULT_CHARS;

            using (StreamReader reader = new StreamReader(filename))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (!string.IsNullOrEmpty(line))
                    {
                        if (line.StartsWith(CHARS_LINE_BEGIN))
                        {
                            line = line.Substring(CHARS_LINE_BEGIN.Length);
                            colorChars = line.ToCharArray();
                            continue;
                        }
                    }
                    lines.Add(line);
                }
            }

            return ReadTextImageFromRows(lines.ToArray(), colorChars, matrixOrientation);
        }

        public static int[,] ReadTextImageFromRows(string[] rows, MatrixOrientation matrixOrientation = MatrixOrientation.AlignedToScreen)
        {
            return ReadTextImageFromRows(rows, null, matrixOrientation);
        }

        public static int[,] ReadTextImageFromRows(string[] rows, char[] colorChars, MatrixOrientation matrixOrientation = MatrixOrientation.AlignedToScreen)
        {
            if (colorChars == null || colorChars.Length == 0) colorChars = DEFAULT_CHARS;

            int height = rows != null ? rows.Length : 0;

            int width = 0;
            for (int i = 0; i < rows.Length; i++)
                width = Math.Max(width, rows[i] != null? rows[i].Length : 0);

            int[,] matrix = matrixOrientation == MatrixOrientation.Transposed
                ? new int[width, height]
                : new int[height, width];

            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    matrix[i, j] = PaintStyle.BACKGROUND_COLOR_INDEX;
                }
            }

            int y = 0;
            foreach (string row in rows)
            {
                char[] rowChars = row.ToCharArray();
                for (int x = 0; x < rowChars.Length; x++)
                {
                    int color = -1;
                    for (int c = 0; c < colorChars.Length; c++)
                    {
                        if (colorChars[c] == rowChars[x])
                        {
                            color = c;
                            break;
                        }
                    }

                    if (matrixOrientation == MatrixOrientation.Transposed)
                        matrix[x, y] = color;
                    else
                        matrix[y, x] = color;
                }
                y++;
            }

            return matrix;
        }

        
    }
}
