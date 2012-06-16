using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace NesHd.Core.Misc
{
    public class State
    {
        private readonly NesEngine _engine;
        public StateHolder Holder = new StateHolder();

        /// <summary>
        /// The state saver / loader
        /// </summary>
        /// <param name="NesEmu">The current system you want to save / load state from / into</param>
        public State(NesEngine NesEmu)
        {
            _engine = NesEmu;
        }

        public bool SaveState(string FilePath)
        {
            try
            {
                _engine.Pause();
                Holder.LoadNesData(_engine);
                var fs = new FileStream(FilePath, FileMode.Create);
                var formatter = new BinaryFormatter();
                formatter.Serialize(fs, Holder);
                fs.Close();
                _engine.Resume();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool LoadState(string FilePath)
        {
            try
            {
                if (File.Exists(FilePath))
                {
                    _engine.Pause();
                    var fs = new FileStream(FilePath, FileMode.Open);
                    var formatter = new BinaryFormatter();
                    Holder = (StateHolder) formatter.Deserialize(fs);
                    fs.Close();
                    Holder.ApplyDataToNes(_engine);
                    _engine.Resume();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
            ;
        }
    }
}