using AutoMapper;
using PandaBank.SharedService.Contract;
using PandaBank.Account.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;
using PandaBank.SharedService.Contract.Account.Create;

namespace PandaBank.Account.DAL.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<PandaStatement, PandaStatementCreateContract>()
                .ReverseMap();
            CreateMap<PandaAccount, PandaAccountCreateContract>()
               .ForMember(f => f.PandaStatement, opt => opt.Ignore())
               .ReverseMap()
               .ForMember(f => f.PandaStatement, opt => opt.Ignore());
            CreateMap<UserAccount, UserAccountCreateContract>()
               .ReverseMap();
        }
    }
}
