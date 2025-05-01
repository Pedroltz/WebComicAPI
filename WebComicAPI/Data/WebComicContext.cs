using Microsoft.EntityFrameworkCore;
using WebComicAPI.Models;

namespace WebComicAPI.Data
{
    public class WebComicContext : DbContext
    {
        public WebComicContext(DbContextOptions<WebComicContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Manga> Mangas { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<MangaGenre> MangaGenres { get; set; }
        public DbSet<Chapter> Chapters { get; set; }
        public DbSet<Page> Pages { get; set; }
        public DbSet<MyList> MyLists { get; set; }
        public DbSet<BannerHighlight> BannerHighlights { get; set; }
        public DbSet<Admin> Admins { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurar chave composta MangaGenre
            modelBuilder.Entity<MangaGenre>()
                .HasKey(mg => new { mg.MangaId, mg.GenreId });

            modelBuilder.Entity<MangaGenre>()
                .HasOne(mg => mg.Manga)
                .WithMany(m => m.MangaGenres)
                .HasForeignKey(mg => mg.MangaId);

            modelBuilder.Entity<MangaGenre>()
                .HasOne(mg => mg.Genre)
                .WithMany(g => g.MangaGenres)
                .HasForeignKey(mg => mg.GenreId);

            // Configurar chave composta MyList
            modelBuilder.Entity<MyList>()
                .HasKey(ml => new { ml.UserId, ml.MangaId });

            modelBuilder.Entity<MyList>()
                .HasOne(ml => ml.User)
                .WithMany(u => u.MyLists)
                .HasForeignKey(ml => ml.UserId);

            modelBuilder.Entity<MyList>()
                .HasOne(ml => ml.Manga)
                .WithMany()
                .HasForeignKey(ml => ml.MangaId);
        }
    }
}
