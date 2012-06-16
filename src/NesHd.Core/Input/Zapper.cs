namespace NesHd.Core.Input
{
    public class Zapper
    {
        private readonly NesEngine _engine;
        private int _detected = 1;
        private int _trigger = 1;

        public Zapper(NesEngine engine)
        {
            _engine = engine;
        }

        public void PullTrigger(bool pull, int x, int y)
        {
            _trigger = pull ? 0 : 1;
            if (pull)
                _detected = 1;
            _engine.Ppu.ZapperX = x;
            _engine.Ppu.ZapperY = y;
            _engine.Ppu.CheckZapperHit = true;
            _engine.Ppu.ZapperFrame = 0;
        }

        public byte GetData()
        {
            return (byte) ((_detected | (_trigger << 1)) << 3);
        }

        public void SetDetect(int V)
        {
            _detected = V;
        }
    }
}