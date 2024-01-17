using System;
using System.Collections.Generic;

namespace BHYT.API.Models.DbModels;

public partial class Role
{
    public int Id { get; set; }

    public Guid? Guid { get; set; }

    public string Name { get; set; }
}
