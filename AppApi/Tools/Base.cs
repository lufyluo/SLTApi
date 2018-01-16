using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using AppApi.Models;
using AppApi.App_Data;

namespace AppApi.Tools
{
    public class Base
    {
        private static TCRMEntities db = new TCRMEntities();
        private static TCRMFILEEntities fdb = new TCRMFILEEntities();
        private static TCRMMAILEntities mdb = new TCRMMAILEntities();

        public static Dictionary<String, String> imgformat = new Dictionary<string, string>() {
            { "jpg","jpeg"},
            { "png","png"},
            { "gif","gif"},
            { "ico","x-icon"},
            { "jpeg","jpeg"}
           };
        public static string timestrformat(string timestr)
        {
            if ( timestr==null||timestr.Length <= 0 )
                return "";
            else
            {
                try
                {
                    return Convert.ToDateTime(timestr).ToString();
                }
                catch
                {
                    try
                    {
                        return Convert.ToDateTime(timestr.Substring(0, timestr.IndexOf("(") == -1 ? timestr.Length : timestr.IndexOf("("))).ToString();
                    }
                    catch
                    {
                        return "";
                    }
                }

            }
        }

        internal static bool IsUseFolder(GainParameter GP, int FolderId)
        {
            try
            {
                int MailBoxId = (int)db.MailBoxRoot_T.Where(MBRW => MBRW.Id == FolderId).Select(MBRS => MBRS.MailBoxId).FirstOrDefault();
                if (IsUseMailBox(GP, MailBoxId)) return true;
            }
            catch
            {
                return false;
            }
            return false;
        }

        public static void Get(object obj, HttpContext context)
        {
            Type t = obj.GetType();
            PropertyInfo[] PropertyList = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo item in PropertyList)
            {
                string name = item.Name;
                try
                {
                    item.SetValue(obj, context.Request.Form[name].ToString(), null);
                }
                catch
                {
                    if(!item.PropertyType.Name.StartsWith("String"))
                    {
                        try
                        {
                            item.SetValue(obj, Convert.ToInt32(context.Request.Form[name].ToString()), null);
                        }
                        catch
                        {
                            if (item.PropertyType.Name.StartsWith("int?"))
                            {
                                item.SetValue(obj, 0, null);
                            }
                            else
                            {
                                throw new Exception();
                            }
                        }
                    }
                    
                }

            }
        }
        /// <summary>  
        /// 转义  
        /// </summary>  
        /// <returns></returns>  
        public static String Escape(String str)
        {
            if (str != null)
            {
                //将json字符串进行转义           
                //str = str.Replace(" ", "&nbsp;");
                str = str.Replace("\"", "&quot;");
                //str = str.Replace("\\", "\\\\");
                //str = str.Replace("\n", "\\n");
                //str = str.Replace("\r", "\\r");
                str = str.Replace("\t", "");
                return str;
            }
            else
                return null;
        }
        /// <summary>  
        /// 获取时间戳  
        /// </summary>  
        /// <returns></returns>  
        public static string GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }
        /// <summary>  
        /// 转义  
        /// </summary>  
        /// <returns></returns>  
        public static String CheckUser(String UserId, String Password)
        {
            TCRMEntities db = new TCRMEntities();
            User_T user=db.Database.SqlQuery<User_T>("select * from user_t where userid='"+ UserId+"' ").FirstOrDefault();
            if (user == null)
                return BackCode.NoUser;
            else
            {
                if (user.Password != Password)
                    return BackCode.PasswordError;
                else
                    return BackCode.Success;
            }
        }
        /// <summary>
        ///判断是否为下属
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="subid"></param>
        /// <returns></returns>
        public static bool CheckSub(string userid, string subid)
        {
            TCRMEntities db = new TCRMEntities();
            User_T BiM = db.Database.SqlQuery<User_T>("select * from [dbo].[User_T] where  userid='" + userid + "'").FirstOrDefault();
            Models.User.Gain.userallmenu BiMs;
            if (BiM == null) return false;
            if (BiM.UserType == 1)
            {
                BiMs = db.Database.SqlQuery<Models.User.Gain.userallmenu>("select * from [dbo].[User_T] where  BranchId=" + BiM.BranchId + " and userid ='" + subid + "'").FirstOrDefault();
            }
            else if (BiM.UserType == 2)
            {
                BiMs = db.Database.SqlQuery<Models.User.Gain.userallmenu>("select * from [dbo].[User_T] where  DeptId=" + BiM.DeptId + " and userid ='" + subid + "'").FirstOrDefault();
            }
            else
            {
                BiMs = null;
            }
            if (BiMs != null)
                return true;
            else
                return false; ;

        }

        public static String Check(GainParameter obj)
        {
            if (Tools.Sign.CheckSign(obj.Ran, obj.Sign))
            {

                return CheckUser(obj.UserId, obj.Password);
            }
            else
                return BackCode.SignError;
        }
        public static Boolean IsUseMailBox(GainParameter GP, int MailBoxid)
        {
            if (db.MailBoxShare_T.Where(a => a.UserId == GP.UserId && a.Type == -1 && a.MailBoxId == MailBoxid).Count() > 0)
                return true;
            return false;
        }
        public static Boolean IsUseMail(GainParameter GP, int Mailid)
        {
            int MailBoxid = (int)mdb.Mail_T.Where(mw => mw.Id == Mailid).Select(ms => ms.MailBoxId).FirstOrDefault();
            return IsUseMailBox(GP, MailBoxid);
        }
        public static String GetUseMailBox(GainParameter GP)
        {
            try
            {
                List<int?> mb = db.MailBoxShare_T.Where(a => a.UserId == GP.UserId & a.Type == -1).Select(b =>b.MailBoxId).ToList();
                return string.Join(",",mb);
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public static String GetUserName(String UserId)
        {
            return db.User_T.Where(uw => uw.UserId == UserId).Select(uw => uw.UserName).FirstOrDefault();
        }
        public static Boolean IsUseFile(Models.GainParameter GP, long FileId)
        {   
            try
            {
                int mailid = (int)mdb.MailAcc_T.Where(maw => maw.FileId == FileId).Select(mas => mas.MailId).FirstOrDefault();
                if (IsUseMail(GP, mailid))
                    return true;
            }
            catch { return false; }
            return false;
        }
        public static Boolean IsFileCreater(GainParameter GP, long FileId)
        {
            try
            {
                String UserId = mdb.MailAcc_T.Where(maw => maw.FileId == FileId).Select(mas => mas.Creater).FirstOrDefault();
                if (GP.UserId==UserId)
                    return true;
            }
            catch { return false; }
            return false;
        }
        public static Boolean HasPower(GainParameter GP, String Power)
        {
            if (db.UserPower_T.Where(UPW => UPW.UserId == GP.UserId && UPW.FuncCode == Power).Count() > 0)
                return true;
            return false;
        }
        public static Boolean HasPubLimit(GainParameter GP, String Limit)
        {
            if (db.ClientPubLimit_T.Where(CPLW => CPLW.UserId == GP.UserId && CPLW.Class == Limit).Count() > 0)
                return true;
            return false;
        }
        public static Boolean HasClient(GainParameter GP, int clientid)
        {
            Tools.Where where = new Tools.Where();
            where.And("id=" + clientid);
            String username = Tools.Base.GetUserName(GP.UserId);
            where.And(String.Format("'{0}' in (select Value from [dbo].[SplitString_F](sale,',',1))", username));
            Models.Client.Back.Get BGC = db.Database.SqlQuery<Models.Client.Back.Get>("select * from Client_T " + where.ToWhere()).FirstOrDefault();
            if (BGC != null) return true;
            return false;
        }
        public static Boolean HasContact(GainParameter GP, int Contactid)
        {
            try
            {
                int clientid = db.Database.SqlQuery<int>("select clientid from clientadd_t where id=" + Contactid.ToString()).FirstOrDefault();
                return HasClient(GP, clientid);
            }
            catch {
                return false;
            }
        }
        public static SysDictionary_T GetDictionary(Models.Sys.Gain.Dictionary D)
        {
            return db.SysDictionary_T.Where(SDW => SDW.DataClass == D.Class && SDW.DataType == D.Type && SDW.DataText == D.Text).FirstOrDefault();
        }
        public static string GetmD5jm(string MD)
        {
            int i = 0;
            string zh = "";
            for (i = 0; i < MD.Length; i++)
            {
                if (i % 2 == 0 && i <= 15)
                {
                    char mj = Convert.ToChar((int)MD[i] - (15 - i));
                    zh += mj;
                }
                else
                    zh += MD[i];
            }
            return zh;
        }
    }
}