using System;
using KeepCalm.Models.Base;

namespace KeepCalm.Models.Entities
{
    public class FocusSession : Entity
    {
        public int DurationMinutes { get; set; } = 25;
        public long ElapsedSeconds { get; set; } = 0;
        public SessionStatus Status { get; set; } = SessionStatus.Idle;
        public SessionType Type { get; set; } = SessionType.Focus;
        public Guid? TaskId { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime? PausedAt { get; set; }
        public DateTime StartedAt { get; set; } = DateTime.UtcNow;
        public int RoundsCompleted { get; set; } = 0;

        public void Start()
        {
            Status = SessionStatus.Running;
            StartedAt = DateTime.UtcNow;
            UpdateTimestamp();
        }

        public void Pause()
        {
            Status = SessionStatus.Paused;
            PausedAt = DateTime.UtcNow;
            UpdateTimestamp();
        }

        public void Resume()
        {
            Status = SessionStatus.Running;
            PausedAt = null;
            UpdateTimestamp();
        }

        public void Complete()
        {
            Status = SessionStatus.Completed;
            ElapsedSeconds = (long)(DateTime.UtcNow - StartedAt).TotalSeconds;
            CompletedAt = DateTime.UtcNow;
            RoundsCompleted++;
            UpdateTimestamp();
        }

        public bool IsActive => Status == SessionStatus.Running;
        public bool IsBreak => Type == SessionType.Break;
    }

    public enum SessionStatus
    {
        Idle = 0,
        Running = 1,
        Paused = 2,
        Completed = 3
    }

    public enum SessionType
    {
        Focus = 0,
        Break = 1
    }
}
