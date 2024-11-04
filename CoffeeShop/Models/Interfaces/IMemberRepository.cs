using CoffeeShop.Models.Dtos;
using CoffeeShop.Models.EFModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeShop.Models.Interfaces
{
    public interface IMemberRepository
    {
        void Active(int memberId);
        void Create(RegisterDto dto);
        MemberDto Get(int memberId);
        MemberDto Get(string account);
        Member GetMemberById(int memberId);
        bool IsEmailExist(string account);
        void Update(MemberDto memberIndb);

        void UpdateMemberData(MemberDto memberIndb);
    }
}
