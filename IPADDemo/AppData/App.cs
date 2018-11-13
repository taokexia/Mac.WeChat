using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPADDemo.AppData
{
    public class App
    {
        public static string PYQContent = "<TimelineObject><id>0</id><username>{0}</username><createTime>0</createTime><contentDescShowType>0</contentDescShowType><contentDescScene>0</contentDescScene><private>0</private><contentDesc>%s</contentDesc><contentattr>0</contentattr><sourceUserName></sourceUserName><sourceNickName></sourceNickName><statisticsData></statisticsData><ContentObject><contentStyle>1</contentStyle><title></title><description></description><contentUrl></contentUrl><mediaList>{1}</mediaList></ContentObject><actionInfo></actionInfo><appInfo><id></id></appInfo><location city=\"\" poiClassifyId=\"\" poiName=\"\" poiAddress=\"\" poiClassifyType=\"0\"></location><publicUserName></publicUserName></TimelineObject>";
        public static string PYQContentImage = "<media><id>0</id><type>2</type><title></title><description></description><private>0</private><url type=\"1\">{0}</url><thumb type=\"1\">{1}</thumb><size totalSize=\"{2}\" height=\"{3}\" width=\"{4}\"></size></media>";

        public static string AppMsgXml= "<appmsg appid=\"$appid$\" sdkver=\"sdkver\"><title>$title$</title><des>$des$</des><action>view</action><type>5</type><showtype>0</showtype><content></content><url>$url$</url><thumburl>$thumburl$</thumburl><lowurl></lowurl><appattach><totallen>0</totallen><attachid></attachid><fileext></fileext></appattach><extinfo></extinfo></appmsg>";
    }
}
