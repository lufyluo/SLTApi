using AppApi.App_Data;
using AppApi.Filter;
using AppApi.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AppApi.Controllers
{
    public class ContactController:BaseControllers
    {
        [Check]
        [HttpPost]
        public Models.BackParameter Get([FromBody]Models.Contact.Gain.GetList G)
        {
            
            string UserId = G.UserId; int Clientid = G.ClientId; int PageIndex = G.PageIndex; int PageMax = G.PageMax; string where = G.Where;
            IEnumerable<Models.Contact.Back.GetList> BGL = null;
            string ordertime = " order by CreateTM desc ";
            String username = Base.GetUserName(G.UserId);
            string keys = "";
            SqlClass sqlc=new SqlClass ();
            if (G.Key != null && G.Key.Length > 0)
            {
                keys = selectkey("Name,Sex,email,Mobile,tel,Job,HomeTel,Bak", G.Key);
            }
            if (Clientid != 0)
            {
                if (db.Client_T.Any(cw => cw.Id == Clientid))
                {
                    if (string.IsNullOrEmpty(where))
                    {
                        BGL = db.Database.SqlQuery<Models.Contact.Back.GetList>(sqlc.Page(PageIndex, PageMax, String.Format("select *,ROW_NUMBER() OVER ( Order by  CreateTm Desc ) AS RowNumber from ClientAdd_T where clientid={0}" + keys, G.ClientId), "", ""));
                    }
                    else
                    {
                        BGL = db.Database.SqlQuery<Models.Contact.Back.GetList>(sqlc.Page(PageIndex, PageMax, String.Format("select *,ROW_NUMBER() OVER ( Order by  CreateTm Desc ) AS RowNumber from ClientAdd_T where clientid={0}" + keys, G.ClientId), G.Where, ""));
                    }
                }
                else
                {
                    BP.code = BackCode.NoClient;
                    BP.back = BackCode.CodeStr[BP.code];
                    return BP;
                }
            }
            else
            {
                IEnumerable<int> id = db.Client_T.Where(cw => cw.Sale.Contains(username) && (cw.Pub != 1 || cw.Pub == null) && (cw.IsDel != 1 || cw.IsDel == null)).OrderByDescending(co => co.CreateTm).Select(cas => cas.Id);
                string id_list = string.Join(",", id.ToArray());
                if (where == null || where.Length == 0)
                {
                    string sql = sqlc.Page(G.PageIndex, G.PageMax, "select *,ROW_NUMBER() OVER ( Order by  CreateTm Desc ) AS RowNumber  from ClientAdd_T ", "where clientid in (" + id_list + ")" + keys, ordertime);
                    BGL = db.Database.SqlQuery<Models.Contact.Back.GetList>(sql);
                }
                else
                {
                    string sql = sqlc.Page(G.PageIndex, G.PageMax, "select *,ROW_NUMBER() OVER ( Order by  CreateTm Desc ) AS RowNumber  from ClientAdd_T ",G.Where+ " clientid in (" + id_list + ")" + keys, ordertime);
                    BGL = db.Database.SqlQuery<Models.Contact.Back.GetList>(sql);
                }

            }
            string back = "";
            int count = BGL.Count();
            Models.BackList NBL = new Models.BackList()
            {
                count = count,
                index = G.PageIndex,
                list = BGL
            };
            BP.back = NBL;
            return BP;
        }
        public string selectkey(string key,string value)
        {
            string likestr = "";
            string[] keys = key.Split(',');
            for (int i = 0; i < keys.Length; i++)
            {
                if(i==0)
                    likestr = keys[i]+" like '%" + value + "%'";
                else
                    likestr += " or  " + keys[i] + " like '%" + value + "%'";
            }
            return " And ("+likestr+")";
        }
    }
}
