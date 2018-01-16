using AppApi.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using TCRMCLASS.Mail;

namespace AppApi.Tools
{

    public class OperationList
    {
        private static Dictionary<string, ListValue> OList = new Dictionary<string, ListValue>();

        public static String AddSend(GainParameter GP, SendWork W)
        {
            ListValue LV = new ListValue()
            {
                Userid = GP.UserId,
                Work = W
            };
            String Key = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(DateTime.Now.ToString("yyyyMMddhhmmssfff"), "MD5");
            OList.Add(Key, LV);
            return Key;
        }
        public static RecvObj AddRecv(GainParameter GP, string MailBoxId)
        {
            foreach (KeyValuePair<string,ListValue> item in OList)
            {
                if (item.Value.Work.GetType().Name == "RecvWork")
                {
                    if (((RecvWork)item.Value.Work).strMailBoxId == MailBoxId)
                        return new Tools.RecvObj() { key=item.Key,RW=(RecvWork)item.Value.Work };
                }
            }
            RecvWork RW = new RecvWork()
            {

                strMailBoxId = MailBoxId,
                strUserId = GP.UserId,
                sType=3
            };
          RW.runwork();
          //int countjs = 0;
          //while (RW.intRec == 0)
          //{
          //    System.Threading.Thread.Sleep(1000);
          //    countjs++;
          //    if (countjs == 10) break;
          //}
            ListValue LV = new ListValue()
            {
              
                Userid = GP.UserId,
                Work = RW
            };
            String Key = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(DateTime.Now.ToString("yyyyMMddhhmmssfff"), "MD5");
            OList.Add(Key, LV);
            return new Tools.RecvObj() { key = Key, RW = RW };
        }
        public static object Get(GainParameter GP, string key)
        {
            ListValue LV = OList[key];
            if (LV.Userid == GP.UserId)
                return LV.Work;
            return null;
        }
        public static Boolean Stop(GainParameter GP, string key)
        {
            ListValue LV = OList[key];
            if (LV.Userid == GP.UserId)
            {
                try
                {
                    ((RecvWork)LV.Work).Stop = true;
                    return true;
                }
                catch
                { return false; }
            }
            return false;
        }
        public static void Remove(GainParameter GP, string key)
        {
            ListValue LV = OList[key];
            if (LV.Userid == GP.UserId)
            {
                try
                {
                    ((RecvWork)LV.Work).Stop = true;
                }
                catch
                {  }
                OList.Remove(key);
            }
        }
    }
    public class ListValue
    {
        public string Userid { get; set; }
        public object Work { get; set; }
    }
    public class RecvObj
    {
        public string key { get; set; }
        public RecvWork RW { get; set; }
    }
}