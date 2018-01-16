﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text.RegularExpressions;
using AppApi.App_Data;
using AppApi;
using AppApi.Controllers;
using System.IO;

namespace AppApi.Tools
{
    public class file 
    {
        public static string luj = System.AppDomain.CurrentDomain.BaseDirectory + @"file\"; //获取文件路径
        public static filereturn Savefile(filest filest)//上传文件
        {
            Stream StreamObject = FileBinaryConvertHelper.BytesToStream(filest.filebyte);
            filereturn result = new Tools.filereturn();
            TCRMCLASS.FileClass fc = new TCRMCLASS.FileClass();
            TCRMCLASS.FileInfoEx fileinfo = new TCRMCLASS.FileInfoEx();
            fileinfo.fileclass = filest.fileclass; //文件种类(0普通附件，1邮件原文，2邮件附件，3文档附件)
            fileinfo.filename = filest.FileName; //文件名称
            fileinfo.filetype = filest.ContentType; //文件的mime类型
            TCRMCLASS.FileResult fResult = fc.saveFile(StreamObject, fileinfo);
            if (!fResult.IsSuccess)
            {
                result.ErrorMessage = fResult.ErrMsg;

                result.State = 0;
                return result;
            }
            TCRMCLASS.DBClass db1 = new TCRMCLASS.DBClass();
            string tcrmdb = ConfigurationManager.AppSettings["dbname"];
            string filedb = db1.getServerConfig("SYSTEM_FILEDB");
            SqlCommand comm = db1.getSqlCommand(@"insert into " + tcrmdb + @"MAIL.dbo.MailAcc_T (MailBoxId, MailId, FileName, FileType, FileSize, ContentID, DownloadID,FileDb, FileId, FileStoreId, FileRoot, FileKey, FileStore,FileZip,Creater)
        values (@MailBoxId, @MailId, @FileName, @FileType, @FileSize, @ContentID, @DownloadID, @FileDb, @FileId, @FileStoreId, @FileRoot, @FileKey, @FileStore,@FileZip,@Creater)");
            if(filest.mailboxid==null)
                comm.Parameters.AddWithValue("@MailBoxId", DBNull.Value);
            else
                 comm.Parameters.AddWithValue("@MailBoxId", filest.mailboxid);
            if (filest.mailid == null)
                comm.Parameters.AddWithValue("@MailId", DBNull.Value);
            else
                comm.Parameters.AddWithValue("@MailId", filest.mailid);

            comm.Parameters.AddWithValue("@FileName", TCRMCLASS.DealString.GetRealSizeStringR(Path.GetFileName(filest.FileName), 150));
            comm.Parameters.AddWithValue("@FileType", filest.ContentType);
            comm.Parameters.AddWithValue("@FileSize", filest.filebyte.Length);
            if (filest.strDid == null)
                comm.Parameters.AddWithValue("@ContentID", DBNull.Value);
            else
                comm.Parameters.AddWithValue("@ContentID", filest.strDid);
            comm.Parameters.AddWithValue("@DownloadID", filest.DownloadID);
            if (fResult.FileStore == "DB")
            {
                comm.Parameters.AddWithValue("@FileDb", fResult.FileDb);
                comm.Parameters.AddWithValue("@FileId", fResult.FileId);
                comm.Parameters.AddWithValue("@FileStoreId", DBNull.Value);
                comm.Parameters.AddWithValue("@FileRoot", DBNull.Value);
                comm.Parameters.AddWithValue("@FileKey", DBNull.Value);
            }
            else
            {
                comm.Parameters.AddWithValue("@FileDb", DBNull.Value);
                comm.Parameters.AddWithValue("@FileId", DBNull.Value);
                comm.Parameters.AddWithValue("@FileStoreId", fResult.FileStoreId);
                comm.Parameters.AddWithValue("@FileRoot", fResult.FileRoot);
                comm.Parameters.AddWithValue("@FileKey", fResult.FileKey);
            }
            comm.Parameters.AddWithValue("@FileStore", fResult.FileStore);
            comm.Parameters.AddWithValue("@FileZip", fResult.FileZip);

            comm.Parameters.AddWithValue("@Creater", filest.userid);
            comm.ExecuteNonQuery();

            result.strId = db1.GetMaxID();
            result.State = 1;
            return result;

        }

        public static string savewfile(filest filest)//上传文档
        {
            TCRMCLASS.DBClass db1 = new TCRMCLASS.DBClass();
            string tcrmdb = ConfigurationManager.AppSettings["dbname"];
            db1.getSqlCommand("insert into " + tcrmdb + @"DOC.dbo.file_t (Name) values('')");
            string strId = db1.GetField("select ident_current('file_t')");
            try
            {
                string fileTag = "";//默认空
                string strPreviewPic1 = "";//默认空
                string strPreviewPic2 = "";//默认空

                string fileNo = "";
                string fileType =filest.ContentType;
                string fileName = filest.FileName;
                string fileBak = "";
                string fileSecurity = "";


                Stream fileStream = FileBinaryConvertHelper.BytesToStream(filest.filebyte);
                string strExName = filest.FileName; ;
                string strFileMimeType = filest.ContentType;
                int FileSize = filest.filebyte.Length;
                string gPath = Guid.NewGuid().ToString() + strExName;

                string strServerId = db1.GetField("select top 1 Id from " + tcrmdb + @"DOC.dbo.FileServer_T order by IsDef desc");
                string strServerRoot = db1.GetField("select top 1 ServerRoot from " + tcrmdb + @"DOC.dbo.FileServer_T order by IsDef desc");
                string strFileRoot = DateTime.Now.ToString("yyyyMM");
                Directory.CreateDirectory(strServerRoot + "\\" + strFileRoot);
                string strSaveName = strServerRoot + "\\" + strFileRoot + "\\" + gPath;

                FileStream get_file = new FileStream(strSaveName, FileMode.Open, FileAccess.Read, FileShare.Read);

                System.Security.Cryptography.MD5CryptoServiceProvider get_md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();

                byte[] hash_byte = get_md5.ComputeHash(get_file);

                string strIdenCode = System.BitConverter.ToString(hash_byte);
                strIdenCode = strIdenCode.Replace("-", "");
                get_file.Close();
                string strFileUrl = strServerRoot + "\\" + strFileRoot + "\\" + gPath;

                string strOldServerId = db1.GetField("select top 1 ServerId from File_T where Id=" + strId);
                string strOldServerRoot = db1.GetField("select top 1 ServerRoot from FileServer_T where Id=" + strOldServerId);
                string strOldFileRoot = db1.GetField("select top 1 FileRoot from File_T where Id=" + strId);
                string strOldFileName = db1.GetField("select top 1 FileName from File_T where Id=" + strId);
                string strOldFlashName = db1.GetField("select top 1 FlashName from File_T where Id=" + strId);
                string strOldPreviewPic1 = db1.GetField("select top 1 PreviewPic1 from File_T where Id=" + strId);
                string strOldPreviewPic2 = db1.GetField("select top 1 PreviewPic2 from File_T where Id=" + strId);
                string sql = @"update " + tcrmdb + @"DOC.dbo.File_T set Code=@Code,Type=@Type,Name=@Name,FileName=@FileName,FlashName=null,FileRoot=@FileRoot,ServerId=@ServerId,ConNo=@ConNo,
                                                    Bak=@Bak,FileSize=@FileSize,FileType=@FileType,FileMimeType=@FileMimeType,Updater=@Updater,UpdaterName=@UpdaterName,IdenCode=@IdenCode,Security=@Security,TagClass=dbo.GetTagClassStr_F(@TagId),Tag=dbo.GetTagStr_F(@TagId),TagId=@TagId,PreviewPic1=@PreviewPic1,PreviewPic2=@PreviewPic2 where Id=@Id";
                SqlCommand comm = db1.getSqlCommand(sql);
                comm.Parameters.AddWithValue("@Code", fileNo);
                comm.Parameters.AddWithValue("@Type", fileType);
                comm.Parameters.AddWithValue("@Name", fileName);
                comm.Parameters.AddWithValue("@FileName", gPath);
                comm.Parameters.AddWithValue("@FileRoot", strFileRoot);
                comm.Parameters.AddWithValue("@ServerId", strServerId);
                comm.Parameters.AddWithValue("@ConNo", "");
                comm.Parameters.AddWithValue("@Bak", fileBak);
                comm.Parameters.AddWithValue("@FileSize", FileSize.ToString());
                comm.Parameters.AddWithValue("@FileType", strExName);
                comm.Parameters.AddWithValue("@FileMimeType", strFileMimeType);
                comm.Parameters.AddWithValue("@Updater", filest.userid);
                comm.Parameters.AddWithValue("@UpdaterName", filest.userid);
                comm.Parameters.AddWithValue("@IdenCode", strIdenCode);
                comm.Parameters.AddWithValue("@Security", fileSecurity);
                comm.Parameters.AddWithValue("@Id", strId);
                comm.Parameters.AddWithValue("@TagId", fileTag);
                comm.Parameters.AddWithValue("@PreviewPic1", strPreviewPic1);
                comm.Parameters.AddWithValue("@PreviewPic2", strPreviewPic2);
                comm.ExecuteNonQuery();

                File.Delete(strOldServerRoot + "\\" + strOldFileRoot + "\\" + strOldFileName);
                if (strOldFlashName != "")
                    File.Delete(strOldServerRoot + "\\" + strOldFileRoot + "\\" + strOldFlashName);

                if (strOldPreviewPic1 != "")
                    File.Delete(strOldServerRoot + "\\" + strOldFileRoot + "\\" + strOldPreviewPic1);
                if (strOldPreviewPic2 != "")
                    File.Delete(strOldServerRoot + "\\" + strOldFileRoot + "\\" + strOldPreviewPic2);



            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                db1.CloseConn();
            }
            return strId;
        }
        public static string download(string fileclass, string fileid,string filename)
        {
            if (!Directory.Exists(luj))
            {
                Directory.CreateDirectory(System.AppDomain.CurrentDomain.BaseDirectory + @"file");
            }
            try
            {
                TCRMCLASS.FileClass fc = new TCRMCLASS.FileClass();
                string strtemppath = fc.GetDownloadUrl(fileclass, fileid);
                if (strtemppath == "")
                    return "传入类型错误";
                byte[] wj = FileBinaryConvertHelper.FiletoBytes(strtemppath);
                FileBinaryConvertHelper.BytestoFile(wj,luj+filename);
               // FileBinaryConvertHelper.DeleteFile(strtemppath);
                return @"file\" + filename;
            }
            catch(Exception ee)
            {
                return ee.ToString();
            }
        }
        public static string downloadToUniquePlace(string fileclass, string fileid, string filename)
        {
            if (!Directory.Exists(luj))
            {
                Directory.CreateDirectory(System.AppDomain.CurrentDomain.BaseDirectory + @"file");
            }
            var currentPath = luj + GetFileNameWithOutFileType(filename) + fileid +"\\";
            if (!Directory.Exists(currentPath))
            {
                Directory.CreateDirectory(currentPath);
            }
            try
            {
                TCRMCLASS.FileClass fc = new TCRMCLASS.FileClass();
                string strtemppath = fc.GetDownloadUrl(fileclass, fileid);
                if (strtemppath == "")
                    return "传入类型错误";
                byte[] wj = FileBinaryConvertHelper.FiletoBytes(strtemppath);
                FileBinaryConvertHelper.BytestoFile(wj, GetlocalFilePath(filename, fileid));
                // FileBinaryConvertHelper.DeleteFile(strtemppath);
                return @"file\" + GetFileNameWithOutFileType(filename) + fileid + "\\" +filename;
            }
            catch (Exception ee)
            {
                return ee.ToString();
            }
        }
        public static string GetlocalFilePath(string fileName, string fileId)
        {
            var localRootFilePath = ConfigurationManager.AppSettings["LocalFilePath"];
            if (string.IsNullOrEmpty(localRootFilePath))
            {
                localRootFilePath = AppDomain.CurrentDomain.BaseDirectory;
            }
            var path = Path.Combine(luj, GetFileNameWithOutFileType(fileName) + fileId, fileName);
            return path;
        }

        public static string GetFileNameWithOutFileType(string fileName)
        {
            return fileName.Split('.')[0].Trim();
        }
        public static string delete(string fileclass, string fileid)
        {
            try
            {
                TCRMCLASS.FileClass fc = new TCRMCLASS.FileClass();
                fc.delFile(fileclass, fileid);
                TCRMCLASS.DBClass db1 = new TCRMCLASS.DBClass();
                string tcrmdb = ConfigurationManager.AppSettings["dbname"];
                SqlCommand comm = db1.getSqlCommand("delete " + tcrmdb + "MAIL.dbo.MailAcc_T where Id in (select ColName from dbo.StringToTable_F(@strid,','))");
                comm.Parameters.AddWithValue("@strid", fileid);
                comm.ExecuteNonQuery();

                return "finish";
            }
            catch (Exception ee)
            {
                return ee.ToString();
            }

        }

    }

    /// <summary>
    /// 工具类：文件与二进制流间的转换
    /// </summary>
    public class FileBinaryConvertHelper
    {
        /// <summary>
        /// 将文件转换为byte数组
        /// </summary>
        /// <param name="path">文件地址</param>
        /// <returns>转换后的byte数组</returns>
        public static byte[] FiletoBytes(string path)
        {
            if (!System.IO.File.Exists(path))
            {
                return new byte[0];
            }

            FileInfo fi = new FileInfo(path);
            byte[] buff = new byte[fi.Length];

            FileStream fs = fi.OpenRead();
            fs.Read(buff, 0, Convert.ToInt32(fs.Length));
            fs.Close();

            return buff;
        }

        /// <summary>
        /// 将byte数组转换为文件并保存到指定地址
        /// </summary>
        /// <param name="buff">byte数组</param>
        /// <param name="savepath">保存地址</param>
        public static void BytestoFile(byte[] buff, string savepath)
        {
            if (System.IO.File.Exists(savepath))
            {
                System.IO.File.Delete(savepath);
            }

            FileStream fs = new FileStream(savepath, FileMode.CreateNew);
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(buff, 0, buff.Length);
            bw.Close();
            fs.Close();
        }
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static int DeleteFile(string path)
        {
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
                return 1;
            }
            else
                return 0;
        }
        /// <summary>
        /// 将stream转换为byte
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static byte[] StreamToBytes(Stream stream)
        {
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            // 设置当前流的位置为流的开始   
            stream.Seek(0, SeekOrigin.Begin);
            return bytes;
        }

        /// 将 byte[] 转成 Stream  
        public static Stream BytesToStream(byte[] bytes)
        {
            Stream stream = new MemoryStream(bytes);
            return stream;
        }
    }

    public class filest
    {
        public string path { get; set; }
        public string userid { get; set; }
        public string fileclass { get; set; }
        public string filetype { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public Nullable<int> mailboxid { get; set; }
        public Nullable<int> mailid { get; set; }
        public string strDid { get; set; }
        public string DownloadID { get; set; }

        public byte[] filebyte
        {
            get; set;
        }
    }

    public class filereturn
    {
        public string ErrorMessage { get; set; }
        public int State { get; set; }
        public string strId { get; set; }
    }

}