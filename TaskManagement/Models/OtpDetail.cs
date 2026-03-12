using System;
using System.Collections.Generic;

namespace TaskManagement.Models;

public class OtpDetail
{
    public int Id { get; set; }

    public string Apikey { get; set; } = null!;

    public string WhoToSend { get; set; } = null!;

    public string Channel { get; set; } = null!;

    public int PinAttempts { get; set; }

    public int PinTimeTolive { get; set; }

    public int PinLength { get; set; }

    public string Apiurl { get; set; } = null!;
}
