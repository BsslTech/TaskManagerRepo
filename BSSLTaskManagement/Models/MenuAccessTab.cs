using System;
using System.Collections.Generic;

namespace BSSLTaskManagement.Models;

public partial class MenuAccessTab
{
    public int Id { get; set; }

    public string? RoleId { get; set; }

    public int? SubMenusetupTabId { get; set; }

    public bool Suspend { get; set; }

    public virtual SubMenusetupTab? SubMenusetupTab { get; set; }
}
