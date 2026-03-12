using System;

namespace Rased.Core.Entities
{
    public class Category
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;

        public virtual ICollection<Complaint> Complaints { get; set; } = new List<Complaint>();
    }
}
