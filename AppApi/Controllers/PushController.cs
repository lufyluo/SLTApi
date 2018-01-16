using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Google.ProtocolBuffers;
using com.gexin.rp.sdk.dto;
using com.igetui.api.openservice;
using com.igetui.api.openservice.igetui;
using com.igetui.api.openservice.igetui.template;
using com.igetui.api.openservice.payload;
using AppApi.App_Data;
using AppApi.Filter;
using AppApi.Tools;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AppApi.Controllers
{
    public class PushController : BaseControllers
    {
        //参数设置 <-----参数需要重新设置----->
        //http的域名
        private static String HOST = "http://sdk.open.api.igexin.com/apiex.htm";
        //https的域名
        //private static String HOST = "https://api.getui.com/apiex.htm";

        //定义常量, appId、appKey、masterSecret 采用本文档 "第二步 获取访问凭证 "中获得的应用配置
        private static string APPID = System.Web.Configuration.WebConfigurationManager.AppSettings["APPID"];
        private static string APPKEY = System.Web.Configuration.WebConfigurationManager.AppSettings["APPKEY"];
        private static string MASTERSECRET = System.Web.Configuration.WebConfigurationManager.AppSettings["MASTERSECRET"];
        ////private static String APPID = "Xg1sgtTNZJA03Ij2IEkwy3";
        //private static String APPKEY = "EdW5PcIdHyAnRPcU6SJMQ4";
        //private static String MASTERSECRET = "fs8BbO2bZz7TQAVjuWFdz1";

        //private static String APPID = "lgcPRZG17s6y4V4e8kbZU2";
        //private static String APPKEY = "yjUJQv5G44ArKDve8fQb61";
        //private static String MASTERSECRET = "8EtjZWCfTG8OUd30cmRTD2";
        private static String DeviceToken = "";  //填写IOS系统的DeviceToken
        [HttpPost]
        public Models.BackParameter pushemail(Models.push.Gain.PushEmail E) 
        {
            try
            {
                string log = "";
                log = "push email data:" + E.data + "\r\n";
                if (E.data == null)
                {
                    BP.back = "data不能为null";
                    return BP;
                }
                IGtPush push = new IGtPush(HOST, APPKEY, MASTERSECRET);
                SingleMessage message = singconfig();//  ListMessage message = new ListMessage();
                JObject jsonObj = JObject.Parse(E.data);
                List<string> mailid = JsonConvert.DeserializeObject<List<string>>(jsonObj["mailid"].ToString());
                List<string> userid = JsonConvert.DeserializeObject<List<string>>(jsonObj["userid"].ToString());
                string mailidlistwhere = "";
                string useridlistwhere = "";
                for (int i = 0; i < mailid.Count; i++)
                {
                    if (mailidlistwhere == "")
                        mailidlistwhere = " id=" + mailid[i];
                    else
                        mailidlistwhere += " or id=" + mailid[i];
                }
                for (int i = 0; i < userid.Count; i++)
                {
                    if (useridlistwhere == "")
                        useridlistwhere = " userid='" + userid[i] + "'";
                    else
                        useridlistwhere += " or userid='" + userid[i] + "'";
                }
                IEnumerable<Models.push.Back.GetApp> BiMuser = db.Database.SqlQuery<Models.push.Back.GetApp>("Select  * from User_T where " + useridlistwhere);
                IEnumerable<Models.Mail.Back.item.MailAll> BiMmail = dbm.Database.SqlQuery<Models.Mail.Back.item.MailAll>("Select  * from Mail_T where " + mailidlistwhere);
                foreach (Models.push.Back.GetApp userappId in BiMuser)
                {
                    foreach (Models.Mail.Back.item.MailAll mailss in BiMmail)
                    {
                        string result = mailss.textbody;
                        result = result.Replace("\r\n", "").TrimStart(); ;
                        if (result.Length < 50)
                        {
                            if (result.Length >= 1)
                                result = result.Substring(0, result.Length - 1);
                        }
                        else
                            result = result.Substring(0, 50);

                        if (userappId.appid.Substring(0, 1) == "1")
                        {
                            TransmissionTemplate template = TransmissionTemplateDemo(mailss.Subject, result,"mailid", mailss.Id.ToString());
                            message.Data = template;
                        }
                        else
                        {
                            NotificationTemplate template = NotificationTemplateDemo(mailss.Subject, result, "mailid", mailss.Id.ToString());
                            message.Data = template;
                        }
                        com.igetui.api.openservice.igetui.Target target = new com.igetui.api.openservice.igetui.Target();
                        target.appId = APPID;
                        target.clientId = userappId.appid.Substring(1, userappId.appid.Length - 1);
                        String pushResult = "";
                        try
                        {
                            pushResult = push.pushMessageToSingle(message, target);
                        }
                        catch (RequestException e)
                        {
                            String requestId = e.RequestId;
                            //发送失败后的重发
                            pushResult = push.pushMessageToSingle(message, target, requestId);
                        }
                        log += "appid:" + userappId.appid.Substring(1, userappId.appid.Length - 1) + " mailid:" + mailss.Id.ToString() + "result:" + pushResult + "\r\n";
                    }
                }

                /*
                            foreach (Models.Mail.Back.item.MailAll mailss in BiMmail)
                            {
                                string result = System.Text.RegularExpressions.Regex.Replace(mailss.textbody, "[\r\n]", "");
                             //  TransmissionTemplate template = TransmissionTemplateDemo(mailss.Subject, result, mailss.Id.ToString());
                                NotificationTemplate template = NotificationTemplateDemo(mailss.Subject, result, mailss.Id.ToString());
                                message.Data = template;
                                List<com.igetui.api.openservice.igetui.Target> targetList = new List<com.igetui.api.openservice.igetui.Target>();
                                foreach (Models.push.Back.GetApp userappId in BiMuser)
                                {
                                    log += "appid:" + userappId.appid + " mailid:" + mailss.Id.ToString()+"\r\n";
                                    com.igetui.api.openservice.igetui.Target target = Targetconfig(userappId.appid);
                                    targetList.Add(target);
                                }
                                String contentId = push.getContentId(message);
                                try
                                {
                                    String pushResult = push.pushMessageToList(contentId, targetList);
                                    BP.back = pushResult;
                                }
                                catch (RequestException e)
                                {
                                    BP.back = e.ToString();
                                }
                            }
                 */
                Tools.Log.WriteLogFile("pushlog", log);
                BP.back = log;
                return BP;
            }
            catch (Exception ee)
            {
                BP.back = ee.ToString();
                return BP;
            }
        }
        [HttpPost]
        public Models.BackParameter pushmsg(Models.push.Gain.PushEmail E)
        {
            try
            {
                string log = "";
                log = "push msg data:" + E.data + "\r\n";
                if (E.data == null)
                {
                    BP.back = "data不能为null";
                    return BP;
                }
                IGtPush push = new IGtPush(HOST, APPKEY, MASTERSECRET);
                SingleMessage message = singconfig();//  ListMessage message = new ListMessage();
                JObject jsonObj = JObject.Parse(E.data);
                List<string> msgid = JsonConvert.DeserializeObject<List<string>>(jsonObj["msgid"].ToString());
                List<string> userid = JsonConvert.DeserializeObject<List<string>>(jsonObj["userid"].ToString());
                string mailidlistwhere = "";
                string useridlistwhere = "";
                for (int i = 0; i < msgid.Count; i++)
                {
                    if (mailidlistwhere == "")
                        mailidlistwhere = " id=" + msgid[i];
                    else
                        mailidlistwhere += " or id=" + msgid[i];
                }
                for (int i = 0; i < userid.Count; i++)
                {
                    if (useridlistwhere == "")
                        useridlistwhere = " userid='" + userid[i] + "'";
                    else
                        useridlistwhere += " or userid='" + userid[i] + "'";
                }
                IEnumerable<Models.push.Back.GetApp> BiMuser = db.Database.SqlQuery<Models.push.Back.GetApp>("Select  * from User_T where " + useridlistwhere);
                IEnumerable<Models.WorkDt.Back.msg> BiMmsg = db.Database.SqlQuery<Models.WorkDt.Back.msg>("Select  * from Msg_T where " + mailidlistwhere);
                IEnumerable<SysDictionary_T> Dictionary = db.SysDictionary_T.Where(CPLW => CPLW.DataClass == "消息管理");
                foreach (Models.push.Back.GetApp userappId in BiMuser)
                {
                    foreach (Models.WorkDt.Back.msg mailmsgs in BiMmsg)
                    {
                        string result = mailmsgs.Html;
                        result = result.Replace("\r\n", "").TrimStart(); ;
                        if (result.Length < 50)
                        {
                            if (result.Length >= 1)
                                result = result.Substring(0, result.Length - 1);
                        }
                        else
                            result = result.Substring(0, 50);
                        string subject = xiaoxi(mailmsgs.MenuNo, Dictionary);
                        if (userappId.appid.Substring(0, 1) == "1")
                        {
                            TransmissionTemplate template = TransmissionTemplateDemo(subject, result,"msgid", mailmsgs.Id.ToString());
                            message.Data = template;
                        }
                        else
                        {
                            NotificationTemplate template = NotificationTemplateDemo(subject, result, "msgid", mailmsgs.Id.ToString());
                            message.Data = template;
                        }
                        com.igetui.api.openservice.igetui.Target target = new com.igetui.api.openservice.igetui.Target();
                        target.appId = APPID;
                        target.clientId = userappId.appid.Substring(1, userappId.appid.Length - 1);
                        String pushResult = "";
                        try
                        {
                            pushResult = push.pushMessageToSingle(message, target);
                        }
                        catch (RequestException e)
                        {
                            String requestId = e.RequestId;
                            //发送失败后的重发
                            pushResult = push.pushMessageToSingle(message, target, requestId);
                        }
                        log += "appid:" + userappId.appid.Substring(1, userappId.appid.Length - 1) + " mailid:" + mailmsgs.Id.ToString() + "result:" + pushResult + "\r\n";
                    }
                }
                Tools.Log.WriteLogFile("pushmsglog", log);
                BP.back = log;
                return BP;
            }
            catch (Exception ee)
            {
                BP.back = ee.ToString();
                return BP;
            }
        }
        public static string xiaoxi(string value, IEnumerable<SysDictionary_T> T)
        {
            string r = "";
            foreach (SysDictionary_T Tson in T)
            {
                if (Tson.DataValue2 == value)
                    return Tson.DataText;
            }
            return "";
        }
        public SingleMessage singconfig()
        {
            SingleMessage message = new SingleMessage();
            message.IsOffline = true;                         // 用户当前不在线时，是否离线存储,可选
            message.OfflineExpireTime = 1000 * 3600 * 12;            // 离线有效时间，单位为毫秒，可选
            //判断是否客户端是否wifi环境下推送，2为4G/3G/2G，1为在WIFI环境下，0为不限制环境
            message.PushNetWorkType = 0;
            return message;
        }
        public ListMessage listconfig()
        {
            ListMessage message = new ListMessage();
            message.IsOffline = true;                         // 用户当前不在线时，是否离线存储,可选
            message.OfflineExpireTime = 1000 * 3600 * 12;            // 离线有效时间，单位为毫秒，可选
            //判断是否客户端是否wifi环境下推送，2为4G/3G/2G，1为在WIFI环境下，0为不限制环境
            message.PushNetWorkType = 0;
            return message;
        }
        public com.igetui.api.openservice.igetui.Target Targetconfig(string clientid)
        {
            com.igetui.api.openservice.igetui.Target target = new com.igetui.api.openservice.igetui.Target();
            target.appId = APPID;
            target.clientId = clientid;
            return target;
        }
        #region 模板类型
        //通知透传模板动作内容
        public static NotificationTemplate NotificationTemplateDemo(string subject, string htmltext,string lx, string lxid)
        {
            NotificationTemplate template = new NotificationTemplate();
            template.AppId = APPID;
            template.AppKey = APPKEY;
            //通知栏标题
            template.Title = subject;
            //通知栏内容     
            template.Text = htmltext;
            //通知栏显示本地图片
            template.Logo = "";
            //通知栏显示网络图标
            template.LogoURL = "http://116.62.232.164:9898/picimage/logo-tui.png";
            //应用启动类型，1：强制应用启动  2：等待应用启动
            template.TransmissionType = "1";
            //透传内容  
            template.TransmissionContent = lx+"="+lxid;
            //接收到消息是否响铃，true：响铃 false：不响铃   
            template.IsRing = true;
            //接收到消息是否震动，true：震动 false：不震动   
            template.IsVibrate = true;
            //接收到消息是否可清除，true：可清除 false：不可清除    
            template.IsClearable = true;
            //设置通知定时展示时间，结束时间与开始时间相差需大于6分钟，消息推送后，客户端将在指定时间差内展示消息（误差6分钟）
            String begin = DateTime.Now.ToString();
            String end = DateTime.Now.AddMinutes(6).ToString();
            template.setDuration(begin, end);

            return template;
        }

        //透传模板动作内容
        public static TransmissionTemplate TransmissionTemplateDemo(string subject, string htmltext,string lx, string lxid)
        {
            TransmissionTemplate template = new TransmissionTemplate();
            template.AppId = APPID;
            template.AppKey = APPKEY;
            //应用启动类型，1：强制应用启动 2：等待应用启动
            template.TransmissionType = "1";
            //透传内容  
            template.TransmissionContent = "";

            APNPayload apnpayload = new APNPayload();
            DictionaryAlertMsg alertMsg = new DictionaryAlertMsg();
            alertMsg.Body = htmltext;         //通知文本消息字符串
            alertMsg.ActionLocKey = "";
            alertMsg.LocKey = "";
            alertMsg.addLocArg("");
            alertMsg.LaunchImage = "";//指定启动界面图片名
            //IOS8.2支持字段
            alertMsg.Title = subject;     //通知标题
            alertMsg.TitleLocKey = "";
            alertMsg.addTitleLocArg("");

            apnpayload.AlertMsg = alertMsg;
            apnpayload.Badge = 1;//应用icon上显示的数字
            apnpayload.ContentAvailable = 1;//推送直接带有透传数据
            apnpayload.Category = "";
            apnpayload.Sound = "";//通知铃声文件名
            apnpayload.addCustomMsg(lx, lxid);//增加自定义的数据
            template.setAPNInfo(apnpayload);





            //设置通知定时展示时间，结束时间与开始时间相差需大于6分钟，消息推送后，客户端将在指定时间差内展示消息（误差6分钟）
            String begin = DateTime.Now.ToString();
            String end = DateTime.Now.AddMinutes(6).ToString();
            template.setDuration(begin, end);

            return template;
        }

        //网页模板内容
        public static LinkTemplate LinkTemplateDemo()
        {
            LinkTemplate template = new LinkTemplate();
            template.AppId = APPID;
            template.AppKey = APPKEY;
            //通知栏标题
            template.Title = "请填写通知标题";
            //通知栏内容 
            template.Text = "请填写通知内容";
            //通知栏显示本地图片 
            template.Logo = "";
            //通知栏显示网络图标，如无法读取，则显示本地默认图标，可为空
            template.LogoURL = "https://b-ssl.duitang.com/uploads/item/201405/20/20140520235745_NcAhN.jpeg";
            //打开的链接地址    
            template.Url = "http://www.baidu.com";
            //接收到消息是否响铃，true：响铃 false：不响铃   
            template.IsRing = true;
            //接收到消息是否震动，true：震动 false：不震动   
            template.IsVibrate = true;
            //接收到消息是否可清除，true：可清除 false：不可清除
            template.IsClearable = true;
            return template;
        }

        //通知栏弹框下载模板
        public static NotyPopLoadTemplate NotyPopLoadTemplateDemo()
        {
            NotyPopLoadTemplate template = new NotyPopLoadTemplate();
            template.AppId = APPID;
            template.AppKey = APPKEY;
            //通知栏标题
            template.NotyTitle = "请填写通知标题";
            //通知栏内容
            template.NotyContent = "请填写通知内容";
            //通知栏显示本地图片
            template.NotyIcon = "icon.png";
            //通知栏显示网络图标
            template.LogoURL = "http://www-igexin.qiniudn.com/wp-content/uploads/2013/08/logo_getui1.png";
            //弹框显示标题
            template.PopTitle = "弹框标题";
            //弹框显示内容    
            template.PopContent = "弹框内容";
            //弹框显示图片    
            template.PopImage = "";
            //弹框左边按钮显示文本    
            template.PopButton1 = "下载";
            //弹框右边按钮显示文本    
            template.PopButton2 = "取消";
            //通知栏显示下载标题
            template.LoadTitle = "下载标题";
            //通知栏显示下载图标,可为空 
            template.LoadIcon = "file://push.png";
            //下载地址，不可为空
            template.LoadUrl = "http://www.appchina.com/market/d/425201/cop.baidu_0/com.gexin.im.apk ";
            //应用安装完成后，是否自动启动
            template.IsActived = true;
            //下载应用完成后，是否弹出安装界面，true：弹出安装界面，false：手动点击弹出安装界面 
            template.IsAutoInstall = true;
            //接收到消息是否响铃，true：响铃 false：不响铃
            template.IsBelled = true;
            //接收到消息是否震动，true：震动 false：不震动   
            template.IsVibrationed = true;
            //接收到消息是否可清除，true：可清除 false：不可清除    
            template.IsCleared = true;
            return template;
        }


        #endregion
    }
}