using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Mp3Sharp;
using WaveConvertNet;

namespace MP3DecodeStream
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            Mp3Stream mp3Stream = new Mp3Stream(AppDomain.CurrentDomain.BaseDirectory + @"/Sample/sample.mp3");

            Console.WriteLine("Sound Format: {0}", mp3Stream.Format);
            Console.WriteLine("Frequency: {0}", mp3Stream.Frequency);

            WaveFile waveFile = new WaveFile();
            int openfileResult = waveFile.OpenFile(AppDomain.CurrentDomain.BaseDirectory + @"Sample/sample.wav", WaveFormat.WF_PCM_S16LE, false, 44100, 2);
            if (openfileResult == 0)
            {
                short[,] data = null;
                byte[] buffer = new byte[512];
                waveFile.CreateData(ref data, 128);
                Console.WriteLine("Data Length: [{0},{1}]", data.GetLength(0), data.GetLength(1));

                int ret = -1;
                double p1, p2;
                while (ret != 0)
                {
                    ret = mp3Stream.Read(buffer, 0, buffer.Length);
                    int i;
                    for (i = 0; i < data.GetLength(1); i++)
                    {
                        if (i * 4 >= ret) break;
                        //Single channel with sound mix.
                        // p1 = (double)BitConverter.ToInt16(buffer, i * 4);
                        // p2 = (double)BitConverter.ToInt16(buffer, i * 4 + 2);
                        //data[0, i] = (short)((p1 * p1 + p2 * p2) / (p1 + p2));
                        //data[0, i] = (short)((p2+p1)/2);
                        data[0, i] = BitConverter.ToInt16(buffer, i * 4);
                        data[1, i] = BitConverter.ToInt16(buffer, i * 4 + 2);
                    }
                    waveFile.PutData(data, i);
                }
                waveFile.FlushFile();
                waveFile.CloseFile();
            }
            else
            {
                Console.WriteLine("Can not create waveFile:{0}", openfileResult);
            }

            Console.WriteLine("Finished");
            Console.ReadLine();
            return;
        }
    }
}
