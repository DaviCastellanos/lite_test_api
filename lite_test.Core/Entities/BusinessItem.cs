using lite_test.Core.Entities;

namespace lite_test.Core.Entities
{
    public class BusinessItem : BaseEntity
    {
        public string NIT { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public string Phone { get; set; }

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
