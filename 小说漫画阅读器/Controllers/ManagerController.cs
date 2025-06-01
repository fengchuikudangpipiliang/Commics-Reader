using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace 小说漫画阅读器.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ManagerController : ControllerBase
    {
        private readonly CancelController _cancelController;

        public ManagerController(CancelController cancelController)
        {
            _cancelController = cancelController;
        }

        [HttpPost]
        public async Task<ActionResult> CancelAction(string action)
        {
            _cancelController.Cancel(action);
            await Task.Delay(TimeSpan.FromSeconds(0.1));
            return Ok("撤销成功");
        }
    }
}
