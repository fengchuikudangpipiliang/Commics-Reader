using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace 小说漫画阅读器
{
    public class ImgFileConfig : IEntityTypeConfiguration<ImgFile>
    {
        public void Configure(EntityTypeBuilder<ImgFile> builder)
        {
            builder.ToTable("T_ImgFiles");
            builder.HasOne<MangaChapter>(e => e.MangaChapter).WithMany(e => e.imgFiles);
        }
    }
}
