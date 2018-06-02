
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms; // нужно для MessageBox'ов

using ZXing;
using ZXing.Common;

namespace QRCR
{
    [Guid("2A44957D-A704-4370-9C4B-5D6EE147ABB6")]
    internal interface IQRCR
    {
        [DispId(1)]
        string findAndParseQRCode(string path);
    }

    [Guid("CC441039-F00D-4617-B2AD-36082B7D4D9D"), InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface IMyEvents
    {
    }
    
    [Guid("E60248B4-F573-4EAE-93E9-5E77341D05BA"), ClassInterface(ClassInterfaceType.None), ComSourceInterfaces(typeof(IMyEvents))]
    public class QRCR : IQRCR
    {
        // Пример MessageBox'a для отладки
        // MessageBox.Show("Это текст", "Это заголовок", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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

        public string findAndParseQRCode(string path)
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

        // не забудь убрать static
    }

}