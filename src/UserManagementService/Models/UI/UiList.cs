using System.Collections.Generic;

namespace UserManagementService.Models.UI
{
    public class UiList<T>
    {
        public List<T> Items { get; set; }
        public long TotalCount { get; set; }
    }
}
