﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using ScpmBe.Repositories.Infrastructure;
using System;
using System.Collections.Generic;

namespace ScpmaBe.Repositories.Entities;

public partial class ParkingStatusSensor : IEntityBase
{
    public int ParkingStatusSensorId { get; set; }
    public string Name { get; set; }

    public int ParkingSpaceId { get; set; }

    public string ApiKey { get; set; }

    public bool IsActive { get; set; }

    public virtual ParkingSpace ParkingSpace { get; set; }
}