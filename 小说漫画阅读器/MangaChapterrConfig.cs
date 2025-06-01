using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace 小说漫画阅读器
{
    public class MangaChapterrConfig : IEntityTypeConfiguration<MangaChapter>
    {
        public void Configure(EntityTypeBuilder<MangaChapter> builder)
        {
            builder.ToTable("T_MangaChapters");
            builder.HasOne<MangaWithUser>(e => e.MangaWithUser).WithMany(e=>e.MangaChapters);
        }
    }
}
