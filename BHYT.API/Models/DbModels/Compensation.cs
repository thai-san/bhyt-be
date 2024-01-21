using System;
using System.Collections.Generic;

namespace BHYT.API.Models.DbModels;

public partial class Compensation
{
    public int Id { get; set; }

    public Guid? Guid { get; set; }

    public int? PolicyId { get; set; }

    public int? EmployeeId { get; set; }

    public DateTime? Date { get; set; }
    
    public double? Amount { get; set; }

    public string HoptitalName { get; set; }

    public string HopitalCode { get; set; }

    public DateTime? DateRequest { get; set; }

    public string? UsedServices { get; set; }

    public int GetOption { get; set; }

    public string? Note { get; set; }

    public bool? Status { get; set; }
}
