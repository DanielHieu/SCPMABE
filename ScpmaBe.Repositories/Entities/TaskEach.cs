﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using ScpmBe.Repositories.Infrastructure;
using System;
using System.Collections.Generic;

namespace ScpmaBe.Repositories.Entities;

public partial class TaskEach : IEntityBase
{
    public int TaskEachId { get; set; }

    public int OwnerId { get; set; }

    public string Description { get; set; }

    public virtual ICollection<AssignedTask> AssignedTasks { get; set; } = new List<AssignedTask>();

    public virtual Owner Owner { get; set; }
}