using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace 小说漫画阅读器
{
    public class UsersLoveConfig : IEntityTypeConfiguration<UsersLove>
    {
        public void Configure(EntityTypeBuilder<UsersLove> builder)
        {
            builder.ToTable("T_UsersLoves");
            builder.HasOne<User>(e => e.User).WithMany(e => e.UsersLove);
        }
    }
}
