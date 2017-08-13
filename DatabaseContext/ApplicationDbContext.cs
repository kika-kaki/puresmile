using DatabaseContext.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace DatabaseContext
{
    public class ApplicationDbContext : IdentityDbContext<User, Role, int, UserLogin, UserRole, UserClaim>
    {
        public ApplicationDbContext() : base("DefaultConnection")
        {
        }

        public DbSet<Booking> Bookings { get; set; }
        public DbSet<ClientComment> ClientComments { get; set; }
        public DbSet<ClientData> ClientDatas { get; set; }
        public DbSet<Clinic> Clinics { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<NotificationTemplate> NotificationTemplates { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Treatment> Treatments { get; set; }
        public DbSet<TreatmentCategory> TreatmentCategories { get; set; }
        public DbSet<ClinicHours> ClinicHours { get; set; }
        public DbSet<Email> Emails { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<EmailConfirmation> EmailConfirmation { get; set; }
        public DbSet<GoogleCredentials> GoogleCredentials { get; set; }
        public DbSet<AppointmentBlock> AppointmentBlocks { get; set; }


        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Role>().ToTable("Role");
            modelBuilder.Entity<UserRole>().ToTable("UserRole");
            modelBuilder.Entity<UserClaim>().ToTable("UserClaim");
            modelBuilder.Entity<UserLogin>().ToTable("UserLogin");

            modelBuilder.Entity<User>().HasMany(c => c.PaymentRefundUsers).WithOptional().WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(c => c.PaymentUsers).WithRequired().WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(c => c.ClientCommentUsers).WithRequired().WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(c => c.ClientCommentAuthors).WithRequired().WillCascadeOnDelete(false);

            modelBuilder.Entity<Booking>().HasMany(c => c.Payments).WithRequired().WillCascadeOnDelete(false);

            modelBuilder.Entity<Clinic>().HasMany(c => c.TreatmentCategories).WithMany(x => x.Clinics)
                .Map(m =>
                {
                    m.MapLeftKey("ClinicId");
                    m.MapRightKey("TreatmentCategoryId");
                    m.ToTable("ClinicToTreatmentCategory");
                });
        }        
    }
}
