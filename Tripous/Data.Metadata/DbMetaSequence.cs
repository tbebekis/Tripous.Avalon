namespace Tripous;

public class DbMetaSequence : DbMetaObject
{
    public long CurrentValue { get; set; }
    public long InitialValue { get; set; }
    public long IncrementBy { get; set; }
}

 