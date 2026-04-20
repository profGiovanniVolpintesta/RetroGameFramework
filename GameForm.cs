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

        private bool[,] _matrix = null;
        public bool[,] Matrix
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

        private Brush foreBrush = null;
        private Brush backBrush = null;

        public GameForm(bool[,] matrix) : base()
        {
            _matrix = matrix;
            base.BackColor = Initializer.BackgroundColor;
            base.ForeColor = Initializer.ForegroundColor;
            this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            this.Text = Initializer.Title;
            OnRefreshMatrix();
            OnRefreshBrushes();
        }

        private void OnRefreshMatrix()
        {
            this.ClientSize = new Size(Matrix.GetLength(0) * PixelSize, Matrix.GetLength(1) * PixelSize);
        }

        private void OnRefreshBrushes()
        {
            foreBrush = new SolidBrush(ForeColor);
            backBrush = new SolidBrush(BackColor);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            
            for (int x = 0; x < Matrix.GetLength(0); x++)
            {
                for (int y = 0; y < Matrix.GetLength(1); y++)
                {
                    e.Graphics.FillRectangle(Matrix[x, y] ? foreBrush : backBrush, x * PixelSize, y * PixelSize, PixelSize, PixelSize);
                }
            }            
        }
    }
}
