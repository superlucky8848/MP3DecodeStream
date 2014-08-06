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

            Mp3Stream mp3Stream = new Mp3Stream(AppDomain.CurrentDomain.BaseDirectory + @"/Sample/sample2.mp3");

            WaveFile waveFile = new WaveFile();
            int openfileResult = waveFile.OpenFile(AppDomain.CurrentDomain.BaseDirectory + @"/Sample/sample2.wav", WaveFormat.WF_PCM_S16LE, false, 8000, 1);
            if (openfileResult == 0)
            {
                short[,] data = null;
                byte[] buffer = new byte[512];
                int ret;

                ret = mp3Stream.Read(buffer, 0, buffer.Length);
                Console.WriteLine("File Format: {0}", mp3Stream.Format);
                Console.WriteLine("Frequency: {0}", mp3Stream.Frequency);
                Console.WriteLine("Channel Count: {0}", mp3Stream.ChannelCount);
                
                if (mp3Stream.ChannelCount <= 0 || mp3Stream.Frequency <= 0)
                {
                    Console.WriteLine("Cannnot decode file.");
                }
                else
                {
                    waveFile.CreateData(ref data, 128);
                    Console.WriteLine("Data Length [{0},{1}]", data.GetLength(0), data.GetLength(1));

                    while (ret != 0)
                    {
                        int i;
                        for (i = 0; i < data.GetLength(1); i++)
                        {
                            if (i * 4>= ret) break;
                            double p1 = (double)BitConverter.ToInt16(buffer, i * 2) / 32767.0;
                            data[0, i] = (short)(
                                                    (     (double)BitConverter.ToInt16(buffer, i * 4) / 32767.0
                                                        + (double)BitConverter.ToInt16(buffer, i * 4 + 2) / 32767.0
                                                    ) * 32767.0 / 2.0);
                        }
                        waveFile.PutData(data, i);
                        ret = mp3Stream.Read(buffer, 0, buffer.Length);
                    }
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
