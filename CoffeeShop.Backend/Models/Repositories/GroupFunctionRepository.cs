using CoffeeShop.Backend.Models.Dtos;
using CoffeeShop.Backend.Models.EFModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Dapper;
using CoffeeShop.Backend.Models.Interfaces;

namespace CoffeeShop.Backend.Models.Repositories
{
    public class GroupFunctionRepository: IGroupFunctionRepository
    {
        private readonly string _connectionString;

        public GroupFunctionRepository()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["AppDbContext"].ConnectionString;
        }

        public GroupFunctionRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IEnumerable<GroupFunctionDto> GetByGroupId(int groupId)
        {
            string sql = @"SELECT GF.*, G.GroupName, F.FunctionName 
                           FROM GroupFunctions GF
                           JOIN Groups G ON G.Id = GF.GroupId
                           JOIN Functions F ON GF.FunctionId = F.Id
                           WHERE GF.GroupId = @GroupId";

            using (var connection = new SqlConnection(_connectionString))
            {
                return connection.Query<GroupFunctionDto>(sql, new { GroupId = groupId });
            }
        }

        public IEnumerable<GroupFunction> GetAllGroupFunctions()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = "SELECT * FROM GroupFunctions";
                return connection.Query<GroupFunction>(sql);
            }
        }

        public IEnumerable<Function> GetAllFunctions()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = "SELECT * FROM Functions";
                return connection.Query<Function>(sql);
            }
        }

        public IEnumerable<GroupDto> GetGroup()
        {
            string sql = @"SELECT g.Id, g.GroupName, g.Enabled 
                           FROM Groups g 
                           WHERE g.Enabled = 1";

            using (var connection = new SqlConnection(_connectionString))
            {
                return connection.Query<GroupDto>(sql);
            }
        }

        public void SyncGroupFunctions(int modifyUserId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @"
                    INSERT INTO GroupFunctions (GroupId, FunctionId, Enabled, ModifyUser, ModifyTime)
                    SELECT g.Id, f.Id, 0, @ModifyUser, GETDATE()
                    FROM Groups g
                    CROSS JOIN Functions f
                    LEFT JOIN GroupFunctions gf ON gf.GroupId = g.Id AND gf.FunctionId = f.Id
                    WHERE gf.Id IS NULL;
                ";

                connection.Execute(sql, new { ModifyUser = modifyUserId });
            }
        }
        public void UpdateGroupFunction(GroupFunctionDto groupFunctionDto)
        {
            string sql = @"
        UPDATE GroupFunctions
        SET Enabled = @Enabled,
            ModifyUser = @ModifyUser,
            ModifyTime = @ModifyTime
        WHERE Id = @Id
    ";

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Execute(sql, new
                {
                    groupFunctionDto.Enabled,
                    groupFunctionDto.ModifyUser,
                    groupFunctionDto.ModifyTime,
                    groupFunctionDto.Id
                });
            }
        }
    }
}