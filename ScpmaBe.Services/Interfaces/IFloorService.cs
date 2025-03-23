﻿using ScpmaBe.Repositories.Entities;
using ScpmaBe.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScpmaBe.Services.Interfaces
{
    public  interface IFloorService
    {
        Task<List<Floor>> GetPaging(int pageIndex, int pageSize);
        Task<Floor> GetById(int id);
        Task<Floor> AddFloorAsync(AddFloorRequest request);
        Task<Floor> UpdateFloorAsync(UpdateFloorRequest request);
        Task<bool> DeleteFloorAsync(int id);

        Task<List<Floor>> GetFloorsByArea(int areaId);
    }
}
