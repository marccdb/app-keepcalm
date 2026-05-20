using System;
using System.ComponentModel.DataAnnotations;
using KeepCalm.Models.Entities;

namespace KeepCalm.DTOs
{
    using TStatus = TaskItemStatus;

    public class CreateTaskDto
    {
        [Required]
        [StringLength(200, MinimumLength = 1)]
        public string Title { get; set; } = string.Empty;

        [StringLength(2000)]
        public string? Description { get; set; }

        public TaskPriority Priority { get; set; } = TaskPriority.Normal;

        [StringLength(100)]
        public string? Folder { get; set; }
    }

    public class UpdateTaskDto
    {
        [StringLength(200, MinimumLength = 1)]
        public string Title { get; set; } = string.Empty;

        [StringLength(2000)]
        public string? Description { get; set; }

        public TaskPriority? Priority { get; set; }

        public TStatus? Status { get; set; }

        public List<string>? Tags { get; set; }
    }

    public class ReorderTaskDto
    {
        public int NewIndex { get; set; }
    }

    public class TaskDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public TaskPriority Priority { get; set; }
        public TStatus Status { get; set; }
        public int OrderIndex { get; set; }
        public List<string> Tags { get; set; } = new();
        public double Progress { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<MicroStepDto> MicroSteps { get; set; } = new();
    }

    public class StartFocusSessionDto
    {
        [Range(1, 120)]
        public int DurationMinutes { get; set; } = 25;

        public SessionType Type { get; set; } = SessionType.Focus;

        public Guid? TaskId { get; set; }
    }

    public class FocusSessionDto
    {
        public Guid Id { get; set; }
        public int DurationMinutes { get; set; }
        public long ElapsedSeconds { get; set; }
        public SessionStatus Status { get; set; }
        public SessionType Type { get; set; }
        public Guid? TaskId { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime? PausedAt { get; set; }
        public DateTime StartedAt { get; set; }
        public int RoundsCompleted { get; set; }
    }

    public class ErrorResponse
    {
        public string Error { get; set; } = string.Empty;
        public string? Details { get; set; }
    }
}
