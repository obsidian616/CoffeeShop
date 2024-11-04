using CoffeeShop.Models.Dtos;
using CoffeeShop.Models.EFModels;
using CoffeeShop.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CoffeeShop.Models.Repositories
{
    public class MemberRepository : IMemberRepository
    {
        public AppDbContext _db;

        public MemberRepository()
        {
            _db = new AppDbContext();
        }

        public MemberRepository(AppDbContext db)
        {
            _db = db;
        }
        public void Create(RegisterDto dto)
        {
            _db.Members.Add(new Member
            {
                Name = dto.Name,
                Email = dto.Email,
                EncryptedPassword = dto.EncryptedPassword,
                Phone = dto.Phone,
                ConfirmCode = dto.ConfirmCode,
                IsConfirmed = dto.isConfirmed,
                Birthday = dto.Birthday,
                Gender = dto.Gender,
                CreateTime = DateTime.UtcNow.ToLocalTime(), // 設置創建時間
                EmailConfirmed = dto.EmailConfirmed,
            });
            _db.SaveChanges();
        }

        public void Active(int memberId)
        {
            var member = _db.Members.FirstOrDefault(x => x.Id == memberId);
            member.IsConfirmed = true;
            member.ConfirmCode = null;

            _db.SaveChanges();
        }

        public bool IsEmailExist(string email)
        {
            var member = _db.Members
            .AsNoTracking()
                .FirstOrDefault(x => x.Email == email);
            return member != null;
        }

        public MemberDto Get(int memberId)
        {
            var member = _db.Members
                 .AsNoTracking()
                 .FirstOrDefault(x => x.Id == memberId);
            if (member == null) return null;

            return new MemberDto
            {
                Id = member.Id,
                Email = member.Email,
                EncryptedPassword = member.EncryptedPassword,
                Name = member.Name,
                Phone = member.Phone,
                ConfirmCode = member.ConfirmCode,
                IsConfirmed = member.IsConfirmed,
                Birthday = (DateTime)member.Birthday,
                Gender = (bool)member.Gender,
            };
        }

        public MemberDto Get(string email)
        {

            var member = _db.Members
                .AsNoTracking()
                .FirstOrDefault(x => x.Email == email);
            if (member == null) return null;

            return MvcApplication._mapper.Map<MemberDto>(member);
        }


        public void Update(MemberDto dto)
        {
            Member member = MvcApplication._mapper.Map<Member>(dto);
            member.CreateTime = dto.CreateTime;
            //member.ModifyTime = DateTime.UtcNow; ; // 設置修改時間為當前時間

            //更新 member 到 db
            _db.Entry(member).State = System.Data.Entity.EntityState.Modified;

            _db.SaveChanges();

            //// 獲取資料庫中的現有會員資料
            //var memberInDb = _db.Members.Find(dto.Id); // 使用 Find 或其他方法根據 ID 獲取會員
            //if (memberInDb == null) throw new Exception("會員不存在");

            //// 更新需要修改的屬性
            ////memberInDb.Name = dto.Name;
            //memberInDb.Email = dto.Email;
            //memberInDb.Phone = dto.Phone;
            //memberInDb.Gender = dto.Gender;

            //// 設置修改時間為當前的 UTC 時間
            //memberInDb.ModifyTime = DateTime.Now;

            //// 標記為已修改
            //_db.Entry(memberInDb).State = System.Data.Entity.EntityState.Modified;

            //// 保存變更
            //_db.SaveChanges();
        }

        public Member GetMemberById(int memberId)
        {
            return _db.Members.Find(memberId);
        }

        public void UpdateMemberData(MemberDto dto)
        {
            // 獲取資料庫中的現有會員資料
            var memberInDb = _db.Members.Find(dto.Id); // 使用 Find 或其他方法根據 ID 獲取會員
            if (memberInDb == null) throw new Exception("會員不存在");

            // 更新需要修改的屬性
            //memberInDb.Name = dto.Name;
            memberInDb.Email = dto.Email;
            memberInDb.Phone = dto.Phone;
            memberInDb.Gender = dto.Gender;

            // 設置修改時間為當前的 UTC 時間
            memberInDb.ModifyTime = DateTime.Now;

            // 標記為已修改
            _db.Entry(memberInDb).State = System.Data.Entity.EntityState.Modified;

            // 保存變更
            _db.SaveChanges();
        }

        
    }
}