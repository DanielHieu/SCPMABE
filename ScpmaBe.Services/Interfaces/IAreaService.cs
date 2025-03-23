using ScpmaBe.Repositories.Entities;
using ScpmaBe.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScpmaBe.Services.Interfaces
{
    public  interface  IAreaService
    {
        Task<List<Area>> GetPaging(int pageIndex, int pageSize);
        Task<Area> GetById(int id);
        Task<Area> AddAreaAsync(AddAreaRequest request);
        Task<Area> UpdateAreaAsync(UpdateAreaRequest request);
        Task<bool> DeleteAreaAsync(int id);

        Task<List<Area>> GetAreasByParkingLot(int parkingLotId);
    }
}
