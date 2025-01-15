using System;
using System.Collections.Generic;

namespace TaskManagerAPI.Models;

public partial class Task
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime CreateAt { get; set; }

    public DateTime? DoneAt { get; set; }

    public virtual User? User { get; set; }
}
