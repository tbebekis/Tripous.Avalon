namespace Tripous
{
    /// <summary>
    /// Represents an item in an exchangable list
    /// </summary>
    public class ListerItem : IInExListerItem
    {
        /* construction */
        /// <summary>
        /// Constructor
        /// </summary>
        public ListerItem()
        {
        }
        /// <summary>
        /// Constructor
        /// </summary>
        public ListerItem(object Id, string Title)
            : this(Id, Title, null)
        {
        }
        /// <summary>
        /// Constructor
        /// </summary>
        public ListerItem(object Id, string Title, object Tag)
        {
            this.Id = Id;
            this.Title = Title;
            this.Tag = Tag;
        }

        /* public overrides */
        /// <summary>
        /// Override
        /// </summary>
        public override string ToString()
        {
            return Title;
        }


        /* properties */
        /// <summary>
        /// Gets the Id
        /// </summary>
        public object Id { get; set; }
        /// <summary>
        /// Gets the Title
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Gets the Tag
        /// </summary>
        public object Tag { get; set; }
    }
}
