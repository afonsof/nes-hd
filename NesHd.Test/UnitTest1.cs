using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NesHd.Core;
using NesHd.Core.PPU;

namespace NesHd.Test
{
    struct Xy
    {
        public Xy(int x, int y)
        {
            X = x;
            Y = y;
        }
        public int X;
        public int Y;

        public override string ToString()
        {
            return X + ", " + Y;
        }
    }

    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    /// 
    /// 
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestPpuCoord()
        {
            var ppu = new Ppu(TvFormat.Ntsc, new PaletteFormat(), null);

            //Multiplicador 1
            Assert.AreEqual(new Xy(0,0), GetXy(0x8010,0));
            Assert.AreEqual(new Xy(1, 0), GetXy(0x8010, 1));
            Assert.AreEqual(new Xy(2, 0), GetXy(0x8010, 2));
            Assert.AreEqual(new Xy(3, 0), GetXy(0x8010, 3));
            Assert.AreEqual(new Xy(4, 0), GetXy(0x8010, 4));
            Assert.AreEqual(new Xy(5, 0), GetXy(0x8010, 5));
            Assert.AreEqual(new Xy(6, 0), GetXy(0x8010, 6));
            Assert.AreEqual(new Xy(7, 0), GetXy(0x8010, 7));

            Assert.AreEqual(new Xy(48, 136), GetXy(0x9170, 0));

            Assert.AreEqual(new Xy(80, 128), GetXy(0x90B0, 0));
            Assert.AreEqual(new Xy(320, 512), GetXy(0x90B0, 0, 4));

            Assert.AreEqual(new Xy(48, 136), GetXy(0x9170, 0));
            Assert.AreEqual(new Xy(192, 544), GetXy(0x9170, 0, 4));
       }

        private Xy GetXy(int offset, int desl, int multi = 1)
        {
            offset -= 0x8010;
            var ppu = new Ppu(TvFormat.Ntsc, new PaletteFormat(), null);

            var w = 128*multi;

            return new Xy
                       {
                           X = ppu.GetX(offset, desl, multi),
                           Y = ppu.GetY(offset, w, multi)
                       };
        }
    }
}
