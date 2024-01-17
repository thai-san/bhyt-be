using System;
using System.Collections.Generic;

namespace BHYT.API.Models.DbModels;

public partial class Session
{
    public int Id { get; set; }

    public Guid? Guid { get; set; }

    public string? AccountId { get; set; }

    public DateTime? LoginDate { get; set; } //

    public string? Appversion { get; set; } //

    public string? SessionLastIp { get; set; } 

    public DateTime? SessionLastRefresh { get; set; } //

    public string? SessionToken { get; set; }

    public string? ServiceDescription { get; set; }

    public int? IsValid { get; set; }
}
