using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using AppApi.App_Data;
using TCRMCLASS;

namespace AppApi.Tools
{
    public class MailTool
    {
        private TCRMEntities db;
        public MailTool(TCRMEntities db)
        {
            this.db = db;
        }

        public string BuildSQL(string clientid)
        {
            string strMDB = "[TCRMMAIL]";
            string strSql = "";
            string strSqlGd = "";
            string strSqlGd1 = "";
            string strWhere = ""; //查询条件
            string strMyMailBoxId = "8,11,29,15,22";
            string strClientEmailList = getStrEmailClientSqlCONTAINS(clientid);
            if(strMyMailBoxId != "0") strSqlGd += " and MailBoxId in (" + strMyMailBoxId + ")";
          
            if (strClientEmailList.Trim() == "")
            {
                strSqlGd += $" and (InqClientId={clientid} OR GdClientId={clientid})";
            }
            else
            {
                strSqlGd1 = strSqlGd;
                strSqlGd1 += $" and (InqClientId={clientid} OR GdClientId={clientid})";
                strSqlGd += " and " + strClientEmailList + $" and isnull(InqClientId,0)<>{clientid} and isnull(GdClientId,0)<>{clientid}";
            }

            strSql = "select MailSyn,MailSynMailId,MailSynMailBoxId,MailSynBoxName,GdClientId,ClientId=isnull(InqClientId,ClientId),ClientAddId,ClientInfo=isnull(InqClientName,ClientInfo),ClientAddInfo,Inq,InqYfp,InqClientName,InqClientClass,InqCountry,InqSource,InqBak,InqProName,InqSaleUserName,InqDevUserName,InqFpUserName,InqFpTime,InqReTime,Bak,TopTime,Inside,RevUserName,MailBoxId,Id,FromName,FromEmail,Subject,itFrom,itTo,MailSize,AccCount,RecDate,[Read],ReadUID,ReadMode,priority,Box,BoxBase,star,redflag,ischeck,checkmsg,MailLabel,RE,REALFROM,ReturnPath,CreateTime,PfDate,IsDeal from " + strMDB + ".dbo.Mail_T left join " + strMDB + ".dbo.MailClient_T on Mail_T.Id=MailClient_T.ClientMailId left join " + strMDB + ".dbo.MailInq_T on Mail_T.Id=MailInq_T.MailId where 1=1 " + strSqlGd1 + strWhere;
            strSql += "\r\n union \r\nselect MailSyn,MailSynMailId,MailSynMailBoxId,MailSynBoxName,GdClientId,ClientId=isnull(InqClientId,ClientId),ClientAddId,ClientInfo=isnull(InqClientName,ClientInfo),ClientAddInfo,Inq,InqYfp,InqClientName,InqClientClass,InqCountry,InqSource,InqBak,InqProName,InqSaleUserName,InqDevUserName,InqFpUserName,InqFpTime,InqReTime,Bak,TopTime,Inside,RevUserName,MailBoxId,Id,FromName,FromEmail,Subject,itFrom,itTo,MailSize,AccCount,RecDate,[Read],ReadUID,ReadMode,priority,Box,BoxBase,star,redflag,ischeck,checkmsg,MailLabel,RE,REALFROM,ReturnPath,CreateTime,PfDate,IsDeal from " + strMDB + ".dbo.Mail_T left join " + strMDB + ".dbo.MailClient_T on Mail_T.Id=MailClient_T.ClientMailId left join " + strMDB + ".dbo.MailInq_T on Mail_T.Id=MailInq_T.MailId where 1=1 " + strSqlGd + strWhere;

            return strSql;

        }



        /// <summary>
        /// 取客户的邮箱串SQL(CONTAINS)全文检索专用
        /// </summary>
        /// <param name="strCrmId"></param>
        /// <param name="bladd">是否联系人ID,默认是false,取用客户ID</param>
        /// <returns></returns>
        private  string getStrEmailClientSqlCONTAINS(string strCrmId, bool bladd = false)
        {
            DBClass db = new DBClass();
            ArrayList al = new ArrayList();
            string strsql = "";
            string str = "";
            string strtemp = "";
            SqlCommand comm;
            if (bladd)
                comm = db.getSqlCommand("select Email,Email1,Email2 from ClientAdd_T where Id=" + strCrmId);
            else
                comm = db.getSqlCommand("select Email,Email1,Email2 from ClientAdd_T where ClientId=" + strCrmId);
            SqlDataReader dr = comm.ExecuteReader();
            while (dr.Read())
            {
                if (dr["Email"].ToString().Trim() != "")
                {
                    strtemp = "\"" + dr["Email"].ToString().Trim().Replace(",", "，").Replace("'", "‘") + "\" OR ";
                    if ((strtemp.Length + str.Length) > 4000)
                    {
                        al.Add(str.Substring(0, str.Length - 4));
                        str = "";
                    }
                    str += strtemp;
                }

                if (dr["Email1"].ToString().Trim() != "")
                {
                    strtemp = "\"" + dr["Email1"].ToString().Trim().Replace(",", "，").Replace("'", "‘") + "\" OR ";
                    if ((strtemp.Length + str.Length) > 4000)
                    {
                        al.Add(str.Substring(0, str.Length - 4));
                        str = "";
                    }
                    str += strtemp;
                }

                if (dr["Email2"].ToString().Trim() != "")
                {
                    strtemp = "\"" + dr["Email2"].ToString().Trim().Replace(",", "，").Replace("'", "‘") + "\" OR ";
                    if ((strtemp.Length + str.Length) > 4000)
                    {
                        al.Add(str.Substring(0, str.Length - 4));
                        str = "";
                    }
                    str += strtemp;
                }
            }
            dr.Close();
            if (str.Trim() != "")
            {
                al.Add(str.Substring(0, str.Length - 4));
                for (int i = 0; i < al.Count; i++)
                {
                    if (i == 0)
                        strsql += "CONTAINS(Es, '" + al[i].ToString() + "')";
                    else
                        strsql += " OR CONTAINS(Es, '" + al[i].ToString() + "')";
                }
                if (al.Count > 1) strsql = "(" + strsql + ")";
            }
            db.CloseConn();

            return strsql;
        }



    }
}