using System;
using System.Collections.Generic;

namespace BSSLTaskManagement.Models;

public partial class MainMenu
{
    public int Id { get; set; }

    public string? Description { get; set; }

    public int ModuleSetupId { get; set; }

    public int? OrderNo { get; set; }

    public virtual ICollection<MenusetupTab> MenusetupTabs { get; set; } = new List<MenusetupTab>();

    public virtual ModuleSetup ModuleSetup { get; set; } = null!;
}
