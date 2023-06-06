using AutoMapper;
using DataConnection.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModel.Advise.Response;
using ViewModel.Category.View;
using ViewModel.CategoryMapping.View;
using ViewModel.Chat.Response;
using ViewModel.Expert;
using ViewModel.User;

namespace Service.MapperConfig
{
    public class ConfigMapper : Profile
    {
        public ConfigMapper()
        {
            CreateMap<User, UserViewModel>();
            CreateMap<Expert, ExpertViewModel>();
            CreateMap<CategoryMapping, CategoryMappingViewModel>();
            CreateMap<Category, CategoryViewModel>();
            CreateMap<Chat, ChatViewModel>();
            CreateMap<Advise, AdviseViewModel>();
        }
    }
}
