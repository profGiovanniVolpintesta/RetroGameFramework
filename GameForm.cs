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
using System.Windows.Forms;

namespace RetroGameFramework
{
    internal class GameForm : Form
    {
        private int _pixelSize = 1;
        public int PixelSize {
            get { return _pixelSize; }
            set
            { 
                _pixelSize = Math.Max(value, 0);
                OnRefreshMatrix();
            }
        }

        private int[,] _matrix = null;
        public int[,] Matrix
        {
            get { return _matrix; }
            set
            {
                _matrix = value;
                OnRefreshMatrix();
            }
        }

        public override Color BackColor
        {
            get { return base.BackColor; }
            set
            {
                base.BackColor = value;
                OnRefreshBrushes();
            }
        }

        public override Color ForeColor
        {
            get { return base.ForeColor; }
            set
            {
                base.ForeColor = value;
                OnRefreshBrushes();
            }
        }

        private Color[] _additionalColors;

        public Color[] AdditionalColors
        {
            get
            {
                Color[] copy = new Color[_additionalColors.Length];
                for (int i = 0; i < copy.Length; i++) copy[i] = _additionalColors[i];
                return copy;
            }
            set
            {
                _additionalColors = new Color[value != null ? value.Length : 0];
                for (int i = 0; i < _additionalColors.Length; i++) _additionalColors[i] = value[i];
                OnRefreshBrushes();
            }
        }

        private Color _invalidColor;
        public Color InvalidColor
        {
            get { return _invalidColor; }
            set { _invalidColor = value; }
        }

        private Brush[] _brushes;
        private Brush _invalidColorBrush;
        public Brush GetBrush (int index)
        {
            // clampa l'indice all'interno dei valori possibili
            return (index >= 0 && index < _brushes.Length) ? _brushes[index] : _invalidColorBrush;
        }
        public Brush GetBackBrush() { return GetBrush(0); }
        public Brush GetForeBrush() { return GetBrush(1); }

        public GameForm(GameConfig GameConfig, int[,] matrix) : base()
        {
            _matrix = matrix;

            _pixelSize = GameConfig.PixelSize;
            base.BackColor = GameConfig.BackgroundColor;
            base.ForeColor = GameConfig.ForegroundColor;

            _additionalColors = new Color[GameConfig.AdditionalColors.Length];
            for (int i = 0; i < _additionalColors.Length; i++) _additionalColors[i] = GameConfig.AdditionalColors[i];

            _invalidColor = GameConfig.InvalidColor;

            this.FormBorderStyle = FormBorderStyle.FixedToolWindow;

            this.Text = GameConfig.Title;

            this.DoubleBuffered = true;
            SetStyle(ControlStyles.UserPaint, true);

            OnRefreshMatrix();
            OnRefreshBrushes();
        }

        private void OnRefreshMatrix()
        {
            this.ClientSize = new Size(Matrix.GetLength(0) * PixelSize, Matrix.GetLength(1) * PixelSize);
        }

        private void OnRefreshBrushes()
        {
            int count = _additionalColors != null ? _additionalColors.Length : 0;
            count += 2; // Include background and foreground
            _brushes = new Brush[count];

            _brushes[0] = new SolidBrush(this.BackColor);
            _brushes[1] = new SolidBrush(this.ForeColor);

            if (_additionalColors != null)
            {
                for (int i = 0; i < _additionalColors.Length; i++)
                {
                    _brushes[i + 2] = new SolidBrush(_additionalColors[i]);
                }
            }

            _invalidColorBrush = new SolidBrush(_invalidColor);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            for (int x = 0; x < Matrix.GetLength(0); x++)
            {
                for (int y = 0; y < Matrix.GetLength(1); y++)
                {
                    // The matrix is transposed, so the X screen coordinate is aligned to the matrix row index,
                    // and the Y screen coordinate is aligned to the matrix column index
                    e.Graphics.FillRectangle(GetBrush(Matrix[x, y]), x * PixelSize, y * PixelSize, PixelSize, PixelSize);
                }
            }            
        }
    }
}
