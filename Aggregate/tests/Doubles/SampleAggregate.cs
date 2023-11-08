namespace Aggregate.tests.Doubles;

public class SampleAggregate : Aggregate<SampleAggregate>
{
    private List<string> _appliedEvents = new List<string>();

    
    public void ApplyFirstEvent(FirstEvent aggregateEvent)
    {
        _appliedEvents.Add("FirstEvent");
    }
    
    public void ApplySecondEvent(SecondEvent aggregateEvent)
    {
        _appliedEvents.Add("SecondEvent");
    }
    
    public void ApplyThirdEvent(ThirdEvent aggregateEvent)
    {
        _appliedEvents.Add("ThirdEvent");
    }
    
    public bool WasApplied(AggregateEvent aggregateEvent)
    {
        foreach(string appliedEvent in _appliedEvents)
        {
            if (appliedEvent == aggregateEvent.GetType().Name)
            {
                return true;
            }
        }

        return false;
    }
}