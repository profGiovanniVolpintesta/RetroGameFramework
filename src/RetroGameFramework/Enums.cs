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

namespace RetroGameFramework
{
    public enum PressReleaseRaceConditionRule
    {
        PressWins,
        ReleaseWins,
        CallBoth,
        CallNone
    }

    public enum AnchorType
    {
        TopLeft = 0,
        BottomLeft,
        TopRight,
        BottomRight,
        Left,
        Top,
        Bottom,
        Right,
        Center
    }

    public enum MatrixOrientation
    {
        Transposed = 0, // default
        AlignedToScreen
    }
}
