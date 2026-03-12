using System;
using System.Collections.Generic;

namespace TaskManagement.Models;

public class AssignTaskTab
{
    public int Id { get; set; }

    public string? TaskReferenceNumber { get; set; }

    public int? StaffTabId { get; set; }

    public int? ProjectSubCategoryTabId { get; set; }

    public string? TaskDescription { get; set; }

    public decimal Duration { get; set; }

    public DateTime AssignedDate { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public bool Completed { get; set; }

    public string? Remarks { get; set; }

    public bool TaskStatus { get; set; }

    public bool AcceptTask { get; set; }

    public ProjectSubCategoryTab? ProjectSubCategoryTab { get; set; }

    public StaffTab? StaffTab { get; set; }
}
