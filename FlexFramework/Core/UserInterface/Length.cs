namespace FlexFramework.Core.UserInterface;

public enum Unit
{
    Pixel,
    Percent
}

public struct Length
{
    public static Length Zero { get; } = new Length(0.0f, Unit.Pixel);
    public static Length Full { get; } = new Length(1.0f, Unit.Percent);

    public float Value { get; set; }
    public Unit Unit { get; set; }
    
    public Length(float value, Unit unit)
    {
        Value = value;
        Unit = unit;
    }
    
    public float Calculate(float parentSize)
    {
        return Unit switch
        {
            Unit.Pixel => Value,
            Unit.Percent => parentSize * Value,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
    
    public static Length operator +(Length a, Length b)
    {
        if (a.Unit != b.Unit)
        {
            throw new InvalidOperationException("Cannot add lengths with different units");
        }
        
        return new Length(a.Value + b.Value, a.Unit);
    }
    
    public static Length operator -(Length a, Length b)
    {
        if (a.Unit != b.Unit)
        {
            throw new InvalidOperationException("Cannot subtract lengths with different units");
        }
        
        return new Length(a.Value - b.Value, a.Unit);
    }
    
    public static Length operator *(Length a, float b)
    {
        return new Length(a.Value * b, a.Unit);
    }
    
    public static Length operator *(float a, Length b)
    {
        return new Length(a * b.Value, b.Unit);
    }
    
    public static Length operator /(Length a, float b)
    {
        return new Length(a.Value / b, a.Unit);
    }
    
    public static Length operator /(float a, Length b)
    {
        return new Length(a / b.Value, b.Unit);
    }
    
    public static Length operator -(Length a)
    {
        return new Length(-a.Value, a.Unit);
    }
    
    public static bool operator ==(Length a, Length b)
    {
        return a.Value == b.Value && a.Unit == b.Unit;
    }
    
    public static bool operator !=(Length a, Length b)
    {
        return !(a == b);
    }
    
    public override bool Equals(object? obj)
    {
        return obj is Length length && this == length;
    }
    
    public override int GetHashCode()
    {
        return HashCode.Combine(Value, Unit);
    }
    
    public override string ToString()
    {
        return $"{Value} {Unit}";
    }
    
    public static implicit operator Length(float value)
    {
        return new Length(value, Unit.Pixel);
    }
}