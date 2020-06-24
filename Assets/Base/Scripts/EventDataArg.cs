using System;

public class EventDataArg<T> : EventArgs
{
    public T Value { get { return Value; } }
    protected T value;

    public EventDataArg(T value)
    {
        this.value = value;
    }
}