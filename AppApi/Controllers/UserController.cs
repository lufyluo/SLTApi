using AppApi.App_Data;
using AppApi.Filter;
using AppApi.Models.Result;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Security.Cryptography;
using System.IO;
using System.Text;
using AppApi.Controllers.Filter;

namespace AppApi.Controllers
{
    public class UserController:BaseControllers
    {
        [Check]
        [AppIdCheck]
        [HttpPost]
        public Models.BackParameter Get([FromBody]Models.User.Gain.Get G)
        {
          

            string sj = newsj();
            Models.User.Back.Get BG= db.Database.SqlQuery<Models.User.Back.Get>(String.Format("select id=UID,name=UserName,Sex,Phone,State,type=UserType,department=(select top 1 Name from Dept_T where id=DeptId),branch=(select top 1 name from Branch_T where ID= BranchId),Role=(select top 1 name from role_t where id=(select top 1 RoleId from RoleUser_T where UserId=UserId)) from User_T where UserId='{0}'",G.UserId)).FirstOrDefault();
            IEnumerable< Models.User.Back.item.MailLabel> BiM = db.Database.SqlQuery<Models.User.Back.item.MailLabel>(string.Format("select Name=Name + '|' + Class from MailLabel_T where UserId='{0}'",G.UserId));
            BG.maillabel = BiM;
            BP.back = BG;
            stopwatch.Stop();
            TimeSpan timeSpan = stopwatch.Elapsed; //  获取总时间
           // BP.code += timeSpan.TotalMilliseconds.ToString();
            return BP;
        }

        [Check]
        [HttpPost]
        public Models.BackParameter GetMenu([FromBody]Models.User.Gain.GetMenu GM)
        {
            List<GetMenu_Result> Menu = db.Database.SqlQuery<GetMenu_Result>(String.Format("GetMenu '{0}'",GM.UserId)).ToList();//获取用户菜单  
            Dictionary<string, object> M = new Dictionary<string, object>();
            List<GetMenu_Result> Menus = Menu.Where(MW => MW.ParentNo == "0").ToList();
            foreach (GetMenu_Result item in Menus)
            {
                MenuLevel ML = new MenuLevel()
                {
                    menuno = item.MenuNo,
                    name = item.MenuName,
                    next=new List<object>()
                };
                M.Add(item.MenuNo, ML);
                Menu.Remove(item);
            }
            while (true)
            {
                List<string> NoList = M.Keys.ToList();
                Menus = Menu.Where(MW =>NoList.Contains(MW.ParentNo)).ToList();
                if (Menus.Count <= 0) break;
                foreach (GetMenu_Result item in Menus)
                {
                    MenuLevel ML = new MenuLevel()
                    {
                        menuno = item.MenuNo,
                        name = item.MenuName,
                        next = new List<object>()
                    };
                    ((MenuLevel)M[item.ParentNo]).next.Add(ML);
                    M.Add(item.MenuNo, ML);
                    Menu.Remove(item);
                }
                if (Menu.Count <= 0) break;
            }
            Dictionary<string, object> MB = new Dictionary<string, object>();
            foreach (string item in M.Keys)
            {
                if (item.Length == 2)
                    MB.Add(item, M[item]);
            }
            BP.back = MB.Values.ToList();
            return BP;
        }

        [Check]
        [HttpPost]
        public Models.BackParameter GetPower([FromBody]Models.User.Gain.GetPower GP)
        {
            string powername = GP.PowerName;
            dynamic fc = null;
            fc = db.PowerList_T.Where(pw => pw.MenuNo == powername).Select(pw => new { funcode = pw.MenuNo + pw.PowerId, name = db.Menu_T.Where(mw => mw.MenuNo == pw.MenuNo).Select(ms => ms.MenuName).FirstOrDefault() + pw.PowerName, has = db.UserPower_T.Where(uw => uw.FuncCode == pw.MenuNo + pw.PowerId).Count() > 0 ? 1 : 0 });
            BP.back = fc;
            return BP;
            
        }
        [Check]
        [HttpPost]
        public Models.BackParameter AllPower([FromBody]Models.User.Gain.AllPower AP)
        {    
            dynamic fc = null;
            fc = db.PowerList_T.Select(pw => new { funcode = pw.MenuNo + pw.PowerId, name = db.Menu_T.Where(mw => mw.MenuNo == pw.MenuNo).Select(ms => ms.MenuName).FirstOrDefault() + pw.PowerName, has = db.UserPower_T.Where(uw => uw.FuncCode == pw.MenuNo + pw.PowerId).Count() > 0 ? 1 : 0 });
            BP.back = fc;
            return BP;

        }
        [Check]
        [HttpPost]
        public Models.BackParameter GetPubLimit([FromBody]Models.GainParameter GP)
        {
            IQueryable<ClientPubLimit_T> CPL = db.ClientPubLimit_T.Where(CPLW => CPLW.UserId == GP.UserId);
            BP.back = CPL.Select(CPLS => CPLS.Class);
            return BP;
        }
        [Check]
        [HttpPost]
        public Models.BackParameter GetList([FromBody]Models.GainParameter GP,[FromUri]string Operation)
        {
            IEnumerable<Models.User.Back.GetList> BGL = null;
            switch (Operation.ToLower())
            {
                case "underling":
                    BGL = db.Database.SqlQuery<Models.User.Back.GetList>(string.Format("select * from GetUnderling_F('{0}',1)", GP.UserId));
                    break;
                case "department":
                    BGL = db.Database.SqlQuery<Models.User.Back.GetList>(string.Format("select uid, UserId, UserName from User_T where DeptId =[TCRM].[dbo].[GetDeptId_F]('{0}')", GP.UserId));
                    break;
                case "branch":
                    BGL = db.Database.SqlQuery<Models.User.Back.GetList>(string.Format("select uid,UserId,UserName from User_T where BranchId=[TCRM].[dbo].[GetBranchId_F] ('{0}')", GP.UserId));
                    break;
            }
            BP.back = BGL;
            return BP;
        }
        [Check]
        [HttpPost]
        public Models.BackParameter GetAllUser([FromBody]Models.GainParameter GP)
        {
            try
            {
                Models.User.Back.userall BGL = new Models.User.Back.userall();
                IEnumerable<Models.User.Gain.menu> menu = db.Database.SqlQuery<Models.User.Gain.menu>("select id,name from [dbo].[Branch_T]");
                IEnumerable<Models.User.Gain.menusecend> menusecend = db.Database.SqlQuery<Models.User.Gain.menusecend>("select id,name,branchid,parentid from [dbo].[Dept_T]");
                IEnumerable<Models.User.Gain.userallmenu> userallmenu = db.Database.SqlQuery<Models.User.Gain.userallmenu>("select BranchId,DeptId,username,MOBILE,EMAIL,SEX,branchid,duty,userid,PicName,Hxid from [dbo].[User_T]");
                List<Models.User.Gain.userallmenu> usersss=new List<Models.User.Gain.userallmenu>();
                string sj = newsj();
                foreach (Models.User.Gain.userallmenu son in userallmenu)
                {
                    usersss.Add(son);
                }
                BGL.menufirst = menu;
                BGL.menusecend = menusecend;
                BGL.userallmenu = usersss;
                BP.back = BGL;
            }
            catch (Exception ee)
            {
                BP.back = ee.ToString();
            }
            return BP;
        }
        [Check]
        [HttpPost]
        public Models.BackParameter Pdyhzc([FromBody]Models.AddHxuser GP)
        {
            try
            {
                string sj = newsj();
                if (GP.touserid != null)
                {
                    BP.back = sj + '_' + GP.touserid;
                }
                else
                {
                    BP.back = "请输入联系人userid";
                }
            }
            catch (Exception ee)
            {
                BP.back = ee.ToString();
            }
            return BP;
        }
        [Check]
        [HttpPost]
        public Models.BackParameter setpic([FromBody]Models.User.Gain.Pic GP)
        {
            try
            {
                Tools.Czexcel.importexcel("");
                DateTime dt = DateTime.Now;
                string str = dt.ToString("yyyyMMddHHmmssms");
                string picname = str + GP.UserId;
                string luj = System.AppDomain.CurrentDomain.BaseDirectory+@"picimage\"; //获取文件路径
                if (!Directory.Exists(luj))
                {
                    Directory.CreateDirectory(System.AppDomain.CurrentDomain.BaseDirectory+@"picimage");
                }
                GP.picdata = GP.picdata.Replace(" ", "+");
                string delpicname = db.Database.SqlQuery<string>("select PicName from user_t where userid='" + GP.UserId + "'").FirstOrDefault();
                if (System.IO.File.Exists(luj + delpicname + ""))
                {
                    System.IO.File.Delete(luj + delpicname + "");
                }
             
                db.Database.ExecuteSqlCommand("update user_t set PicName='" + picname + "." + GP.pictype + "',pictype='" + GP.pictype + "',picdata='" + Convert.FromBase64String(GP.picdata) + "',picsize=" + Convert.FromBase64String(GP.picdata).Length+ " where userid='" + GP.UserId + "'");
                byte[] arr = Convert.FromBase64String(GP.picdata);
                using (MemoryStream ms = new MemoryStream(arr))
                {
                    System.Drawing.Bitmap bmp2 = new System.Drawing.Bitmap(ms);

                    if (GP.pictype=="jpg")
                        bmp2.Save(luj + picname+".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                    if (GP.pictype == "bmp")
                        bmp2.Save(luj + picname + ".bmp", System.Drawing.Imaging.ImageFormat.Bmp);
                    if (GP.pictype == "gif")
                        bmp2.Save(luj + picname + ".gif", System.Drawing.Imaging.ImageFormat.Gif);
                    if (GP.pictype == "png")
                        bmp2.Save(luj + picname + ".png", System.Drawing.Imaging.ImageFormat.Png);
                }
                BP.back = picname + "." + GP.pictype;
                //string luj = System.AppDomain.CurrentDomain.BaseDirectory; //获取文件路径
                //if (GP.picname == null)
                //{
                //    BP.back = "请输入图片名字picname";
                //    return BP;
                //}
                //if (!System.IO.File.Exists(luj + @"picimage\" + GP.picname + ""))
                //{
                //    BP.back = "未上传该图片";
                //    return BP;
                //}
                //string picname = db.Database.SqlQuery<string>("select PicName from user_t where userid='" + GP.UserId + "'").FirstOrDefault();
                //if (picname != "")
                //{
                //    if (System.IO.File.Exists(luj + @"picimage\" + picname + "") && picname != GP.picname)
                //    {
                //        System.IO.File.Delete(luj + @"picimage\" + picname + "");
                //        BP.back = "头像修改成功";
                //        db.Database.ExecuteSqlCommand("update user_t set PicName='" + GP.picname + "' where userid='" + GP.UserId + "'");
                //    }
                //    else
                //        BP.back = "头像不能与原头像相同";
                //}
                //else
                //{
                //    BP.back = "头像添加成功";
                //    db.Database.ExecuteSqlCommand("update user_t set PicName='" + GP.picname + "' where userid='" + GP.UserId + "'");
                //}
            }
            catch (Exception ee)
            {
                BP.back = ee.ToString();
            }
            return BP;
        }
        [Check]
        [HttpPost]
        public Models.BackParameter getpic([FromBody]Models.User.Gain.picuserid GP)
        {
            User_T user = new User_T();
            if (GP.PICUSERID == null&& GP.HXUSERID == null)
            {
                BP.back = "请输入picuserid或者HXUSERID";
                return BP;
            }
            try
            {
                user = db.Database.SqlQuery<User_T>("select * from user_t where userid='" + GP.PICUSERID + "'").FirstOrDefault();
                if (user.PicName != null)
                {
                    BP.back = @"/picimage/" + user.PicName;
                    PicFileCheck(BP.back.ToString(), user.PicData);
                }
                else
                {
                    BP.back = "该用户暂无头像";
                }
            }
            catch (Exception ee)
            {
                BP.back = ee.ToString();
            }
            return BP;
        }

        private void PicFileCheck(string file,byte[] fileBytes)
        {
            var path = AppDomain.CurrentDomain.BaseDirectory + file;
            if (!System.IO.File.Exists(path))
            {
                CreateImageFromBytes(path, fileBytes);
            }
        }
        /// <summary>
        /// Convert Byte[] to a picture and Store it in file
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        private static string CreateImageFromBytes(string fileName, byte[] buffer)
        {
            System.IO.FileInfo info = new System.IO.FileInfo(fileName);
            System.IO.Directory.CreateDirectory(info.Directory.FullName);
            System.IO.File.WriteAllBytes(fileName, buffer);
            return fileName;
        }
        /// <summary>
        /// Convert Byte[] to Image
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static Image BytesToImage(byte[] buffer)
        {
            MemoryStream ms = new MemoryStream(buffer);
            Image image = System.Drawing.Image.FromStream(ms);
            return image;
        }

        [Check]
        [HttpPost]
        public Models.BackParameter updatepw([FromBody]Models.User.Gain.updatepassword GP)
        {
            if (GP.newpassword == null)
            {
                BP.back = "请输入新密码";
                return BP;
            }
            try
            {
                string cl = GP.newpassword;
                string pwd = "";
                MD5 md5 = MD5.Create(); //实例化一个md5对像
                // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　
                byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(cl));
                // 通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得
                for (int i = 0; i < s.Length; i++)
                {
                    // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符 
                    pwd = pwd + s[i].ToString("X").PadLeft(2, '0'); ;
                }
                db.Database.ExecuteSqlCommand("update user_t set Password='"+pwd+"' where userid='" + GP.UserId + "'");
                BP.back = "密码修改成功";
             
            }
            catch (Exception ee)
            {
                BP.back = ee.ToString();
            }
            return BP;
        }
        //[Check]
        //[HttpPost]
        //public Models.BackParameter getLJ([FromBody]Models.GainParameter GP)
        //{
        //    try
        //    {
        //        string luj =  @"/picimage/"; //获取文件路径
        //        BP.back = luj;
        //    }
        //    catch (Exception ee)
        //    {
        //        BP.back = ee.ToString();
        //    }
        //    return BP;
        //}
        public string newsj()
        {
            string sj = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            if (db.Database.SqlQuery<string>("select setting from  [dbo].[ServerConfig_T] where Type='HX'").FirstOrDefault() ==null)
            {
                db.Database.ExecuteSqlCommand("insert into [dbo].[ServerConfig_T] (Type,ParamName,Descript,setting) values ('HX','HX','环信配置','" + sj + "')");
                return sj;
            }
            else
                return db.Database.SqlQuery<string>("select setting from  [dbo].[ServerConfig_T] where Type='HX'").FirstOrDefault();
        }

        [Check]     
        [HttpPost]
        public Models.BackParameter Getsub([FromBody]Models.GainParameter GP)
        {
            try
            {
                var sql = $@"select u.* from [dbo].[User_T] u
inner join (SELECT * FROM dbo.GetUnderling_F('{GP.UserId}',1)) gu on u.UserId = gu.UserId";
                var subs = db.Database.SqlQuery<Models.User.Gain.userallmenu>(sql).ToList();
                if (!subs.Any())
                {
                    BP.back = "你没有下属";
                    return BP;
                }
                List<Models.User.Gain.userallmenu> BiMjm = new List<Models.User.Gain.userallmenu>();
                foreach (Models.User.Gain.userallmenu bison in subs)
                {
                    bison.Password = Tools.Base.GetmD5jm(bison.Password);
                    BiMjm.Add(bison);
                }
                BP.back = BiMjm;
            }
            catch (Exception ee)
            {
                string BG = "";
                BG = ee.ToString();
                BP.back = BG;
            }

            return BP;
        }
       
        [HttpPost]
        public Models.BackParameter getnewapp([FromBody]Models.User.Gain.Get GP)
        {
            BP.back = Tools.Log.getbeh(int.Parse(GP.appid.Substring(0, 1)));
            return BP;
        }
    }
    internal class MenuLevel 
    {
        public  string menuno { get; set; }
        public string name { get; set; }
        public List<object> next { get; set; }
    }//拥有下级的菜单类
}
