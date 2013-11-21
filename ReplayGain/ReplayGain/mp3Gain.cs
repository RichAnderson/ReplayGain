using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace ReplayGain
{

    class mp3GainInterface
    {
        public enum GainType { Track, Album, Constant }
        public void runMp3Gain(List<String> list, GainType gainType = GainType.Track, double gain = 89)
        {
            StringBuilder sb = new StringBuilder();

            if (gain > 255 || gain < 0)
            {
                throw new ArgumentOutOfRangeException("Gain must be 0-255");
            }

            switch (gainType)
            {
                case GainType.Track:
                    sb.Append(" /r ");
                    break;
                case GainType.Album:
                    sb.Append(" /a ");
                    break;
                case GainType.Constant:
                    sb.Append(String.Format(" /g {0} /c ", gain));
                    break;
            }

            foreach (string s in list)
            {
                if (File.Exists(s))
                {
                    sb.AppendFormat("\"{0}\" ", s);
                }
            }

            System.Diagnostics.Process mp3GainProcess = new Process();

            try
            {
                mp3GainProcess.StartInfo.UseShellExecute = false;
                mp3GainProcess.StartInfo.CreateNoWindow = false; //Change to true to hide the window
                mp3GainProcess.StartInfo.FileName = "mp3gain\\mp3gain.exe";
                mp3GainProcess.StartInfo.Arguments = sb.ToString();
                mp3GainProcess.Start();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
