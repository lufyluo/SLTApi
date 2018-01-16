using AppApi.App_Data;
using AppApi.Filter;
using AppApi.Tools;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AppApi.Controllers
{
    public class FileController : BaseControllers
    {
        public int buflen = 102400;
        private const string RelativeRootPath = "file";
        [Check]
        [Filter.File]
        [HttpPost]
        public Models.BackParameter Download([FromBody]Models.File.Gain.Download D)//下载文件
        {
            byte[] filedata = db.Database.SqlQuery<byte[]>(String.Format("GetMailFile {0},{1},{2}", D.FileId, D.Position, buflen), "").FirstOrDefault();
            string data = Convert.ToBase64String(filedata);
            Models.File.Back.Download BD = new Models.File.Back.Download()
            {
                fileid = D.FileId,
                data = data,
                position = D.Position
            };
            BP.back = BD;
            return BP;
        }
        [Check]
        [HttpPost]
        public Models.BackParameter Submit([FromBody]Models.File.Gain.Submit U)//提交文件
        {
            U.Data = U.Data.Replace(" ", "+");
            Tools.filereturn filereturn = new Tools.filereturn();
            byte[] filebyte = Convert.FromBase64String(U.Data);
            Tools.filest filelist = new Tools.filest()
            {
                FileName = U.Name,
                fileclass = U.fileclass,
                ContentType = U.Name.Split('.')[1],
                filebyte = filebyte,
                userid = U.UserId,
                mailid = null,
                mailboxid = null,


            };
            if (filelist.fileclass == "2")
            {
                filelist.mailboxid = U.mailboxid;
                filelist.mailid = U.mailid;
                // filelist.strDid = System.Guid.NewGuid().ToString().Replace("-", "");
                filelist.strDid = null;
            }

            filelist.DownloadID = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(DateTime.Now.ToString("yyyyMMddHHmmssfff"), "MD5").ToUpper();

            if (filelist.fileclass == "3")
            {
                BP.back = Tools.file.savewfile(filelist);
            }
            else
            {
                filereturn = Tools.file.Savefile(filelist);
                BP.back = filereturn;
            }


            return BP;



        }

        [Check]
        [FileCreater]
        [HttpPost]
        public Models.BackParameter Upload([FromBody]Models.File.Gain.Upload C)//续传文件
        {
            Models.File.Back.Upload BU = new Models.File.Back.Upload();
            int datasize = dbf.Database.SqlQuery<int>("select datasize=DATALENGTH(isnull([Data],'')) from maildata_T where id=" + C.FileId).FirstOrDefault();
            int filesize = dbm.Database.SqlQuery<int>("select FileSize from MailAcc_T where Fileid=" + C.FileId).FirstOrDefault();
            if (filesize > 0)
            {
                C.Data = C.Data.Replace(" ", "+");
                if (filesize > datasize)
                {
                    byte[] data = Convert.FromBase64String(C.Data);
                    dbf.UpdateMailFile(C.FileId, data, C.Position);
                    int nowsize = C.Position + data.Length;
                    if (filesize <= nowsize)
                    {
                        BP.code = Tools.BackCode.FileIsFinish;
                        BP.back = Tools.BackCode.CodeStr[BP.code];
                        return BP;
                    }
                    else
                    {
                        BU.fileid = C.FileId;
                        BU.length = nowsize;
                        BU.msg = "上传成功";
                        BP.back = BU;
                        return BP;
                    }
                }
                else
                {
                    BP.code = Tools.BackCode.FileIsFinish;
                    BP.back = Tools.BackCode.CodeStr[BP.code];
                    return BP;
                }

            }
            else
            {
                BP.code = Tools.BackCode.Err;
                BP.back = "获取文件大小失败";
                return BP;
            }

        }

        [Check]
        [HttpPost]
        public Models.BackParameter downloaduri([FromBody]Models.File.Gain.download C)//uri下载文件
        {
            Models.File.Gain.filereturn result = new Models.File.Gain.filereturn();
            string filename = "";
            if (false)
            {
                Models.File.Gain.filed filew = new Models.File.Gain.filed();
                filew = dbd.Database.SqlQuery<Models.File.Gain.filed>("select * from file_t where id=" + C.fileid + "").FirstOrDefault();
                string strServerRoot = dbd.Database.SqlQuery<string>("select top 1 ServerRoot from FileServer_T where Id=" + filew.ServerId).FirstOrDefault();
                string filePath = strServerRoot + "\\" + filew.FileRoot + "\\" + filew.FileName;
                byte[] wj = FileBinaryConvertHelper.FiletoBytes(filePath);
                FileBinaryConvertHelper.BytestoFile(wj, luj + filew.Name);
                BP.back = @"file\" + filew.Name;
            }
            else
            {
                filename = dbm.Database.SqlQuery<string>("select filename from [dbo].[MailAcc_T] where id=" + C.fileid + "").FirstOrDefault();
                result.uri = Tools.file.download(C.fileclass, C.fileid, filename);
                BP.back = result;
            }
            return BP;
        }
        /// <summary>
        /// 从远程地址下载本机没有的文件再返回
        /// </summary>
        /// <param name="C"></param>
        /// <returns></returns>
        [Check]
        [HttpPost]
        public Models.BackParameter downloadFromRemote([FromBody]Models.File.Gain.download C)//uri下载文件
        {
            Models.File.Gain.filereturn result = new Models.File.Gain.filereturn();
            string filename = "";
            if (false)
            {
                Models.File.Gain.filed filew = new Models.File.Gain.filed();
                filew = dbd.Database.SqlQuery<Models.File.Gain.filed>("select * from file_t where id=" + C.fileid + "").FirstOrDefault();
                string strServerRoot = dbd.Database.SqlQuery<string>("select top 1 ServerRoot from FileServer_T where Id=" + filew.ServerId).FirstOrDefault();
                string filePath = strServerRoot + "\\" + filew.FileRoot + "\\" + filew.FileName;
                byte[] wj = FileBinaryConvertHelper.FiletoBytes(filePath);
                FileBinaryConvertHelper.BytestoFile(wj, luj + filew.Name);
                BP.back = @"file\" + filew.Name;
            }
            else
            {
                filename = dbm.Database.SqlQuery<string>("select filename from [dbo].[MailAcc_T] where id=" + C.fileid + "").FirstOrDefault();
                result.uri = Tools.file.downloadToUniquePlace(C.fileclass, C.fileid, filename);
                BP.back = result;
                DownloadFileIfNotExist(result.uri, C.fileid);
            }
            return BP;
        }

        private void DownloadFileIfNotExist(string url, string fileId)
        {
            var path = file.GetlocalFilePath(Path.GetFileName(url), fileId);
            if (!System.IO.File.Exists(path))
            {
                string remoteFileAddress = ConfigurationManager.AppSettings["RemoteFileServer"];
                var fileUri = url.Replace("\\", "/");
                DownLoadRemoteFile(remoteFileAddress + "/" + fileUri, path);
            }
        }

        private void DownLoadRemoteFile(string remoteUrl, string localPath)
        {
            WebClient client = new WebClient();
            client.DownloadFile(remoteUrl, localPath);
        }

        /// <summary>
        /// 递归取得下级节点
        /// </summary>
        /// <param name="kindId"></param>
        /// <returns></returns>
        [Check]
        [HttpPost]
        public Models.BackParameter GetMenu(Models.File.Gain.filelist C)
        {
            Models.File.Gain.GetMenu menu = new Models.File.Gain.GetMenu();
            DataSet ds;
            DataTable dt = new DataTable();
            TCRMCLASS.DBClass db = new TCRMCLASS.DBClass();
            string tcrmdb = ConfigurationManager.AppSettings["dbname"];
            SqlCommand comm;
            try
            {
                string sql = String.Format(@"select Id as id,convert(nvarchar(10),ParentId)  as parent,case when UIDType = '' and RoleType = '' then 'images/myfolder.png' else 'images/sharedfolder.png' end as icon ,case when Bak = '' then Name else Name+'('+Bak+')' end as text ,Bak,Sort from " + tcrmdb + @"DOC.dbo.Root_T where Creater = '{0}' order by Sort ", C.UserId);
                comm = db.getSqlCommand(sql);
                ds = db.getDataSetBySqlCommand(comm);
                dt = ds.Tables[0];
                DataRow root = dt.NewRow();
                root["id"] = 0;
                root["parent"] = "#";
                root["icon"] = "images/myfolder.png";
                root["text"] = "我的文件夹";
                root["Sort"] = 0;
                dt.Rows.Add(root);/*添加新行*/
                                  //我共享的文件夹
                root = dt.NewRow();
                root["id"] = -1;
                root["parent"] = "#";
                root["icon"] = "images/myfolder.png";
                root["text"] = "共享的文件夹";
                root["Sort"] = 1;
                dt.Rows.Add(root);/*添加新行*/
                                  //共享的文件
                root = dt.NewRow();
                root["id"] = -2;
                root["parent"] = "#";
                root["icon"] = "images/myfolder.png";
                root["text"] = "共享的文件";
                root["Sort"] = 2;
                dt.Rows.Add(root);/*添加新行*/
                                  //加入有共享的文件夹
                string strRoleId = db.GetField("select " + ConfigurationManager.AppSettings["dbname"] + ".dbo.GetRoleIdByUserId_F('" + C.UserId + "')");
                string UID = db.GetField("select " + ConfigurationManager.AppSettings["dbname"] + ".dbo.GetUIDByUserId_F('" + C.UserId + "')");
                TCRMCLASS.DBClass db1 = new TCRMCLASS.DBClass();
                string strIsPower = "";
                if (!db1.checkPower(C.UserId, "AJ05"))
                    strIsPower = " and " + ConfigurationManager.AppSettings["dbname"] + "DOC.dbo.NewIsRootPower_F('ZAODUMR',Id,'" + UID + "','" + strRoleId + "')= 1";
                db1.CloseConn();
                sql = String.Format(@"select Id as id,-1 as parent , Name+'('+Creater+')'  as text ,'images/myfolder.png' as  icon,Bak,Sort from " + tcrmdb + @"DOC.dbo.Root_T where Creater <> '{0}' " + strIsPower + " order by Sort", C.UserId);
                comm = db.getSqlCommand(sql);
                ds = db.getDataSetBySqlCommand(comm);
                //在别人创建的文件夹中找到自己有权限的文件夹
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    //string isPower = db.GetField("select dbo.NewIsRootPower_F('ZAODUMR'," + row["id"] + ",'" + UID + "','" + strRoleId + "')");
                    //if (isPower == "1") {
                    root = dt.NewRow();
                    root["id"] = row["id"];
                    root["parent"] = "-1";
                    root["icon"] = "images/myfolder.png";
                    root["text"] = row["text"];
                    root["Sort"] = row["Sort"];
                    dt.Rows.Add(root);
                    //}
                }

                menu.menulist = dt;

                /*
                string filelist = "(";
                foreach (DataRow row in dt.Rows)
                {
                    if (filelist == "(")
                        filelist += row["id"];
                    else
                        filelist += "," + row["id"];
                }
                filelist += ")";
                menu.filelist = dbd.Database.SqlQuery<Models.File.Gain.filed>("select *  from File_T where rootid in " + filelist + "");
                */

                BP.back = menu;

                return BP;

            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                db.CloseConn();
            }


        }

        /// <summary>
        /// 递归取得下级节点
        /// </summary>
        /// <param name="kindId"></param>
        /// <returns></returns>
        [Check]
        [HttpPost]
        public Models.BackParameter Gefilelist(Models.File.Gain.filelist G)
        {

            SqlClass.Where where = new SqlClass.Where();
            try
            {
                SqlClass sqlclass = new SqlClass();
                SqlClass.Select select = new SqlClass.Select("File_T");
                SqlClass.Where wheres = new SqlClass.Where();
                SqlClass.Order orderby = new SqlClass.Order();
                select.Add("*");
                where.And("rootid = " + G.parentid + "");
                where.And("" + G.Where + "");
                orderby.Desc("CreateTm");
                string sql = sqlclass.Page(G.PageIndex, G.PageMax, select, where, orderby);
                Models.File.Gain.fileback BGL = new Models.File.Gain.fileback();
                IEnumerable<Models.File.Gain.filed> BiM = dbd.Database.SqlQuery<Models.File.Gain.filed>(sql);
                BGL.count = dbd.Database.SqlQuery<int>("select count(*) from File_T where rootid = " + G.parentid + "").FirstOrDefault();
                BGL.filelist = BiM;

                BP.back = BGL;
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
