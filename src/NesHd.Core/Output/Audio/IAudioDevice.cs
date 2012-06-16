using System.Windows.Forms;

namespace NesHd.Core.Output.Audio
{
    public interface IAudioDevice
    {
        Control SoundDevice { get; }
        bool Stereo { get; set; }
    }
}