using System;
using System.Collections.Generic;

namespace TaskManagement.Models;

public class SystemTypeTab
{
    public int Id { get; set; }

    public string? SystemType { get; set; }

    public string? Description { get; set; }

    public string? SystemTypeFileName { get; set; }

    public string? FolderPath { get; set; }

    public virtual ICollection<ModuleSetup> ModuleSetups { get; set; } = new List<ModuleSetup>();
}
