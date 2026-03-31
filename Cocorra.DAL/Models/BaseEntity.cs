using System.ComponentModel.DataAnnotations.Schema;

namespace Cocorra.DAL.Models
{
    public abstract class BaseEntity
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("UpdateAt")] // Preserves DB column name to avoid migration
        public DateTime? UpdatedAt { get; set; }
    }
}