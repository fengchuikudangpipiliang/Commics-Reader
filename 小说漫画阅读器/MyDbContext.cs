using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
namespace 小说漫画阅读器
{
    public class MyDbContext : DbContext
    {
        private readonly ILoggerFactory _loggerFactory = LoggerFactory.Create(log => log.AddConsole());

        public DbSet<User> Users { get; set; }
        public DbSet<MangaWithUser> MangaWithsUsers {  get; set; }
        public DbSet<MangaChapter> mangaChapters { get; set; }
        public DbSet<ImgFile> ImgFiles { get; set; }
        public DbSet<UsersLove> UsersLoves { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer("Server=(local);Database=Comics;TrustServerCertificate=True;Integrated Security=True;Encrypt=True;");
            optionsBuilder.UseLoggerFactory(_loggerFactory);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }
    }
}
