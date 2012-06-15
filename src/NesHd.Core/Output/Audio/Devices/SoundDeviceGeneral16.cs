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
        Control _Control;
        bool _Stereo = false;
        public SoundDeviceGeneral16(Control Cont)
        { this._Control = Cont; }
        public System.Windows.Forms.Control SoundDevice
        {
            get { return this._Control; }
        }

        public bool Stereo
        {
            get
            {
                return this._Stereo;
            }
            set
            {
                this._Stereo = value;
            }
        }
    }
}
