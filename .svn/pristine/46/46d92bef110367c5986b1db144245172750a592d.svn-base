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
    public class FolderController:BaseControllers
    {
        [Check]
        [HttpPost]
        public Models.BackParameter Get([FromBody]Models.Folder.Gain.Get G)
        {
            String boxlist = Tools.Base.GetUseMailBox(G);
            IEnumerable<Models.Folder.Back.Get> GFIE = db.Database.SqlQuery<Models.Folder.Back.Get>(String.Format("select * from MailBoxRoot_t where MailBoxId in ({0})", boxlist));
            BP.back = GFIE;
            return BP;
        }
        [Check]
        [Folder]
        [HttpPost]
        public Models.BackParameter Move([FromBody]Models.Folder.Gain.Move M)
        {
            MailBoxRoot_T MBR = db.MailBoxRoot_T.Where(MBRW => MBRW.Id == M.FolderId).FirstOrDefault();
                String MoveMailBoxId = M.ParentId.Split('_')[1];
                if (MBR.MailBoxId != int.Parse(MoveMailBoxId))
                {
                BP.code = Tools.BackCode.MoveError;
                BP.back = Tools.BackCode.CodeStr[BP.code];
                return BP;
                }
            if (Tools.Base.IsUseMailBox(M, (int)MBR.MailBoxId))
            {
                MBR.ParentId = M.ParentId;
                db.SaveChanges();
                BP.back = "移动成功";
            }
            else
            {
                BP.code = Tools.BackCode.FileNotIsYou;
                BP.back = Tools.BackCode.CodeStr[BP.code];
            }
                return BP;
        }


    }
}
