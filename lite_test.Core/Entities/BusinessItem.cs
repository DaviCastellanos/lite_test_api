using lite_test.Core.Entities.Base;

namespace lite_test.Core.Entities
{
    public class BusinessItem : BaseEntity
    {
        /// <summary>
        ///     Business NIT
        /// </summary>
        public string NIT { get; set; }
        /// <summary>
        ///     Business Name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        ///     Business address
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        ///     Business phone number
        /// </summary>
        public string Phone { get; set; }


        /// <summary>
        ///     Whether the To-Do-Item is done
        /// </summary>
        public bool IsCompleted { get; private set; }

        public void MarkComplete()
        {
            IsCompleted = true;
        }

        public override string ToString()
        {
            string status = IsCompleted ? "Done!" : "Not done.";
            return $"{Id}: Status: {status} - {Name}";
        }
    }
}
