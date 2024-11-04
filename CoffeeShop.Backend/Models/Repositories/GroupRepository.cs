using CoffeeShop.Backend.Models.Components;
using CoffeeShop.Backend.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using Dapper;
using CoffeeShop.Backend.Models.Interfaces;

namespace CoffeeShop.Backend.Models.Repositories
{
    public class GroupRepository : IGroupRepository
    {
        private readonly string _connectionString;

        public GroupRepository()
        {
            _connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["AppDbContext"].ConnectionString;
        }
        public GroupRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        public IEnumerable<GroupDto> GetAllGroups()
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = "SELECT * FROM [Groups]";
                return db.Query<GroupDto>(sql);
            }
        }


        public GroupDto GetGroupById(int id)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = "SELECT * FROM [Groups] WHERE Id = @Id";
                return db.QueryFirstOrDefault<GroupDto>(sql, new { Id = id });
            }
        }

        public Result AddGroup(GroupDto groupDto)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"
                INSERT INTO [Groups] (GroupName, CreatedBy, CreatedTime, ModifyUser, ModifyTime, Enabled)
                VALUES (@GroupName, @CreatedBy, @CreatedTime, @ModifyUser, @ModifyTime, @Enabled)";

                int rowsAffected = db.Execute(sql, groupDto);
                return rowsAffected > 0 ? Result.Success() : Result.Fail("新增群組失敗");
            }
        }

        public Result UpdateGroup(GroupDto groupDto)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"
                UPDATE [Groups]
                SET GroupName = @GroupName, ModifyUser = @ModifyUser, ModifyTime = @ModifyTime, Enabled = @Enabled
                WHERE Id = @Id";

                int rowsAffected = db.Execute(sql, groupDto);
                return rowsAffected > 0 ? Result.Success() : Result.Fail("更新群組失敗");
            }
        }

        public Result DeleteGroup(int id)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = "DELETE FROM [Groups] WHERE Id = @Id";
                int rowsAffected = db.Execute(sql, new { Id = id });
                return rowsAffected > 0 ? Result.Success() : Result.Fail("刪除群組失敗");
            }
        }
    }
}