using AutoMapper;
using eCom.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCom.Models.AutoMapper
{
    public class AccountProfile : Profile
    {
        public AccountProfile() 
        {
            CreateMap<UserAddress, UserAddressVM>()
            .ForMember(des => des.Id, opt=> opt.MapFrom(src=>src.Id))
            .ForMember(des => des.NearestLandMark, opt=>opt.MapFrom(src=>src.NearestLandMark))
            .ForMember(des => des.CityId, opt => opt.MapFrom(src => src.CityId))
            .ForMember(des => des.FullName, opt => opt.MapFrom(src => src.FullName))
            .ForMember(des => des.AdditionalInfo, opt => opt.MapFrom(src => src.AdditionalInfo))
            .ForMember(des => des.Phone, opt => opt.MapFrom(src => src.Phone))
            .ForMember(des => des.Street, opt => opt.MapFrom(src => src.Street))
            .ForMember(des => des.IsDefault, opt => opt.MapFrom(src => src.IsDefault))
            .ReverseMap();
        }
    }
}
