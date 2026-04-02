namespace Infrastructure.Caching.Memory;

internal class DoublyLinkedList<TKey, TValue>
{
    private Node<TKey, TValue> _head;
    private Node<TKey, TValue> _tail;

    public Node<TKey, TValue> Head => _head;
    public Node<TKey, TValue> Tail => _tail;

    // new items
    public void AddToHead(Node<TKey, TValue> node)
    {
        node.Next = _head;
        node.Prev = null;

        if (_head != null)
            _head.Prev = node;

        _head = node;

        if (_tail == null)
            _tail = node;
    }

    // existing items
    public void MoveToHead(Node<TKey, TValue> node)
    {
        RemoveNode(node);
        AddToHead(node);
    }


    // Remove the tail node (least recently used)
    public Node<TKey, TValue>? RemoveTail()
    {
        if (_tail == null) return null;

        var tail = _tail;
        RemoveNode(tail);
        return tail;
    }

    private void RemoveNode(Node<TKey, TValue> node)
    {
        if (node.Prev != null)
            node.Prev.Next = node.Next;
        else
            _head = node.Next;

        if (node.Next != null)
            node.Next.Prev = node.Prev;
        else
            _tail = node.Prev;

        node.Next = null;
        node.Prev = null;
    }
}

internal class Node<TKey, TValue>
{
    public TKey Key { get; set; }
    public TValue Value { get; set; }

    public Node<TKey, TValue> Next { get; set; }
    public Node<TKey, TValue> Prev { get; set; }
}