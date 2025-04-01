﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using ScpmBe.Repositories.Infrastructure;
using System;
using System.Collections.Generic;

namespace ScpmaBe.Repositories.Entities;

public partial class Car : IEntityBase
{
    public int CarId { get; set; }

    public int CustomerId { get; set; }

    public string Thumbnail { get; set; }
    public string Brand { get; set; }

    public string Model { get; set; }

    public string Color { get; set; }

    public string LicensePlate { get; set; }

    public DateTime RegistedDate { get; set; }

    public bool Status { get; set; }

    public virtual ICollection<Contract> Contracts { get; set; } = new List<Contract>();

    public virtual Customer Customer { get; set; }
}