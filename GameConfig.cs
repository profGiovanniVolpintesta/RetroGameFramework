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

        public bool forceRandomGeneratorSeed = false;
        public int randomGeneratorSeed = 0;

    }
}
