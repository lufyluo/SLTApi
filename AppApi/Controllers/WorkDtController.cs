using AppApi.App_Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AppApi.Tools;
using AppApi.Filter;


namespace AppApi.Controllers
{
    public class WorkDtController : BaseControllers
    {
        
        /// <summary>
        /// 获取待办日程
        /// </summary>
        /// <param name="G"></param>
        /// <returns></returns>
        [Check]
        [HttpPost]
        public Models.BackParameter Getpeding([FromBody]Models.WorkDt.Gain.workdt G)
        {
            SqlClass.Where where = new SqlClass.Where();
            if (!(G.act == null || G.act == "" || G.act.ToUpper() == "ALL"))
                where.And("MenuNo='"+G.act+"'");
            if (!(G.date == null || G.date == ""))
            {
                where.And("End_date >= '" + G.date + "'");
                where.And("Start_date <= '" + G.date + "'");
            }
            int uidtext = db.Database.SqlQuery<int>("select UID from user_t where userid='" + G.UserId + "'").FirstOrDefault();
            if (uidtext != 0)
            {
                where.And("(isnull(creater,'') = '" + G.UserId + "' or (isnull(reminder,'') like '%," + uidtext + ",%' or isnull(ShareUID1,'')='-1'))");

            }
            try
            {
                Models.WorkDt.Back.GetAll BGL = new Models.WorkDt.Back.GetAll();
                IEnumerable<Models.WorkDt.Back.Dt> BiM = db.Database.SqlQuery<Models.WorkDt.Back.Dt>("select * from Pending_T  " + where.ToString() + " ");
                BGL.count = db.Database.SqlQuery<int>("select count(*) from Pending_T  " + where.ToString() + " ").FirstOrDefault();
                BGL.getall = BiM;
                BP.back = BGL;
                return BP;
            }
            catch (Exception ee)
            {
                BP.back = ee.ToString();
                return BP;
            }
        }
        /// <summary>
        /// 修改待办日程
        /// </summary>
        /// <param name="G"></param>
        /// <returns></returns>
        [Check]
        [HttpPost]
        public Models.BackParameter updatepeding([FromBody]Models.WorkDt.Gain.update G)
        {
            string update = "";
            if (G.updid == null || G.updid == 0)
            {
                BP.back = "请输入id";
                return BP;
            }
            if (!(G.upd == null || G.upd == ""))
            {
                if(G.upd=="true")
                    update = " isend=1";
                else
                    update = " isend=0";
            }
            try
            {
                db.Database.ExecuteSqlCommand("update Pending_T set "+update+" where id="+G.updid+"");
                BP.back = "处理成功";
                return BP;
            }
            catch (Exception ee)
            {
                BP.back = ee.ToString();
                return BP;
            }
        }
        /// <summary>
        /// 添加待办日程
        /// </summary>
        /// <param name="G"></param>
        /// <returns></returns>
        [Check]
        [HttpPost]
        public Models.BackParameter Addpeding([FromBody]Models.WorkDt.Gain.Add G)
        {
            try
            {
                string sqlstr = "insert into Pending_T (menuno,start_date," +
                   "end_date,is_repeat,clientid,note,Urgency_level,Reminder,Creater," +
                   "CreaterName,CreateTm) values('" + G.menuno + "','" + G.start_date + "'," +
                "'" + G.end_date + "'," + G.is_repeat + "," + G.clientid + ",'" + G.note + "'," +
                "" + G.Urgency_level + "," + G.Reminder + ",'" + G.UserId + "','" + db.Database.SqlQuery<string>("select username from user_t  where userid ='" + G.UserId + "'").FirstOrDefault() + "'," +
                "'" + DateTime.Now.ToShortDateString() + "')";
                db.Database.ExecuteSqlCommand(sqlstr);
        
                BP.back = "添加成功";
                return BP;
            }
            catch (Exception ee)
            {
                BP.back = ee.ToString();
                return BP;
            }
        }

        /// <summary>
        /// 获取工作动态和评论
        /// </summary>
        /// <param name="GL"></param>
        /// <returns></returns>
        [Check]
        [HttpPost]
        public Models.BackParameter GetWorkDt([FromBody]Models.Mail.Gain.GetList GL)
        {
            SqlClass sqlclass = new SqlClass();
            SqlClass.Select select = new SqlClass.Select("WorkDynamic_T");
            SqlClass.Where where = new SqlClass.Where();
            SqlClass.Order orderby = new SqlClass.Order();
            orderby.Desc("Id");
            try
            {
                int count = db.Database.SqlQuery<int>(sqlclass[select.Count("count"), where, new SqlClass.Order()]).FirstOrDefault();
                if (GL.PageIndex < 1)
                {
                    BP.code = "-1";
                    BP.back = "分页超出范围";
                    return BP;
                }
                string sql;
                int uidtext = db.Database.SqlQuery<int>("select UID from user_t where userid='" + GL.UserId + "'").FirstOrDefault();
                where.And("(isnull(ShareUID1,'') like '%," + uidtext + ",%' or (isnull(ShareUID,'') like '%," + uidtext + ",%' or isnull(ShareUID1,'')='-1'))");
                select.Add("[Id],[MenuNo],[KeyId],[TableName],[ShareUID],[TitleHtml],[Bak],[CommCount],[Creater],[CreaterName],[CreateTm],[ShareUID1]");
                sql = sqlclass.Page(GL.PageIndex, GL.PageMax, select, where, orderby);
                IEnumerable<Models.WorkDt.Back.workdt> BiM = db.Database.SqlQuery<Models.WorkDt.Back.workdt>(sql);
                List<Models.WorkDt.Back.workdt> commlist = new List<Models.WorkDt.Back.workdt>();
                foreach (Models.WorkDt.Back.workdt son in BiM)
                {
                    IEnumerable<Models.WorkDt.Back.Comm> Comm = db.Database.SqlQuery<Models.WorkDt.Back.Comm>("select * from comm_t where keyid=" + son.Id + " and MenuNo='AT' and (isnull(ShareUID,'') like '%," + uidtext + ",%' or isnull(ShareUID,'') like '%," + uidtext + ",%' or isnull(ShareUID1,'')='-1')");
                    son.commlist = Comm;
                    commlist.Add(son);
                }
                Models.WorkDt.Back.GetworkDt BGLS = new Models.WorkDt.Back.GetworkDt();
                BGLS.workdt = commlist;
                BGLS.count = count;
                BP.back = BGLS;
               return BP;
            }
            catch (Exception ee)
            {
                BP.back = ee.ToString();
                return BP;
            }
        }
        /// <summary>
        /// 获取评论数据
        /// </summary>
        /// <param name="GL"></param>
        /// <returns></returns>
        [Check]
        [HttpPost]
        public Models.BackParameter Getcommlist([FromBody]Models.Mail.Gain.GetList GL)
        {
            SqlClass sqlclass = new SqlClass();
            SqlClass.Select select = new SqlClass.Select("Comm_T left join user_t on Comm_T.creater=userid");
            SqlClass.Where where = new SqlClass.Where();
            SqlClass.Order orderby = new SqlClass.Order();
           // where.And("Comm_T.creater=userid");
            if (!(GL.Act == null || GL.Act == "" || GL.Act.ToUpper() == "ALL"))
            {
                where.And("MenuNo='" + GL.Act + "'");
            }
          //  orderby.Desc("keyid");
            int uidtext = db.Database.SqlQuery<int>("select UID from user_t where userid='" + GL.UserId + "'").FirstOrDefault();
            where.And("(isnull(ShareUID1,'') like '%," + uidtext + ",%' or (isnull(ShareUID,'') like '%," + uidtext + ",%' or isnull(ShareUID1,'')='-1'))");
            orderby.Desc("Comm_T.CreateTm");
            try
            {
                int count = db.Database.SqlQuery<int>(sqlclass[select.Count("count"), where, new SqlClass.Order()]).FirstOrDefault();
                if (GL.PageIndex < 1)
                {
                    BP.code = "-1";
                    BP.back = "分页超出范围";
                    return BP;
                }
                string sql;
                select.Add("[Id],[MenuNo],[WdId],[KeyId],[TableName],[ShareUID],[TitleHtml],[Bak],Creater=Comm_T.[Creater],CreaterName=Comm_T.[CreaterName],CreateTm=Comm_T.[CreateTm],[ShareUID1],[picname]");
                sql = sqlclass.Page(GL.PageIndex, GL.PageMax, select, where, orderby);
                IEnumerable<Models.WorkDt.Back.Comm> BiM = db.Database.SqlQuery<Models.WorkDt.Back.Comm>(sql);
                //Nullable<int> commid=-1;
                //List<Models.WorkDt.Back.Comm> commlists=new List<Models.WorkDt.Back.Comm>();
                //int count = -1;
                //foreach (Models.WorkDt.Back.Comm son in BiM)
                //{

                //    if (commid == -1)
                //    {
                //        commid = son.KeyId;
                //        count++;
                //        if (count >= (GL.PageIndex - 1) * GL.PageMax)
                //        {
                //            commlists.Add(son);
                //        }
                //    }
                //    else if (commid != son.KeyId)
                //    {
                //        count++;
                //        if (count >= (GL.PageIndex - 1) * GL.PageMax)
                //        {
                //            commlists.Add(son);
                //        }
                //    }
                //    commid = son.KeyId;
                //    if (commlists.Count == GL.PageMax)
                //        break;
                //}
                Models.WorkDt.Back.Getcomm coml= new Models.WorkDt.Back.Getcomm();
                //coml.count = commlists.Count;
                //coml.commlists = commlists;
                coml.commlistss = BiM;
                coml.count = count;
                BP.back = coml;
                return BP;
            }
            catch (Exception ee)
            {
                BP.back = ee.ToString();
                return BP;
            }
        }

        /// <summary>
        /// 获取详细评论
        /// </summary>
        /// <param name="G"></param>
        /// <returns></returns>
        [Check]
        [HttpPost]
        public Models.BackParameter Getcomm([FromBody]Models.WorkDt.Gain.coom G)
        {
            SqlClass.Where where = new SqlClass.Where();
            int uidtext = db.Database.SqlQuery<int>("select UID from user_t where userid='" + G.UserId + "'").FirstOrDefault();
            where.And("(isnull(ShareUID1,'') like '%," + uidtext + ",%' or (isnull(ShareUID,'') like '%," + uidtext + ",%' or isnull(ShareUID1,'')='-1' or isnull(ShareUID1,'')=''))");
            if (!(G.commid == 0))
                where.And("id=" + G.keyid + "");
            else
            {
                if (!(G.keyid == 0))
                    where.And(" keyid=" + G.keyid + "");
                if (!(G.menuno == null || G.menuno == ""))
                    where.And(" MenuNo='" + G.menuno + "'");
            }

            try
            {
                string sql = "select [Id],[MenuNo],[WdId],[KeyId],[TableName],[ShareUID],[TitleHtml],[Bak],Creater=Comm_T.[Creater],CreaterName=Comm_T.[CreaterName],CreateTm=Comm_T.[CreateTm],[ShareUID1],[picname] from Comm_T left join user_t on Comm_T.creater=userid " + where.ToString() + " order by Comm_T.createtm desc";
                IEnumerable<Models.WorkDt.Back.Comm> BiM = db.Database.SqlQuery<Models.WorkDt.Back.Comm>(sql);
                BP.back = BiM;
                return BP;
            }
            catch (Exception ee)
            {
                BP.back = ee.ToString();
                return BP;
            }
        }

        /// <summary>
        /// 评论
        /// </summary>
        /// <param name="G"></param>
        /// <returns></returns>
        [Check]
        [HttpPost]
        public Models.BackParameter Setcomm([FromBody]Models.WorkDt.Gain.coom G)
        {
            if (G.keyid == 0)
            {
                BP.back = "请输入keyid";
                return BP;
            }
            if (G.bak == null || G.bak == "")
            {
                BP.back = "请输入评论内容";
                return BP;
            }
            if (G.menuno == null || G.menuno == "")
            {
                BP.back = "请输入菜单号 如：AT";
                return BP;
            }
            try
            {
                //string menuno=db.Database.SqlQuery<string>("select menuno from Comm_T where keyid="+G.keyid+"").FirstOrDefault();
                string wdid = "0";
                if (G.menuno.ToUpper() == "AT")
                    wdid = G.keyid.ToString();
                string strsql = "[dbo].[SavePl]" + G.menuno + "," + wdid + "," + G.keyid + ",'" + G.bak + "','" + G.UserId + "','-1'";
                db.Database.ExecuteSqlCommand(strsql);
                BP.back = "评论成功";
                return BP;
            }
            catch (Exception ee)
            {
                BP.back = ee.ToString();
                return BP;
            }
        }
        /// <summary>
        /// 获取消息
        /// </summary>
        /// <param name="G"></param>
        /// <returns></returns>
        [Check]
        [HttpPost]
        public Models.BackParameter Getmsg([FromBody]Models.Mail.Gain.GetList GL)
        {
            SqlClass sqlclass = new SqlClass();
            SqlClass.Select select = new SqlClass.Select("MSG_T");
            SqlClass.Where where = new SqlClass.Where();
            SqlClass.Order orderby = new SqlClass.Order();
            if (!(GL.Act == null || GL.Act == "" || GL.Act.ToUpper() == "ALL"))
            {
                where.And("MenuNo='" + GL.Act + "'");
            }
            
            try
            {

                select.Addtable("MsgUser_T");
                where.And("MSG_T.Id=MsgUser_T.msgid");
                where.And("userid='" + GL.UserId + "'");
                orderby.Desc("MSG_T.CreateTm");
                string sql;
                select.Add("MSG_T.[Id] as Id,[MenuNo],[KeyId],[ExId],[ExId1],[Type],[Html],[Creater],[CreaterName],MSG_T.[CreateTm] as [CreateTm],[AutoClose],[IsRead],[IsShow]");
                sql = sqlclass.Page(GL.PageIndex, GL.PageMax, select, where, orderby);
                int count = db.Database.SqlQuery<int>(sqlclass[select.Count("count"), where, new SqlClass.Order()]).FirstOrDefault();
                if (GL.PageIndex < 1)
                {
                    BP.code = "-1";
                    BP.back = "分页超出范围";
                    return BP;
                }
                IEnumerable<Models.WorkDt.Back.msg> BiM = db.Database.SqlQuery<Models.WorkDt.Back.msg>(sql);
                Models.WorkDt.Back.Getmsg coml = new Models.WorkDt.Back.Getmsg();
                coml.count = count;
                coml.msg = BiM;
                //已读未读
                {
                    where.And("isread=1");
                    sql = sqlclass.Page(GL.PageIndex, GL.PageMax, select, where, orderby);
                    int yd = db.Database.SqlQuery<int>(sqlclass[select.Count("count"), where, new SqlClass.Order()]).FirstOrDefault();
                    where.And("isread=0");
                    sql = sqlclass.Page(GL.PageIndex, GL.PageMax, select, where, orderby);
                    int wd = db.Database.SqlQuery<int>(sqlclass[select.Count("count"), where, new SqlClass.Order()]).FirstOrDefault();
                    coml.yd = yd;
                    coml.wd = wd;
                }
                BP.back = coml;
                return BP;
            }
            catch (Exception ee)
            {
                BP.back = ee.ToString();
                return BP;
            }
        }

        /// <summary>
        /// 全部已读 全部未读
        /// </summary>
        /// <param name="G"></param>
        /// <returns></returns>
        [Check]
        [HttpPost]
        public Models.BackParameter upmsg([FromBody]Models.WorkDt.Gain.Msgup GL)
        {
            db.Database.ExecuteSqlCommand("update MsgUser_T set IsRead="+GL.ydwd+" where userid='"+GL.UserId+"'");
            BP.back = "修改成功";
            return BP;
        }
        /// <summary>
        /// 获取商机
        /// </summary>
        /// <param name="G"></param>
        /// <returns></returns>
        [Check]
        [HttpPost]
        public Models.BackParameter GetChance([FromBody]Models.Mail.Gain.GetList G)
        {
            SqlClass.Where where = new SqlClass.Where();
            try
            {
                SqlClass sqlclass = new SqlClass();
                SqlClass.Select select = new SqlClass.Select("Chance_T");
                SqlClass.Where wheres = new SqlClass.Where();
                SqlClass.Order orderby = new SqlClass.Order();
                select.Add("*");
                orderby.Desc("CreateTm");
                string sql = sqlclass.Page(G.PageIndex, G.PageMax, select, where, orderby);
                Models.WorkDt.Back.GetChance BGL = new Models.WorkDt.Back.GetChance();
                IEnumerable<Models.WorkDt.Back.Chance> BiM = db.Database.SqlQuery<Models.WorkDt.Back.Chance>(sql);
                BGL.count = db.Database.SqlQuery<int>("select count(*) from Chance_T").FirstOrDefault();
                List<Models.WorkDt.Back.Chance> blist = new List<Models.WorkDt.Back.Chance>();
                foreach (Models.WorkDt.Back.Chance son in BiM)
                {
                    IEnumerable<Models.WorkDt.Back.Comm> Comm = db.Database.SqlQuery<Models.WorkDt.Back.Comm>("select * from comm_t where keyid=" + son.id + " and MenuNo='AC'");
                    son.commlist = Comm;
                    blist.Add(son);
                }
                BGL.Chance = blist;

                BP.back = BGL;
                return BP;
            }
            catch (Exception ee)
            {
                BP.back = ee.ToString();
                return BP;
            }
        }
        /// <summary>
        /// 获取日报
        /// </summary>
        /// <param name="G"></param>
        /// <returns></returns>
        [Check]
        [HttpPost]
        public Models.BackParameter Getdayrepot([FromBody]Models.Mail.Gain.GetList G)
        {
            try
            {
                User_T MBR = db.User_T.Where(MBRW => MBRW.UserId ==G.UserId).FirstOrDefault();
                Models.WorkDt.Back.GetDayReport BGL = new Models.WorkDt.Back.GetDayReport();
                string type = "";
                if (MBR.UserType == 1)
                {
                    type += "(";
                    IEnumerable<int> dep = db.Database.SqlQuery<int>("select id from dept_t where BranchId=" + MBR.BranchId + "");
                    foreach (int depson in dep)
                    {
                        if (type != "" && type != "(") type += " or ";

                        type += "DeptId=" + depson + "";

                    }
                    type += ")";
                }
                else if (MBR.UserType == 2)
                    type = "DeptId=" + MBR.DeptId + "";
                else
                    type = "Creater='" + MBR.UserId + "'";
                IEnumerable<Models.WorkDt.Back.WorkPort> BiM = db.Database.SqlQuery<Models.WorkDt.Back.WorkPort>("select * from [dbo].[WorkReport_T] where  " + type + "  and type=1 order by CreateTm ");
                BGL.count = db.Database.SqlQuery<int>("select count(*) from [dbo].[WorkReport_T] where  " + type + "  and type=1").FirstOrDefault();
                BGL.BGL = BiM;
                BP.back = BGL;
                return BP;
            }
            catch (Exception ee)
            {
                BP.back = ee.ToString();
                return BP;
            }
        }
        /// <summary>
        /// 获取周报
        /// </summary>
        /// <param name="G"></param>
        /// <returns></returns>
        [Check]
        [HttpPost]
        public Models.BackParameter GetWeekrepot([FromBody]Models.Mail.Gain.GetList G)
        {
            try
            {
                User_T MBR = db.User_T.Where(MBRW => MBRW.UserId == G.UserId).FirstOrDefault();
                Models.WorkDt.Back.GetWeekRport BGL = new Models.WorkDt.Back.GetWeekRport();
                string type = "";
                if (MBR.UserType == 1)
                {
                    type += "(";
                    IEnumerable<int> dep = db.Database.SqlQuery<int>("select id from dept_t where BranchId="+MBR.BranchId+"");
                    foreach (int depson in dep)
                    {
                        if (type != ""&&type!="(") type += " or ";
                        
                        type += "DeptId=" + depson + "";

                    }
                    type += ")";
                }
                else if (MBR.UserType == 2)
                    type = "DeptId=" + MBR.DeptId + "";
                else
                    type = "Creater='" + MBR.UserId + "'";
                IEnumerable<Models.WorkDt.Back.WorkWeekPort> BiM = db.Database.SqlQuery<Models.WorkDt.Back.WorkWeekPort>("select * from [dbo].[WorkReport_T] where  " + type + "  and type=2 order by CreateTm ");
                BGL.count = db.Database.SqlQuery<int>("select count(*) from [dbo].[WorkReport_T] where "+ type + "  and type=2").FirstOrDefault();
                BGL.BGL = BiM;
               BP.back = BGL;
                return BP;
            }
            catch (Exception ee)
            {
                BP.back = ee.ToString();
                return BP;
            }
        }
        /// <summary>
        /// 获取商品
        /// </summary>
        /// <param name="GL"></param>
        /// <returns></returns>
        [Check]
        [HttpPost]
        public Models.BackParameter GetProduct([FromBody]Models.Mail.Gain.GetList GL)
        {
            SqlClass sqlclass = new SqlClass();
            SqlClass.Select select = new SqlClass.Select("Product_T");
            SqlClass.Where where = new SqlClass.Where();
            SqlClass.Order orderby = new SqlClass.Order();

            try
            {
                select.Add("*");
                if (GL.Key != null && GL.Key.Length > 0)
                {
                    where.Like("CnName,EnName,CiqCode,Model,Spec,Type,BeUserName,Currency,MOQ", GL.Key);
                }
                where.And("rootid in (select rootid from #ProductTmpTable)");
                orderby.Desc("No");
                string sql;
                sql = sqlclass.Page(GL.PageIndex, GL.PageMax, select, where, orderby);
                string sqlqz = @"IF OBJECT_ID('tempdb..#TmpTable') is not null
drop table #ProductTmpTable
Create Table #ProductTmpTable(Id int,
		Name nvarchar(50),
		ParentId int,
		Sort int)
Insert Into #ProductTmpTable Exec  [dbo].[GetProductRoot]'"+GL.UserId+"',1 ";
                sql = sqlqz+sql;
                int count = db.Database.SqlQuery<int>(sqlqz+sqlclass[select.Count("count"), where, new SqlClass.Order()]).FirstOrDefault();
                if (GL.PageIndex < 1)
                {
                    BP.code = "-1";
                    BP.back = "分页超出范围";
                    return BP;
                }
                IEnumerable<Product_T> BiM = db.Database.SqlQuery<Product_T>(sql);
                Models.WorkDt.Back.Product coml = new Models.WorkDt.Back.Product();
                coml.count = count;
                coml.Pro = BiM;
                BP.back = coml;
                return BP;
            }
            catch (Exception ee)
            {
                BP.back = ee.ToString();
                return BP;
            }
        }
        /// <summary>
        /// 获取价格询盘
        /// </summary>
        /// <param name="GL"></param>
        /// <returns></returns>
        [Check]
        [HttpPost]
        public Models.BackParameter GetInquiry([FromBody]Models.WorkDt.Gain.Inquiry GL)
        {
            SqlClass sqlclass = new SqlClass();
            SqlClass.Select select = new SqlClass.Select("[dbo].[Inquiry_T]");
            SqlClass.Where where = new SqlClass.Where();
            SqlClass.Order orderby = new SqlClass.Order();

            try
            {
                select.Add("*");
                where.And("state="+GL.state+"");
                orderby.Desc("No");
                string sql;
                sql = sqlclass.Page(GL.PageIndex, GL.PageMax, select, where, orderby);

                int count = db.Database.SqlQuery<int>(sqlclass[select.Count("count"), where, new SqlClass.Order()]).FirstOrDefault();
                if (GL.PageIndex < 1)
                {
                    BP.code = "-1";
                    BP.back = "分页超出范围";
                    return BP;
                }
                IEnumerable<Inquiry_T> BiM = db.Database.SqlQuery<Inquiry_T>(sql);
                Models.WorkDt.Back.InquiryBack coml = new Models.WorkDt.Back.InquiryBack();
                coml.count = count;
                coml.Inq= BiM;
                BP.back = coml;
                return BP;
            }
            catch (Exception ee)
            {
                BP.back = ee.ToString();
                return BP;
            }
        }
        /// <summary>
        /// 获取消息目录
        /// </summary>
        /// <param name="GL"></param>
        /// <returns></returns>
        [Check]
        [HttpPost]
        public Models.BackParameter GetmsgLb([FromBody]Models.Mail.Gain.GetList GL)
        {
            try
            {
                string sql = "select * from [dbo].[SysDictionary_T] where DataType='消息来源'";
                IEnumerable<SysDictionary_T> BiM = db.Database.SqlQuery<SysDictionary_T>(sql);
                List<Models.WorkDt.Back.MsgLb> coml = new  List<Models.WorkDt.Back.MsgLb>();
                //已读未读
                foreach (SysDictionary_T bimson in BiM)
                {              
                    sql = "select count(*) from [dbo].[Msg_T],[dbo].[MsgUser_T] where [dbo].[Msg_T].Id=[dbo].[MsgUser_T].MsgId and MenuNo='"+bimson.DataValue2+"' and isread=1 and userid='"+GL.UserId+"'";
                    int yd = db.Database.SqlQuery<int>(sql).FirstOrDefault();
                    sql = "select count(*) from [dbo].[Msg_T],[dbo].[MsgUser_T] where [dbo].[Msg_T].Id=[dbo].[MsgUser_T].MsgId and MenuNo='" + bimson.DataValue2 + "' and isread=0 and userid='" + GL.UserId + "'";
                    int wd = db.Database.SqlQuery<int>(sql).FirstOrDefault();
                    sql = "select count(*) from [dbo].[Msg_T],[dbo].[MsgUser_T] where [dbo].[Msg_T].Id=[dbo].[MsgUser_T].MsgId and MenuNo='" + bimson.DataValue2 + "' and userid='" + GL.UserId + "'";
                    int zs = db.Database.SqlQuery<int>(sql).FirstOrDefault();
                    Models.WorkDt.Back.MsgLb comson = new Models.WorkDt.Back.MsgLb();
                    comson.menuno = bimson.DataValue2;
                    comson.menuname = bimson.DataText;
                    comson.yd = yd;
                    comson.wd = wd;
                    comson.zs = zs;
                    coml.Add(comson);
                }
                BP.back = coml;
                return BP;
            }
            catch (Exception ee)
            {
                BP.back = ee.ToString();
                return BP;
            }
        }
    }
}