using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppApi.Tools
{
    public class Where
    {
        public String wherestr = "";
        public void And(String str)
        {
            if (wherestr.Length > 0 && str != null && str.Length > 0)
                wherestr += " And ";
            wherestr += str;
        }
        public void Or(String str)
        {
            if (wherestr.Length > 0)
                wherestr += " Or ";
            wherestr += str;
        }
        public String ToWhere()
        {
            return " Where " + wherestr;
        }
        public void Like(string key, string value)
        {
            string likestr = string.Join(" like '%" + value + "%' Or ", key.Split(','));
            likestr += " like '%" + value + "%'";
            And("(" + likestr + ")");
        }
    }
}