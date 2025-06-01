using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
namespace 小说漫画阅读器.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DeepseekController : ControllerBase
    {
        private readonly HttpClient httpClient;
        public DeepseekController(IHttpClientFactory httpClientFactory)
        {
            this.httpClient=httpClientFactory.CreateClient("DeepseelClient");
        }
        /// <summary>
        /// 得到deepseek的回复
        /// </summary>
        /// <param name="query">用户的话</param>
        /// <param name="prompt">提示词</param>
        /// <returns></returns>
        [HttpGet]
        
        public async Task<ActionResult<string>> GetMsg([FromQuery]string query, [FromQuery] string prompt= "你是本漫画网站的 Q 萌看板娘“小绘”（18px 小人形象）• 始终用活泼可爱的语气回答，偶尔加上颜文字 (≧▽≦)• 回复长度保持 1～2 句，结尾附 1 个相关 emoji• 不讨论政治/敏感内容；遇到不合规问题礼貌拒绝喵")
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "sk-c29de8c5b1794732ab180534735d910c");
            var requestBody = new
            {
                model = "deepseek-chat",  // 使用DeepSeek-R1模型
                messages = new[]
                {    new { role="system",content=prompt},
                     new { role = "user", content = query }
                },
                stream = false
            };
          
            string json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await httpClient.PostAsync(
"/chat/completions", content);
            if(!response.IsSuccessStatusCode)
            {
                return BadRequest(response);
            }
            string responseJon= await response.Content.ReadAsStringAsync();
            JsonDocument json1=JsonDocument.Parse(responseJon);

            string res = "";
            foreach(var item in json1.RootElement.GetProperty("choices").EnumerateArray())
            {
                res += item.GetProperty("message").GetProperty("content").GetString();
            }
            return Ok(res);     
        }
    }
}
