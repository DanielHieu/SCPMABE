﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using ScpmBe.Repositories.Infrastructure;
using System;
using System.Collections.Generic;

namespace ScpmaBe.Repositories.Entities;

public partial class Area : IEntityBase
{
    public int AreaId { get; set; }

    public int ParkingLotId { get; set; }

    public string AreaName { get; set; }

    public int TotalFloor { get; set; }

    public int Status { get; set; }

    public int RentalType { get; set; }

    public virtual ICollection<Floor> Floors { get; set; } = new List<Floor>();

    public virtual ParkingLot ParkingLot { get; set; }
}