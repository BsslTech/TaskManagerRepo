using System;
using System.Collections.Generic;

namespace BSSLTaskManagement.Models;

public partial class SystemMenuTab
{
    public int Id { get; set; }

    public string? Code { get; set; }

    public string? Desc { get; set; }

    public virtual ICollection<SystemSubMenuTab> SystemSubMenuTabs { get; set; } = new List<SystemSubMenuTab>();
}
