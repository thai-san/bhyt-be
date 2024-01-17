using System;
using System.Collections.Generic;

namespace BHYT.API.Models.DbModels;

public partial class PolicyApproval
{
    public int Id { get; set; }

    public Guid? Guid { get; set; }

    public int? PolicyId { get; set; }

    public int? EmployeeId { get; set; }

    public DateTime? ApprovalDate { get; set; }

    public int? StatusId { get; set; }
}
