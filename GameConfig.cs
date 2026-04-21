using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetroGameFramework
{
    internal class GameConfig
    {
        public Type GameLogicClass = typeof(GameLogic);

        public int PixelsMatrixWidth = 50;
        public int PixelsMatrixHeight = 50;
        public int PixelSize = 10;

        public int FrameRate = 30;
        
        public Color BackgroundColor = Color.Black;
        public Color ForegroundColor = Color.White;
        public Color[] AdditionalColors = new Color[0];
        public Color InvalidColor = Color.Magenta;
        public string Title = "My Retro Game";
    }
}
