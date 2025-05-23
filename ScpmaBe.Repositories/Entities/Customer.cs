﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using ScpmBe.Repositories.Infrastructure;
using System;
using System.Collections.Generic;

namespace ScpmaBe.Repositories.Entities;

public partial class Customer : IEntityBase
{
    public int CustomerId { get; set; }

    public int OwnerId { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Phone { get; set; }

    public string Email { get; set; }

    public string Username { get; set; }

    public string Password { get; set; }
    public string PasswordTemp { get; set; }
    public string ActivationCode { get; set; }

    public bool IsActive { get; set; }

    public string Note { get; set; }

    public virtual ICollection<Car> Cars { get; set; } = new List<Car>();

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual Owner Owner { get; set; }
}