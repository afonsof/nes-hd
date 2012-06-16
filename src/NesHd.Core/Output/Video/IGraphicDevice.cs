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

        /// <summary>
        /// Get the buffer ready and start, call this at scanline 0 
        /// (at the begining of the frame)
        /// </summary>
        void Begin();

        /// <summary>
        /// Add a scanline into the video buffer, must be 256 length
        /// </summary>
        /// <param name="line">The line index number, 0 is the first line</param>
        /// <param name="scanlineBuffer">The scanline to add, 
        /// must be 256 length, 32-bit color format (RGB)</param>
        void AddScanline(int line, int[] scanlineBuffer);

        /// <summary>
        /// Draw a pixel into the video buffer
        /// </summary>
        /// <param name="x">The pixel x coordinate</param>
        /// <param name="y">The pixel y coordinate</param>
        /// <param name="color">The 32-bit (RGB) color of the pixel</param>
        void DrawPixel(int x, int y, int color);

        void DrawAbsolutePixel(int x, int y, int color);

        /// <summary>
        /// After you complete the buffer, call this to draw the buffer 
        /// into the screen (at the end of the frame, scanline # 240)
        /// </summary>
        void RenderFrame();

        /// <summary>
        /// Take and save snapshot
        /// </summary>
        /// <param name="snapPath">The path where to save the image</param>
        /// <param name="format">The image format (e.g : bmp)</param>
        void TakeSnapshot(string snapPath, string format);

        /// <summary>
        /// Draw a text into the screen
        /// </summary>
        /// <param name="text">The text to draw</param>
        /// <param name="frames">The frames you want it to appear</param>
        void DrawText(string text, int frames);

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
        /// <param name="x">The x coordinate</param>
        /// <param name="y">The y coordinate</param>
        /// <param name="w">The screen width</param>
        /// <param name="h">The screen height</param>
        void UpdateSize(int x, int y, int w, int h);
    }
}