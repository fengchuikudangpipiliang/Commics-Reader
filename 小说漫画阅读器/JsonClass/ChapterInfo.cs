namespace 小说漫画阅读器.JsonClass
{
    public record ChapterInfo(string baseUrl,ChapterImageInfo chapter);
    public record ChapterImageInfo(string hash,List<string> data);

}
