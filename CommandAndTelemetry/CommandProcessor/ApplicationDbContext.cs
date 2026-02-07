using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq;

namespace CommandProcessor
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Command> Commands { get; set; } = null!;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var dictConverter = new ValueConverter<Dictionary<string, string>, string>(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<Dictionary<string, string>>(v) ?? new Dictionary<string, string>());

            var dictComparer = new ValueComparer<Dictionary<string, string>>(
                (l, r) => JsonSerializer.Serialize(l, (JsonSerializerOptions?)null) == JsonSerializer.Serialize(r, (JsonSerializerOptions?)null),
                v => v == null ? 0 : v.Aggregate(0, (h, kv) => HashCode.Combine(h, kv.Key != null ? kv.Key.GetHashCode() : 0, kv.Value!= null ? kv.Value.GetHashCode() : 0)),
                v => v == null ? new Dictionary<string, string>() : new Dictionary<string, string>(v)
            );

            modelBuilder.Entity<Command>(b =>
            {
                b.HasKey(e => e.Id);
                b.Property(e => e.PacketName).IsRequired();
                b.Property(e => e.Target).IsRequired();

                var prop = b.Property(e => e.Parameters)
                    .HasConversion(dictConverter)
                    .HasColumnType("jsonb");

                prop.Metadata.SetValueComparer(dictComparer);
            });
        }
    }
}
