using CoffeeShop.Backend.Models.Dtos;
using CoffeeShop.Backend.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CoffeeShop.Backend.Models.Services
{
    public class UserGroupServices
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IUserGroupRepository _userGroupRepository;

        public UserGroupServices(IGroupRepository groupRepository, IUserGroupRepository userGroupRepository)
        {
            _groupRepository = groupRepository;
            _userGroupRepository = userGroupRepository;
        }

        public IEnumerable<GroupDto> GetAllGroups()
        {
            return _groupRepository.GetAllGroups();
        }

        public List<int> GetUserGroups(int userId)
        {
            return _userGroupRepository.GetGroupIdsByUserId(userId);
        }

        public void AddUserGroups(int userId, int[] groupIds, int modifyUserId)
        {


            _userGroupRepository.AddUserGroups(userId, groupIds, modifyUserId);
        }

        public void DeleteUserGroupsByUserId(int userId)
        {
            _userGroupRepository.DeleteUserGroupsByUserId(userId);
        }

        public void UpdateUserGroups(int userId, int[] groupIds, int modifyUserId)
        {
  
            DeleteUserGroupsByUserId(userId);

            AddUserGroups(userId, groupIds, modifyUserId);
        }
    }
}