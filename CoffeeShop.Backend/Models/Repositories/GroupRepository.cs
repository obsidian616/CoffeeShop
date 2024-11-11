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
using CoffeeShop.Backend.Models.EFModels;

namespace CoffeeShop.Backend.Models.Repositories
{
    public class GroupRepository : IGroupRepository
    {
        private readonly AppDbContext _context;

        public GroupRepository()
        {
            _context = new AppDbContext();
        }

        public GroupRepository(AppDbContext db)
        {
            _context = db;
        }

        public IEnumerable<GroupDto> GetAllGroups()
        {
            // 手動映射 Entity 到 DTO
            return _context.Groups
                .Select(g => new GroupDto
                {
                    Id = g.Id,
                    GroupName = g.GroupName,
                    CreatedBy = g.CreatedBy,
                    CreatedTime = g.CreatedTime,
                    ModifyUser = g.ModifyUser,
                    ModifyTime = g.ModifyTime,
                    Enabled = g.Enabled
                })
                .ToList();
        }

        public GroupDto GetGroupById(int id)
        {
            var group = _context.Groups
                .Where(g => g.Id == id)
                .Select(g => new GroupDto
                {
                    Id = g.Id,
                    GroupName = g.GroupName,
                    CreatedBy = g.CreatedBy,
                    CreatedTime = g.CreatedTime,
                    ModifyUser = g.ModifyUser,
                    ModifyTime = g.ModifyTime,
                    Enabled = g.Enabled
                })
                .FirstOrDefault();

            return group;
        }

        public Result AddGroup(GroupDto groupDto)
        {
            // 手動映射 DTO 到 Entity
            var group = new Group
            {
                GroupName = groupDto.GroupName,
                CreatedBy = groupDto.CreatedBy,
                CreatedTime = groupDto.CreatedTime,
                ModifyUser = groupDto.ModifyUser,
                ModifyTime = groupDto.ModifyTime,
                Enabled = groupDto.Enabled
            };

            _context.Groups.Add(group);
            var rowsAffected = _context.SaveChanges();
            return rowsAffected > 0 ? Result.Success() : Result.Fail("新增群組失敗");
        }

        public Result UpdateGroup(GroupDto groupDto)
        {
            var group = _context.Groups.Find(groupDto.Id);
            if (group == null)
                return Result.Fail("群組不存在");

            // 更新屬性
            group.GroupName = groupDto.GroupName;
            group.ModifyUser = groupDto.ModifyUser;
            group.ModifyTime = groupDto.ModifyTime;
            group.Enabled = groupDto.Enabled;

            var rowsAffected = _context.SaveChanges();
            return rowsAffected > 0 ? Result.Success() : Result.Fail("更新群組失敗");
        }

        public Result DeleteGroup(int id)
        {
            var group = _context.Groups.Find(id);
            if (group == null)
                return Result.Fail("群組不存在");

            _context.Groups.Remove(group);
            var rowsAffected = _context.SaveChanges();
            return rowsAffected > 0 ? Result.Success() : Result.Fail("刪除群組失敗");
        }



        //private readonly string _connectionString;

        //public GroupRepository()
        //{
        //    _connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["AppDbContext"].ConnectionString;
        //}
        //public GroupRepository(string connectionString)
        //{
        //    _connectionString = connectionString;
        //}
        //public IEnumerable<GroupDto> GetAllGroups()
        //{
        //    using (IDbConnection db = new SqlConnection(_connectionString))
        //    {
        //        string sql = "SELECT * FROM [Groups]";
        //        return db.Query<GroupDto>(sql);
        //    }
        //}


        //public GroupDto GetGroupById(int id)
        //{
        //    using (IDbConnection db = new SqlConnection(_connectionString))
        //    {
        //        string sql = "SELECT * FROM [Groups] WHERE Id = @Id";
        //        return db.QueryFirstOrDefault<GroupDto>(sql, new { Id = id });
        //    }
        //}

        //public Result AddGroup(GroupDto groupDto)
        //{
        //    using (IDbConnection db = new SqlConnection(_connectionString))
        //    {
        //        string sql = @"
        //        INSERT INTO [Groups] (GroupName, CreatedBy, CreatedTime, ModifyUser, ModifyTime, Enabled)
        //        VALUES (@GroupName, @CreatedBy, @CreatedTime, @ModifyUser, @ModifyTime, @Enabled)";

        //        int rowsAffected = db.Execute(sql, groupDto);
        //        return rowsAffected > 0 ? Result.Success() : Result.Fail("新增群組失敗");
        //    }
        //}

        //public Result UpdateGroup(GroupDto groupDto)
        //{
        //    using (IDbConnection db = new SqlConnection(_connectionString))
        //    {
        //        string sql = @"
        //        UPDATE [Groups]
        //        SET GroupName = @GroupName, ModifyUser = @ModifyUser, ModifyTime = @ModifyTime, Enabled = @Enabled
        //        WHERE Id = @Id";

        //        int rowsAffected = db.Execute(sql, groupDto);
        //        return rowsAffected > 0 ? Result.Success() : Result.Fail("更新群組失敗");
        //    }
        //}

        //public Result DeleteGroup(int id)
        //{
        //    using (IDbConnection db = new SqlConnection(_connectionString))
        //    {
        //        string sql = "DELETE FROM [Groups] WHERE Id = @Id";
        //        int rowsAffected = db.Execute(sql, new { Id = id });
        //        return rowsAffected > 0 ? Result.Success() : Result.Fail("刪除群組失敗");
        //    }
        //}
    }
}