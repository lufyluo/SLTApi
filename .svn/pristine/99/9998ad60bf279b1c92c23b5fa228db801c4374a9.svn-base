using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace AppApi.Models.File.Gain
{
    public class filed
    {
        public Int64 Id { get; set; }
        public int RootId { get; set; }
        public string Code { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string FileName { get; set; }
        public string FlashName { get; set; }
        public string FileRoot { get; set; }
        public int ServerId { get; set; }
        public string ConNo { get; set; }
        public string Bak { get; set; }
        public Int64 FileSize { get; set; }
        public string FileType { get; set; }
        public string FileMimeType { get; set; }
        public string Creater { get; set; }

        public string CreaterName { get; set; }
        public Nullable<DateTime> CreateTm { get; set; }
        public string Updater { get; set; }
        public string UpdaterName { get; set; }
        public Nullable<DateTime> UpdateTm { get; set; }
        public Nullable<int> Sort { get; set; }
        public string IdenCode { get; set; }
        public int DeptId { get; set; }

        public string DeptName { get; set; }


    }
    public class filelist : GainParameter
    {
        public int parentid { get; set; }
        public int PageIndex { get; set; }
        public int PageMax { get; set; }
        public string Act { get; set; }
        public String order { get; set; }
        public String Where { get; set; }
        public string Key
        {
            get; set;
        }
    }
    public class GetMenu
    {
        public DataTable menulist { get; set; }
        public IEnumerable<filed> filelist {get;set;}
    }
    public class fileback: BackList
    {
        public IEnumerable<filed> filelist { get; set; }
    }
}