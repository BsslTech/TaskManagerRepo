using System;
using System.Collections.Generic;

namespace BSSLTaskManagement.Models;

public partial class Curtab
{
    public string Curcode { get; set; } = null!;

    public string? Curname { get; set; }

    public decimal Raten { get; set; }

    public DateTime Ratedate { get; set; }

    public string? Symbol { get; set; }

    public string? Exequacct { get; set; }

    public int Id { get; set; }

    public bool? BaseCurrency { get; set; }

    public string? Isocode { get; set; }

    public string? CurNairaName { get; set; }

    public string? CurKoboName { get; set; }
}
