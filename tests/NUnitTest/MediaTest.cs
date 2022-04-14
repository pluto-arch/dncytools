#if NETFRAMEWORK
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using NUnit.Framework;
using Dncy.Tools.Media;

namespace NUnitTest
{

    

    public class MediaTest
    {

        [SetUp]
        public void Setup()
        {

        }

        private const string path = @"C:\Users\62052\Pictures\Camera Roll\Crusader_f.png";
        
        [Test]
        public void Test_Image_CutForSquare()
        {
            var savePath = AppDomain.CurrentDomain.BaseDirectory + "\\CutForSquare.png";
            using var ms = new FileStream(path, FileMode.Open);
            ms.CutForSquare(savePath,720,10);
        }


        [Test]
        public void Test_Image_CutForCustom()
        {
            var savePath = AppDomain.CurrentDomain.BaseDirectory + "\\CutForCustom.png";
            using var ms = new FileStream(path, FileMode.Open);
            ms.CutForCustom(savePath,1920,1080,0);
        }

        
        [Test]
        public void Test_Image_ZoomAuto()
        {
            var savePath = AppDomain.CurrentDomain.BaseDirectory + "\\ZoomAuto.png";
            using var ms = new FileStream(path, FileMode.Open);
            ms.ZoomAuto(savePath, 300, 280);
        }


        [Test]
        public void Test_Fluat()
        {
            var savePath = AppDomain.CurrentDomain.BaseDirectory + "\\Fluat.png";
            using var image = Image.FromFile(path);
            image.ZoomAuto(300, 280).AppendWatermark("hello", 80, 20, new Font("微软雅黑", 40)).Save(savePath,image.RawFormat);
        }       
        
    }
}
#endif