using System.Collections.Generic;

namespace NesHd.Core.Output.Video
{
    internal class BitmapF
    {
        public int BitmapId;
        public int Color1;
        public int Color2;
        public int Color3;
        public int X;
        public int Y;
    }

    internal class TileData
    {
        public List<BitmapF> BitmapP;
        public int DefaultId;
    }
}