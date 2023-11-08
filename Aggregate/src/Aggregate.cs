namespace Aggregate;

public abstract class Aggregate<T>
{
    private int _versionNumber;

    private readonly List<AggregateEvent> _events = new();

    public int Version()
    {
        return _versionNumber;
    }

    public static Aggregate<T> BuildFromEvents(List<AggregateEvent> events)
    {
        var aggregate = (Aggregate<T>)Activator.CreateInstance(typeof(T))!;

        foreach (AggregateEvent aggregateEvent in events)
        {
            aggregate.Apply(aggregateEvent);
        }

        return aggregate;
    }

    public void Raise(AggregateEvent aggregateEvent)
    {
        Apply(aggregateEvent);
        _events.Add(aggregateEvent);
    }

    public List<AggregateEvent> Flush()
    {
        var events = _events.ToList();

        _events.Clear();
        
        return events;
    }

    private void Apply(AggregateEvent aggregateEvent)
    {
        var methodName = "Apply" + aggregateEvent.GetType().Name;

        var invocable = GetType().GetMethod(methodName);

        if (null == invocable)
        {
            throw new NoAggregateEventApplicationMethodException(
                "The " + GetType().Name + " class does not have a callable method named " + methodName
            );
        }

        invocable.Invoke(this, new Object[] { aggregateEvent });
        _versionNumber++;
    }
}