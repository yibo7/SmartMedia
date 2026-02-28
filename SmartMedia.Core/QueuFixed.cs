namespace SmartMedia.Core;

/// <summary>
/// 可以保留最后N条的固定长度的队列
/// </summary>
/// <typeparam name="T"></typeparam>
public class QueuFixed<T> : Queue<T>
{
    private int _maxSize;

    public QueuFixed(int maxSize)
    {
        _maxSize = maxSize;
    }

    public new void Enqueue(T item)
    {
        while (Count >= _maxSize)
        {
            Dequeue();
        }
        base.Enqueue(item);
    }
}
