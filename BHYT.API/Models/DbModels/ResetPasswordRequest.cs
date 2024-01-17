using System;
using System.Collections.Generic;

namespace BHYT.API.Models.DbModels;

public partial class ResetPasswordRequest
{
    public int Id { get; set; }

    public Guid? Guid { get; set; }

    public string? UserId { get; set; }

    public string? Resetrequestcode { get; set; }

    public DateTime? Requestdate { get; set; }

    public DateTime? Resetdate { get; set; }
}
