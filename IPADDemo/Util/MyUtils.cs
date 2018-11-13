using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPADDemo.Util
{
    public static class MyUtils
    {
        public static bool IsFileInUse(string fileName)
        {
            bool inUse = true;
            FileStream fs = null;
            try
            {

                fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);

                inUse = false;
            }
            catch (Exception ex)
            {
                try
                {
                    Process[] pcs = Process.GetProcesses();
                    foreach (Process p in pcs)
                    {
                        if (p.MainModule.FileName == fileName)
                        {
                            p.Kill();
                        }
                    }
                }
                catch (Exception e) { }
            }
            finally
            {
                if (fs != null)

                    fs.Close();
            }
            return inUse;//true表示正在使用,false没有使用  
        }

        public static Bitmap Base64StringToImage(string basestr)
        {
            Bitmap bitmap = null;
            try
            {
                String inputStr = basestr;
                byte[] arr = Convert.FromBase64String(inputStr);
                MemoryStream ms = new MemoryStream(arr);
                Bitmap bmp = new Bitmap(ms);
                ms.Close();
                bitmap = bmp;
                //MessageBox.Show("转换成功！");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Base64StringToImage 转换失败\nException：" + ex.Message);
            }

            return bitmap;
        }

        public static string ImageToBase64String(this Image image)
        {
            return Convert.ToBase64String(image.ImageToBytes());
        }
    }
}
