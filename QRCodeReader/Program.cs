using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using ZXing;
using ZXing.Common;

namespace QRCodeReader
{
    class Program
    {

        public static string parse(Binarizer bin)
        {
            var bitmap = new BinaryBitmap(bin);
            var reader = new ZXing.QrCode.QRCodeReader();
            var result = reader.decode(bitmap);
            if (result == null)
            {
                return null;
            }
            return result.Text;
        }

        public static string parseWithAllBinarizers(LuminanceSource lum)
        {
            var res1 = parse(new GlobalHistogramBinarizer(lum)); // основная бинаризация
            if (res1 == null)
            {
                res1 = parse(new HybridBinarizer(lum)); // вспомогательная бинаризация, если основная не справилась
            }
            return res1;
        }

        public static string parseLumWithRotations(LuminanceSource lum)
        {
            for (var i = 0; i < 4; i++)
            {
                Console.WriteLine("lum" + i);
                var res = parseWithAllBinarizers(lum);
                if (res != null)
                {
                    return res;
                }
                if (i == 3) break;
                lum = lum.rotateCounterClockwise();
            }
            return null;
        }
       
        public static string findAndParseQRCode(string path)
        {
            var barcodeImg = (Bitmap)Image.FromFile(path);
            var lum = new BitmapLuminanceSource(barcodeImg);
            barcodeImg.Dispose();
            if (!lum.RotateSupported)
            {
                return "Поворачивания картинок с яркостями не поддерживаются, что бы это не значило. Надо поворачивать до преобразования :)";
            }
            return parseLumWithRotations(lum);
        }
        
        static void Main(string[] args)
        {
            var result = findAndParseQRCode(@"..\..\..\examples\2018.04.05\3_crop.jpg");

            if (result != null)
            {
                Console.Write("Найденный текст: " + result);
            } else
            {
                Console.Write("result is null");

            }
            Console.ReadKey();
        }
    }
}
