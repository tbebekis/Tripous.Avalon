namespace Tripous.Data
{

    /// <summary>
    /// Represents a schema item, actually only table and view is supported. 
    /// </summary>
    public class SchemaItem  
    {
 
        /// <summary>
        /// Constructor
        /// </summary>
        public SchemaItem()
        {
        }

        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        public override string ToString()
        { 
            if (!string.IsNullOrWhiteSpace(Name))
                return this.Name; 

            return base.ToString();
        }

        /// <summary>
        /// The name of this table or view schema item.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the sql statement text of the item.
        /// </summary>
        public string SqlText { get; set; }
    }
}
