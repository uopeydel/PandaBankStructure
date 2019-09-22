using AutoMapper;
using PandaBank.SharedService.Contract;
using PandaBank.User.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PandaBank.User.DAL.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<PandaUser, PandaUserContract>();
        }
    }
}
