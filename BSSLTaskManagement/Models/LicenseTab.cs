using System;
using System.Collections.Generic;

namespace BSSLTaskManagement.Models;

public partial class LicenseTab
{
    public int Id { get; set; }

    public string? CompCode { get; set; }

    public string? ClientName { get; set; }

    public string? LicenseType { get; set; }

    public int? LicenseNo { get; set; }

    public int? GraceDays { get; set; }

    public DateTime? DateFrm { get; set; }

    public DateTime? DateTo { get; set; }

    public string? SystemType { get; set; }

    public string? SystemTypeDescr { get; set; }

    public bool Deactivate { get; set; }
}
