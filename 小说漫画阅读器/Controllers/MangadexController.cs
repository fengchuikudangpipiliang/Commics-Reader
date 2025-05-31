using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel;
using System.Net.Http;
using System.Text.Json;
using 小说漫画阅读器.JsonClass;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
namespace 小说漫画阅读器.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MangadexController : ControllerBase
    {
        private readonly HttpClient httpClient;


        public MangadexController(IHttpClientFactory factory)
        {
            httpClient = factory.CreateClient("MangaClient");
        }

        /// <summary>
        /// 根据漫画标题模糊搜索 ID
        /// </summary>
        /// <param name="title">漫画标题关键字</param>
        /// <param name="limit">一次请求的个数</param>
        /// <returns>匹配到的漫画 ID 列表</returns>
        [HttpGet]
        [ResponseCache(Duration = 20)]
        //根据标题得到漫画id，模糊匹配
        public async Task<ActionResult<List<Guid>>> GetMangaIdByTitle([FromQuery] string title, [FromQuery] int limit = 10)
        {
            if (string.IsNullOrWhiteSpace(title))
                return BadRequest("title 不能为空");
            //string title = "Kanojyo to Himitsu to Koimoyou";
            string url = $"manga?title={Uri.EscapeDataString(title)}&limit={limit}";
            //string url = $"manga?title={title}& limit=100";
            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();

            using JsonDocument doc = JsonDocument.Parse(json);

            var ids = doc.RootElement.GetProperty("data")
                                 .EnumerateArray()
                                 .Select(m => m.GetProperty("id").GetGuid()) // GetGuid() 把 UUID 字符串转 Guid
                                 .ToList();

            return StatusCode(200, ids); // 会自动序列化为 JSON
        }


        /// <summary>
        /// 根据标签得到匹配的漫画uuid
        /// </summary>
        /// <param name="includedTagNames">要包括的标签</param>
        /// <param name="excludedTagNames">要去除的标签</param>
        ///  <param name="limit">一次请求的个数</param>
        /// <returns>匹配到的漫画 ID 列表</returns>
        [HttpGet]
        public async Task<ActionResult<List<string>>> GetMangaByTags([FromQuery] List<string> includedTagNames, [FromQuery] List<string> excludedTagNames, [FromQuery] int limit = 10)
        {
            // Step 1: 标签名称
            //var includedTagNames = new[] { "Action", "Romance" };
            //var excludedTagNames = new[] { "Harem" };

            // Step 2: 获取所有标签
            var tagResp = await httpClient.GetAsync("manga/tag");
            tagResp.EnsureSuccessStatusCode();
            var tagJson = await tagResp.Content.ReadAsStringAsync();


            using var tagDoc = JsonDocument.Parse(tagJson);
            var tagList = tagDoc.RootElement.GetProperty("data").EnumerateArray();

            // Step 3: 找到 UUID
            var includedTagIDs = tagList
                .Where(tag => includedTagNames.Contains(tag.GetProperty("attributes").GetProperty("name").GetProperty("en").GetString()))
                .Select(tag => tag.GetProperty("id").GetString())
                .ToList();

            var excludedTagIDs = tagList
                .Where(tag => excludedTagNames.Contains(tag.GetProperty("attributes").GetProperty("name").GetProperty("en").GetString()))
                .Select(tag => tag.GetProperty("id").GetString())
                .ToList();

            // Step 4: 构造查询字符串
            var queryParams = new List<string>();

            foreach (var id in includedTagIDs)
                queryParams.Add($"includedTags[]={id}");

            foreach (var id in excludedTagIDs)
                queryParams.Add($"excludedTags[]={id}");

            // limit 可选
            queryParams.Add($"limit={limit}");

            var queryString = string.Join("&", queryParams);
            var searchUrl = $"manga?{queryString}";

            // Step 5: 搜索漫画
            var searchResp = await httpClient.GetAsync(searchUrl);
            searchResp.EnsureSuccessStatusCode();
            var searchJson = await searchResp.Content.ReadAsStringAsync();

            // Step 6: 取 manga ID 列表
            using var searchDoc = JsonDocument.Parse(searchJson);
            var mangas = searchDoc.RootElement.GetProperty("data").EnumerateArray()
                .Select(m => m.GetProperty("id").GetString())
                .ToList();

            return mangas;
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
        /// 根据标题得到漫画的的所有信息（是漫画的所有信息加封面信息）
        /// </summary>
        /// <param name="title">想要搜索的漫画标题</param>
        /// <param name="limit">sss</param>
        /// <returns></returns>
        [HttpGet]
        [ResponseCache(Duration = 60, VaryByQueryKeys = new[] { "title","limit","offset"})]
        public async Task<ActionResult<MangaSearchTitleResponse>> GetMangaWithRelationship([FromQuery] string title, [FromQuery] int limit = 10, [FromQuery]int offset=0)
        {
            if (string.IsNullOrWhiteSpace(title))
                return BadRequest("title 不能为空");
            //string title = "Kanojyo to Himitsu to Koimoyou";
            string url = $"manga?title={Uri.EscapeDataString(title)}&offset={offset}&limit={limit}&includes[]=cover_art&includes[]=author&includes[]=artist";
            //string url = $"manga?title={title}& limit=100";
            var response = await httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, "获取失败");
            }
        

           using var json1 = await response.Content.ReadAsStreamAsync();


            var json = JsonSerializer.Deserialize<MangaSearchTitleResponse>(json1,new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

            if (json != null)
           {
                foreach (var rel in json.Data)
                {
                    foreach(var item in rel.Relationships)
                    if (item.Type == "cover_art" && rel.Attributes != null)
                    {
                        var coverAttr = item.Attributes.Deserialize<CoverArtAttributes1>();
                        // 用 coverAttr.FileName
                    }
                    else if ((rel.Type == "author" || rel.Type == "artist") && rel.Attributes != null)
                    {
                        var personAttr = item.Attributes.Deserialize<PersonAttributes>();
                        // 用 personAttr.Name
                    }
                }
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

            foreach (var fileName in chapterInfo.chapter.data)
            {
                string imageUrl = $"{baseUrl}/data/{hash}/{fileName}";
                var imgRes = await httpClient.GetAsync(imageUrl);

                if (!imgRes.IsSuccessStatusCode) continue;

                var bytes = await imgRes.Content.ReadAsByteArrayAsync();
                string base64 = $"data:image/jpeg;base64,{Convert.ToBase64String(bytes)}";

                // SSE 格式，每个事件以 "data:" 开头，"\n\n" 结尾
                await writer.WriteAsync($"data: {base64}\n\n");
                await writer.FlushAsync(); // 推送这一张图片
            }
            await writer.WriteAsync("event: done\n");
            await writer.WriteAsync("data: complete\n\n");
            await writer.FlushAsync();
        }
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
