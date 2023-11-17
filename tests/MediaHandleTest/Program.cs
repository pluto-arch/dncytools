using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dncy.Tools.Media;

namespace MediaHandleTest
{
    internal class Program
    {
        private const string path = @"C:\Users\62052\Pictures\Saved Pictures\wallhaven-lq7vor.png";

        static void Main(string[] args)
        {
            var savePath = AppDomain.CurrentDomain.BaseDirectory + "\\CutForCustom.png";
            using (var ms = new FileStream(path, FileMode.Open))
            {
                ms.CutForCustom(savePath,720,540,100);
            }
        }
    }
}
