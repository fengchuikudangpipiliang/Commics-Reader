namespace 小说漫画阅读器.JsonClass
{
    public class ChapterResponse
    {
        public string? Result { get; set; }
        public string? Response { get; set; }
        public List<ChapterData>? Data { get; set; }
        public int? limit {  get; set; }
        public int? offset {  get; set; }
        public int? total { get; set; }
    }

    public class ChapterData
    {
        public string? Id { get; set; }
        public string? Type { get; set; } // "chapter"
        public ChapterAttributes? Attributes { get; set; }
        public List<ChapterRelationship>? Relationships { get; set; }
    }

    public class ChapterAttributes
    {
        public string? Volume { get; set; }
        public string? Chapter { get; set; } // 必须是 string
        public string? Title { get; set; }
        public string? TranslatedLanguage { get; set; }
        public string? ExternalUrl { get; set; }
        public DateTime? PublishAt { get; set; }
        public DateTime? ReadableAt { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? Pages { get; set; }
        public int? Version { get; set; }
    }

    public class ChapterRelationship
    {
        public string? Id { get; set; }
        public string? Type { get; set; } // "manga", "user", "scanlation_group"
    }

}
