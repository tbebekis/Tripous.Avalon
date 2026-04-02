using System;

namespace PivotTestApp;

 
public sealed class SalesLine
{
    public DateTime OrderDate { get; set; }
    public string Region { get; set; } = string.Empty;
    public string Segment { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Product { get; set; } = string.Empty;
    public double Sales { get; set; }
    public double Profit { get; set; }
    public int Quantity { get; set; }
}