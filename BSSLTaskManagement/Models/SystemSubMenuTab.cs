using System;
using System.Collections.Generic;

namespace BSSLTaskManagement.Models;

public partial class SystemSubMenuTab
{
    public int Id { get; set; }

    public int? SystemMenuTabId { get; set; }

    public string? Code { get; set; }

    public string? Desc { get; set; }

    public virtual ICollection<SystemDefTab> SystemDefTabs { get; set; } = new List<SystemDefTab>();

    public virtual SystemMenuTab? SystemMenuTab { get; set; }
}
