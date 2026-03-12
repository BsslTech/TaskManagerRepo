using System;
using System.Collections.Generic;

namespace TaskManagement.Models;

public class ProjectTab
{
    public int Id { get; set; }

    public string? ProjectName { get; set; }

    public string? ProjectCode { get; set; }

    public virtual ICollection<ProjectSubCategoryTab> ProjectSubCategoryTabs { get; set; } = new List<ProjectSubCategoryTab>();
}
