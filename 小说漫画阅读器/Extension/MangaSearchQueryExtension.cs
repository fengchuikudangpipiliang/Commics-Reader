using System.Reflection;
using 小说漫画阅读器.JsonClass;

namespace 小说漫画阅读器.Controllers
{
    public static class MangaSearchQueryExtension
    {
        public static bool IsNull(this MangaSearchQuery option)
        {
            if (option == null)
                return true;

            return string.IsNullOrWhiteSpace(option.Title)
                && (string.IsNullOrEmpty(option.OriginalLanguage))
                && (option.AvailableTranslatedLanguages == null || option.AvailableTranslatedLanguages.Count == 0)
                && (string.IsNullOrEmpty(option.Status))
                && (option.ContentRatings == null || option.ContentRatings.Count == 0)
                && (option.Limit == null || option.Limit == 0)
                && (option.Offset == null || option.Offset == 0)
                && (string.IsNullOrEmpty(option.Order))
                && (option.Year == null||option.Year==0)
                && (option.IncludedTags == null || option.IncludedTags.Count == 0)
                && (option.ExcludedTags == null || option.ExcludedTags.Count == 0);
        }
        public static string GetQueryString(this MangaSearchQuery option)
        {
            string baseUrl = "";
            if (!option.IsNull())
            {
                if (!string.IsNullOrWhiteSpace(option.Title))
                {
                    baseUrl += "title=" + Uri.EscapeDataString(option.Title)+ "&";
                }
                if (!(string.IsNullOrEmpty(option.OriginalLanguage)))
                {
                    baseUrl += "originalLanguage[]=" + option.Title + "&";
                }
                if (!(option.AvailableTranslatedLanguages == null || option.AvailableTranslatedLanguages.Count == 0))
                {
                    foreach (var language in option.AvailableTranslatedLanguages)
                    {
                        baseUrl += "availableTranslatedLanguage[]=" + language + "&";
                    }
                }
                if (!(string.IsNullOrEmpty(option.Status)))
                {
                    baseUrl += "status[]=" + option.Status + "&";
                }
                if (!(option.ContentRatings == null || option.ContentRatings.Count == 0))
                {
                    foreach (var item in option.ContentRatings)
                    {
                        baseUrl += "contentRating[]=" + item + "&";
                    }
                }
                if(!(option.Limit == null || option.Limit == 0))
                {
                    baseUrl += "limit=" + option.Limit + "&";
                }
                if(!(option.Offset == null || option.Offset == 0))
                {
                    baseUrl += "offset=" + option.Offset + "&";
                }
                if (!(string.IsNullOrEmpty(option.Order)))
                {
                    string[] orders= option.Order.Split(',');
                    if(orders.Length!=2)
                    {
                        throw new Exception("参数不对");
                    }
                    baseUrl += $"order[{orders[0]}]={orders[1]}&";
                }
                if(!(option.Year == null || option.Year == 0))
                {
                    baseUrl += "year=" + option.Year + "&";
                }
                if(!(option.IncludedTags == null || option.IncludedTags.Count == 0))
                {
                    foreach (var item in option.IncludedTags)
                    {
                        baseUrl += $"includedTags[]={MangaTagDic.TagNameToId[item]}&";
                    }
                }
                if(!(option.ExcludedTags == null || option.ExcludedTags.Count == 0))
                {
                    foreach (var item in option.ExcludedTags)
                    {
                        baseUrl += $"excludedTags[]={MangaTagDic.TagNameToId[item]}&";
                    }
                }
                baseUrl= baseUrl.Remove(baseUrl.Length - 1);
            }
            return baseUrl;
        }
    }
}
