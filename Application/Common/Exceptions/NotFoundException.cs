namespace Application.Common.Exceptions;

public class NotFoundException<T> : Exception where T : class
{
    public NotFoundException(object key)
        : base($"{typeof(T).Name} with key '{key}' was not found.")
    {
    }
}
