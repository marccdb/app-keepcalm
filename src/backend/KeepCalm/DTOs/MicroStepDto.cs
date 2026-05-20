using System.ComponentModel.DataAnnotations;
using KeepCalm.Models.Entities;

namespace KeepCalm.DTOs
{
    public class MicroStepDto
    {
        public Guid Id { get; set; }
        public Guid TaskId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsCompleted { get; set; }
        public int OrderIndex { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateMicroStepDto
    {
        [Required]
        [StringLength(200, MinimumLength = 1)]
        public string Title { get; set; } = string.Empty;

        [StringLength(2000)]
        public string? Description { get; set; }
    }

    public class UpdateMicroStepDto
    {
        [StringLength(200, MinimumLength = 1)]
        public string? Title { get; set; }

        [StringLength(2000)]
        public string? Description { get; set; }

        public bool? IsCompleted { get; set; }
    }

    public class ReorderMicroStepsDto
    {
        public List<int> OrderIndices { get; set; } = new();
    }
}
