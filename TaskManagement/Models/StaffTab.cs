using System;
using System.Collections.Generic;

namespace TaskManagement.Models;

public class StaffTab
{
    public int Id { get; set; }

    public string? StaffId { get; set; }

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public string? StaffName { get; set; }

    public string? StaffType { get; set; }

    public virtual ICollection<AssignTaskTab> AssignTaskTabs { get; set; } = new List<AssignTaskTab>();
}
