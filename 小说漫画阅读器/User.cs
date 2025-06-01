namespace 小说漫画阅读器
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PassWord { get; set; }
        public List<MangaWithUser> Mangas { get; set; } = new List<MangaWithUser>();
        public List<UsersLove> UsersLove { get; set; }
    }
}
