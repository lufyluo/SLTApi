using AppApi.App_Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AppApi.Filter;
using AppApi.Tools;
using System.Data;
using System.Configuration;

namespace AppApi.Controllers
{
    public class MailController : BaseControllers
    {
        private static string dbname = System.Web.Configuration.WebConfigurationManager.AppSettings["dbname"];
        [Check]
        [Mail]
        [HttpPost]
        public Models.BackParameter Get([FromBody]Models.Mail.Gain.Get G)
        {

            string UserId = G.UserId;

            int MailId = G.MailId;

            Models.Mail.Back.Get BG = dbm.Database.SqlQuery<Models.Mail.Back.Get>("Select top 1 * from Mail_T where id=" + MailId).FirstOrDefault();

            IEnumerable<Models.Mail.Back.item.File> BiF = dbm.Database.SqlQuery<Models.Mail.Back.item.File>("Select id=id,name=FileName,size=FileSize from MailAcc_T where DownloadId!='' and DownloadId is not null and mailid=" + MailId);

            //将图片转换为base64让图片直接读取
            //获取图片contectid    
            IEnumerable<MailAcc_T> pic = dbm.MailAcc_T.Where(maw => maw.MailId == MailId && maw.ContentID.Length > 0).ToList();//获取所有pic
            List<long> list = pic.Select(ps => ps.Id).ToList();//获取所有pic对应的fileid
            String html = BG.htmlbody;
            foreach (MailAcc_T picson in pic)
            {
                TCRMCLASS.FileClass fc = new TCRMCLASS.FileClass();
                string strtemppath = fc.GetDownloadUrl("2", picson.Id.ToString());
                byte[] wj = FileBinaryConvertHelper.FiletoBytes(strtemppath);                
                FileBinaryConvertHelper.DeleteFile(strtemppath);
                try
                {
                    html = html.Replace(String.Format("cid:{0}", picson.ContentID), String.Format("data:image/{0};base64,", Base.imgformat[picson.FileName.Substring(picson.FileName.LastIndexOf(".") + 1).ToLower()]) + Convert.ToBase64String(wj));
                }
                catch(Exception ee)
                {

                }
            }
            BG.htmlbody = html;
            BG.file = BiF.ToList(); ;
            BP.back = BG;
            return BP;
        }
        [Check]
        [HttpPost]
        public Models.BackParameter GetClientmail([FromBody]Models.Mail.Gain.Get G)
        {
            Nullable<int> cclientid;
            if (G.clientid == null||G.clientid==0)
            {
                string UserId = G.UserId;

                int MailId = G.MailId;
                 cclientid = dbm.Database.SqlQuery<Nullable<int>>("select ClientId from mailclient_t where ClientMailId=" + MailId + "").FirstOrDefault();
                if (cclientid == null)
                {
                    BP.back = "该用户不是客户";
                    return BP;
                }
            }
            else  cclientid=G.clientid;
            string sqlstr = "select * from mail_t where id in (select clientmailid from mailclient_t where clientid=" + cclientid + ")";
            IEnumerable<Models.Mail.Back.Get> BG = dbm.Database.SqlQuery<Models.Mail.Back.Get>(sqlstr);
            BP.back = BG;
            return BP;
        }
        [Check]
        [HttpPost]
        public Models.BackParameter GetList([FromBody]Models.Mail.Gain.GetList GL)
        {

            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start(); //  开始监视代码
 
            int uid = db.Database.SqlQuery<int>("select uid from  [dbo].[User_T] where userid='" + GL.UserId + "'").FirstOrDefault();
            string UserId = GL.UserId;
            int BoxId = GL.BoxId;
            int PageIndex = GL.PageIndex;
            String Act = GL.Act;
            int PageMax = GL.PageMax;
            string order = GL.order;
            String wherestr = GL.Where;
            SqlClass sqlclass = new SqlClass();
            SqlClass.Select select = new SqlClass.Select("Mail_T");
            SqlClass.Where where = new SqlClass.Where();
            SqlClass.Order orderby = new SqlClass.Order();
            int count = 0;
            if (BoxId == 0)
            {
                String boxidlist = "(";
                //}//获取能全部邮件邮箱  
                boxidlist += Base.GetUseMailBox(GL);
                if (boxidlist.Length > 0)
                {
                    boxidlist += ")";
                    where.And("[dbo].[Mail_T].mailboxid in " + boxidlist);
                }

            }
            else
            {
                if (!Base.IsUseMailBox(GL, BoxId))//不能查看
                {
                    // return Tools.Back.ToJson("", Tools.BackCode.NotinBox);//返回错误
                    BP.code = BackCode.NotinBox;
                    BP.back = BackCode.CodeStr[BP.code];
                    return BP;
                }
                else//可以参看
                {
                    where.And("[dbo].[Mail_T].mailboxid=" + BoxId);
                    // ma = db.Mail_T.Where(m => m.MailBoxId == BoxId);//筛选邮件
                }
            }
            String sql = "";
            string MAIL_SORT = db.Database.SqlQuery<string>("select isnull(Setting,'') from [dbo].UserConfig_T where UserId='" + UserId + "' and ParamName='MAIL_SORT'").FirstOrDefault();
            switch (MAIL_SORT)
            {
                case "按邮件日期":
                    MAIL_SORT = "RecDate desc";
                    break;
                case "按存储顺序":
                    MAIL_SORT = "Id desc";
                    break;
                default:
                    MAIL_SORT = "RecDate desc";
                    break;
            }
            if (wherestr == null)
            {
                switch (Act.ToUpper())//数据属性
                {
                    case "ALL":
                        // ma = ma.Where(mw => mw.Box != "CGX" && mw.Box != "LJX" && mw.Box != "" && mw.Box != "YSC");
                        where.And("Box <>'CGX'");
                        where.And("Box <>'LJX'");
                        where.And("Box <>'YSC'");
                        break;
                    case "ZDYJ":
                        // ma = ma.Where(mw => mw.TopTime != null && mw.Box != "LJX" && mw.Box != "" && mw.Box != "YSC" && mw.Box != "CGX");
                        where.And("TopTime is not null");
                        where.And("Box <>'CGX'");
                        where.And("Box <>'LJX'");
                        where.And("Box <>'YSC'");
                        //where.And("Box <>''");
                        break;
                    case "XBKH":   //星标邮件
                        // ma = ma.Where(mw => mw.star == "1" && mw.Box != "LJX" && mw.Box != "" && mw.Box != "YSC" && mw.Box != "CGX");
                        where.And("star = 1");
                        where.And("Box <>'CGX'");
                        where.And("Box <>'LJX'");
                        where.And("Box <>'YSC'");
                       // where.And("Box <>''");
                        break;
                    case "NB": //内部邮件
                        // ma = ma.Where(mw => mw.Inside == 1 && mw.Box != "LJX" && mw.Box != "" && mw.Box != "YSC" && mw.Box != "CGX");
                        where.And("Inside = 1");
                        where.And("Box <>'CGX'");
                        where.And("Box <>'LJX'");
                        where.And("Box <>'YSC'");
                     //   where.And("Box <>''");
                        break;
                    case "XP": //询盘邮件
                        // ma = ma.Where(mw => mw.Inq == 1 && mw.Box != "LJX" && mw.Box != "" && mw.Box != "YSC" && mw.Box != "CGX");
                        select.SetTable("[dbo].[MailInq_T],[dbo].[Mail_T]");
                        where.And("Inq = 1");
                        where.And("Box <>'CGX'");
                        where.And("Box <>'LJX'");
                        where.And("Box <>'YSC'");
                        where.And("[dbo].[Mail_T].id=[dbo].[MailInq_T].mailid");
                        break;
                    case "DGZ":
                        // ma = ma.Where(mw => (mw.RemoteTrack == 1 || mw.LocalTrack != -1) && mw.Box != "LJX" && mw.Box != "" && mw.Box != "YSC" && mw.Box != "CGX");
                        where.And("isnull(LocalTrack,-1)<>-1");
                        where.And("Box <>'CGX'");
                        where.And("Box <>'LJX'");
                        where.And("Box <>'YSC'");
                       // where.And("Box <>''");
                        break;

                    case "YTX":
                        //ma = ma.Where(mw => mw.RemindTime != null && mw.Box != "LJX" && mw.Box != "" && mw.Box != "YSC" && mw.Box != "CGX");
                        where.And("Id in (select ExId from " + dbname + ".dbo.Pending_T where Start_date<getdate() and MenuNo='AA')");
                      //  where.And("RemindTime is null");
                        where.And("Box <>'CGX'");
                        where.And("Box <>'LJX'");
                        where.And("Box <>'YSC'");
                      //  where.And("Box <>''");
                        break;
                    case "HQYJ":
                        // ma = ma.Where(mw => mw.redflag == "1" && mw.Box != "LJX" && mw.Box != "" && mw.Box != "YSC" && mw.Box != "CGX");
                        where.And("redflag='1'");
                        where.And("Box <>'CGX'");
                        where.And("Box <>'LJX'");
                        where.And("Box <>'YSC'");
                       // where.And("Box <>''");
                        break;
                    case "BQYJ":
                        // ma = ma.Where(mw => mw.MailLabel.Contains("maillabel-") && mw.Box != "LJX" && mw.Box != "" && mw.Box != "YSC" && mw.Box != "CGX");
                        where.And("isnull(MailLabel,'') <> ''");
                        where.And("Box <>'YSC'");
                        where.And("Box <>'LJX'");
                        where.And("Box <>'CGX'");
                        //where.And("Box <>''");
                        break;
                    case "LJX": //查找垃圾箱
                        where.And("Box ='LJX'");
                        break;
                    case "DTL": //待处理 
                        where.And("IsDeal is null");
                        where.And("Box <>'CGX'");
                        where.And("Box <>'LJX'");
                        where.And("Box <>'YSC'");
                        where.And("(BoxBase='SJX' OR Box='SJX')");
                        break;
                    case "DTL2": //客户待处理
                        select.Addtablejoin(" left join [dbo].[MailClient_T] on id=clientmailid");
                        where.And("IsDeal is null");
                        where.And("(BoxBase='SJX' OR Box='SJX')");
                        where.And("Box <>'CGX'");
                        where.And("Box <>'LJX'");
                        where.And("Box <>'YSC'");
                        where.And("ClientId is not null");
                        break;
                    case "DTL1": //陌生人待处理
                        select.Addtablejoin(" left join [dbo].[MailClient_T] on id=clientmailid");
                        where.And("IsDeal is null");
                        where.And("(BoxBase='SJX' OR Box='SJX')");
                        where.And("Box <>'CGX'");
                        where.And("Box <>'LJX'");
                        where.And("Box <>'YSC'");
                        where.And("ClientId is null");
                        break;
                    case "CACHEMAIL": //获取所有邮件
                        break;

                    case "YFPXP": //已分配询盘
                        select.SetTable("[dbo].[MailInq_T],[dbo].[Mail_T]");
                        where.And("InqYfp = 1");
                        where.And("Box <>'CGX'");
                        where.And("Box <>'LJX'");
                        where.And("Box <>'YSC'");
                        where.And("[dbo].[Mail_T].id=[dbo].[MailInq_T].mailid");
                        break;
                    case "DSP": //待审批邮件
                        where.And("SPTime is null");
                        where.And("SPUserId is not null");
                        where.And("SUID=" + uid + "");
                        break;
                    case "YSS": //已送审邮件
                        where.And("SPUserId is not null");
                        where.And("SUID = " + uid + "");
                        break;
                    case "BH": //驳回邮件
                        where.And("SPUserId is not null");
                        where.And("ischeck=0");
                        where.And("SUID = " + uid + "");
                        break;
                    case "DTX":  //待提醒
                        where.And("Id in (select ExId from "+dbname+".dbo.Pending_T where Start_date>getdate() and MenuNo='AA')");
                        where.And("Box <>'CGX'");
                        where.And("Box <>'LJX'");
                        where.And("Box <>'YSC'");
                      //  where.And("Box <>''");
                        break;
                    case "DFF": //待分发邮件
                        string strMyMailBoxId = db.Database.SqlQuery<string>("select dbo.GetFFMailBoxIdByUserId_F('" + UserId + "')").FirstOrDefault();
                        if (strMyMailBoxId == "") strMyMailBoxId = "0";
                        where.And("Box in ('SJX')");
                        where.And("MailBoxId in (" + strMyMailBoxId + ")");
                        where.And("isnull(UID,'')=''");
                        break;
                    case "SJX": //收件箱              
                        where.And("Box ='SJX'");
                        where.And("RootId is null");
                        break;
                    case "ALLR": //收件箱  
                        where.And("Box <>'CGX'");
                        where.And("Box <>'LJX'");
                        where.And(" Box <>'YSC'");
                        where.And("Box ='SJX'");
                        
                        break;
                    case "YFS": //已发送   
                        where.And("Box ='YFS'");
                        where.And("RootId is null");
                        break;
                    case "ALLS": //已发送        
                        where.And("Box ='YFS'");
                        break;
                    case "CGX": //草稿箱   
                        where.And("Box ='CGX'");
                        where.And("RootId is null");
                        break;
                    case "YSC": //已删除              
                        where.And("Box ='YSC'");
                        where.And("RootId is null");
                        break;
                    case "CLIENT":
                        select.Addtablejoin(" left join [dbo].[MailClient_T] on id=clientmailid");
                        where.And("clientid="+GL.clientid+"");
                        break;
                    default:
                        if (Act.IndexOf("WJJ") >= 0)
                        {
                            int wjjid = 0;
                            try { wjjid = int.Parse(Act.Replace("WJJ", "")); }
                            catch (Exception e) { }
                            //ma = ma.Where(mw => mw.RootId == wjjid);
                            where.And("RootId=" + wjjid);
                        }
                        else if (Act.IndexOf("LBL") >= 0)
                        {
                            String label = "";
                            label = Act.Replace("LBL", "");
                            // ma = ma.Where(mw => mw.MailLabel.Contains(label));
                            where.And("MailLabel like '%" + label + "%'");
                        }
                        else
                        {
                            //ma = ma.Where(mw => mw.Box == Act && mw.RootId == null);
                            where.And("Box = '" + Act + "'");
                            where.And("RootId is null");
                        }
                        break;
                }
                if (order == null || order.Length == 0)
                {
                    orderby.Desc("TopTime");
                    orderby.Add(MAIL_SORT);
                }
                else
                {
                    switch (order.ToUpper())
                    {
                        case "ALL":
                            orderby.Desc("TopTime");
                            orderby.Desc("recDate");
                            break;
                        default:
                            orderby.Add(order);
                            break;
                    }
                }
            }
            else
            {
                where.And(wherestr);
                // sql = (new Select(new Models.Mail.Back.item.Mail(), "Mail_T")).Get() + where.ToWhere();
                if (order == null || order.Length == 0)
                {
                    orderby.Desc("TopTime");
                    orderby.Add(MAIL_SORT);
                }
                else
                {
                    switch (order.ToUpper())
                    {
                        case "ALL":
                            orderby.Desc("TopTime");
                            orderby.Desc("recDate");
                            break;
                        default:
                            orderby.Add(order);
                            break;
                    }
                }
            }
            if (GL.Key != null && GL.Key.Length > 0)
            {
                where.Like("FromName,FromEmail,Subject,itFrom,itto,MailLabel", GL.Key);
            }

            try
            {
                count = dbm.Database.SqlQuery<int>(sqlclass[select.Count("count"), where, new SqlClass.Order()]).FirstOrDefault();
                if (PageIndex < 1)
                {
                    BP.code = "-1";
                    BP.back = "分页超出范围";
                    return BP;
                }
                if (Act.ToUpper() != "CLIENT" && Act.ToUpper() != "DTL1" && Act.ToUpper() != "DTL2")
                    select.Addtablejoin(" left join [dbo].[MailClient_T] on id=clientmailid");
                if (Act == null || Act.ToUpper() != "CACHEMAIL")
                {
                    select.Add(new Models.Mail.Back.item.Mail());

                    sql = sqlclass.Page(PageIndex, PageMax, select, where, orderby); // SqlClass[(new SqlClass.Select()).Add(new Models.Mail.Back.item.Mail()),where,new SqlClass.Order];
                    //数据分页

                    IEnumerable<Models.Mail.Back.item.Mail> BiM = dbm.Database.SqlQuery<Models.Mail.Back.item.Mail>(sql, "");
                    where.And("[Read]='否'");
                    sql = sqlclass[select.Count("unread"), where, new SqlClass.Order()];
                    int unread = dbm.Database.SqlQuery<int>(sql).FirstOrDefault();
                    Models.Mail.Back.GetList BGL = new Models.Mail.Back.GetList();
                    BGL.count = count;
                    BGL.index = PageIndex;
                    BGL.unread = unread;
                    BGL.list = BiM;
                    BP.back = BGL;
                    stopwatch.Stop(); //  停止监视
                    TimeSpan timeSpan = stopwatch.Elapsed; //  获取总时间
                    double hours = timeSpan.TotalHours; // 小时
                    double minutes = timeSpan.TotalMinutes;  // 分钟
                    double seconds = timeSpan.TotalSeconds;  //  秒数
                    double milliseconds = timeSpan.TotalMilliseconds;  //  毫秒数
                    return BP;
                }
                else
                {
                    select.Add(new Models.Mail.Back.item.MailAll());
                    // select.Add("[dbo].[Mail_T].[Id] as [Id],[dbo].[Mail_T].[MailBoxId] as [MailBoxId],[FromName],[FromEmail],[itFrom],[itTo],[Subject],[MailDate],[RecDate],[SendDate],[CreateTime],[MailLabel],[cc],[bcc],[ReplyTo],[MailType],[Read],[priority],[RE],[MailSize],[RevUserId],[RevUserName],[Bak],[IP],[Area],[star],[redflag],[BoxBase],[Box],[RootId],[TopTime],[ReadMode],[AccCount],[guid],[UIDL],[Inside],[TextBody],[HtmlBody],[Notification],[notify],[UID],[ischeck],[checkmsg],[SPUserId],[SPUserName],[SPTime],[SUID],[BaseDb],[BaseId],[REALFROM],[ReturnPath],[ConType],[Type],[RFMailId],[LocalTrack],[RemoteTrack],[IsReceipt],[ModId],[RemindTime],[PfDate],[IsDeal]");
                    // select.Add("[FileId],[FileName],[FileSize]");
                    // select.SetTable("Mail_T,MailAcc_T");y
                    // where.And("[dbo].[Mail_T].[Id]=[dbo].[MailAcc_T].[mailid]");
                    sql = sqlclass.Page(PageIndex, PageMax, select, where, orderby); // SqlClass[(new SqlClass.Select()).Add(new Models.Mail.Back.item.Mail()),where,new SqlClass.Order];
                    //数据分页
                    IEnumerable<Models.Mail.Back.item.MailAll> BiM = dbm.Database.SqlQuery<Models.Mail.Back.item.MailAll>(sql, "");
                    string mailidlist = "";
                    foreach (Models.Mail.Back.item.MailAll BiMson in BiM)
                    {
                        if (mailidlist == "")
                            mailidlist = "mailid=" + BiMson.Id + " ";
                        else
                            mailidlist += " or mailid=" + BiMson.Id + " ";
                    }
                    string sql2 = sqlclass.Page(PageIndex, PageMax, "Select mailid,id=FileId,name=FileName,size=FileSize,ROW_NUMBER() OVER ( Order By mailid) AS RowNumber from MailAcc_T", " where DownloadId!='' and DownloadId is not null and mailid is not null and (" + mailidlist + ")", "");
                    IEnumerable<Models.Mail.Back.item.Fileall> BiF = dbm.Database.SqlQuery<Models.Mail.Back.item.Fileall>(sql2);
                    where.And("[Read]='否'");
                    sql = sqlclass[select.Count("unread"), where, new SqlClass.Order()];
                    int unread = dbm.Database.SqlQuery<int>(sql).FirstOrDefault();
                    Models.Mail.Back.GetListAll BGL = new Models.Mail.Back.GetListAll();
                    BGL.count = count;
                    BGL.index = PageIndex;
                    BGL.unread = unread;
                    BGL.list = BiM;
                    BGL.listfile = BiF;
                    BP.back = BGL;
                    stopwatch.Stop(); //  停止监视
                    TimeSpan timeSpan = stopwatch.Elapsed; //  获取总时间
                    double hours = timeSpan.TotalHours; // 小时
                    double minutes = timeSpan.TotalMinutes;  // 分钟
                    double seconds = timeSpan.TotalSeconds;  //  秒数
                    double milliseconds = timeSpan.TotalMilliseconds;  //  毫秒数
                    return BP;
                }



            }
            catch (Exception ex)
            {
                BP.code = "xxxx";
                BP.back = ex.ToString();
                return BP;
                throw;
            }
        }
        [Check]
        [Client]
        [HttpPost]
        public Models.BackParameter Client([FromBody]Models.Mail.Gain.Client C, [FromUri]String Operation)
        {
            string sql = string.Format("select ContactIdList= '<' +  Email + '>' from ClientAdd_T where Email!='' and Email is not null and clientid={0}", C.clientid);
            sql += string.Format(" union select ContactIdList='<' +  Email1 + '>' from ClientAdd_T where Email1!='' and Email1 is not null and clientid={0}", C.clientid);
            sql += string.Format(" union select ContactIdList='<' +  Email2 + '>' from ClientAdd_T where Email2!='' and Email2 is not null and clientid={0}", C.clientid);
            List<string> MailBoxList = db.Database.SqlQuery<string>(sql).ToList();
            Tools.Where where = new Where();
            where.And(C.Where);
            string o = OrderBy.Desc(C.OrderBy);
            IEnumerable<Models.Mail.Back.item.Mail> BiM = null;
            switch (Operation.ToLower())
            {
                case "itto":
                    BiM = dbm.Database.SqlQuery<Models.Mail.Back.item.Mail>((new Select(new Models.Mail.Back.item.Mail(), "Mail_T")).Get() + string.Format(" where (itto like '%{0}%') {1} order by {2}", string.Join("%' OR itto Like '%", MailBoxList), C.Where == null ? "" : "and " + where.ToWhere(), o));
                    break;
                case "from":
                    BiM = dbm.Database.SqlQuery<Models.Mail.Back.item.Mail>((new Select(new Models.Mail.Back.item.Mail(), "Mail_T")).Get() + string.Format(" where (itFrom like '%{0}%') {1} order by {2}", string.Join("%' OR itfrom Like '%", MailBoxList, C.Where == null ? "" : "and " + where.ToWhere(), o)));
                    break;
                case "all":
                    BiM = dbm.Database.SqlQuery<Models.Mail.Back.item.Mail>((new Select(new Models.Mail.Back.item.Mail(), "Mail_T")).Get() + string.Format(" where (itto like '%{0}%' Or itFrom like '%{1}%') {2} order by {3}", string.Join("%' OR itto Like '%", MailBoxList), string.Join("%' OR itfrom Like '%", MailBoxList), C.Where == null ? "" : "and " + where.ToWhere(), o));
                    break;
            }
            int count = BiM.Count();
            //数据分页
            BiM = BiM.Skip((C.PageIndex - 1) * C.PageMax).Take(C.PageMax);
            Models.Mail.Back.GetList BGL = new Models.Mail.Back.GetList();
            BGL.count = count;
            BGL.index = (C.PageIndex);
            BGL.list = BiM;
            BP.back = BGL;
            return BP;
        }
        [Check]
        [Contact]
        [HttpPost]
        public Models.BackParameter Contact([FromBody]Models.Mail.Gain.Contact C, [FromUri]String Operation)
        {
            string sql = string.Format("select ContactIdList= '<' +  Email + '>' from ClientAdd_T where Email!='' and Email is not null and id={0}", C.ContactId);
            sql += string.Format(" union select ContactIdList='<' +  Email1 + '>' from ClientAdd_T where Email1!='' and Email1 is not null and id={0}", C.ContactId);
            sql += string.Format(" union select ContactIdList='<' +  Email2 + '>' from ClientAdd_T where Email2!='' and Email2 is not null and id={0}", C.ContactId);
            List<string> MailBoxList = db.Database.SqlQuery<string>(sql).ToList();
            Tools.Where where = new Where();
            where.And(C.Where);
            string o = OrderBy.Desc(C.OrderBy);
            IEnumerable<Models.Mail.Back.item.Mail> BiM = null;
            switch (Operation.ToLower())
            {
                case "itto":
                    BiM = dbm.Database.SqlQuery<Models.Mail.Back.item.Mail>((new Select(new Models.Mail.Back.item.Mail(), "Mail_T")).Get() + string.Format(" where (itto like '%{0}%') {1} order by {2}", string.Join("%' OR itto Like '%", MailBoxList), C.Where == null ? "" : "and " + where.ToWhere(), o));
                    break;
                case "from":
                    BiM = dbm.Database.SqlQuery<Models.Mail.Back.item.Mail>((new Select(new Models.Mail.Back.item.Mail(), "Mail_T")).Get() + string.Format(" where (itFrom like '%{0}%') {1} order by {2}", string.Join("%' OR itfrom Like '%", MailBoxList, C.Where == null ? "" : "and " + where.ToWhere(), o)));
                    break;
                case "all":
                    BiM = dbm.Database.SqlQuery<Models.Mail.Back.item.Mail>((new Select(new Models.Mail.Back.item.Mail(), "Mail_T")).Get() + string.Format(" where (itto like '%{0}%' Or itFrom like '%{1}%') {2} order by {3}", string.Join("%' OR itto Like '%", MailBoxList), string.Join("%' OR itfrom Like '%", MailBoxList), C.Where == null ? "" : "and " + where.ToWhere(), o));
                    break;
            }
            int count = BiM.Count();
            //数据分页
            BiM = BiM.Skip((C.PageIndex - 1) * C.PageMax).Take(C.PageMax);
            Models.Mail.Back.GetList BGL = new Models.Mail.Back.GetList();
            BGL.count = count;
            BGL.index = (C.PageIndex);
            BGL.list = BiM;
            BP.back = BGL;
            return BP;
        }
        [Check]
        [Mail]
        [HttpPost]
        public Models.BackParameter Delete([FromBody]Models.Mail.Gain.Delete D)
        {
            dbm.Database.ExecuteSqlCommand(String.Format("Delete Mail_t Where id in ({0})", D.MailId), "");
            BP.back = "删除成功";
            return BP;
        }
        [Check]
        [Mail("Id")]
        [FileCreater]
        [HttpPost]
        public Models.BackParameter Update([FromBody]Models.Mail.Gain.Update S)
        {
            try
            {
                SqlClass sql = new Tools.SqlClass();
                SqlClass.Update udm = new SqlClass.Update("Mail_T");
                SqlClass.Where wm = new SqlClass.Where();
                udm.Revise(S);
                udm.Revise("RecDate", DateTime.Now);
                udm.Remove(new Models.GainParameter());
                udm.Remove("Fileid");
                udm.Remove("id");
                wm.And("id=" + S.Id);
                dbm.Database.ExecuteSqlCommand(sql[udm, wm]);
                if (S.Fileid != null && S.Fileid.Length > 0)
                {
                    SqlClass.Update udma = new SqlClass.Update("MailAcc_T");
                    SqlClass.Where wma = new SqlClass.Where();
                    udma.Revise("mailboxid", (int)S.MailBoxId);
                    udma.Revise("mailid", (int)S.Id);
                    wma.And("fileid in (" + S.Fileid + ")");
                    dbm.Database.ExecuteSqlCommand(sql[udma, wma]);
                }
                BP.back = "修改成功";
                return BP;
            }
            catch (Exception e)
            {
                BP.back = e.Message;
                BP.code = Tools.BackCode.Err;
                return BP;
            }

        }
        [Check]
        [HttpPost]
        public Models.BackParameter Add([FromBody]Models.Mail.Gain.Add A)
        {
            //Mail_T m = new Mail_T();
            //m.MailBoxId = A.MailBoxId;
            //m.ReadMode = A.ReadMode;
            //m.Inside = A.Inside;
            //m.FromName = A.FromName;
            //m.FromEmail = A.FromEmail;
            //m.Subject = A.Subject;
            //m.SendDate =DateTime.Parse(A.SendDate).GetDateTimeFormats()[140];
            //m.MailType = A.MailType;
            //m.itFrom = A.itFrom;
            //m.itTo = A.itTo;
            //m.cc = A.cc;
            //m.bcc = A.bcc;
            //m.TextBody = A.TextBody;
            //m.HtmlBody = A.HtmlBody;
            //m.ReplyTo = A.ReplyTo;
            //m.Read = A.Read;
            //m.ReadUID = A.ReadUID;
            //m.BoxBase = A.BoxBase;
            //m.Box = A.Box;
            //m.priority = A.priority;
            //m.star = A.star;
            //m.redflag = A.redflag;
            //m.UID = A.UID;
            //m.SUID = A.SUID;
            //m.MailDate = DateTime.Parse(A.MailDate).GetDateTimeFormats()[140];
            //m.RevUserId = A.RevUserId;
            //m.RevUserName = A.RevUserName;
            //m.MailLabel = A.MailLabel;
            //m.Type = A.Type;
            //m.RFMailId = A.RFMailId;
            //m.LocalTrack = A.LocalTrack;
            //m.RemoteTrack = A.RemoteTrack;
            //m.IsReceipt = A.IsReceipt;
            //m.ModId = A.ModId;
            //m.RecDate = DateTime.Now;
            //m.CreateTime = DateTime.Now;
            SqlClass.Insert In = new SqlClass.Insert("Mail_T");
            In.Revise(A);
            In.Remove(new Models.GainParameter());
            In.Remove("fileid");
            In.Revise("RecDate", DateTime.Now);
            In.Revise("CreateTime", DateTime.Now);
            Decimal Id = dbm.Database.SqlQuery<Decimal>(In.ToString()).FirstOrDefault();
            dbm.Database.ExecuteSqlCommand("update Mail_T set MailSize=dbo.GetMailSize_F(Id),AccCount=dbo.GetAccCount_F(" + Id + ")  where Id=" + Id);
            SqlClass sql = new SqlClass();
            if (A.Fileid != null && A.Fileid.Length > 0)
            {
                SqlClass.Update udma = new SqlClass.Update("MailAcc_T");
                SqlClass.Where wma = new SqlClass.Where();
                udma.Revise("mailboxid", (int)A.MailBoxId);
                udma.Revise("mailid", (int)Id);
                wma.And("id in (" + A.Fileid + ")");
                dbm.Database.ExecuteSqlCommand(sql[udma, wma]);
            }
            BP.back = new Models.Mail.Back.Add() { id = (int)Id, msg = "新建邮件成功" };
            return BP;
        }
        [Check]
        [Mail]
        [HttpPost]
        public Models.BackParameter Star([FromBody]Models.Mail.Gain.Star S)
        {
            dbm.Database.ExecuteSqlCommand(String.Format("Update Mail_t Set Star={0} Where id in ({1})", S.Is ? "'1'" : "null", S.MailId), "");
            BP.back = (S.Is ? "标记" : "取消") + "星标成功";
            return BP;
        }
        [Check]
        [Mail]
        [HttpPost]
        public Models.BackParameter RedFlag([FromBody]Models.Mail.Gain.RedFlag RF)
        {
            dbm.Database.ExecuteSqlCommand(String.Format("Update Mail_t Set RedFlag={0} Where id in ({1})", RF.Is ? "'1'" : "null", RF.MailId), "");
            BP.back = (RF.Is ? "标记" : "取消") + "红旗成功";
            return BP;
        }
        [Check]
        [Mail]
        [HttpPost]
        public Models.BackParameter Top([FromBody]Models.Mail.Gain.Top T)
        {
            dbm.Database.ExecuteSqlCommand(String.Format("Update Mail_t Set TopTime={0} Where id in ({1})", T.Is ? "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" : "null", T.MailId), "");
            BP.back = (T.Is ? "标记" : "取消") + "置顶成功";
            return BP;
        }
        [Check]
        [Mail]
        [HttpPost]
        public Models.BackParameter Read([FromBody]Models.Mail.Gain.Read R)
        {
            dbm.Database.ExecuteSqlCommand(String.Format("Update Mail_t Set [Read]='{0}' Where id in ({1})", R.Is ? "是" : "否", R.MailId), "");
            BP.back = (R.Is ? "标记" : "取消") + "已读成功";
            return BP;
        }
        [Check]
        [HttpPost]
        public Models.BackParameter Move([FromBody]Models.Mail.Gain.Move M)
        {
            dbm.Database.ExecuteSqlCommand(string.Format("update mail_t set box='{0}',rootid={1},mailboxid={2} where id in ({3})", M.Box, M.RootId ?? "null", M.MailBoxId, M.MailId));
            BP.back = "移动成功";
            return BP;
        }
        [Check]
        [Mail]
        [HttpPost]
        public Models.BackParameter Send([FromBody]Models.Mail.Gain.Send S)
        {
            string strMDB = ConfigurationManager.AppSettings["dbname"] + "MAIL";
            TCRMCLASS.DBClass db1 = new TCRMCLASS.DBClass();
            string MailId = S.MailId;
            DataSet ds = db1.getDataSetBySql("select MailBoxId,IsReceipt from " + strMDB + ".dbo.Mail_T where Id=" + MailId);
            TCRMCLASS.TDataSet tds = new TCRMCLASS.TDataSet(ds);
            tds.read();
            string BoxId = tds["MailBoxId"].ToString();
            db1.CloseConn();
            bool Notification = (tds["IsReceipt"].ToString() == "1");
            bool blBack = (S.Check != null);
            TCRMCLASS.Mail.SendWork SW = null;
            string key = "";
            SW = new TCRMCLASS.Mail.SendWork()
            {
                strMailBoxId = BoxId,
                strMailId = MailId,
                strNotify = Notification,
                blBack = blBack,
                strUserId = S.UserId,
            };

            SW.runwork();
            key = OperationList.AddSend(S, SW);
            int wState = 0;
            lock (SW)
            {
                wState = SW.State;
            }
            Models.Mail.Back.Send BS = new Models.Mail.Back.Send()
            {
                state = wState.ToString(),
                key = key,
                msg = SW.strErrMessage
            };
            if (wState == 2 || wState == 3 || wState == 4)
                OperationList.Remove(S, key);
            BP.back = BS;
            return BP;
        }
        [Check]
        [MailBox]
        [HttpPost]
        public Models.BackParameter Recv([FromBody]Models.Mail.Gain.Recv R)
        {

            RecvObj RO = OperationList.AddRecv(R, R.MailBoxId);
            int wState = 0;
            lock (RO.RW)
            {
                wState = RO.RW.State;
            }
            Models.Mail.Back.Recv BR = new Models.Mail.Back.Recv()
            {
                state = wState.ToString(),
                key = RO.key,
                msg = RO.RW.strMessage,
                errmsg = RO.RW.strErrMessage,
                count = RO.RW.intCount,
                rec = RO.RW.intRec,
                err = RO.RW.intErr
            };
            if (wState == 2 || wState == 3 || wState == 4)
                OperationList.Remove(R, RO.key);
            BP.back = BR;
            return BP;
        }
        [Check]
        [HttpPost]
        public Models.BackParameter State([FromBody]Models.Mail.Gain.State S)
        {
            object W = OperationList.Get(S, S.Key);
            int wState = 0;
            lock (W)
            {
                if (W.GetType().Name == "SendWork")
                {
                    wState = ((TCRMCLASS.Mail.SendWork)W).State;
                    Models.Mail.Back.Send BS = new Models.Mail.Back.Send()
                    {
                        state = wState.ToString(),
                        key = S.Key,
                        msg = ((TCRMCLASS.Mail.SendWork)W).strErrMessage
                    };
                    BP.back = BS;
                }
                else if (W.GetType().Name == "RecvWork")
                {
                    wState = ((TCRMCLASS.Mail.RecvWork)W).State;
                    Models.Mail.Back.Recv BR = new Models.Mail.Back.Recv()
                    {
                        state = wState.ToString(),
                        key = S.Key,
                        count = ((TCRMCLASS.Mail.RecvWork)W).intCount,
                        rec = ((TCRMCLASS.Mail.RecvWork)W).intRec,
                        err = ((TCRMCLASS.Mail.RecvWork)W).intErr,
                        msg = ((TCRMCLASS.Mail.RecvWork)W).strMessage,
                        errmsg = ((TCRMCLASS.Mail.RecvWork)W).strErrMessage
                    };
                    BP.back = BR;
                }
            }
            if (wState == 2 || wState == 3 || wState == 4)
                OperationList.Remove(S, S.Key);
            return BP;
        }
        [Check]
        [HttpPost]
        public Models.BackParameter StopRecv([FromBody]Models.Mail.Gain.StopRecv S)
        {
            try
            {
                if (Tools.OperationList.Stop(S, S.key))
                {
                    BP.back = "停止成功";
                }
                else
                {
                    BP.code = BackCode.RecvStopErr;
                    BP.back = BackCode.CodeStr[BP.code];
                }
            }
            catch
            {
                BP.code = BackCode.RecvStopErr;
                BP.back = BackCode.CodeStr[BP.code];
            }
            return BP;
        }
        [Check]
        [FileCreater]
        [HttpPost]
        public Models.BackParameter Addfolder([FromBody]Models.Mail.Gain.floder A)
        {
            if (A.parentid == null)
            {
                BP.back = "请输入父级文件夹id";
                return BP;
            }
            if (A.floderName == null || A.floderName == "")
            {
                BP.back = "请输入文件夹名字";
                return BP;
            }
            try
            {
                string prenidstr = dbm.Database.SqlQuery<string>("select parentid from tcrm.dbo.MailBoxRoot_T where id=" + A.parentid + "").FirstOrDefault();
                if (prenidstr == null) prenidstr = A.parentid;
                string[] parentids = prenidstr.Split('_');
                string maiboxid = "";
                string mailparentid = "";
                if (parentids.Length == 1)
                {
                    //int maiboxidpd = dbm.Database.SqlQuery<int>("select id from tcrm.dbo.MailBox_T where creater='" + A.UserId + "' and id="+A.parentid+"").FirstOrDefault();
                    //if (maiboxidpd == 0)
                    //{
                    //    BP.back = "无权在该文件夹下创建目录";
                    //    return BP;
                    //}
                    maiboxid = parentids[0];
                    mailparentid = "m_" + maiboxid;
                    if (A.sysfolder != "" || A.sysfolder != null)
                    {
                        if (A.sysfolder == "SJX" || A.sysfolder == "CGX" || A.sysfolder == "YFS" || A.sysfolder == "LJX" || A.sysfolder == "SSC")
                            mailparentid = "b_" + maiboxid + "_" + A.sysfolder;
                    }
                }
                if (parentids.Length >= 2)
                {
                    maiboxid = parentids[1];
                    mailparentid = "f_" + maiboxid + "_" + A.parentid;
                }
                dbm.Database.ExecuteSqlCommand(" insert into  tcrm.dbo.MailBoxRoot_T (mailboxid,parentid,name,sort)values(" + maiboxid + ",'" + mailparentid + "','" + A.floderName + "',(select top 1 sort from tcrm.dbo.MailBoxRoot_T  order by id desc)+1)");
                BP.back = "新建文件夹成功";
                return BP;
            }
            catch (Exception ee)
            {
                BP.back = ee.ToString();
                return BP;
            }
        }
        [Check]
        [HttpPost]
        public Models.BackParameter getlabel([FromBody]Models.Mail.Gain.setnoet G)
        {


            string MailId = G.MailId;
            string bak = G.bak;
            try
            {
                IEnumerable<Models.Mail.Back.item.getlabel> BiM = db.Database.SqlQuery<Models.Mail.Back.item.getlabel>("select name,class from MailLabel_T where userid='' or  userid='" + G.UserId + "'", "");
                BP.back = BiM;
            }
            catch (Exception ee)
            {
                string BG = "";
                BG = ee.ToString();
                BP.back = BG;
            }

            return BP;
        }
        [Check]
        [HttpPost]
        public Models.BackParameter ISCL([FromBody]Models.Mail.Gain.Get G)
        {

            try
            {
                Nullable<int> BiM = dbm.Database.SqlQuery<Nullable<int>>("select isdeal from Mail_T where id=" + G.MailId + "", "").FirstOrDefault();
                 if (BiM==null||BiM==0)
                {
                    if(dbm.Database.ExecuteSqlCommand("update Mail_T set isdeal=1  where id=" + G.MailId + "")==1)
                         BP.back = G.MailId+"  修改成功";
                    else
                        BP.back = G.MailId + "  不存在";
                }
                else
                    BP.back = G.MailId + "  已处理";
            }
            catch (Exception ee)
            {
                string BG = "";
                BG = ee.ToString();
                BP.back = BG;
            }

            return BP;
        }
        [Check]
        [HttpPost]
        public Models.BackParameter GetMb([FromBody]Models.Mail.Gain.Get G)
        {
            try
            {
                IEnumerable<Models.Mail.Back.GetMb> BiM = db.Database.SqlQuery<Models.Mail.Back.GetMb>("select * from [dbo].[MailMod_T] where sys=1 or userid='" + G.UserId + "'");
                List<Models.Mail.Back.GetMb> bimz = new List<Models.Mail.Back.GetMb>();
                foreach (Models.Mail.Back.GetMb bimson in BiM)
                {
                    try
                    {
                        IEnumerable<File_T> filelist = db.Database.SqlQuery<File_T>("select * from File_T where DownloadID is not null and tablename='mailmod_t' and KeyId=" + bimson.id);
                        foreach (File_T file in filelist)
                        {
                            byte[] wj = dbf.Database.SqlQuery<byte[]>("select Data from FileData_T where Id=" + file.FileId).FirstOrDefault();
                            //Tools.FileBinaryConvertHelper.BytestoFile(wj,System.AppDomain.CurrentDomain.BaseDirectory + @"file\"+file.FileName);
                            bimson.Html = bimson.Html.Replace(String.Format("cid:{0}", file.DownloadID), String.Format("data:image/{0};base64,", Base.imgformat[file.FileName.Substring(file.FileName.LastIndexOf(".") + 1).ToLower()]) + Convert.ToBase64String(wj));
                        }
                    }
                    catch(Exception ee)
                    {

                    }
                    bimz.Add(bimson);
                }
                BP.back = bimz;
            }
            catch (Exception ee)
            {
                string BG = "";
                BG = ee.ToString();
                BP.back = BG;
            }

            return BP;

        }

        //[Check]
        //[HttpPost]
        //public Models.BackParameter Setnote([FromBody]Models.Mail.Gain.setnoet G)
        //{


        //    string MailId = G.MailId;
        //    string bak = G.bak;
        //    string BG = "";
        //    try
        //    {
        //        dbm.Database.ExecuteSqlCommand("update Mail_T set bak='" + G.bak + "'  where Id=" + MailId);
        //        BG="备注修改成功";
        //    }
        //    catch(Exception ee)
        //    {

        //        BG=ee.ToString();
        //    }
        //    BP.back = BG;
        //    return BP;
        //}
    }
}
