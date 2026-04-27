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

namespace RetroGameFramework
{
    public struct PaintStyle
    {
        public static readonly int BACKGROUND_COLOR_INDEX = 0;
        public static readonly int FOREGROUND_COLOR_INDEX = 1;

        public bool _transparentBackground;
        public bool transparentBackground { get { return _transparentBackground; } set { _transparentBackground = value; } }

        private int[] _colorsRemap;
        public int[] getColorsRemapCopy()
        {
            int[] tmp = new int[_colorsRemap != null ? _colorsRemap.Length : 0];
            for (int i = 0; i < tmp.Length; i++)
            {
                tmp[i] = _colorsRemap[i];
            }
            return tmp;
        }

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

        public bool ExistsColorRemap(int color)
        {
            return _colorsRemap != null && color >= 0 && color < _colorsRemap.Length && _colorsRemap[color] != color;
        }

        // Retrieves the remapped color, or the original one if it has been remapped
        public int GetRemappedColor(int fromColor)
        {
            return (_colorsRemap != null && fromColor >= 0 && fromColor < _colorsRemap.Length) ? _colorsRemap[fromColor]
                : ((fromColor == 0 || fromColor == 1) ? fromColor : -1);
        }

        public void EnsureColorRemapSize(int colorsCount)
        {
            if (_colorsRemap == null || _colorsRemap.Length < colorsCount)
            {
                if (colorsCount > 255) throw new IndexOutOfRangeException("Cannot remap more than 256 colors");

                int[] oldColors = _colorsRemap;
                _colorsRemap = new int[colorsCount];
                if (oldColors != null)
                {
                    for (int i = 0; i < oldColors.Length; i++) _colorsRemap[i] = oldColors[i];
                }
                for (int i = (oldColors != null) ? oldColors.Length : 0; i < _colorsRemap.Length; i++) _colorsRemap[i] = i;
            }
        }

        public void SetColorRemap(int fromColor, int toColor)
        {
            if (fromColor < 0) throw new IndexOutOfRangeException("Colors cannot be negative!");

            // if EnsureColorRemapSize method has not been called in advance,
            // a bigger space is allocated to prevent memory allocation ad each call
            if (_colorsRemap == null || _colorsRemap.Length <= fromColor)
                EnsureColorRemapSize(fromColor * 2 + 1);

            _colorsRemap[fromColor] = toColor;
        }
    }
}
