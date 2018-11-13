using IPADDemo.AppData;
using IPADDemo.Model;
using IPADDemo.Util;
using IPADDemo.WeChat;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IPADDemo
{
    public partial class FormDemo : Form
    {
        public static FormDemo _formDemo;
        private XzyWeChatThread weChatThread;

        public FormDemo()
        {
            InitializeComponent();
            WxDelegate.qrCode += new QrCode(this.calback_qrcode);
            WxDelegate.show += new Show(this.calback_show);
            WxDelegate.getContact += new GetContact(this.calback_getContact);
            WxDelegate.getGroup += new GetGroup(this.calback_getGroup);
            WxDelegate.getGZH += new GetGZH(this.calback_getGZH);
            WxDelegate.msgCallBack += new MsgCallBack(this.calback_msgCallBack);
            CheckForIllegalCrossThreadCalls = false;
            _formDemo = this;
        }

        private void FormDemo_Load(object sender, EventArgs e)
        {
        }

        private void calback_qrcode(string qrcode)
        {
            byte[] arr2 = Convert.FromBase64String(qrcode);
            using (MemoryStream ms2 = new MemoryStream(arr2))
            {
                System.Drawing.Bitmap bmp2 = new System.Drawing.Bitmap(ms2);
                pictureBox1.Image = bmp2;
            }
        }

        private void calback_show(string str)
        {
            textBox1.Text = str;
        }

        private void calback_getContact(Contact contact)
        {
            string str = $"{contact.UserName}-{contact.NickName}-{contact.Country}-{contact.Provincia}-{contact.Remark}";
            lb_friend.Items.Add(str);
        }

        private void calback_getGroup(Contact contact)
        {
            string str = $"{contact.UserName}-{contact.NickName}-{contact.Country}-{contact.Provincia}-{contact.Remark}";
            lb_group.Items.Add(str);
        }

        private void calback_getGZH(Contact contact)
        {
            string str = $"{contact.UserName}-{contact.NickName}-{contact.Country}-{contact.Provincia}-{contact.Remark}";
            lb_gzh.Items.Add(str);
        }

        private void calback_msgCallBack(WxTtsMsg wxTtsMsg) {
            txt_msgcallback.Text += JsonConvert.SerializeObject(wxTtsMsg) + "\r\n";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            weChatThread.Wx_SendMsg(txt_msgWxid.Text, txt_msgText.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.Filter = "图片文件 |*.jpg;*.png";
            ofd.ValidateNames = true;
            ofd.CheckPathExists = true;
            ofd.CheckFileExists = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string FileName = ofd.FileName;
                weChatThread.Wx_SendImg(txt_msgWxid.Text, FileName);
            }
        }

        private void button17_Click(object sender, EventArgs e)
        {
            string str = weChatThread.Wx_SendVoice(txt_msgWxid.Text, "测试音频1.silk", 1);
        }

        private void button23_Click(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() => {
                string mp3Str = "1.mp3";
                string silkStr= "1.silk";
                bool isCovert = ffmpegUtils.GetInstance().ConvertMp3ToAmr(mp3Str, silkStr);
                if (isCovert)
                {
                    while (MyUtils.IsFileInUse(silkStr))
                    {
                        Thread.Sleep(100);
                    }
                }
                else
                {
                    return;
                }
                string str = weChatThread.Wx_SendVoice(txt_msgWxid.Text, silkStr, 1);
            });          
        }

        private void button24_Click(object sender, EventArgs e)
        {
            string xml = App.AppMsgXml.
                Replace("$appid$", txt_linkAppId.Text).
                 Replace("$sdkver$", txt_linkSdkVer.Text).
                  Replace("$title$", txt_linkTitle.Text).
                   Replace("$des$", txt_linkDesc.Text).
                    Replace("$url$", txt_linkUrl.Text).
                     Replace("$thumburl$", txt_linkImgUrl.Text);
            weChatThread.Wx_SendAppMsg(txt_msgWxid.Text, xml);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            weChatThread.Wx_SendMoment(txt_snsText.Text);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            List<string> imgPath = new List<string>();
            foreach (var a in listBox1.Items) {
                imgPath.Add(ImgToBase64String(a.ToString()));
                //imgPath.Add(FileToBase64String(a.ToString()));
            }
            weChatThread.Wx_SendMoment(txt_snsText.Text, imgPath);
        }

        private string FileToBase64String(string path) {
            try
            {
                System.Text.Encoding encode = System.Text.Encoding.ASCII;
                FileStream fileStreamObj;
                FileInfo fileInfoObj = new FileInfo(path);
                fileStreamObj = fileInfoObj.OpenRead();//.OpenWrite();
                byte[] bytedata = new byte[fileInfoObj.Length];
                fileStreamObj.Read(bytedata, 0, bytedata.Length);
                fileStreamObj.Flush();
                fileStreamObj.Close();

                string audioBase64Result = Convert.ToBase64String(bytedata, 0, bytedata.Length);
                return audioBase64Result;
            }
            catch (Exception e) {
                return "";
            }
        }

        private string ImgToBase64String(string Imagefilename)
        {
            try
            {
                Bitmap bmp = new Bitmap(Imagefilename);

                MemoryStream ms = new MemoryStream();
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                byte[] arr = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(arr, 0, (int)ms.Length);
                ms.Close();
                return Convert.ToBase64String(arr);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                listBox1.Items.Add(ofd.FileName);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            var res = weChatThread.Wx_CreateChatRoom(txt_GroupUsers.Text);
        }

        private void button39_Click(object sender, EventArgs e)
        {
            var res = weChatThread.Wx_GetChatRoomMember(txt_groupwxid.Text);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            var res = weChatThread.Wx_AddChatRoomMember(txt_groupwxid.Text,txt_groupuserwxid.Text);
        }

        private void button38_Click(object sender, EventArgs e)
        {
           var res=weChatThread.Wx_InviteChatRoomMember(txt_groupwxid.Text, txt_groupuserwxid.Text);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            var res = weChatThread.Wx_DeleteChatRoomMember(txt_groupwxid.Text, txt_groupuserwxid.Text);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            var res = weChatThread.Wx_SetChatroomName(txt_groupwxid.Text, txt_groupname.Text);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            var res = weChatThread.Wx_SetChatroomAnnouncement(txt_groupwxid.Text, txt_groupgg.Text);
        }

        private void button11_Click(object sender, EventArgs e)
        {
            var res = weChatThread.Wx_QuitChatRoom(txt_groupwxid.Text);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            string str= weChatThread.Wx_GenerateWxDat();
            WxDat wxDat = JsonConvert.DeserializeObject<WxDat>(str);
            txt_login62.Text = wxDat.data;
        }

        private void button18_Click(object sender, EventArgs e)
        {
            string str = weChatThread.Wx_GetLoginToken();
            WxToken wxToken = JsonConvert.DeserializeObject<WxToken>(str);
            txt_loginToken.Text= wxToken.token;
        }

        private void button19_Click(object sender, EventArgs e)
        {
            string str=weChatThread.Wx_LoginRequest(txt_loginToken.Text,txt_login62.Text);
        }

        private void button20_Click(object sender, EventArgs e)
        {
            var res = weChatThread.Wx_AutoLogin(txt_loginToken.Text);
        }

        private void button14_Click(object sender, EventArgs e)
        {
            weChatThread = new XzyWeChatThread();
        }

        private void button13_Click(object sender, EventArgs e)
        {
            weChatThread = new XzyWeChatThread(txt_login62.Text,txt_loginName.Text,txt_loginPassword.Text);
        }

        private void button15_Click(object sender, EventArgs e)
        {
           txt_positionReturn.Text= weChatThread.Wx_GetPeopleNearby(float.Parse(txt_Lat.Text), float.Parse(txt_Lng.Text));
        }

        private void button16_Click(object sender, EventArgs e)
        {
            string [] types= cb_addtype.Text.Split('-');
            int type = int.Parse(types[0]);
            string result = weChatThread.Wx_AddUser(txt_v1.Text, txt_v2.Text, type, txt_hellotext.Text);
        }

        private void button21_Click(object sender, EventArgs e)
        {
            string result = weChatThread.Wx_AddUser(txt_gzhid.Text, "", 0, "");
        }

        private void button22_Click(object sender, EventArgs e)
        {
            txt_gzhlog.Text = weChatThread.GetSubscriptionInfo(txt_gzhid.Text);
        }

        private void button25_Click(object sender, EventArgs e)
        {
            string str = weChatThread.Wx_GetContactLabelList();
            txt_labelList.Text = str;
        }

        private void button26_Click(object sender, EventArgs e)
        {
            var res = weChatThread.Wx_SetContactLabel(txt_labelWxid.Text, txt_labelId.Text);

        }

        private void button27_Click(object sender, EventArgs e)
        {
            var res = weChatThread.Wx_AddContactLabel(txt_labelName.Text);
        }

        private void button28_Click(object sender, EventArgs e)
        {
            var res = weChatThread.Wx_DeleteContactLabel(txt_labelId1.Text);
        }

        private void button29_Click(object sender, EventArgs e)
        {
            var res = weChatThread.Wx_FavSync(txt_favKey.Text);
        }

        private void button30_Click(object sender, EventArgs e)
        {
            var res = weChatThread.Wx_FavAddItem(Encoding.Default.GetString(Encoding.UTF8.GetBytes(txt_favObject.Text)));
        }

        private void button31_Click(object sender, EventArgs e)
        {
            var res = weChatThread.Wx_FavGetItem(txt_favId.Text);
            txt_favItem.Text = res.ConvertToString();
        }

        private void button32_Click(object sender, EventArgs e)
        {
            var res = weChatThread.Wx_SnsUserPage(txt_snswxid.Text,"");
        }

        private void button33_Click(object sender, EventArgs e)
        {
            var res = weChatThread.Wx_SnsObjectDetail(txt_snsId.Text);
        }

        private void button34_Click(object sender, EventArgs e)
        {
            var res = weChatThread.Wx_SnsComment(txt_snsId.Text, Encoding.Default.GetString(Encoding.UTF8.GetBytes(txt_snsContext.Text)), txt_snsPlId.Text.ConvertToInt32());
        }

        private void button35_Click(object sender, EventArgs e)
        {
            var res = weChatThread.Wx_GetContact(txt_friendwxid.Text);
        }

        private void button36_Click(object sender, EventArgs e)
        {
            var res = weChatThread.Wx_SetUserRemark(txt_friendwxid.Text, txt_friendRemark.Text);
        }

        private void button37_Click(object sender, EventArgs e)
        {
            var res = weChatThread.Wx_DeleteUser(txt_friendwxid.Text);
        }

        
    }
}