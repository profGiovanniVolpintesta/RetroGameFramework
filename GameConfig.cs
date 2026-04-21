using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RetroGameFramework
{
    internal class GameConfig
    {
        public enum PressReleaseRaceConditionRule
        {
            PressWins,
            ReleaseWins,
            CallBoth,
            CallNone
        }

        public int PixelsMatrixWidth = 50;
        public int PixelsMatrixHeight = 50;
        public int PixelSize = 10;

        public int FrameRate = 30;
        
        public Color BackgroundColor = Color.Black;
        public Color ForegroundColor = Color.White;
        public Color[] AdditionalColors = new Color[0];
        public Color InvalidColor = Color.Magenta;
        public string Title = "My Retro Game";

        // Sorted array of key priorities.
        // KeyCodes found earlier in this array are served before others found later.
        // The input events of the same key are served in the following order: KeyDown, KeyUp, KeyPressed
        public Keys[] KeysPriority = null;

        // What happens if a key if pressed and released in the same frame?
        public PressReleaseRaceConditionRule PressReleaseRaceConditionPolicy = PressReleaseRaceConditionRule.CallBoth;
    }
}
