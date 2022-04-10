using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Dncy.Tools.Media.Images
{
    public static class ImageTools
    {
        #region 正方型裁剪并缩放
        /// <summary>
        /// 正方型裁剪
        /// 以图片中心为轴心，截取正方型，然后等比缩放
        /// 用于头像处理
        /// </summary>
        /// <param name="fromFile">原图Stream对象</param>
        /// <param name="fileSaveUrl">缩略图存放地址</param>
        /// <param name="side">指定的边长（正方型）</param>
        /// <param name="quality">质量（范围0-100）</param>
        public static void CutForSquare(this Stream fromFile, string fileSaveUrl, int side, int quality)
        {
            
        }

        #endregion
    }
}