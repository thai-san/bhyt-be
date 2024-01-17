using System;
using System.Collections.Generic;

namespace BHYT.API.Models.DbModels;

public partial class Status
{
    public int Id { get; set; }

    public Guid? Guid { get; set; }

    public bool? Name { get; set; }
}
