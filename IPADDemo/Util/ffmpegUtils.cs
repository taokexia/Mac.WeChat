using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace IPADDemo.Util
{
    public class ffmpegUtils
    {
        private static string url = System.IO.Directory.GetCurrentDirectory();
        private static ffmpegUtils instance;
        private static object _lock = new object();
        private static int maxNum = 10;

        private ffmpegUtils()
        {
            Init();
        }

        public static ffmpegUtils GetInstance()
        {
            if (instance == null)
            {
                lock (_lock)
                {
                    if (instance == null)
                    {
                        instance = new ffmpegUtils();
                    }
                }
            }
            return instance;
        }

        /// <summary>
        ///删除中间件pcm文件
        /// </summary>
        private static void Init()
        {
            if (!Directory.Exists(System.IO.Directory.GetCurrentDirectory() + @"\cache\"))
            {
                Directory.CreateDirectory(System.IO.Directory.GetCurrentDirectory() + @"\cache\");
            }
            foreach (string d in Directory.GetFileSystemEntries(System.IO.Directory.GetCurrentDirectory() + @"\cache\"))
            {
                if (File.Exists(d))
                {
                    try
                    {
                        FileInfo fi = new FileInfo(d);
                        if (fi.Extension.ToLower() == ".pcm")
                        {
                            if (fi.Attributes.ToString().IndexOf("ReadOnly") != -1)
                                fi.Attributes = FileAttributes.Normal;
                            File.Delete(d);//删除pcm中间文件
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// amr 转换  mp3 格式
        /// </summary>
        /// <param name="amrFile">amr文件所在路径</param>
        /// <returns></returns>
        public bool ConvertAmrToMp3(string amrFile, string savePath)
        {
            //if (!File.Exists(amrFile))
            //{
            //    MyLog.LogW("当前目录不存在amr文件" + amrFile);
            //    return false;
            //}
            string filePath = savePath;
            string fileName = amrFile.Substring(amrFile.LastIndexOf(@"\"));
            string finalName = fileName.Substring(1, fileName.IndexOf(".") - 1);

            string guid = Guid.NewGuid().ToString();
            string copyFile = @"silk_v3_encoder_" + guid;
            //string copyPath = url + @"\software\" + copyFile + ".exe";
            //System.IO.File.Copy(url + @"\silk_v3_decoder.exe", copyPath, true);
            string copyPath = url + @"\ffmpeg\silk_v3_decoder.exe";
            //amr解码为pcm格式
            string amrTopcm = "\"" + copyPath + "\"  \"" + amrFile + "\"  \"" + url + @"\cache\" + finalName + ".pcm\" -quiet";
            //pcm转码为MP3格式
            string pcmTomp3 = "\"" + url + "\\ffmpeg\\ffmpeg.exe\" -y -f s16le -ar 24000 -ac 1 -i  \"" + url + @"\cache\" + finalName + ".pcm\"" + "  \"" + filePath+ "\"";

            int indexNum = 1;
            try
            {
                bool b = FormatConvert(amrTopcm);
                string pcmdir = url + @"\cache\" + finalName + ".pcm";
                return ContinueTryConvert(amrFile, pcmdir, b, pcmTomp3, filePath, finalName, indexNum, copyFile, savePath);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// MP3 转换为 amr格式
        /// </summary>
        /// <param name="mp3File"></param>
        /// <returns></returns>
        public bool ConvertMp3ToAmr(string mp3File, string savePath)
        {
            //if (!File.Exists(mp3File))
            //{
            //    return false;
            //}
            string filePath = savePath;
            string fileName = "";
            if (mp3File.LastIndexOf(@"\") != -1)
            {
                fileName = mp3File.Substring(mp3File.LastIndexOf(@"\"));
            }
            else {
                fileName = mp3File;
            }
            string finalName = fileName.Substring(1, fileName.IndexOf(".") - 1);

            string guid = Guid.NewGuid().ToString();
            string copyFile = @"silk_v3_encoder_" + guid;
            //string copyPath = url + @"\software\" + copyFile + ".exe";
            //System.IO.File.Copy(url + @"\silk_v3_encoder.exe", copyPath, true);
            string copyPath = url + @"\ffmpeg\silk_v3_encoder.exe";
            //mp3转码位pcm格式
            string mp3Topcm = "\"" + url + "\\ffmpeg\\ffmpeg.exe\" -i \"" + mp3File + "\" -f s16le -acodec pcm_s16le -ac 1 -ar 24000 \"" + url + @"\cache\" + finalName + ".pcm\"";
            //pcm转码为amr格式
            string pcmTomp3 = "\"" + copyPath + "\"  \"" + url + @"\cache\" + finalName + ".pcm\"" + "  \"" + filePath + "\"  -tencent";

            int indexNum = 1;
            try
            {
                bool b = FormatConvert(mp3Topcm);
                string pcmdir = url + @"\cache\" + finalName + ".pcm";
                return ContinueTryConvert(mp3File, pcmdir, b, pcmTomp3, filePath, finalName, indexNum, copyFile, savePath);
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool ContinueTryConvert(string originFile, string pcmdir, bool b, string strCommand, string filePath, string finalName, int indexNum, string copyFile, string savePath)
        {
            if (File.Exists(pcmdir))
            {
                if (b)
                {
                    if (MyUtils.IsFileInUse(pcmdir))
                    {
                        Thread.Sleep(100);
                        return ContinueTryConvert(originFile, pcmdir, b, strCommand, filePath, finalName, indexNum, copyFile, savePath);
                    }
                    bool isSuccese = FormatConvert(strCommand);

                    #region 删除文件

                    Task.Factory.StartNew(() =>
                    {
                        Thread.Sleep(10000);
                        try
                        {
                            File.Delete(pcmdir);
                        }
                        catch (Exception ex)
                        {
                        }
                        try
                        {
                            string path = url + @"\software\" + copyFile + ".exe";
                            if (File.Exists(path))
                            {
                                new FileInfo(path).Attributes = FileAttributes.Normal;
                                File.Delete(path);
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                    });

                    #endregion 删除文件

                    indexNum = 1;
                    return ContinueSecondTryConvert(indexNum, filePath, strCommand);
                }
                else
                {
                    return false;
                }
            }
            else
            {
                indexNum++;
                if (indexNum <= maxNum)
                {
                    Thread.Sleep(200);
                    return ContinueTryConvert(originFile, pcmdir, b, strCommand, filePath, finalName, indexNum, copyFile, savePath);
                }
                else
                {
                    return false;
                }
            }
        }

        private bool ContinueSecondTryConvert(int indexNum, string finaldir, string strCommand)
        {
            if (!File.Exists(finaldir))
            {
                indexNum++;
                if (indexNum <= maxNum)
                {
                    Thread.Sleep(100 * indexNum);
                    return ContinueSecondTryConvert(indexNum, finaldir, strCommand);
                }
                else
                {
                    bool isSuccese = FormatConvert(strCommand);
                    Thread.Sleep(1000);
                    return File.Exists(finaldir);
                }
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        ///解码/转码-格式
        ///@param silk 源silk文件,需要绝对路径! 例:F:\zhuanma\vg2ub41omgipvrmur1fnssd3tq.silk
        ///@param pcm 目标pcm文件,需要绝对路径  例:F:\zhuanma\vg2ub41omgipvrmur1fnssd3tq.pcm
        ///@return
        /// </summary>
        /// <param name="cmdStr"></param>
        /// <returns></returns>
        private static bool FormatConvert(string cmdStr)
        {
            bool flag = true;
            try
            {
                //CmdUtils.RunCmd(cmdStr);
                #region 老版本
                using (Process p = new Process())
                {
                    string strInput = cmdStr;

                    //设置要启动的应用程序
                    p.StartInfo.FileName = "cmd.exe";
                    //是否使用操作系统shell启动
                    p.StartInfo.UseShellExecute = false;
                    // 接受来自调用程序的输入信息
                    p.StartInfo.RedirectStandardInput = true;
                    //输出信息
                    p.StartInfo.RedirectStandardOutput = true;
                    // 输出错误
                    p.StartInfo.RedirectStandardError = true;
                    //不显示程序窗口
                    p.StartInfo.CreateNoWindow = true;
                    //启动程序
                    p.Start();
                    //向cmd窗口发送输入信息
                    p.StandardInput.WriteLine(strInput + "&exit");

                    p.StandardInput.AutoFlush = true;

                    //p.WaitForExit();
                    //获取输出信息
                    //string strOuput = p.StandardOutput.ReadToEnd();
                    ////等待程序执行完退出进程
                    //p.Kill();
                    //Thread.Sleep(1000);
                }
                #endregion
            }
            catch (Exception)
            {
                flag = false;
            }
            return flag;
        }

       
    }
}