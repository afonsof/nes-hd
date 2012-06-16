using System.Windows.Forms;

namespace NesHd.Core.Output.Audio.Devices
{
    /*
     * This should do all the audio rendering operations
     * just like the video devices.
     * However, it's incompleted yet.
     */

    public class SoundDeviceGeneral16 : IAudioDevice
    {
        private readonly Control _Control;

        public SoundDeviceGeneral16(Control Cont)
        {
            _Control = Cont;
        }

        #region IAudioDevice Members

        public Control SoundDevice
        {
            get { return _Control; }
        }

        public bool Stereo { get; set; }

        #endregion
    }
}