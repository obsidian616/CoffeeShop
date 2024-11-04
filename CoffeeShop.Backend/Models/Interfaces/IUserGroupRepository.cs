using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeShop.Backend.Models.Interfaces
{
    public interface IUserGroupRepository
    {
        void AddUserGroups(int userId, int[] groupIds, int modifyUserId);

        void DeleteUserGroupsByUserId(int userId);
        List<int> GetGroupIdsByUserId(int userId);
        void UpdateUserGroups(int userId, int[] selectedGroupIds, int modifyId);
    }
}
