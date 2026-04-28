namespace Tripous.Data
{

    /// <summary>
    /// The registry of database schemas.
    /// <para>A <see cref="Schema"/> has a unique domain name and a connection name. </para>
    /// <para>The <see cref="Schema.Domain"/> could be System, Application, or the unique identifier name of an external plugin. </para>
    /// <para>The <see cref="Schema.ConnectionName"/> connection name, used in creating the relevant <see cref="SqlStore"/> instance that executes the schema.</para>
    /// <para>A <see cref="Schema"/> has also a list of <see cref="SchemaVersion"/> items. </para>
    /// <para>A <see cref="SchemaVersion"/> item has an integer version,  
    /// and collections of <see cref="SchemaItem"/> items for tables and views,
    /// along with collections of statements to be executed before and after the schema version is executed.</para>
    /// </summary>
    static public class Schemas
    {
        /* construction */
        /// <summary>
        /// Static constructor
        /// </summary>
        static Schemas()
        {
        }

        /* public */
        /// <summary>
        /// Executes schemas, that is creates tables etc.
        /// </summary>
        static public void Execute()
        {
            foreach (var Schema in List)
                Schema.Execute();
        }

        /// <summary>
        /// Finds and returns a schema under a specified Domain and Connection name.
        /// <para>If the schema does not exist, then it adds the schema.</para>
        /// </summary>
        static public Schema FindOrAdd(string Domain, string ConnectionName)
        {
            Schema Result = Find(Domain, ConnectionName);
            if (Result == null)
            {
                Result = new Schema() { Domain = Domain, ConnectionName = ConnectionName };
                List.Add(Result);
            }
            return Result;
        }
        /// <summary>
        /// Returns a schema registered under a specified Domain name, if any, else null.
        /// </summary>
        static public Schema Find(string Domain, string ConnectionName)
        {
            return string.IsNullOrWhiteSpace(Domain) || string.IsNullOrWhiteSpace(ConnectionName)
                ? null
                : List.FirstOrDefault(item => item.Domain.IsSameText(Domain) && item.ConnectionName.IsSameText(ConnectionName));
        }
        /// <summary>
        /// Returns true if finds a schema registered under a specified Domain name,  else false.
        /// </summary>
        static public bool Exists(string Domain, string ConnectionName)
        {
            return Find(Domain, ConnectionName) != null;
        }

        /// <summary>
        /// Returns the schema registered under the Domain System and the Default Connection.
        /// </summary>
        static public Schema GetSystemSchema()
        {
            return FindOrAdd(Sys.SYSTEM, SysConfig.DefaultConnectionName);
        }
        /// <summary>
        /// Returns the schema registered under the Domain Application and the Default Connection.
        /// </summary>
        static public Schema GetApplicationSchema()
        {
            return FindOrAdd(Sys.APPLICATION, SysConfig.DefaultConnectionName);
        }

        /* properties */
        /// <summary>
        /// The list of registered schemas
        /// </summary>
        static public List<Schema> List { get; } = new List<Schema>();
 
    }
}
