using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Dtos;
using API.Entities;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace API.Interfaces
{
    public interface IUserrRepository
    {
        void Update(AppUser user);
        Task<bool>SaveAllAsync();
        Task<IEnumerable<AppUser>>GetUsersAsync();
        Task<AppUser?>GetUserByIdAsync(int id);
        Task<AppUser?>GetUserByUsernameAsync(string username);
        Task<IEnumerable<MemberDto>>GetMembersAsync();
        Task<MemberDto?>GetMemberAsync(string username);
    }
}