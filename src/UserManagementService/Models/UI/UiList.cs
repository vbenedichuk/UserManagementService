using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserManagementService.Models.UI
{
    public class UiList<T>
    {
        public List<T> Items { get; set; }
        public long TotalCount { get; set; }
    }
}
