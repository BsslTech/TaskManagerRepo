using System;
using System.Collections.Generic;

namespace BSSLTaskManagement.Models;

public partial class ModuleSetup
{
    public int Id { get; set; }

    public int SystemTypeTabId { get; set; }

    public string? SystemType { get; set; }

    public string? Description { get; set; }

    public string? ModuleFileName { get; set; }

    public string? FolderPath { get; set; }

    public string? ModuleCode { get; set; }

    public string? VideoUrl { get; set; }

    public virtual ICollection<MainMenu> MainMenus { get; set; } = new List<MainMenu>();

    public virtual SystemTypeTab SystemTypeTab { get; set; } = null!;
}
