using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppApi.Cache.Core
{
    interface ICacheService
    {
        void Insert(object key, object value);
        void Get(object key, object value);
        void Update(object key, object value);
    }
}
