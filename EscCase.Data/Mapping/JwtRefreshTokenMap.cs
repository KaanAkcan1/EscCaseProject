using EscCase.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EscCase.Data.Mapping
{
    public class JwtRefreshTokenMap : IEntityTypeConfiguration<JwtRefreshToken>
    {
        public void Configure(EntityTypeBuilder<JwtRefreshToken> builder)
        {
            builder.ToTable("JwtRefreshTokens");
        }
    }
}
