using System;
using System.Collections.Generic;

namespace BHYT.API.Models.DbModels;

public partial class Benefit
{
    public int Id { get; set; }

    public Guid? Guid { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }
}
