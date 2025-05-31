using System.Text.Json.Nodes;
using 小说漫画阅读器.Controllers;

namespace 小说漫画阅读器.JsonClass
{
    public record MangaSearchTitleResponse
  (
      string? Result,
      string? Response,
      List<MangaData1>? Data,
      int? Limit,
      int? Offset,
      int? Total
  );

    public record MangaData1(
        string? Id,
        string? Type,
        MangaAttributes? Attributes,
        List<MangaRelationship>? Relationships
    );

    public record MangaAttributes(
        Title? Title,
        List<Dictionary<string, string>>? AltTitles,
        Dictionary<string, string>? Description,
        bool? IsLocked,
        Dictionary<string, string>? Links,
        string? OriginalLanguage,
        string? LastVolume,
        string? LastChapter,
        string? PublicationDemographic,
        string? Status,
        int? Year,
        string? ContentRating,
        List<MangaTag>? Tags,
        string? State,
        bool? ChapterNumbersResetOnNewVolume,
        DateTime? CreatedAt,
        DateTime? UpdatedAt,
        int? Version,
        List<string>? AvailableTranslatedLanguages,
        string? LatestUploadedChapter
    );

    public record Title(
        string? En
    );

    public record MangaTag(
        string? Id,
        string? Type,
        TagAttributes? Attributes
    );

    public record TagAttributes(
        Dictionary<string, string>? Name,
        Dictionary<string, string>? Description,
        string? Group,
        int? Version
    );

    public record MangaRelationship(
        string? Id,
        string? Type,
        string? Related,
        JsonObject? Attributes
    );

    public record CoverArtAttributes1(
        string? Description,
        string? Volume,
        string? FileName,
        string? Locale,
        DateTime? CreatedAt,
        DateTime? UpdatedAt,
        int? Version
    );
    public record PersonAttributes(
    string? Name,
    string? ImageUrl,
    Dictionary<string, string>? Biography,
    string? Twitter,
    string? Pixiv,
    string? MelonBook,
    string? FanBox,
    string? Booth,
    string? Namicomi,
    string? NicoVideo,
    string? Skeb,
    string? Fantia,
    string? Tumblr,
    string? Youtube,
    string? Weibo,
    string? Naver,
    string? Website,
    DateTime? CreatedAt,
    DateTime? UpdatedAt,
    int? Version
);

}
