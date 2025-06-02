using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.ComponentModel;
using System.Net.Http;
using System.Text.Json;
using 小说漫画阅读器.JsonClass;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using static System.Net.WebRequestMethods;
namespace 小说漫画阅读器.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MangadexController : ControllerBase
    {
        private readonly HttpClient httpClient;                                  
        private readonly CancellationTokenPool cancellationTokenPool;
        public MangadexController(IHttpClientFactory factory, CancellationTokenPool cancellationTokenPool)
        {
            httpClient = factory.CreateClient("MangaClient");
            this.cancellationTokenPool = cancellationTokenPool;
        }

        /// <summary>
        /// 得到漫画封面的图片的方法,uuid+文件名,可以在.jpg前面加.256或者.512
        /// </summary>
        /// <param name="uuid">漫画的Uuid</param>
        /// <param name="fileName">漫画对应的封面文件名</param>
        /// <returns></returns>
        [HttpGet]
        [ResponseCache(Duration = 20,VaryByQueryKeys = new[] { "uuid","fileName" })]
        public async Task<ActionResult> GetCoverImg([FromQuery] string uuid, [FromQuery] string fileName)
        {
            //fileName = "0f411c81-14a6-4d59-9e57-869d839c4972.jpg";
            //uuid = "831b12b8-2d0e-4397-8719-1efee4c32f40";
            var response = await httpClient.GetAsync($"https://uploads.mangadex.org/covers/{uuid}/{fileName}");

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, "获取封面失败");
            }
            var contentType = response.Content.Headers.ContentType?.MediaType ?? "image/jpeg";
            var imageBytes = await response.Content.ReadAsByteArrayAsync();

            return File(imageBytes, contentType);
        }
        /// <summary>
        /// 根据漫画的uuid得到漫画的评分
        /// </summary>
        /// <param name="uuid">漫画的uuid</param>
        /// <returns></returns>
        [HttpGet]
        [ResponseCache(Duration = 20,VaryByQueryKeys = new[] { "uuid" })]
        public async Task<ActionResult<MangaStatisticsResponse>> GetCommetnById([FromQuery] string uuid)
        {
            //uuid = "0301208d-258a-444a-8ef7-66e433d801b1";

            var response = await httpClient.GetAsync($"/statistics/manga/{uuid}");
            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, "评分获取失败");
            }

            using var sJson = await response.Content.ReadAsStreamAsync();
            var json = await JsonSerializer.DeserializeAsync<MangaStatisticsResponse>(sJson);
            if (json != null && json.statistics.TryGetValue(uuid, out var stat))
            {
                return Ok(json);
            }

            return NotFound("Manga statistics not found");
        }
        /// <summary>
        /// 根据uuid来查找对应的所有章节信息，里面有最基本各个的章节id,volume是卷，chapter是章
        /// </summary>
        /// <param name="uuid">漫画的uuid</param>
        /// <returns></returns>
        [HttpGet]
        [ResponseCache(Duration = 20, VaryByQueryKeys = new[] { "uuid" })]
        public async Task<ActionResult<ChapterResponse>> GetChapterInfoById([FromQuery]string uuid)
        {
            if (string.IsNullOrWhiteSpace(uuid))
                return BadRequest("uuid 不能为空");
            string url = $"/manga/{uuid}/feed";
            var response = await httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, "获取失败");
            }

            using var sJson = await response.Content.ReadAsStreamAsync();
            //var sJson = await response.Content.ReadAsStringAsync();

            //if (sJson != null)
            //{
            //    return Ok(sJson);
            //}

            var json = JsonSerializer.Deserialize<ChapterResponse>(sJson, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

            if (json != null)
            {
                return Ok(json);
            }
            return NotFound("没有你想要的内容");
        }
        /// <summary>
        /// 根据章节id得到对应的图片内容
        /// </summary>
        /// <param name="chapterId">章节的uuid</param>
        /// <returns></returns>
        [HttpGet]
        [ResponseCache(Duration = 20, VaryByQueryKeys = new[] { "chapterId" })]
        public async Task<ActionResult<string>> GetChapterImg([FromQuery]string chapterId)
        {

            if (string.IsNullOrWhiteSpace(chapterId))
                return BadRequest("参数空缺");

            string GetChapterInfo = $"https://api.mangadex.org/at-home/server/{chapterId}";

            var response = await httpClient.GetAsync(GetChapterInfo);
            if (!response.IsSuccessStatusCode)
            {
                return NotFound("没找到这个章节的信息");
            }
            using var sJson = await response.Content.ReadAsStreamAsync();
            var json = JsonSerializer.Deserialize<ChapterInfo>(sJson, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

            var base64Images = new List<string>();
            if (json != null)
            {
                string baseUrl = json.baseUrl;
                string hash = json.chapter.hash;
                foreach (var fileName in json.chapter.data)
                {
                    string imageUrl = $"{baseUrl}/data/{hash}/{fileName}";
                    var imgResponse = await httpClient.GetAsync(imageUrl);
                    if (!imgResponse.IsSuccessStatusCode) continue;

                    var bytes = await imgResponse.Content.ReadAsByteArrayAsync();
                    string base64 = $"data:image/jpeg;base64,{Convert.ToBase64String(bytes)}";
                    base64Images.Add(base64);
                }
                return Ok(base64Images);
            }
            return NotFound("json为空");
        }
        /// <summary>
        /// 流式传输这个章节的原画质图片
        /// </summary>
        /// <param name="chapterId">章节的uuid</param>
        /// <returns></returns>
        [HttpGet]
        [ResponseCache(Duration = 20, VaryByQueryKeys = new[] { "chapterId" })]
        public async Task GetChapterImgStream([FromQuery] string chapterId)
        {
            CancellationToken token= cancellationTokenPool.GetToken("/api/Mangadex/GetChapterImgStream");
            if (string.IsNullOrWhiteSpace(chapterId))
            {
                Response.StatusCode = 400;
                await Response.WriteAsync("参数空缺");
                return ;
            }

            string url = $"https://api.mangadex.org/at-home/server/{chapterId}";
            var response = await httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                Response.StatusCode = 404;
                await Response.WriteAsync("章节信息未找到");
                return;
            }

            using var sJson = await response.Content.ReadAsStreamAsync();
            var chapterInfo = await JsonSerializer.DeserializeAsync<ChapterInfo>(sJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (chapterInfo == null)
            {
                Response.StatusCode = 500;
                await Response.WriteAsync("Json 解析失败");
                return;
            }

            string baseUrl = chapterInfo.baseUrl;
            string hash = chapterInfo.chapter.hash;

            Response.ContentType = "text/event-stream"; // 一行一个 JSON 字符串
            var writer = new StreamWriter(Response.Body);

            int i = 1;
            foreach (var fileName in chapterInfo.chapter.data)
            {
                if (token.IsCancellationRequested)
                {
                    Console.WriteLine("撤销陈工");
                    break;
                }
                string imageUrl = $"{baseUrl}/data/{hash}/{fileName}";
             
                // SSE 格式，每个事件以 "data:" 开头，"\n\n" 结尾
                await writer.WriteAsync($"data: {imageUrl}\n\n");
                await writer.FlushAsync(); // 推送这一张图片
            }
            await writer.WriteAsync("event: done\n");
            await writer.WriteAsync("data: complete\n\n");
            await writer.FlushAsync();
        }
        /// <summary>
        /// 终极版全能搜索，满足条件搜索
        /// </summary>
        /// <param name="option">一个条件参数的对象</param>
        /// <returns></returns>
        [HttpGet]

        public async Task<ActionResult<MangaSearchTitleResponse>> GetMangaByOptions([FromQuery] MangaSearchQuery option)
        {
            var baseUrl = "https://api.mangadex.org/manga";
            string query=option.GetQueryString();
            baseUrl += "?includes[]=cover_art&includes[]=author&includes[]=artist&"+ query;

            var response = await httpClient.GetAsync(baseUrl);
            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, "获取失败");
            }
            using var json1 = await response.Content.ReadAsStreamAsync();


            var json = JsonSerializer.Deserialize<MangaSearchTitleResponse>(json1, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

            if (json != null)
            {
                foreach (var rel in json.Data)
                {
                    foreach (var item in rel.Relationships)
                        if (item.Type == "cover_art" && item.Attributes != null)
                        {
                            var coverAttr = item.Attributes.Deserialize<CoverArtAttributes1>();
                            // 用 coverAttr.FileName
                        }
                        else if ((item.Type == "author" || item.Type == "artist") && item.Attributes != null)
                        {
                            var personAttr = item.Attributes.Deserialize<PersonAttributes>();
                            // 用 personAttr.Name
                        }
                }
                return Ok(json);
            }
            return NotFound("Manga statistics not found");
        }
        [HttpGet]
        public async Task<ActionResult<MangaSearchTitleResponse>>  GetRandomManga()
        {
            string baseUrl1= "https://api.mangadex.org/manga/random?includes[]=cover_art&includes[]=author&includes[]=artist";
            var response = await httpClient.GetAsync(baseUrl1);
            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, "获取失败");
            }
            var json1 = await response.Content.ReadAsStringAsync();
            JsonDocument json=JsonDocument.Parse(json1);
            var data1 = json.RootElement.GetProperty("data").GetProperty("attributes").GetProperty("title").GetProperty("en").GetString();

            MangaSearchQuery query = new MangaSearchQuery() { Title=data1,Limit=1};
            return  await GetMangaByOptions(query);
           
        }
        /// <summary>
        /// 由于传入uuid得到的不是漫画，而是实体，所以会缺少数据或者格式不匹配
        /// 但是可以拿到其他的信息，比如漫画的title
        /// 所以根据title模糊匹配判断所有的数据的uuid是否跟传入的uuid一样
        /// 若一样，则已经拿到，不需要再请求
        /// </summary>
        /// <param name="uuid"></param>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        [HttpGet]
        [ResponseCache(Duration = 20, VaryByQueryKeys = new[] { "uuid","limit","offset" })]
        public async Task<ActionResult<MangaSearchTitleResponse>> GetMangaById([FromQuery] string uuid, [FromQuery]int limit, [FromQuery]int offset)
        {
            string baseUrl1 = $"https://api.mangadex.org/manga/{uuid}?includes[]=cover_art&includes[]=author&includes[]=artist";
            var response = await httpClient.GetAsync(baseUrl1);
            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, "获取失败");
            }
            var json1 = await response.Content.ReadAsStringAsync();
            JsonDocument json = JsonDocument.Parse(json1);

            var data1 = json.RootElement.GetProperty("data").GetProperty("attributes").GetProperty("title").GetProperty("en").GetString();
            MangaSearchQuery query = new MangaSearchQuery() { Title = data1 ,Limit=limit,Offset=offset};
            return await GetMangaByOptions(query);
        }
    }
    public class MangaSearchQuery
    {
        public string? Title { get; set; }
        public string? OriginalLanguage { get; set; }
        public List<string>? AvailableTranslatedLanguages { get; set; }
        public string? Status { get; set; }
        public List<string>? ContentRatings { get; set; }
        public int? Year {  get; set; }
        public int? Limit { get; set; }
        public int? Offset { get; set; }
        public string? Order { get; set; } // e.g. order["createdAt"] = "desc"
        public List<string>? IncludedTags { get; set; }
        public List<string>? ExcludedTags { get; set; }
    }

    public class MangaStatisticsResponse
    {

        public Dictionary<string, MangaStat>? statistics { get; set; }
    }

    public class MangaStat
    {
        public Rating? rating { get; set; }
        public int? follows { get; set; }
    }

    public class Rating
    {
        public double? average { get; set; }
        public double? bayesian { get; set; }
    }
}
