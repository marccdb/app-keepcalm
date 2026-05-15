using System;
using KeepCalm.Models.Base;

namespace KeepCalm.Models.Entities
{
    public class MicroStep : Entity
    {
        public Guid TaskId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsCompleted { get; set; } = false;
        public int OrderIndex { get; set; } = 0;
    }
}
