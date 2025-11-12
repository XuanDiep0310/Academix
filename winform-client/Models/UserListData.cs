namespace Academix.WinApp.Models
{
    public class UserListData
    {
        public List<UserData> Users { get; set; }
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}