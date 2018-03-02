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
    public class ClientController :BaseControllers
    {
        [Check]
        [HttpPost]
        public Models.BackParameter Add([FromBody]Models.Client.Gain.Add A)
        {
            Client_T C = A.ToClient();
            db.Client_T.Add(C);
            db.SaveChanges();
         
            string newNo = db.Database.SqlQuery<string>("exec GetAutoCodeEx 'AB'," + C.Id + ",'" + A.UserId+ "',1").FirstOrDefault();
            BP.back = new Models.Client.Back.Add()
            {
                clientid = C.Id,
                No=newNo
            };
            db.Database.ExecuteSqlCommand("update client_t set No='" + newNo + "' where id=" + C.Id + "");
            return BP;
        }
        [Check]
        [Client("Id")]
        [HttpPost]
        public Models.BackParameter Delete([FromBody]Models.Client.Gain.Delete D)
        {
            db.Database.ExecuteSqlCommand(string.Format("delete client_t where id in ({0})",D.id), "");
            BP.back = "删除成功";
            return BP;
        }
        [Check]
        [Client]
        [HttpPost]
        public Models.BackParameter Update([FromBody]Client_T C)
        {
            
            return BP;
        }
        [Check]
        [HttpPost]
        public Models.BackParameter Get([FromBody]Models.Client.Gain.Get G)
        {
            Models.Client.Back.Get BGC = db.Database.SqlQuery<Models.Client.Back.Get>($@"select C.* from Client_T  C
                INNER JOIN[ClientSale_T] CS
                on C.Id = CS.ClientId and cs.UserId = '{G.UserId}' and CS.ClientId = {G.ClientId}").FirstOrDefault();
            if (BGC != null)
            {
                BP.back = BGC;
                return BP;
            }
            BP.code = Tools.BackCode.NoClient;
            BP.back = BackCode.CodeStr[BP.code];
            return BP;
        }
        [Check]
        [HttpPost]
        public Models.BackParameter Pub([FromBody]Models.Client.Gain.Pub P)
        {
            IEnumerable<Models.Client.Back.GetList> client = null;
            String username = Tools.Base.GetUserName(P.UserId);
            Tools.Where wheres = new Tools.Where();
            String o = " Order by " + Tools.OrderBy.Desc(P.OrderBy);
            wheres.And(String.Format("createrName = '{0}'", username));
            wheres.And("pub=1");
            wheres.And(P.Where);
            client = db.Database.SqlQuery<Models.Client.Back.GetList>("select * from client_t " + wheres.ToWhere() + o);
            int count = client.Count();
            client = client.Skip((P.PageIndex-1) * P.PageMax).Take(P.PageMax);
            Models.BackList BL = new Models.BackList()
            {
                count=count,
                index=P.PageIndex,
                list=client
            };
            BP.back = BL;
            return BP;
        }
        [Check]
        [HttpPost]
        public Models.BackParameter AllPub([FromBody]Models.Client.Gain.AllPub P)
        {
            IEnumerable<Models.Client.Back.GetList> client = null;
            String username = Tools.Base.GetUserName(P.UserId);
            Tools.Where wheres = new Tools.Where();
            String o = " Order by " + Tools.OrderBy.Desc(P.OrderBy);
            wheres.And("pub=1");
            wheres.And(P.Where);
            client = db.Database.SqlQuery<Models.Client.Back.GetList>("select * from client_t " + wheres.ToWhere() + o);
            int count = client.Count();
            client = client.Skip((P.PageIndex - 1) * P.PageMax).Take(P.PageMax);
            Models.BackList BL = new Models.BackList()
            {
                count = count,
                index = P.PageIndex,
                list = client
            };
            BP.back = BL;
            return BP;
        }
        [Check]
        [HttpPost]
        public Models.BackParameter Deleteing([FromBody]Models.Client.Gain.Deleteing P)
        {
            IEnumerable<Models.Client.Back.GetList> client = null;
            String username = Tools.Base.GetUserName(P.UserId);
            Tools.Where wheres = new Tools.Where();
            String o = " Order by " + Tools.OrderBy.Desc(P.OrderBy);
            wheres.And("IsDel = 1");
            wheres.And(P.Where);
            client = db.Database.SqlQuery<Models.Client.Back.GetList>("select * from client_t " + wheres.ToWhere() + o);
            int count = client.Count();
            client = client.Skip((P.PageIndex - 1) * P.PageMax).Take(P.PageMax);
            Models.BackList BL = new Models.BackList()
            {
                count = count,
                index = P.PageIndex,
                list = client
            };
            BP.back = BL;
            return BP;
        }
        [Check]
        [HttpPost]
        public Models.BackParameter Deleteed([FromBody]Models.Client.Gain.Deleteed P)
        {
            IEnumerable<Models.Client.Back.GetList> client = null;
            String username = Tools.Base.GetUserName(P.UserId);
            Tools.Where wheres = new Tools.Where();
            String o = " Order by " + Tools.OrderBy.Desc(P.OrderBy);
            wheres.And("IsDel = 2");
            wheres.And(P.Where);
            client = db.Database.SqlQuery<Models.Client.Back.GetList>("select * from client_t " + wheres.ToWhere() + o);
            int count = client.Count();
            client = client.Skip((P.PageIndex - 1) * P.PageMax).Take(P.PageMax);
            Models.BackList BL = new Models.BackList()
            {
                count = count,
                index = P.PageIndex,
                list = client
            };
            BP.back = BL;
            return BP;
        }
        [Check]
        [HttpPost]
        public Models.BackParameter GetList([FromBody]Models.Client.Gain.GetList P)
        {
            string useridpd = "";
            //if (P.sub != null)
            //{
            //    Models.Client.Back.GetList clientxs = db.Database.SqlQuery<Models.Client.Back.GetList>("select * from user_t where userid='" + P.UserId + "'").FirstOrDefault();
            //    if (P.sub.ToUpper() == "ALL")
            //    {
            //        if (clientxs.UserType == 3)
            //        {
            //            useridpd = "userid='" + clientxs.userid + "' ";
            //        }
            //        if (clientxs.UserType ==2)
            //        {
            //            IEnumerable<string> useridlist = db.Database.SqlQuery<string>("select userid from user_t where DeptId='" + clientxs.DeptId + "'");
            //            foreach (string useridson in useridlist)
            //            {
            //                if (useridpd == "")
            //                    useridpd = "userid='" + useridson + "' ";
            //                else
            //                    useridpd += " or userid='" + useridson + "' ";
            //            }
            //        }
            //        if (clientxs.UserType == 1)
            //        {
            //            IEnumerable<string> useridlist = db.Database.SqlQuery<string>("select userid from user_t where BranchId='" + clientxs.BranchId + "'");
            //            foreach (string useridson in useridlist)
            //            {
            //                if (useridpd == "")
            //                    useridpd = "userid='" + useridson + "' ";
            //                else
            //                    useridpd += " or userid='" + useridson + "' ";
            //            }
            //        }
            //    }
            //    else
            //    {
            //        if (clientxs.UserType == 3)
            //        {
            //            BP.back = "你是普通用户无法查看他人客户";
            //            return BP;
            //        }
            //        if (clientxs.UserType == 2)
            //        {
            //            int Deptqx = db.Database.SqlQuery<int>("select DeptId from user_t where userid='" + P.sub + "'").FirstOrDefault();
            //            if (Deptqx != clientxs.DeptId)
            //            {
            //                BP.back = "该用户不是你的下属你无法查看他的客户";
            //                return BP;
            //            }
                       
            //        }
            //        useridpd = "userid='" + P.sub + "' ";
            //    }
            //}
            if (useridpd == "")
                useridpd = "userid='" + P.UserId + "' ";
                IEnumerable<Models.Client.Back.GetList> client = null;
                String username = Tools.Base.GetUserName(P.UserId);
                Tools.Where wheres = new Tools.Where();
                String o = " Order by " + Tools.OrderBy.Desc(P.OrderBy);
                wheres.And("Id in (select clientid from [dbo].[ClientSale_T] where (" + useridpd + "))");
                wheres.And("(pub is  null or pub!=1)");
                wheres.And(P.Where);
                wheres.And("(IsDel !=2 or IsDel is null)");
                SqlClass sqlc = new SqlClass();

                if (P.Key != null && P.Key.Length > 0)
                {
                    wheres.Like("SName,Name,Address,PostCode,Country,Province,city,Tel", P.Key);
                }
                string sql = sqlc.Page(P.PageIndex, P.PageMax, "select *,ROW_NUMBER() OVER ( Order by  CreateTm Desc ) AS RowNumber from client_t ", wheres.ToWhere(), o);
                client = db.Database.SqlQuery<Models.Client.Back.GetList>(sql);
                int count = client.Count();
                Models.BackList BL = new Models.BackList()
                {
                    count = count,
                    index = P.PageIndex,
                    list = client
                };
                BP.back = BL;
                return BP;
        }
        [Check]
        [HttpPost]
        public Models.BackParameter PubClass([FromBody]Models.Client.Gain.PubClass P)
        {
            IEnumerable<Models.Client.Back.GetList> client = null;
            String username = Tools.Base.GetUserName(P.UserId);
            Tools.Where wheres = new Tools.Where();
            String o = " Order by " + Tools.OrderBy.Desc(P.OrderBy);
            switch (P.pubclass.ToUpper())
            {
                case "WFL":
                    wheres.And("PubClass='' and pub=1");
                    break;
                case "":
                 wheres.And("pub=1");
                    break;
                case "MYCLIENT":
                    wheres.And("pub=1 and creater='"+P.UserId+"'");
                    break;
                case "ALL":
                    wheres.And("pub=1");
                    break;
                default:
                    wheres.And("pub=1 and pubclass='"+P.pubclass+"'");
                    break;
            }
            wheres.And(P.Where);
            client = db.Database.SqlQuery<Models.Client.Back.GetList>("select * from client_t " + wheres.ToWhere() + o);
            int count = client.Count();
            client = client.Skip((P.PageIndex - 1) * P.PageMax).Take(P.PageMax);
            Models.BackList BL = new Models.BackList()
            {
                count = count,
                index = P.PageIndex,
                list = client
            };
            BP.back = BL;
            return BP;
        }
        [Check]
        [HttpPost]
        public Models.BackParameter Getdetai([FromBody]Models.Client.Gain.Getdetail P)
        {     
            IEnumerable<Models.Client.Back.Getdetailbf> ldt = null;
            IEnumerable<Models.Client.Back.Getdetaisjz> sjz = null;
            IEnumerable<Models.Client.Back.Getdetailbj> bj = null;
            IEnumerable<Models.Client.Back.Getdetaildd> dd = null;
            IEnumerable<Models.Client.Back.Getdetaillxr> lxr = null;
            string clientId = P.clientid;
            try
            {
                string ccgz = db.Database.SqlQuery<string>("select Setting from ServerConfig_T where ParamName='CLIENT_PFPROCEDURE'").FirstOrDefault();
                ldt = db.Database.SqlQuery<Models.Client.Back.Getdetailbf>("exec " + ccgz + " " + clientId + ",1,1,1");
                sjz = db.Database.SqlQuery<Models.Client.Back.Getdetaisjz>("select icon,actionMenoNo,actionKeyId,title,html,createtm,url ,creatername from TimeZhou_V where MenoNo='ABAA' and KeyId=" + clientId + " ORDER BY CreateTm DESC ");
                bj = db.Database.SqlQuery<Models.Client.Back.Getdetailbj>("select totalCount=count(*),totalMoney=isnull(sum(TotMoneyRMB),0) from Quote_T where clientid=" + clientId + " and Submit=1 and datediff(d,createtm,getdate())<=30 ");
                dd = db.Database.SqlQuery<Models.Client.Back.Getdetaildd>("select totalCount=count(*),totalMoney=isnull(sum(TotMoneyRMB),0) from Sell_T where clientid=" + clientId + "");
                lxr = db.Database.SqlQuery<Models.Client.Back.Getdetaillxr>(" select * from clientadd_t where clientid=" + clientId + " order by mainadd desc");
                Models.Client.Back.Backdetail BL = new Models.Client.Back.Backdetail()
                {
                    count = 1,
                    listpf = ldt,
                    listsjz = sjz,
                    listbj = bj,
                    listdd = dd,
                    listlxr = lxr,
                    gzcount = dbm.Database.SqlQuery<int>(" select totalCount=count(*) from [TCRM].[dbo].[ClientPro_T] where ClientId=" + clientId + "").FirstOrDefault(),
                    hgindex = dbm.Database.SqlQuery<int>(" select totalCount=count(*) from [TCRM].[dbo].[CustomsData_T] where ClientId=" + clientId + "").FirstOrDefault()
                };
                BP.back = BL;
                return BP;
            }
            catch (Exception ex)
            {
                BP.code = "xxxx";
                BP.back = null;
                return BP;
                throw;
            }
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
                IEnumerable<Models.User.Gain.userallmenu> userallmenu = db.Database.SqlQuery<Models.User.Gain.userallmenu>("select BranchId,DeptId,username,MOBILE,EMAIL,SEX,branchid,duty,userid from [dbo].[User_T]");
                BGL.menufirst = menu;
                BGL.menusecend = menusecend;
                BGL.userallmenu = userallmenu;
                BP.back = BGL;
            }
            catch (Exception ee)
            {
                BP.back = ee.ToString();
            }
            return BP;
        }
    }
}
