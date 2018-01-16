using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace AppApi.Tools
{
    public class Back
    {
        public static String ToJson(String backjson,String backCode)
        {
            if (backCode != Tools.BackCode.Success)
                backjson = "\"" + Tools.BackCode.CodeStr[backCode] + "\"";
            return "{" + String.Format("\"back\":{0},\"code\":\"{1}\",\"timestamp\":\"{2}\"",backjson,backCode,GetTimeStamp()) + "}";
        }
        /// <summary>  
        /// 获取时间戳  
        /// </summary>  
        /// <returns></returns>  
        public static string GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }
        public static String ToJson(object obj)
        {
            String json = "{";
            Type t = obj.GetType();
            PropertyInfo[] PropertyList = t.GetProperties();
            foreach (PropertyInfo item in PropertyList)
            {
                string name = item.Name;
                object value = item.GetValue(obj, null);
                json += String.Format("\"{0}\":\"{1}\"",name,value);
            }
            json +="}";
            return json;
        }
    }
}