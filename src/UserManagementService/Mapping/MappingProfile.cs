using UserManagementService.Models.Database;
using UserManagementService.Models.UI;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserManagementService.Mapping
{
    /// <summary>
    /// Defines class mappings.
    /// </summary>
    public class MappingProfile : Profile
    {
        /// <summary>
        /// Initilizes new instance of <see cref="MappingProfile"/>
        /// </summary>
        public MappingProfile()
        {
            CreateMap<ApplicationUser, UiUserDetails>().ReverseMap();
            CreateMap<ApplicationUser, UiUserListItem>().ReverseMap();
            CreateMap<IdentityRole, UiRoleListItem>().ReverseMap();
            
        }
    }
}
