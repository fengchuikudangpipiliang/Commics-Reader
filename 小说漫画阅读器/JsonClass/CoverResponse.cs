namespace 小说漫画阅读器.JsonClass
{
    public class CoverResponse
    {
        public List<MangaData>? data { get; set; }
    }

    public class MangaData
    {
        public string? id { get; set; }
        public List<Relationship>? relationships { get; set; }
    }

    public class Relationship
    {
        public string? id { get; set; }
        public string? type { get; set; }
        public CoverArtAttributes? attributes { get; set; } // 仅封面图才会有 attributes
    }

    public class CoverArtAttributes
    {
        public string? fileName { get; set; }
    }

}
