using CoffeeShop.Backend.Models.Dtos;
using CoffeeShop.Backend.Models.EFModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CoffeeShop.Backend.Models.Interfaces
{
    public interface IGroupFunctionRepository
    {

        IEnumerable<GroupFunctionDto> GetByGroupId(int groupId);
        IEnumerable<GroupFunction> GetAllGroupFunctions();
        IEnumerable<Function> GetAllFunctions();
        IEnumerable<GroupDto> GetGroup();
        void SyncGroupFunctions(int modifyUserId);
        void UpdateGroupFunction(GroupFunctionDto groupFunctionDto);
    }
}