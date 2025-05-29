using Microsoft.EntityFrameworkCore;

namespace 小说漫画阅读器
{
    public class CancelController
    {
        
        private readonly CancellationTokenPool pool;
        public CancelController(CancellationTokenPool pool)
        {
            this.pool = pool;
        }
        public void Cancel(string key)
        {
            pool.Cancel(key);
        }
    }
}
