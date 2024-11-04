using CoffeeShop.Backend.Models.Components;
using CoffeeShop.Backend.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeShop.Backend.Models.Interfaces
{
    public interface IGroupRepository
    {
        IEnumerable<GroupDto> GetAllGroups();
        GroupDto GetGroupById(int id);
        Result AddGroup(GroupDto groupDto);
        Result UpdateGroup(GroupDto groupDto);
        Result DeleteGroup(int id);
    }
}
