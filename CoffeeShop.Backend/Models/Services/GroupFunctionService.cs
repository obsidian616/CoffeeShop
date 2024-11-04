using CoffeeShop.Backend.Models.Dtos;
using CoffeeShop.Backend.Models.Interfaces;
using CoffeeShop.Backend.Models.Repositories;
using CoffeeShop.Backend.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CoffeeShop.Backend.Models.Services
{
    public class GroupFunctionService
    {
        private readonly IGroupFunctionRepository _groupFunctionRepository;

        public GroupFunctionService()
        {
            _groupFunctionRepository = new GroupFunctionRepository();
        }

        public void SyncGroupFunctions(int modifyUserId)
        {
            _groupFunctionRepository.SyncGroupFunctions(modifyUserId);
        }

        public IEnumerable<GroupDto> GetGroups()
        {
            return _groupFunctionRepository.GetGroup();
        }

        public IEnumerable<GroupFunctionVm> GetFunctionsByGroup(int groupId)
        {
            var groupFunctions = _groupFunctionRepository.GetByGroupId(groupId);

            var groupFunctionsVm = groupFunctions.Select(gf => new GroupFunctionVm
            {
                Id = gf.Id,
                GroupId = gf.GroupId,
                GroupName = gf.GroupName,
                FunctionId = gf.FunctionId,
                FunctionName = gf.FunctionName,
                Enabled = gf.Enabled,
                ModifyUser = gf.ModifyUser,
                ModifyTime = gf.ModifyTime
            });

            return groupFunctionsVm;
        }

        public void UpdateGroupFunctions(int groupId, int[] selectedFunctions, int modifyUserId)
        {
    
            var allGroupFunctions = _groupFunctionRepository.GetByGroupId(groupId).ToList();

     
            foreach (var gf in allGroupFunctions)
            {
       
                if (groupId == 1 && gf.Enabled && !(selectedFunctions != null && selectedFunctions.Contains(gf.FunctionId)))
                {
               
                    continue; 
                }
                bool shouldEnable = selectedFunctions != null && selectedFunctions.Contains(gf.FunctionId);
                if (gf.Enabled != shouldEnable)
                {
                    gf.Enabled = shouldEnable;
                    gf.ModifyUser = modifyUserId;
                    gf.ModifyTime = DateTime.Now;
          
                    _groupFunctionRepository.UpdateGroupFunction(gf);
                }
            }
        }
    }
}
