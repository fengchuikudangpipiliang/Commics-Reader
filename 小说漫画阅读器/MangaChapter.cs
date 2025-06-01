using System.ComponentModel.DataAnnotations;

namespace 小说漫画阅读器
{
    public class MangaChapter
    {

        //具体章节的id
        [Key]
        public string ChapterId {  get; set; }
        public string? Title {  get; set; }
        public string? Pages {  get; set; }
        public string BaseUrl { get; set; }
        public string Hash { get; set; }
        public List<ImgFile> imgFiles { get; set; } = new List<ImgFile>();
        //指的就是这本漫画的uuid
        public MangaWithUser MangaWithUser { get; set; }
    }
}
