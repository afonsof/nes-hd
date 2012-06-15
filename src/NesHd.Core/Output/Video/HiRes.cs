using System.Collections.Generic;

namespace NesHd.Core.Output.Video
{
    class BitmapF
    {
        public int BitmapId;
        public int Color1;
        public int Color2;
        public int Color3;
        public int X;
        public int Y;
    }

    class TileData
    {
        public List<BitmapF> BitmapP;
        public int DefaultId;
    }
}
