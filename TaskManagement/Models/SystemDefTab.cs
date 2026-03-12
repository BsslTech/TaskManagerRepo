using System;
using System.Collections.Generic;

namespace TaskManagement.Models;

public class SystemDefTab
{
    public int Id { get; set; }

    public int? SystemSubMenuTabId { get; set; }

    public string? Code { get; set; }

    public string? Desc { get; set; }

    public string? IsLiquid { get; set; }

    public int? OrderNo { get; set; }

    public virtual SystemSubMenuTab? SystemSubMenuTab { get; set; }
}
