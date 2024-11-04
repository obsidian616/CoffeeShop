using CoffeeShop.Backend.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Dapper;
using CoffeeShop.Backend.Models.Interfaces;

namespace CoffeeShop.Backend.Models.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository()
        {
            _connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["AppDbContext"].ConnectionString;
        }


       
        public int Create(RegisterDto dto)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @"
            INSERT INTO Users (Account, Password, Name, Phone, ConfirmCode, IsConfirmed, CreatedBy, CreatedTime, ModifyUser, ModifyTime, IsStatus)
            VALUES (@Account, @Password, @Name, @Phone, @ConfirmCode, @IsConfirmed, @CreatedBy, @CreatedTime, @ModifyUser, @ModifyTime, @IsStatus);
            SELECT CAST(SCOPE_IDENTITY() as int)"; 

           
                int userId = connection.QuerySingle<int>(sql, new
                {
                    Account = dto.Account,
                    Password = dto.Password,
                    Name = dto.Name,
                    Phone = dto.Phone,
                    ConfirmCode = dto.ConfirmCode,
                    IsConfirmed = dto.IsConfirmed,
                    CreatedBy = dto.CreatedBy,
                    CreatedTime = dto.CreatedTime,
                    ModifyUser = dto.ModifyUser,
                    ModifyTime = dto.ModifyTime,
                    IsStatus = dto.IsStatus
                });

                return userId; 
            }
        }

      
        public UserDto Get(int userId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT * FROM Users WHERE Id = @UserId";
                return connection.QuerySingleOrDefault<UserDto>(sql, new { UserId = userId });
            }
        }


        public UserDto Get(string account)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT * FROM Users WHERE Account = @Account and IsStatus=1";
                return connection.QuerySingleOrDefault<UserDto>(sql, new { @Account = account });
            }
        }


        public void Active(int userId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @"
                    UPDATE Users 
                    SET IsConfirmed = 1, ConfirmCode = NULL 
                    WHERE Id = @UserId";

                connection.Execute(sql, new { UserId = userId });
            }
        }


        public bool IsAccountExist(string account)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT COUNT(1) FROM Users WHERE Account = @Account";
                return connection.ExecuteScalar<int>(sql, new { Account = account }) > 0;
            }
        }
        public void UpdateUser(UserDto user)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @"
                UPDATE Users 
                SET ConfirmCode = @ConfirmCode
                WHERE Id = @Id";

                connection.Execute(sql, new
                {
                    user.ConfirmCode,
                    user.Id
                });
            }
        }


        public void UpdatePasswor(UserDto user)
        {

            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @"
                UPDATE Users 
                SET Password = @Password
                WHERE Id = @Id";

                connection.Execute(sql, new
                {
                    user.Password,
                    user.Id
                });
            }
        }

        public void Update(UserDto dto)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @"
                    UPDATE Users 
                    SET Name = @Name, 
                        Phone = @Phone, 
                        ModifyUser = @ModifyUser, 
                        ModifyTime = @ModifyTime, 
                        IsStatus = @IsStatus
                    WHERE Id = @Id";

                connection.Execute(sql, new
                {
                    Id = dto.Id,
                    Name = dto.Name,
                    Phone = dto.Phone,
                    ModifyUser = dto.ModifyUser,
                    ModifyTime = dto.ModifyTime,
                    IsStatus = dto.IsStatus
                });
            }
        }
        public UserDto GetByIdAndConfirmCode(int userId, string confirmCode)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var sql = "SELECT * FROM Users WHERE Id = @UserId AND ConfirmCode = @ConfirmCode";
                return connection.QuerySingleOrDefault<UserDto>(sql, new { UserId = userId, ConfirmCode = confirmCode });
            }
        }



        public IEnumerable<UserDto> GetAllUsers()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT * FROM Users";
                return connection.Query<UserDto>(sql).ToList();
            }
        }
        public List<int> GetFuncIdsByUserId(string Account)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @"Select distinct gf.FunctionId
                             from Users u
                             join UserGroups ug on u.Id = ug.UserId
                             JOIN Groups g on g.Id = ug.GroupId
                             JOIN GroupFunctions gf ON g.Id = gf.GroupId
                             WHERE u.Account = @Account
                             AND gf.Enabled = 1 and g.Enabled=1 and u.IsStatus=1";
                return connection.Query<int>(sql, new { @Account = Account }).ToList();

            }
        }

        public void UpdateUserGroups(int userId, int[] selectedGroupIds,int modifyUserId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
             
                string deleteSql = @"DELETE FROM UserGroups WHERE UserId = @UserId";
                connection.Execute(deleteSql, new { UserId = userId });

               
                string insertSql = @"INSERT INTO UserGroups (UserId, GroupId, ModifyUser, ModifyTime) VALUES (@UserId, @GroupId, @ModifyUser, @ModifyTime)";

                foreach (var groupId in selectedGroupIds)
                {
                    connection.Execute(insertSql, new
                    {
                        UserId = userId,
                        GroupId = groupId,
                        ModifyUser = modifyUserId, 
                        ModifyTime = DateTime.Now
                    });
                }
            }
        }
   
        public int CreateUserAndReturnId(RegisterDto dto)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @"
                    INSERT INTO Users (Account, Password, Name, Phone, ConfirmCode, IsConfirmed, CreatedBy, CreatedTime, ModifyUser, ModifyTime, IsStatus)
                    VALUES (@Account, @Password, @Name, @Phone, @ConfirmCode, @IsConfirmed, @CreatedBy, @CreatedTime, @ModifyUser, @ModifyTime, @IsStatus);
                    SELECT CAST(SCOPE_IDENTITY() as int)"; // 返回新增的使用者 ID

                return connection.QuerySingle<int>(sql, new
                {
                    Account = dto.Account,
                    Password = dto.Password,
                    Name = dto.Name,
                    Phone = dto.Phone,
                    ConfirmCode = dto.ConfirmCode,
                    IsConfirmed = dto.IsConfirmed,
                    CreatedBy = dto.CreatedBy,
                    CreatedTime = dto.CreatedTime,
                    ModifyUser = dto.ModifyUser,
                    ModifyTime = dto.ModifyTime,
                    IsStatus = dto.IsStatus
                });
            }
        }
        public IEnumerable<UserDto> GetPagedUsers(int pageNumber, int pageSize, string searchString = null)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                // 設置 SQL 查詢
                string sql = @"
                SELECT * 
                FROM Users 
                WHERE (@SearchString IS NULL OR Name LIKE '%' + @SearchString + '%')
                ORDER BY Id 
                OFFSET @Offset ROWS 
                FETCH NEXT @PageSize ROWS ONLY";

                var offset = (pageNumber - 1) * pageSize; // 計算偏移量

                return connection.Query<UserDto>(sql, new
                {
                    SearchString = searchString,
                    Offset = offset,
                    PageSize = pageSize
                }).ToList();
            }
        }

        // 獲取總用戶數
        public int GetTotalUsersCount(string searchString = null)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @"
                SELECT COUNT(1) 
                FROM Users 
                WHERE (@SearchString IS NULL OR Name LIKE '%' + @SearchString + '%')";

                return connection.ExecuteScalar<int>(sql, new { SearchString = searchString });
            }
        }
    }
}
