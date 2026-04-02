using Microsoft.Extensions.Options;

namespace Infrastructure.Caching.Memory;

public class MemoryCacheService<TKey, TValue> : IMemoryCacheService<TKey, TValue>
{
    private readonly Dictionary<TKey, Node<TKey, TValue>> _map;
    private readonly DoublyLinkedList<TKey, TValue> _list;
    private readonly MemoryCacheOptions _options;

    public MemoryCacheService(IOptions<MemoryCacheOptions> options)
    {
        _options = options.Value;
        _map = new Dictionary<TKey, Node<TKey, TValue>>();
        _list = new DoublyLinkedList<TKey, TValue>();
    }

    public void Set(TKey key, TValue value)
    {
        if (!_map.ContainsKey(key))
        {
            if (IsFull())
            {
                var tail = _list.RemoveTail();
                if (tail != null)
                    _map.Remove(tail.Key);
            }

            var node = new Node<TKey, TValue> { Key = key, Value = value };
            _map.Add(node.Key, node);
            _list.AddToHead(node);
        }
        else
        { 
            var node = _map[key]!;
            node.Value = value;
            _list.MoveToHead(node);
        }
    }

    public TValue? Get(TKey key)
    {
        if (_map.TryGetValue(key, out var node))
        {
            _list.MoveToHead(node);
            return node.Value;
        }
        return default;
    }

    private bool IsFull() => _map.Count >= _options.Capacity;
}