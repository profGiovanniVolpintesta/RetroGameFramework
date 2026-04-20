using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static RetroGameFramework.GameForm;

namespace RetroGameFramework
{
    internal class GameForm : Form
    {
        public static class Initializer
        {
            public static int PixelSize = 10;
            public static Color BackgroundColor = Color.Black;
            public static Color ForegroundColor = Color.White;
            public static Color[] AdditionalColors = new Color[0];
            public static Color InvalidColor = Color.Magenta;
            public static string Title = "My Retro Game";
        }

        private int _pixelSize = Initializer.PixelSize;
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

        public GameForm(int[,] matrix) : base()
        {
            _matrix = matrix;

            base.BackColor = Initializer.BackgroundColor;
            base.ForeColor = Initializer.ForegroundColor;

            _additionalColors = new Color[Initializer.AdditionalColors.Length];
            for (int i = 0; i < _additionalColors.Length; i++) _additionalColors[i] = Initializer.AdditionalColors[i];

            _invalidColor = Initializer.InvalidColor;

            this.FormBorderStyle = FormBorderStyle.FixedToolWindow;

            this.Text = Initializer.Title;

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
                    e.Graphics.FillRectangle(GetBrush(Matrix[x, y]), x * PixelSize, y * PixelSize, PixelSize, PixelSize);
                }
            }            
        }
    }
}
