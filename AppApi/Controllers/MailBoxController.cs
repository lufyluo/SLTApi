using AppApi.App_Data;
using AppApi.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AppApi.Controllers
{
    public class MailBoxController:BaseControllers
    {
        [Check]
        [HttpPost]
        public Models.BackParameter Get([FromBody]Models.MailBox.Gain.Get G)
        {
            string UserId = G.UserId;
            dynamic mb = null;
            try
            {
                mb = db.MailBoxShare_T.Where(a => a.UserId == UserId && a.Type == -1).Select(b => new {
                    id = b.MailBoxId,
                    name = db.MailBox_T.Where(d => d.Id == b.MailBoxId).Select(e => e.Name).FirstOrDefault(),
                    email= db.MailBox_T.Where(d => d.Id == b.MailBoxId).Select(e => e.Email).FirstOrDefault()
                });

            }
            catch (Exception ex)
            {
            }
            BP.back = mb;
            return BP;
        }
        [Check]
        [HttpPost]
        public Models.BackParameter GetMenu([FromBody]Models.MailBox.Gain.GetMenu GM)
        {
            string mailboxlist= Tools.Base.GetUseMailBox(GM);
            string sql = "";
            List<Models.MailBox.Back.GetMenu> LGM=new List<Models.MailBox.Back.GetMenu>();
            switch (GM.ParentId)
            {
                case "all"://获取所有邮箱和下级目录
                    sql = string.Format("select id='m_' + CONVERT(varchar,id),name,email,boxid=id from mailbox_t where id in ({0})", mailboxlist);
                    LGM = db.Database.SqlQuery<Models.MailBox.Back.GetMenu>(sql).ToList();//获取所有邮箱
                    foreach (Models.MailBox.Back.GetMenu item in LGM)//游历所有邮箱
                    {
                        //邮箱中的文件夹
                        sql = string.Format("select act=NO,id='b_{0}_' + NO,Name,pic='picimage//'+No+'.png',boxid={1} from MailBoxType_T order by sort", item.id.Replace("m_", ""), item.id.Replace("m_", ""));
                        item.next = db.Database.SqlQuery<Models.MailBox.Back.GetMenu>(sql).ToList();//添加基本目录
                        sql = string.Format("select parentid,act='WJJ'+CONVERT(varchar,id),id='f_' + CONVERT(varchar,MailBoxId) + '_' + CONVERT(varchar,Id),name,boxid=mailboxid   from MailBoxRoot_T where Parentid='{0}' and MailBoxId in ({1})", item.id, mailboxlist);
                        item.next.AddRange(db.Database.SqlQuery<Models.MailBox.Back.GetMenu>(sql).ToList());//添加文件夹
                    }
                    Models.MailBox.Back.GetMenu zngm = new Models.MailBox.Back.GetMenu() { id = "zn", name = "智能文件夹" };
                    sql = "select act=No,id=No,name,pic='picimage\\'+No+'.png',name from MailBoxZn_T Order By Sort";
                    zngm.next=db.Database.SqlQuery<Models.MailBox.Back.GetMenu>(sql).ToList();
                    sql = string.Format("select act='WJJ'+CONVERT(varchar,id),id='f_' + (select top 1 CONVERT(varchar,mailboxid) from MailBoxRoot_T as mbr Where mbr.id=mbrc.rootid) + '_' + CONVERT(varchar,mbrc.rootid),name=(select top 1 name from MailBoxRoot_T as mbr Where mbr.id=mbrc.rootid) from MailBoxRootComm_T as mbrc Where mbrc.UserId='{0}'", GM.UserId);
                    zngm.next.AddRange(db.Database.SqlQuery<Models.MailBox.Back.GetMenu>(sql).ToList());
                    LGM.Insert(0, zngm);
                    BP.back = LGM;
                    return BP;
                case "#":
                    sql = string.Format("select id='m_' + CONVERT(varchar,id),name,email,boxid=id from mailbox_t where id in ({0})", mailboxlist);
                    //根中的邮箱            
                    break;
                case "zn":
                    sql = "select act=No,id=No,name from MailBoxZn_T Order By Sort";
                    LGM.AddRange(db.Database.SqlQuery<Models.MailBox.Back.GetMenu>(sql).ToList());
                    sql = string.Format("select act='WJJ'+CONVERT(varchar,id),id='f_' + (select top 1 CONVERT(varchar,mailboxid) from MailBoxRoot_T as mbr Where mbr.id=mbrc.rootid) + '_' + CONVERT(varchar,mbrc.rootid),name=(select top 1 name from MailBoxRoot_T as mbr Where mbr.id=mbrc.rootid) from MailBoxRootComm_T as mbrc Where mbrc.UserId='{0}'", GM.UserId);
                    LGM.AddRange(db.Database.SqlQuery<Models.MailBox.Back.GetMenu>(sql).ToList());
                    for (int i = 0; i < LGM.Count; i++)
                        LGM[i].pic =  @"picimage\" + LGM[i].id+".png";
                    BP.back = LGM;
                    return BP;
                case "znbx":
                    sql = String.Format("select id=name + '|' + Class,Name from MailLabel_T where UserId='{0}' or userid=''", GM.UserId);
                    break;
                default:
                    
                    if (GM.ParentId.IndexOf("m_") >= 0)
                    {
                        //邮箱中的文件夹
                        sql = string.Format("select act=NO,id='b_{0}_' + NO,Name,pic='picimage//'+No+'.png' from MailBoxType_T order by sort", GM.ParentId.Replace("m_", ""));
                        LGM.AddRange(db.Database.SqlQuery<Models.MailBox.Back.GetMenu>(sql).ToList());
                    }
                    sql = string.Format("select parentid,act='WJJ'+CONVERT(varchar,id),id='f_' + CONVERT(varchar,MailBoxId) + '_' + CONVERT(varchar,Id),name from MailBoxRoot_T where Parentid='{0}' and MailBoxId in ({1})", GM.ParentId, mailboxlist);
                    LGM.AddRange(db.Database.SqlQuery<Models.MailBox.Back.GetMenu>(sql).ToList());
                    BP.back = LGM;
                    return BP;
            }
            LGM = db.Database.SqlQuery<Models.MailBox.Back.GetMenu>(sql).ToList();
            BP.back = LGM;
            return BP;
        }

        [Check]
        [HttpPost]
        public Models.BackParameter Getsonroot([FromBody]Models.MailBox.Gain.GetMenu GM)
        {
            string sql = string.Format("select parentid,act='WJJ'+CONVERT(varchar,id),id='f_' + CONVERT(varchar,MailBoxId) + '_' + CONVERT(varchar,Id),name from MailBoxRoot_T where Parentid='{0}'", GM.ParentId);
            IEnumerable<Models.MailBox.Back.GetMenu> sonroot = db.Database.SqlQuery<Models.MailBox.Back.GetMenu>(sql);
            BP.back = sonroot;
            return BP;
        }

    }
}
