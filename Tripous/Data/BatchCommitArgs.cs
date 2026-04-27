namespace Tripous.Data;

/// <summary>
/// Used with the TableSet   CommitBatch() method.
/// </summary>
public class BatchCommitArgs : EventArgs
{

    /* construction */
    /// <summary>
    /// Constructor
    /// <para>BeforeFunc: Provided by caller. It is called before each iteration.</para>
    /// <para>AfterFunc: Provided by caller. It is called after each iteration.
    /// The whole batch operation terminates when AfterFunc() returns false</para>
    /// <para>TransLimit: The transaction is commited each time the TransLimit is reached</para>
    /// </summary>
    public BatchCommitArgs(Func<bool> BeforeFunc, Func<object, bool> AfterFunc, int TransLimit = 300)
        : base()
    {
        this.BeforeFunc = BeforeFunc;
        this.AfterFunc = AfterFunc;
        this.TransLimit = TransLimit;
    }

    /* properties */
    /// <summary>
    /// The loop main counter. Just counts the iterations and increases in each iteration.
    /// <para>An iteration may end up in posting a row, or NOT.</para>
    /// <para>For an iteration to post, the BeforeFunc() must return true.</para>
    /// </summary>
    public int Counter { get; set; }
    /// <summary>
    /// The transaction is commited each time the TransLimit is reached
    /// </summary>
    public int TransLimit { get; private set; }
    /// <summary>
    /// It increases when an iteration posts a row. The BeforeFunc() should return true, for this to happen.
    /// <para>This is NOT the commit counter. It is the counter for the posts to the database
    /// and is used, along with the TransLimit, to decide if the Transaction.Commit() should be called.</para>
    /// <para>It is set by the TableSet, NOT the caller.</para>
    /// </summary>
    public int PostCounter { get; set; }
    /// <summary>
    /// Provided by caller. It is called before each and any iteration in the loop.
    /// <para>If it returns true, then the TableSet/Broker posts the row and increases the PostCounter counter.</para>
    /// <para>The caller should return true, only when calls Broker.Insert() or Broker.Edit() (NO Broker.Delete()) from inside this method.
    /// Returning true is the sign that a row post is required.</para>
    /// </summary>
    public Func<bool> BeforeFunc { get; private set; }
    /// <summary>
    /// Provided by caller. It is called after each (post or no-post) and any iteration in the loop.
    /// <para>The TableSet passes the LastCommitedId to this method.</para>
    /// <para>The whole batch operation terminates when this call returns false.</para>
    /// </summary>
    public Func<object, bool> AfterFunc { get; private set; }
    /// <summary>
    /// User defined value.
    /// </summary>
    public object Tag { get; set; }
}