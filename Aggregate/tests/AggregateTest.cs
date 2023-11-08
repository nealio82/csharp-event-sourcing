using Aggregate.tests.Doubles;
using Xunit;

namespace Aggregate.tests;

public class AggregateTest
{
    [Fact]
    public void AggregateWithNoEventsHasVersionZero()
    {
        SampleAggregate aggregate = new SampleAggregate();

        Assert.Equal(0, aggregate.Version());
    }

    [Fact]
    public void AggregateVersionIncrementsWhenEventsAreRaised()
    {
        SampleAggregate aggregate = new SampleAggregate();

        aggregate.Raise(new FirstEvent());

        Assert.Equal(1, aggregate.Version());

        aggregate.Raise(new SecondEvent());

        Assert.Equal(2, aggregate.Version());
    }

    [Fact]
    public void AggregateEventIsAppliedToAggregateObject()
    {
        SampleAggregate aggregate = new SampleAggregate();

        Assert.False(aggregate.WasApplied(new FirstEvent()));

        aggregate.Raise(new FirstEvent());

        Assert.True(aggregate.WasApplied(new FirstEvent()));
        Assert.False(aggregate.WasApplied(new SecondEvent()));

        aggregate.Raise(new SecondEvent());

        Assert.True(aggregate.WasApplied(new SecondEvent()));
    }

    [Fact]
    public void ExceptionIsThrownWhenApplyMethodIsNotImplemented()
    {
        SampleAggregate aggregate = new SampleAggregate();

        var exception = Assert.Throws<NoAggregateEventApplicationMethodException>(
            () => aggregate.Raise(new UnhandledEvent())
        );

        Assert.Equal(
            "The SampleAggregate class does not have a callable method named ApplyUnhandledEvent",
            exception.Message
        );
    }

    [Fact]
    public void EventsAreFlushed()
    {
        SampleAggregate aggregate = new SampleAggregate();

        var firstEvent = new FirstEvent();
        var secondEvent = new SecondEvent();
        var thirdEvent = new ThirdEvent();

        aggregate.Raise(firstEvent);
        aggregate.Raise(secondEvent);
        aggregate.Raise(thirdEvent);

        List<AggregateEvent> events = new List<AggregateEvent>() { firstEvent, secondEvent, thirdEvent };

        Assert.Equivalent(events, aggregate.Flush(), true);
        Assert.Equal(3, aggregate.Version());
    }

    [Fact]
    public void PreviouslyFlushedEventsAreNotIncludedInSubsequentFlushes()
    {
        SampleAggregate aggregate = new SampleAggregate();

        var firstEvent = new FirstEvent();
        var secondEvent = new SecondEvent();
        var thirdEvent = new ThirdEvent();

        aggregate.Raise(firstEvent);
        aggregate.Raise(secondEvent);
        aggregate.Raise(thirdEvent);

        aggregate.Flush();

        Assert.Equivalent(new List<AggregateEvent>(), aggregate.Flush(), true);
    }

    [Fact]
    public void AggregateIsRecreatedFromExistingEvents()
    {
        var firstEvent = new FirstEvent();
        var secondEvent = new SecondEvent();

        List<AggregateEvent> events = new List<AggregateEvent>() { firstEvent, secondEvent };

        SampleAggregate aggregate = (SampleAggregate)SampleAggregate.BuildFromEvents(events);

        Assert.True(aggregate.WasApplied(new FirstEvent()));
        Assert.True(aggregate.WasApplied(new SecondEvent()));
        Assert.Equal(2, aggregate.Version());
    }

    [Fact]
    public void AggregateRecreatedFromExistingEventsHasNothingToFlush()
    {
        var firstEvent = new FirstEvent();
        var secondEvent = new SecondEvent();

        List<AggregateEvent> events = new List<AggregateEvent>() { firstEvent, secondEvent };

        SampleAggregate aggregate = (SampleAggregate)SampleAggregate.BuildFromEvents(events);

        Assert.Equivalent(new List<AggregateEvent>(), aggregate.Flush(), true);
    }

    [Fact]
    public void NewEventsAreAddedAfterAggregateHasBeenRebuiltFromPrevious()
    {
        var firstEvent = new FirstEvent();
        var secondEvent = new SecondEvent();
        var thirdEvent = new ThirdEvent();

        List<AggregateEvent> events = new List<AggregateEvent>() { firstEvent, secondEvent };

        SampleAggregate aggregate = (SampleAggregate)SampleAggregate.BuildFromEvents(events);

        aggregate.Raise(thirdEvent);

        Assert.Equivalent(new List<AggregateEvent>() { thirdEvent }, aggregate.Flush(), true);
        Assert.Equal(3, aggregate.Version());
    }
}