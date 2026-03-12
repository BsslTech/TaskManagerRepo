using System;
using System.Collections.Generic;

namespace TaskManagement.Models;

public class ProjectSubCategoryTab
{
    public int Id { get; set; }

    public string? CategoryName { get; set; }

    public string? CategoryCode { get; set; }

    public int? ProjectTabId { get; set; }

    public virtual ICollection<AssignTaskTab> AssignTaskTabs { get; set; } = new List<AssignTaskTab>();

    public virtual ProjectTab? ProjectTab { get; set; }
}
