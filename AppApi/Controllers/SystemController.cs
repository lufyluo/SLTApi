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
    public class SystemController:BaseControllers
    {
        [Check]
        [HttpPost]
        public Models.BackParameter Dictionary([FromBody]Models.Sys.Gain.Dictionary D,[FromUri]string Operation)
        {
            SysDictionary_T SD = Tools.Base.GetDictionary(D);
            switch (Operation.ToLower())
            {
                case "get":
                    BP.back= db.SysDictionary_T.Where(SDW => SDW.DataClass == D.Class && SDW.DataType == D.Type);
                    return BP;
                case "delete":
                    if (SD != null)
                        db.SysDictionary_T.Remove(db.SysDictionary_T.Where(SDW => SDW.DataClass == D.Class && SDW.DataText == D.Text && SDW.DataType == D.Type).FirstOrDefault());
                    db.SaveChanges();
                    BP.back = "删除成功";
                    return BP;
                case "add":
                    if (SD == null)
                    {
                        SysDictionary_T NSD = new App_Data.SysDictionary_T()
                        {
                            DataClass = D.Class,
                            DataText=D.Text,
                            DataType=D.Type
                        };
                        db.SysDictionary_T.Add(NSD);
                    }
                    else
                    {
                        BP.code = Tools.BackCode.HasDictionary;
                        BP.back = Tools.BackCode.CodeStr[BP.code];
                        return BP;
                    }
                    db.SaveChanges();
                    BP.back = "添加成功";
                    return BP;
                case "update":
                    if (SD != null)
                        SD.DataText = D.NewText;
                    else
                    {
                        BP.code = Tools.BackCode.NoHasDictionary;
                        BP.back = Tools.BackCode.CodeStr[BP.code];
                        return BP;
                    }
                    db.SysDictionary_T.Where(SDW => SDW.DataClass == D.Class && SDW.DataText == D.Text && SDW.DataType == D.Type).FirstOrDefault().DataText = D.NewText;
                    db.SaveChanges();
                    BP.back = "修改成功";
                    return BP;             
            }
            return BP; 
        }

        [Check]
        [HttpPost]
        public Models.BackParameter GetMb([FromBody]Models.GainParameter D)
        {
            Models.Sys.Back.mb nml = new Models.Sys.Back.mb();
            nml.xse = db.Database.SqlQuery<Nullable<Decimal>>("select SUM(isnull(TotMoneyRMB,0)) from Sell_T where YEAR(CreateTm)=YEAR(GETDATE()) and MONTH(CreateTm)=MONTH(GETDATE()) and Creater='" + D.UserId + "'").FirstOrDefault();
            if (nml.xse == null)
                nml.xse = 0;
           nml.bj = db.Database.SqlQuery<int>("select COUNT(*) from Quote_T where YEAR(CreateTm)=YEAR(GETDATE()) and MONTH(CreateTm)=MONTH(GETDATE()) and Creater='" + D.UserId + "'").FirstOrDefault();
            nml.sj = db.Database.SqlQuery<int>("select COUNT(*) from Chance_T where YEAR(CreateTm)=YEAR(GETDATE()) and MONTH(CreateTm)=MONTH(GETDATE()) and  Creater='" + D.UserId + "'").FirstOrDefault();
            nml.xzkh = db.Database.SqlQuery<int>("select COUNT(*) from Client_T where YEAR(CreateTm)=YEAR(GETDATE()) and MONTH(CreateTm)=MONTH(GETDATE()) and Creater='" + D.UserId+"'").FirstOrDefault();
            nml.zkh = db.Database.SqlQuery<int>("select count(*) from Client_T where year(SuccessDate)=year(getdate()) and month(SuccessDate)=month(getdate())").FirstOrDefault();
            BP.back = nml;
            return BP;

        }

        [Check]
        [HttpPost]
        public Models.BackParameter GetMk([FromBody]Models.GainParameter D)
        {
            List<string> ba = new List<string>();
            ba.Add("评论中心");
            ba.Add("工作动态");
            ba.Add("工作报告");
            ba.Add("待办日程");
            ba.Add("邮件管理");
            ba.Add("客户管理");
            ba.Add("客户名片");
            ba.Add("客户公海");
            ba.Add("已删客户");
            ba.Add("文档管理");
            // ba.Add("商机中心");
            // ba.Add("报价管理");
            // ba.Add("产品管理");
            // ba.Add("供应商");
            // ba.Add("BI商业智能");
            BP.back = ba;
            return BP;

        }
    }
}
