using System;

namespace BasicTaskManager;

public class TaskItem
{
    public Guid Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
}
