using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using CoffeeShop.Backend.Models.Interfaces;
using Dapper;
namespace CoffeeShop.Backend.Models.Repositories
{
    public class UserGroupRepository : IUserGroupRepository
    {
        private readonly string _connectionString;
 

        public UserGroupRepository()
        {
            _connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["AppDbContext"].ConnectionString;
        }
        public UserGroupRepository(string connectionStringName)
        {
            _connectionString = System.Configuration.ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
        }
        public List<int> GetGroupIdsByUserId(int userId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT GroupId FROM UserGroups WHERE UserId = @UserId";
                return connection.Query<int>(sql, new { UserId = userId }).ToList();


            }
        }


        public void AddUserGroups(int userId, int[] groupIds, int modifyUserId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string insertQuery = @"INSERT INTO UserGroups (UserId, GroupId, ModifyUser, ModifyTime)
                                   VALUES (@UserId, @GroupId, @ModifyUser, @ModifyTime)";

                foreach (var groupId in groupIds)
                {
                    connection.Execute(insertQuery, new
                    {
                        UserId = userId,
                        GroupId = groupId,
                        ModifyUser = modifyUserId,
                        ModifyTime = DateTime.Now
                    });
                }
            }
        }
        public void DeleteUserGroupsByUserId(int userId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @"DELETE FROM UserGroups WHERE UserId = @UserId";
                connection.Execute(sql, new { UserId = userId });
            }
        }
        public void UpdateUserGroups(int userId, int[] selectedGroupIds, int modifyId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
           
                string deleteSql = @"DELETE FROM UserGroups WHERE UserId = @UserId";
                connection.Execute(deleteSql, new { UserId = userId });

           
                string insertSql = @"INSERT INTO UserGroups (UserId, GroupId, ModifyUser, ModifyTime) 
                                 VALUES (@UserId, @GroupId, @ModifyUser, @ModifyTime)";

                foreach (var groupId in selectedGroupIds)
                {
                    connection.Execute(insertSql, new
                    {
                        UserId = userId,
                        GroupId = groupId,
                        ModifyUser = modifyId,
                        ModifyTime = DateTime.Now
                    });
                }
            }
        }
    }
}