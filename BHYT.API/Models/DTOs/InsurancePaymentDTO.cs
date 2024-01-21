using System;
using System.Collections.Generic;

namespace BHYT.API.Models.DbModels;

public partial class InsurancePaymentDTO
{
    public Guid? Guid { get; set; }

    public int? PolicyId { get; set; }

    public DateTime? Date { get; set; }

    public double? Amount { get; set; }

    public bool? Status { get; set; }

    public string? Type { get; set; }

    public string? Note { get; set; }

    public string? SubscriptionId { get; set; }
}
