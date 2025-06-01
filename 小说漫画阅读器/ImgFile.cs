namespace 小说漫画阅读器
{
    public class ImgFile
    {
        public int Id { get; set; }
        public string Name { get; set; }
        //对应章节的id
        public MangaChapter MangaChapter { get; set; }

    }
}
