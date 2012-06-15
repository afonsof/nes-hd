/*
This file is part of My Nes
A Nintendo Entertainment System Emulator.

 Copyright © 2009 - 2010 Ala Hadid (AHD)

My Nes is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation; either version 2 of the License, or
(at your option) any later version.

My Nes is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
*/

namespace NesHd.Core.Output.Video
{
    /*! WARNING !*/
    /*! ANY GRAPHIC DEVICE YOU ADD MUST BE LISTED HERE !*/
    public enum GraphicDevices
    {
        Gdi,
        GdiHiRes,
        SlimDx
    }
    public interface IGraphicDevice
    {
        /// <summary>
        /// The name of this device
        /// </summary>
        string Name { get; }
        /// <summary>
        /// The Description of this device
        /// </summary>
        string Description { get; }
        /// <summary>
        /// Get the buffer ready and start, call this at scanline 0 
        /// (at the begining of the frame)
        /// </summary>
        void Begin();
        /// <summary>
        /// Add a scanline into the video buffer, must be 256 length
        /// </summary>
        /// <param name="Line">The line index number, 0 is the first line</param>
        /// <param name="ScanlineBuffer">The scanline to add, 
        /// must be 256 length, 32-bit color format (RGB)</param>
        void AddScanline(int Line, int[] ScanlineBuffer);
        /// <summary>
        /// Draw a pixel into the video buffer
        /// </summary>
        /// <param name="X">The pixel x coordinate</param>
        /// <param name="Y">The pixel y coordinate</param>
        /// <param name="Color">The 32-bit (RGB) color of the pixel</param>
        void DrawPixel(int X, int Y, int Color);
        /// <summary>
        /// After you complete the buffer, call this to draw the buffer 
        /// into the screen (at the end of the frame, scanline # 240)
        /// </summary>
        void RenderFrame();
        /// <summary>
        /// Take and save snapshot
        /// </summary>
        /// <param name="SnapPath">The path where to save the image</param>
        /// <param name="Format">The image format (e.g : bmp)</param>
        void TakeSnapshot(string SnapPath, string Format);
        /// <summary>
        /// Draw a text into the screen
        /// </summary>
        /// <param name="Text">The text to draw</param>
        /// <param name="Frames">The frames you want it to appear</param>
        void DrawText(string Text, int Frames);
        /// <summary>
        /// Change the settings of this device
        /// </summary>
        void ChangeSettings();
        /// <summary>
        /// Clear the screen (Black it !!)
        /// </summary>
        void Clear();
        /// <summary>
        /// Update the draw size if this video device is sizabel
        /// </summary>
        /// <param name="X">The x coordinate</param>
        /// <param name="Y">The y coordinate</param>
        /// <param name="W">The screen width</param>
        /// <param name="H">The screen height</param>
        void UpdateSize(int X, int Y, int W, int H);
        /// <summary>
        /// Get if this video device is sizable
        /// </summary>
        bool IsSizable { get; }
        /// <summary>
        /// Get if this video is currently rendering or not
        /// </summary>
        bool IsRendering { get; }
        /// <summary>
        /// Get or set if the video device can render a frame (ON/OFF)
        /// </summary>
        bool CanRender { get; set; }
        bool FullScreen { get; set; }
        bool SupportFullScreen { get; }
    }
}
