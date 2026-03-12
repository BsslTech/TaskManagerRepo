using System;
using System.Collections.Generic;

namespace TaskManagement.Models;

public class MenuAccessTab
{
    public int Id { get; set; }

    public string? RoleId { get; set; }

    public int? SubMenusetupTabId { get; set; }

    public bool Suspend { get; set; }

    public SubMenusetupTab SubMenusetupTab { get; set; }
}
