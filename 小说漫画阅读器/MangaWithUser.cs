namespace 小说漫画阅读器
{
    public class MangaWithUser
    {
        //漫画的uuid
        public string Id {  get; set; }
        public string? Title {  get; set; }
        public string? Description { get; set; }
        public string? Author {  get; set; }
        public string? Year {  get; set; }
        public string? Tags {  get; set; }
        public string? State {  get; set; }
        public string? CreateAt {  get; set; }
        public float? CommetRating { get; set; }
        public int? CommentPeople { get; set; }
        public string? CoverFileName { get; set; }
        //用户的id
        public User User { get; set; }
        public List<MangaChapter>? MangaChapters { get; set; }=new List<MangaChapter>();
    }
}
