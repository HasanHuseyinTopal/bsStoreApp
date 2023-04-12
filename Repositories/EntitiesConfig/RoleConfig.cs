using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.EntitiesConfig
{
    public class RoleConfig : IEntityTypeConfiguration<IdentityRole>
    {
        public void Configure(EntityTypeBuilder<IdentityRole> builder)
        {
            builder.HasData(
                new IdentityRole()
                {
                    Name = "user",
                    NormalizedName = "USER",
                },
                new IdentityRole()
                {
                    Name = "Editor",
                    NormalizedName = "EDİTOR"
                },
                new IdentityRole()
                {
                    Name = "Admin",
                    NormalizedName = "ADMİN"
                }
            );
        }
    }
}
