using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace AppApi.Tools
{
    public class SqlClass
    {
        public  string this[Select s,Where w,Order o]
        {
            get { return s.ToString() + w.ToString() + o.ToString(); }
        }
        public string this[Update u, Where w]
        {
            get { return u.ToString() + w.ToString(); }
        }
        public string Page(int pageindex,int pagemax,Select S,Where W,Order O)
        {
            Select Ss = S;
            Ss.Add("ROW_NUMBER() OVER ("+O.ToString()+") AS RowNumber");
            string sql = string.Format("Select Top {0} * from ({1}) as temp where RowNumber > {0} * ({2} - 1 ) order by RowNumber", pagemax, Ss.ToString() + W.ToString(), pageindex);
            return sql;
        }
        public string Page(int pageindex, int pagemax, string S, string W, string O)
        {
            string sql = string.Format("Select Top {0} * from ({1}) as temp where RowNumber > {0} * ({2} - 1 ) order by RowNumber", pagemax, S.ToString() + W.ToString(), pageindex);
            return sql;
        }
        public class Select
        {
            private List<string> selectstr =new List<string>();
            private string tablename = "";
            public Select(string tablename) {
                this.tablename = tablename;
            }
            public override string ToString() {
                if (selectstr.Count > 0 && tablename != "")
                    return "Select " + string.Join(",", selectstr.ToArray()) +" From " + tablename;
                else if (tablename != "")
                    return "Select * From " + tablename;
                return "";            
            }
            public void Add(object obj)
            {
                Type t = obj.GetType();
                PropertyInfo[] PropertyList = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (PropertyInfo item in PropertyList)
                {
                    string name = item.Name;
                    selectstr.Add("[" + name + "]");
                }
            }
            public void Add(string str)
            {
                selectstr.Add(str);
            }
            public void Add(List<string> lst)
            {
                selectstr.AddRange(lst);
            }
            public void SetTable(string tbn)
            {
                this.tablename = tbn;
            }
            public void Addtable(string tbn)
            {
                this.tablename +="," +tbn;
            }
            public void Addtablejoin(string tbn)
            {
                this.tablename +=tbn;
            }
            public Select Count(String str)
            {
                Select ns = new Select(tablename);
                ns.Add(str+"=count(*)");
                ns.SetTable(this.tablename);
                return ns;
            }
        };
        public class Where
        {
            private string wherestr = "";
            public override string ToString()
            {
                if(wherestr.Length>0)
                return " where " + wherestr;
                return "";
            }
            public void And(string cdt)
            {
                if (this.wherestr.Length > 0 && cdt.Length > 0) this.wherestr += " And ";
                this.wherestr += cdt;
            }
            public void Or(string cdt)
            {
                if (this.wherestr.Length > 0 && cdt.Length > 0) this.wherestr += " Or ";
                this.wherestr += cdt;
            }
            public void Like(string key, string value)
            {
                string likestr = string.Join(" like '%" + value + "%' Or ", key.Split(','));
                likestr += " like '%" + value + "%'";
                And("("+likestr + ")");
            }
        };
        public class Order
        {
            private List<string> orderstr = new List<string>();   
            public override string ToString()
            {
                if(orderstr.Count>0)
                return " Order By " + string.Join(",", orderstr.ToArray());
                return "";
            }
            public void Desc(string fd)
            {
                orderstr.Add(fd + " " + "Desc");
            }
            public void Asc(string fd)
            {
                orderstr.Add(fd + " " + "Asc");
            }
            public void Add(string str)
            {
                orderstr.Add(str);
            }

        };

        public class Update {
            private string tablename = "";
            private Dictionary<string, string> Updatedic = new Dictionary<string, string>();
            public Update(string tablename)
            {
                this.tablename = tablename;
            }
            public override string ToString()
            {
                List<String> l = new List<string>();
                foreach (string key in Updatedic.Keys)
                {
                    l.Add(key + "=" + Updatedic[key]);
                }
                if(l.Count >0)
                return "Update " + this.tablename + " set " + string.Join(",",l.ToArray());
                return "";
            }
            public void Revise(object obj)
            {

                Type t = obj.GetType();
                PropertyInfo[] PropertyList = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (PropertyInfo item in PropertyList)
                {
                    string name = item.Name;
                    name = name.ToUpper();
                    if (item.GetValue(obj) != null)
                    { 
                        if(item.PropertyType.FullName.ToLower().Contains("system.int"))
                        Revise(name, (int)item.GetValue(obj));
                        else
                        Revise(name, (string)item.GetValue(obj));
                    }
                }
            }
            public void Revise(string key, int value)
            {
                key = key.ToUpper();
                if (Updatedic.ContainsKey(key))
                    Updatedic.Add(key, value.ToString());
                else
                    Updatedic[key] = value.ToString();
            }
            public void Revise(string key, string value)
            {
                key = key.ToUpper();
                if (Updatedic.ContainsKey(key)) Updatedic.Add(key, "'"+value+"'"); else Updatedic[key] ="'"+ value + "'";
            }
            public void Revise(string key, DateTime value)
            {
                key = key.ToUpper();
                if (Updatedic.ContainsKey(key)) Updatedic.Add(key, "'" + value.ToString() + "'"); else Updatedic[key] = "'" + value.ToString() + "'";
            }
            public void Revise(string key, byte[] value)
            {
                key = key.ToUpper();
                if (Updatedic.ContainsKey(key)) Updatedic.Add(key, "'" + value.ToString() + "'"); else Updatedic[key] = "'" + value.ToString() + "'";
            }
            public void Remove(string key)
            {
                key = key.ToUpper();
                Updatedic.Remove(key);
            }
            public void Remove(object obj)
            {
                Type t = obj.GetType();
                PropertyInfo[] PropertyList = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (PropertyInfo item in PropertyList)
                {
                    string name = item.Name;
                    name = name.ToUpper();
                    Remove(name);
                }
            }
        }
        public class Insert {
            private string tablename = "";
            private Dictionary<string, string> Updatedic = new Dictionary<string, string>();
            public Insert (string tablename)
            {
                this.tablename = tablename;
            }
            public override string ToString()
            {
                List<String> l = new List<string>();
                List<String> v = new List<string>();
                foreach (string key in Updatedic.Keys)
                {
                    l.Add(key);
                    v.Add(Updatedic[key]);
                }
                if (l.Count > 0)
                    return "insert into " + this.tablename + "(" + string.Join(",", l.ToArray()) + ")"+ " values(" + string.Join(",", v.ToArray()) + ") select id=@@IDENTITY";
                return "";
            }
            public void Revise(object obj)
            {

                Type t = obj.GetType();
                PropertyInfo[] PropertyList = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (PropertyInfo item in PropertyList)
                {
                    string name = item.Name;
                    name = name.ToUpper();
                    if (item.GetValue(obj) != null)
                    {
                        if (item.PropertyType.FullName.ToLower().Contains("system.int"))
                            Revise(name, (int)item.GetValue(obj));
                        else
                            Revise(name, (string)item.GetValue(obj));
                    }
                }
            }
            public void Revise(string key, int value)
            {
                key = key.ToUpper();
                if (Updatedic.ContainsKey(key))
                    Updatedic.Add(key, value.ToString());
                else
                    Updatedic[key] = value.ToString();
            }
            public void Revise(string key, string value)
            {
                key = key.ToUpper();
                if (Updatedic.ContainsKey(key)) Updatedic.Add(key, "'" + value + "'"); else Updatedic[key] = "'" + value + "'";
            }
            public void Revise(string key, DateTime value)
            {
                key = key.ToUpper();
                if (Updatedic.ContainsKey(key)) Updatedic.Add(key, "'" + value.ToString() + "'"); else Updatedic[key] = "'" + value.ToString() + "'";
            }
            public void Revise(string key, byte[] value)
            {
                key = key.ToUpper();
                if (Updatedic.ContainsKey(key)) Updatedic.Add(key, "'" + value.ToString() + "'"); else Updatedic[key] = "'" + value.ToString() + "'";
            }
            public void Remove(string key)
            {
                key = key.ToUpper();
                Updatedic.Remove(key);
            }
            public void Remove(object obj)
            {
                Type t = obj.GetType();
                PropertyInfo[] PropertyList = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (PropertyInfo item in PropertyList)
                {
                    string name = item.Name;
                    name = name.ToUpper();
                    Remove(name);
                }
            }
        }
    }
}