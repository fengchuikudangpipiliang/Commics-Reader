using System.Collections.Concurrent;

namespace 小说漫画阅读器
{
    public class CancellationTokenPool
    {
        private readonly ConcurrentDictionary<string, CancellationTokenSource> _tokens = new();
        public CancellationToken GetToken(string key)
        {
            var cts = _tokens.GetOrAdd(key, (key) => new CancellationTokenSource());
            return cts.Token;
        }
        public void Cancel(string key)
        {
            if (_tokens.TryRemove(key, out var cts))
            {
                cts.Cancel();
                cts.Dispose();
            }
        }
        public void Reset(string key)
        {
            Cancel(key); // 取消并移除旧的
            //这里有可能有问题
            _tokens[key] = new CancellationTokenSource(); // 设置新的
        }
        public bool Contains(string key)
        {
            return _tokens.ContainsKey(key);
        }
    }
}
