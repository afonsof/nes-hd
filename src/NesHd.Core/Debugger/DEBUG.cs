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

using System;

namespace NesHd.Core.Debugger
{
    public class Debug
    {
        public static void WriteLine(object Sender, string Line, DebugStatus Status)
        {
            EventHandler<DebugArg> handler = DebugRised;
            if (handler != null)
            {
                handler(Sender, new DebugArg(Line, Status));
            }
        }
        public static void WriteSeparateLine(object Sender, DebugStatus Status)
        {
            EventHandler<DebugArg> handler = DebugRised;
            if (handler != null)
            {
                handler(Sender, new DebugArg("==========================", Status));
            }
        }
        /// <summary>
        /// Rised when the system write a debug
        /// </summary>
        public static event EventHandler<DebugArg> DebugRised;
    }
    public class DebugArg : EventArgs
    {
        public DebugArg(string Line, DebugStatus Status)
        {
            this._DebugLine = Line;
            this._Status = Status;
        }
        DebugStatus _Status;
        string _DebugLine;
        public DebugStatus Status
        { get { return this._Status; } }
        public string DebugLine
        { get { return this._DebugLine; } }
    }
    public enum DebugStatus
    {
        /// <summary>
        /// Color white
        /// </summary>
        None,
        /// <summary>
        /// Color green
        /// </summary>
        Cool,
        /// <summary>
        /// Color Yellow
        /// </summary>
        Warning,
        /// <summary>
        /// Color Red
        /// </summary>
        Error
    }
}
