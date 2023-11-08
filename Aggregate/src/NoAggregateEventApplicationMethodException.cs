using Microsoft.CSharp.RuntimeBinder;

namespace Aggregate;

public class NoAggregateEventApplicationMethodException : RuntimeBinderException
{
    public NoAggregateEventApplicationMethodException(string message) : base(message)
    {
    }
}