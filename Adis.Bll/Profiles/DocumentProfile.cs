using Adis.Bll.Dtos;
using Adis.Dm;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Bll.Profiles
{
    public class DocumentProfile : Profile
    {
        public DocumentProfile() 
        {
            CreateMap<Document, DocumentDto>().ReverseMap();
        }
    }
}
