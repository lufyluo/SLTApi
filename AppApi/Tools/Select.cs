using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace AppApi.Tools
{
    public class Select
    {
        object obj;
        String TableName;
        public Select(object obj, String TableName)
        {
            this.obj = obj;
            this.TableName = TableName;
        }
        public String Get()
        {
            return GetField(obj) + " From " + TableName + " ";
        }
        private static String GetField(object obj)
        {
            String field = "";
            Type t = obj.GetType();
            PropertyInfo[] PropertyList = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo item in PropertyList)
            {
                string name = item.Name;
                field += "[" + name + "],";
            }
            field = field.Substring(0, field.Length - 1);
            return "Select " + field + " ";
        }
    }
}