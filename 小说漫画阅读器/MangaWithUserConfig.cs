using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace 小说漫画阅读器
{
    public class MangaWithUserConfig : IEntityTypeConfiguration<MangaWithUser>
    {
        public void Configure(EntityTypeBuilder<MangaWithUser> builder)
        {
            builder.ToTable("T_MangaWithUserConfigs");
            builder.HasOne<User>(e => e.User).WithMany(e=>e.Mangas);
        }
    }
}
