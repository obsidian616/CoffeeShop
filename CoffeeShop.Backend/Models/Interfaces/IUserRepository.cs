using CoffeeShop.Backend.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeShop.Backend.Models.Interfaces
{
    public interface IUserRepository
    {
        int Create(RegisterDto dto);

        UserDto Get(int userId);

        UserDto Get(string account);

        void Active(int userId);

        bool IsAccountExist(string account);

        UserDto GetByIdAndConfirmCode(int userId, string confirmCode);

        void Update(UserDto dto);
        List<int> GetFuncIdsByUserId(string account);

        IEnumerable<UserDto> GetAllUsers();

        void UpdateUserGroups(int userId, int[] selectedGroupIds, int modifyUserId);
        void UpdateUser(UserDto user);

        void UpdatePasswor(UserDto user);

        IEnumerable<UserDto> GetPagedUsers(int pageNumber, int pageSize, string searchString = null);

        int GetTotalUsersCount(string searchString = null);


    }
}
