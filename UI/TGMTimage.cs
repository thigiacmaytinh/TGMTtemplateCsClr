using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGMTcs
{
    class TGMTimage
    {
        public static Bitmap CorrectOrientation(Bitmap bmp)
        {
            if (Array.IndexOf(bmp.PropertyIdList, 274) > -1)
            {
                var orientation = (int)bmp.GetPropertyItem(274).Value[0];
                switch (orientation)
                {
                    case 1:
                        // No rotation required.
                        break;
                    case 2:
                        bmp.RotateFlip(RotateFlipType.RotateNoneFlipX);
                        break;
                    case 3:
                        bmp.RotateFlip(RotateFlipType.Rotate180FlipNone);
                        break;
                    case 4:
                        bmp.RotateFlip(RotateFlipType.Rotate180FlipX);
                        break;
                    case 5:
                        bmp.RotateFlip(RotateFlipType.Rotate90FlipX);
                        break;
                    case 6:
                        bmp.RotateFlip(RotateFlipType.Rotate90FlipNone);
                        break;
                    case 7:
                        bmp.RotateFlip(RotateFlipType.Rotate270FlipX);
                        break;
                    case 8:
                        bmp.RotateFlip(RotateFlipType.Rotate270FlipNone);
                        break;
                }
                // This EXIF data is now invalid and should be removed.
                bmp.RemovePropertyItem(274);
            }
            return bmp;           
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static string ImageToBase64(string filename)
        {
            FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
            byte[] filebytes = new byte[fs.Length];
            fs.Read(filebytes, 0, Convert.ToInt32(fs.Length));
            fs.Close();
            return "data:image/png;base64," + Convert.ToBase64String(filebytes, Base64FormattingOptions.None);
            
        }
    }
}
