using AutoMapper;
using CoffeeShop.Models.Dtos;
using CoffeeShop.Models.EFModels;
using CoffeeShop.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace CoffeeShop.Models
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<LoginVm, LoginDto>();
            CreateMap<Member, MemberDto>().ReverseMap()
            .ForMember(dest => dest.CreateTime, opt => opt.Ignore()) // 忽略創建時間
            .ReverseMap()
            .ForMember(dest => dest.CreateTime, opt => opt.Ignore()); // 在反向映射時也忽略

            CreateMap<MemberDto, EditProfileVm>();
            CreateMap<EditProfileVm, EditProfileDto>();
            CreateMap<ChangePasswordVm, ChangePasswordDto>();
        }
    }
}