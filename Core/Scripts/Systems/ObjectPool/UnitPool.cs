using System.Collections.Generic;
using System.Linq;

namespace Defong.ObjectPool
{
    public class UnitPool
    {
        private readonly Dictionary<string, Queue<object>> _viewPoolDict = new Dictionary<string, Queue<object>>();

        public void AddViewByName(string prefabName, object mapItemView)
        {
            if (!_viewPoolDict.ContainsKey(prefabName))
            {
                _viewPoolDict.Add(prefabName, new Queue<object>());
            }

            _viewPoolDict[prefabName].Enqueue(mapItemView);
        }

        public T GetViewByName<T>(string prefabName)
        {
            if (_viewPoolDict.ContainsKey(prefabName))
            {
                var queue = _viewPoolDict[prefabName];
                if (queue.Count > 0)
                {
                    return (T) queue.Dequeue();
                }
            }

            return default(T);
        }

        public void RemoveFromPool(string prefabName, object mapItemView)
        {
            if (_viewPoolDict.ContainsKey(prefabName))
            {
                var queue = _viewPoolDict[prefabName];
                if (queue.Count > 0)
                {
                    _viewPoolDict[prefabName] = new Queue<object>(queue.Where(s => s != mapItemView));
                }
            }
        }

        public IEnumerable<T> GetAndRemoveFromPool<T>() where T : class
        {
            List<T> views = new List<T>();
            foreach (var queue in _viewPoolDict.Values)
            {
                views.AddRange(queue.ToList().Select(v=>v as T));
            }
                
            _viewPoolDict.Clear();
            return views;
        }
    }
}