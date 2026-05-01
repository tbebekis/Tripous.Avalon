namespace Tripous.Data;

public interface IRowProvider
{
    DataRow CurrentRow { get; }
    event EventHandler CurrentRowChanged;
}