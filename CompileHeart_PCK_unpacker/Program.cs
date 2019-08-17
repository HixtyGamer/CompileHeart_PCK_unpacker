using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompileHeart_PCK_unpacker
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Path to PCK file");
            string path = Console.ReadLine();
            Console.WriteLine("Folder to save files");
            string pathEnd = Console.ReadLine();
            if (pathEnd[pathEnd.Length - 1] != Convert.ToChar("/"))
                pathEnd += "/";
            using (FileStream fstream = new FileStream(path, FileMode.Open))
            {
                byte[] name_ofs = new byte[4];
                byte[] data_ofs = new byte[4];
                byte[] ofs = new byte[4];
                byte[] n = new byte[1];
                byte[] size = new byte[4];
                int offset = 0, n_offset;
                string name = "", cat = "";

                fstream.Seek(8, SeekOrigin.Begin);
                fstream.Read(data_ofs, 0, 4);
                offset = Convert.ToInt32(fstream.Position);
                fstream.Read(name_ofs, 0, 4);
                n_offset = BitConverter.ToInt32(name_ofs, 0);

                string[] names = new string[n_offset/4];

                for (int i = 0; i < n_offset; i += 4)
                {
                    fstream.Seek(offset + i, SeekOrigin.Begin);
                    fstream.Read(name_ofs, 0, 4);
                    fstream.Seek(BitConverter.ToInt32(name_ofs, 0) + offset, SeekOrigin.Begin);
                    while(1 == 1)
                    {
                        fstream.Read(n, 0, 1);
                        if (n[0] == 0) break;
                        name += Encoding.ASCII.GetString(n, 0, 1);
                    }
                    names[i/4] = name;
                    name = "";
                }
                for (int i = 0; i < n_offset*2; i += 8)
                {
                    fstream.Seek(BitConverter.ToInt32(data_ofs, 0) + i + 16, SeekOrigin.Begin);
                    fstream.Read(name_ofs, 0, 4);   //reuse of byte array to mark file offset
                    fstream.Read(size, 0, 4);
                    fstream.Seek(BitConverter.ToInt32(name_ofs, 0), SeekOrigin.Begin);

                    byte[] file = new byte[BitConverter.ToInt32(size, 0)];

                    for (int h = 0; h < BitConverter.ToInt32(size, 0); h++)
                    {
                        fstream.Read(n, 0, 1);
                        file[h] = n[0];
                    }
                    if (names[i / 8].LastIndexOf("/") >= 0)
                    {
                        cat = names[i / 8].Substring(0,names[i / 8].LastIndexOf("/"));
                        Directory.CreateDirectory(pathEnd + cat);
                    }
                    File.WriteAllBytes(pathEnd + names[i / 8], file);
                    Console.WriteLine("Unpacked: {0}", names[i / 8]);
                }
                Console.WriteLine("Unpacked {0} files.", n_offset / 4);
                Console.ReadKey();
            }
        }
    }
}
