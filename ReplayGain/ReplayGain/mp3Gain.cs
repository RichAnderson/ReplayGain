using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace ReplayGain
{

    class mp3GainInterface
    {
        public enum GainType { Track, Album }
        public void runMp3Gain(List<String> list, GainType gainType = GainType.Track)
        {
            StringBuilder sb = new StringBuilder();

            switch (gainType)
            {
                case GainType.Track:
                    sb.Append(" /r ");
                    break;
                case GainType.Album:
                    sb.Append(" /a ");
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
                mp3GainProcess.StartInfo.CreateNoWindow = false;
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
