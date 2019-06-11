using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.IO.MemoryMappedFiles;

namespace LargeFileReadTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //UnitTest1();  // # ReadAllText
            //UnitTest2();  // # FileStream
            UnitTest3();    // # BinaryReader
        }

        private static void UnitTest1()
        {
            string path = @"D:\Dev\LogTest\test.txt";

            Console.WriteLine("1. Start : " + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff"));


            if (!File.Exists(path))
            {
                string createText = "Hello and Welcome" + Environment.NewLine;
                File.WriteAllText(path, createText, Encoding.UTF8);
            }

            Console.WriteLine("2. Exists : " + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff"));


            string appendText = "This is extra text " + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + Environment.NewLine;
            File.AppendAllText(path, appendText, Encoding.UTF8);

            Console.WriteLine("3. Append : " + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff"));

            string readText = File.ReadAllText(path);
            Console.WriteLine(readText.Substring(0,10));
            Console.WriteLine("4. ReadAllText : " + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff"));
        }

        private static void UnitTest2()
        {
            string path = @"D:\Dev\LogTest\test.txt";

            Console.WriteLine("1. Start : " + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff"));

            if (!File.Exists(path))
            {
                string createText = "Hello and Welcome" + Environment.NewLine;
                File.WriteAllText(path, createText, Encoding.UTF8);
            }

            Console.WriteLine("2. Exists : " + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff"));

            using (FileStream fsSource = new FileStream(path, FileMode.Open, FileAccess.Read))
            {

                // Read the source file into a byte array.
                byte[] bytes = new byte[fsSource.Length];
                int numBytesToRead = (int)fsSource.Length;
                int numBytesRead = 0;
                while (numBytesToRead > 0)
                {
                    // Read may return anything from 0 to numBytesToRead.
                    int n = fsSource.Read(bytes, numBytesRead, numBytesToRead);

                    // Break when the end of the file is reached.
                    if (n == 0)
                        break;

                    numBytesRead += n;
                    numBytesToRead -= n;
                }
                numBytesToRead = bytes.Length;

                Console.WriteLine("3. Read : " + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff"));
                Console.WriteLine("numBytesToRead : " + ConvertBytesToMegabytes(numBytesToRead).ToString() + " MB");
                
            }
        }

        private static void UnitTest3()
        {
            string path = @"D:\Dev\LogTest\test.txt";

            Console.WriteLine("1. Start : " + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff"));

            if (!File.Exists(path))
            {
                string createText = "Hello and Welcome" + Environment.NewLine;
                File.WriteAllText(path, createText, Encoding.UTF8);
            }

            Console.WriteLine("2. Exists : " + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff"));

            MemoryMappedFile mmf = MemoryMappedFile.CreateFromFile(path);
            MemoryMappedViewStream mms = mmf.CreateViewStream();
            int CHUNK_SIZE = 1000000000;

            using (BinaryReader br = new BinaryReader(mms))
            {
                byte[] chunk;

                chunk = br.ReadBytes(CHUNK_SIZE);
                Console.WriteLine("3. ReadBytes : " + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff"));
                Console.WriteLine("Byte Length : " + ConvertBytesToMegabytes(chunk.Length).ToString() + " MB");

                while (chunk.Length > 0)
                {
                    //DumpBytes(chunk, chunk.Length);
                    chunk = br.ReadBytes(CHUNK_SIZE);
                }
                
            }

            Console.WriteLine("4. Read : " + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff"));
            
        }

        private static double ConvertBytesToMegabytes(long bytes)
        {
            return (bytes / 1024f) / 1024f;
        }

        private static void DumpBytes(byte[] bdata, int len)
        {
            int i;
            int j = 0;
            char dchar;
            // 3 * 16 chars for hex display, 16 chars for text and 8 chars
            // for the 'gutter' int the middle.
            StringBuilder dumptext = new StringBuilder("        ", 16 * 4 + 8);
            for (i = 0; i < len; i++)
            {
                dumptext.Insert(j * 3, String.Format("{0:X2} ", (int)bdata[i]));
                dchar = (char)bdata[i];
                //' replace 'non-printable' chars with a '.'.
                if (Char.IsWhiteSpace(dchar) || Char.IsControl(dchar))
                {
                    dchar = '.';
                }
                dumptext.Append(dchar);
                j++;
                if (j == 16)
                {
                    Console.WriteLine(dumptext);
                    dumptext.Length = 0;
                    dumptext.Append("        ");
                    j = 0;
                }
            }
            // display the remaining line
            if (j > 0)
            {
                for (i = j; i < 16; i++)
                {
                    dumptext.Insert(j * 3, "   ");
                }
                Console.WriteLine(dumptext);
            }
        }


    }
}
