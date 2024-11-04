using CoffeeShop.Backend.Models.Components;
using CoffeeShop.Backend.Models.Dtos;
using CoffeeShop.Backend.Models.Interfaces;
using CoffeeShop.Backend.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CoffeeShop.Backend.Models.Services
{
    public class GroupService
    {
        private readonly IGroupRepository _repo;

        public GroupService(IGroupRepository repo)
        {
            _repo = repo;
        }

        public IEnumerable<GroupDto> GetAllGroups()
        {
            try
            {
                return _repo.GetAllGroups();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("獲取群組資料時發生錯誤", ex);
            }
        }

        public GroupDto GetGroupById(int id)
        {
            try
            {
                var group = _repo.GetGroupById(id);
                if (group == null)
                {
                    throw new KeyNotFoundException("群組不存在");
                }
                return group;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"獲取ID為 {id} 的群組資料時發生錯誤", ex);
            }
        }

        public Result AddGroup(GroupVm groupVm, int modifyId)
        {
            if (groupVm == null)
            {
                return Result.Fail("群組資料無效");
            }

            try
            {
                var groupDto = new GroupDto
                {
                    GroupName = groupVm.GroupName,
                    CreatedBy = modifyId,
                    ModifyUser = modifyId,
                    CreatedTime = DateTime.Now,
                    ModifyTime = DateTime.Now,
                    Enabled = groupVm.Enabled
                };

                return _repo.AddGroup(groupDto);
            }
            catch (Exception ex)
            {
                return Result.Fail($"新增群組時發生錯誤: {ex.Message}");
            }
        }

        public Result UpdateGroup(int id, GroupVm groupVm, int modifyId)
        {
            if (groupVm == null)
            {
                return Result.Fail("群組資料無效");
            }

            try
            {
        
                if (id == 1 && !groupVm.Enabled)
                {
                    return Result.Fail("無法將 ID 為 1 的群組設置為禁用");
                }

                var groupDto = new GroupDto
                {
                    Id = id,
                    GroupName = groupVm.GroupName,
                    ModifyUser = modifyId,
                    ModifyTime = DateTime.Now,
                    Enabled = groupVm.Enabled
                };

                return _repo.UpdateGroup(groupDto);
            }
            catch (Exception ex)
            {
                return Result.Fail($"更新群組時發生錯誤: {ex.Message}");
            }
        }

        public Result DeleteGroup(int id)
        {
            try
            {
  
                if (id == 1)
                {
                    return Result.Fail("無法刪除管理員的群組，這是系統保護的群組");
                }

                return _repo.DeleteGroup(id);
            }
            catch (Exception ex)
            {
                return Result.Fail($"刪除群組時發生錯誤: {ex.Message}");
            }
        }
    }
}