using System;
using System.Collections.Generic;
using KeepCalm.Models.Base;

namespace KeepCalm.Models.Entities
{
    public class TaskItem : Entity
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public TaskPriority Priority { get; set; } = TaskPriority.Normal;
        public TaskItemStatus Status { get; set; } = TaskItemStatus.Pending;
        public int OrderIndex { get; set; } = 0;
        public List<string> Tags { get; set; } = new();
        public List<MicroStep> MicroSteps { get; set; } = new();

        public double GetProgress()
        {
            if (!MicroSteps.Any()) return TaskItemStatus.Completed == Status ? 100 : 0;
            var completed = MicroSteps.Count(s => s.IsCompleted);
            return (double)completed / MicroSteps.Count * 100;
        }

        public bool HasMicroSteps => MicroSteps.Any();
    }

    public enum TaskPriority
    {
        Urgent = 0,
        Important = 1,
        Normal = 2,
        Low = 3
    }

    public enum TaskItemStatus
    {
        Pending = 0,
        InProgress = 1,
        Completed = 2,
        Cancelled = 3
    }
}
