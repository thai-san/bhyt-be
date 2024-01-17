using System;
using System.Collections.Generic;

namespace BHYT.API.Models.DbModels;

public partial class CustomerPolicyDTO
{
    public Guid? Guid { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? EndDate { get; set; }

    public double? PremiumAmount { get; set; }

    public bool? PaymentOption { get; set; }

    public string? CoverageType { get; set; }

    public double? DeductibleAmount { get; set; }

    public int? BenefitId { get; set; }

    public int? InsuranceId { get; set; }

    public DateTime? LatestUpdate { get; set; }

    public string? Description { get; set; }

    public bool? Status { get; set; }

    public string? Company { get; set; }
}
