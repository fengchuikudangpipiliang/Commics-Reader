using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace 小说漫画阅读器
{
    public class UserConfig : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("T_Users");
            builder.Property(e => e.Name).HasMaxLength(12);
            builder.Property(e => e.PassWord).HasMaxLength(20);
        }
    }
}
