﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using ScpmBe.Repositories.Infrastructure;
using System;
using System.Collections.Generic;

namespace ScpmaBe.Repositories.Entities;

public partial class Feedback : IEntityBase
{
    public int FeedbackId { get; set; }

    public int CustomerId { get; set; }

    public string Message { get; set; }

    public DateTime DateSubmitted { get; set; }

    public virtual Customer Customer { get; set; }
}