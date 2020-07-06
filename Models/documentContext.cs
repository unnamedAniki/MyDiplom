using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using AccoutingDocs.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Newtonsoft.Json;

namespace AccoutingDocs.Models
{
    public partial class documentContext : DbContext
    {
        public documentContext()
        {
            try
            {
                Database.EnsureCreated();
            }
            catch
            {
                if(MessageBox.Show("Произошла ошибка при подключении к базе данных! Проверьте в папке программы файл 'Строка подключения.txt'!", "Ошибка!", MessageBoxButton.OK) == MessageBoxResult.OK)
                {
                    App.Current.Shutdown();
                }
            }
        }

        public documentContext(DbContextOptions<documentContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Register> Register { get; set; }
        public virtual DbSet<Documents> Documents { get; set; }
        public virtual DbSet<Kind> Kind { get; set; }
        public virtual DbSet<Organization> Organization { get; set; }
        public virtual DbSet<Roles> Roles { get; set; }
        public virtual DbSet<Staff> Staff { get; set; }
        public virtual DbSet<Staffdocuments> Staffdocuments { get; set; }
        public virtual DbSet<Status> Status { get; set; }
        public virtual DbSet<Type> Type { get; set; }
        public virtual DbSet<Userroles> Userroles { get; set; }
        public virtual DbSet<Users> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.NullValueHandling = NullValueHandling.Ignore;
                serializer.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                var temp = new Connection()
                {
                    Database = "document",
                    Username = "root",
                    Password = "dedede223E",
                    Server = "localhost"
                };
                if (!File.Exists(Environment.CurrentDirectory + @"\Строка подключения.txt"))
                {
                    using (StreamWriter sw = new StreamWriter(Environment.CurrentDirectory + @"\Строка подключения.txt"))
                    using (JsonWriter writer = new JsonTextWriter(sw))
                    {
                        serializer.Serialize(writer, temp);
                    }
                }
                else
                {
                    using (StreamReader sw = new StreamReader(Environment.CurrentDirectory + @"\Строка подключения.txt"))
                    using (JsonReader writer = new JsonTextReader(sw))
                    {
                        temp = serializer.Deserialize<Connection>(writer);
                    }
                }
                optionsBuilder.UseMySql(Connection.Connect(temp), x => x.ServerVersion("8.0.18-mysql"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Documents>(entity =>
            {
                entity.ToTable("documents");

                entity.HasIndex(e => e.Iddocument)
                    .HasName("fk_documents_kind_documents_idx");

                entity.HasIndex(e => e.NameFrom)
                    .HasName("fk_mails_idx");

                entity.HasIndex(e => e.StatusId)
                    .HasName("fk_currentdocument_status1_idx");

                entity.HasIndex(e => e.UserId)
                    .HasName("fk_documents_users1_idx");

                entity.HasIndex(e => e.EditUserId)
                    .HasName("fk_documents_users2_idx");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.AddingDate).HasColumnType("datetime");

                entity.Property(e => e.Iddocument)
                    .HasColumnName("iddocument")
                    .HasColumnType("int(11)");

                entity.Property(e => e.MoveDateToArchieve).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Commend)
                    .IsRequired()
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.NameFrom)
                    .HasColumnName("Name_From")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Path)
                    .IsRequired()
                    .HasColumnType("longtext")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.StatusId)
                    .HasColumnName("Status_ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasColumnName("User_ID")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.EditUserId)
                    .HasColumnName("EditUser_ID")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.UsingDate).HasColumnType("datetime");

                entity.HasOne(d => d.IddocumentNavigation)
                    .WithMany(p => p.Documents)
                    .HasForeignKey(d => d.Iddocument)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_documents_kind_documents");

                entity.HasOne(d => d.NameFromNavigation)
                    .WithMany(p => p.Documents)
                    .HasForeignKey(d => d.NameFrom)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_documents_organization");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.Documents)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_documents_status");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Documents)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("fk_documents_users1");

                entity.HasOne(d => d.EditUser)
                    .WithMany(p => p.EditDocuments)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasForeignKey(d => d.EditUserId)
                    .HasConstraintName("fk_documents_users2");
            });

            modelBuilder.Entity<Kind>(entity =>
            {
                entity.ToTable("kind");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.KindName)
                    .IsRequired()
                    .HasColumnName("Kind_Name")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<Organization>(entity =>
            {
                entity.HasKey(e => e.IdOrganization)
                    .HasName("PRIMARY");

                entity.ToTable("organization");

                entity.Property(e => e.IdOrganization)
                    .HasColumnName("idOrganization")
                    .HasColumnType("int(11)");

                entity.Property(e => e.HeadName)
                    .IsRequired()
                    .HasColumnName("Head_Name")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Mail)
                    .IsRequired()
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<Register>(entity =>
            {
                entity.ToTable("register");

                entity.HasIndex(e => e.DocumentId)
                    .HasName("fk_register_documents1_idx");

                entity.HasIndex(e => e.StatusId)
                    .HasName("fk_register_documents2_idx");

                entity.HasIndex(e => e.UserId)
                    .HasName("fk_register_users1_idx");

                entity.HasIndex(e => new { e.DocumentId, e.StatusId })
                    .HasName("fk_register_document1_idx");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.DocumentId)
                    .HasColumnName("document_ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ReturningDate).HasColumnType("datetime");

                entity.Property(e => e.StatusId)
                    .HasColumnName("StatusID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.TakenDate).HasColumnType("datetime");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasColumnName("User_ID")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.HasOne(d => d.Document)
                    .WithMany(p => p.Register)
                    .HasForeignKey(d => d.DocumentId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_register_documents1");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Register)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("fk_register_users1");
            });

            modelBuilder.Entity<Roles>(entity =>
            {
                entity.ToTable("roles");

                entity.Property(e => e.Id)
                    .HasColumnType("varchar(256)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Access)
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(256)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");
            });

            modelBuilder.Entity<Staff>(entity =>
            {
                entity.ToTable("staff");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Staff_Name)
                    .IsRequired()
                    .HasColumnName("Staff_Name")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<Staffdocuments>(entity =>
            {
                entity.ToTable("staffdocuments");

                entity.HasIndex(e => e.DocumentId)
                    .HasName("fk_StaffDocuments_documents1_idx");

                entity.HasIndex(e => e.HeadUserId)
                    .HasName("fk_staffdocuments_users1_idx");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Comments)
                    .IsRequired()
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.DocumentId)
                    .HasColumnName("Document_ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.EndingDate).HasColumnType("datetime");

                entity.Property(e => e.HeadUserId)
                    .IsRequired()
                    .HasColumnName("HeadUser_ID")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.HasOne(d => d.Document)
                    .WithMany(p => p.Staffdocuments)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasForeignKey(d => d.DocumentId)
                    .HasConstraintName("fk_StaffDocuments_documents1");

                entity.HasOne(d => d.HeadUser)
                    .WithMany(p => p.Staffdocuments)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasForeignKey(d => d.HeadUserId)
                    .HasConstraintName("fk_staffdocuments_users1");
            });

            modelBuilder.Entity<Status>(entity =>
            {
                entity.ToTable("status");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Status1)
                    .IsRequired()
                    .HasColumnName("Status")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<Type>(entity =>
            {
                entity.HasKey(e => e.DocumentsId)
                    .HasName("PRIMARY");

                entity.ToTable("type");

                entity.HasIndex(e => e.KindId)
                    .HasName("fk_Type_Kind1_idx");

                entity.Property(e => e.DocumentsId)
                    .HasColumnName("documents_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.DateOfExpire).HasColumnType("datetime");

                entity.Property(e => e.KindId)
                    .HasColumnName("Kind_ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Number)
                    .IsRequired()
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.HasOne(d => d.Kind)
                    .WithMany(p => p.Type)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasForeignKey(d => d.KindId)
                    .HasConstraintName("fk_Type_Kind1");
            });

            modelBuilder.Entity<Userroles>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoleId })
                    .HasName("PRIMARY");

                entity.ToTable("userroles");

                entity.HasIndex(e => e.RoleId)
                    .HasName("IX_UserRoles_RoleId");

                entity.Property(e => e.UserId)
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.RoleId)
                    .HasColumnType("varchar(256)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Userroles)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK_UserRoles_Roles_RoleId1");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Userroles)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_UserRoles_Users_UserId1");
            });

            modelBuilder.Entity<Users>(entity =>
            {
                entity.ToTable("users");

                entity.HasIndex(e => e.StaffId)
                    .HasName("fk_aspneusers_staff_staffid1_idx");

                entity.Property(e => e.Id)
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasColumnType("varchar(256)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Fam)
                    .IsRequired()
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Lastname)
                    .IsRequired()
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.PasswordHash)
                    .IsRequired()
                    .HasColumnType("longtext")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.StaffId).HasColumnType("int(11)");

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasColumnType("varchar(256)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.HasOne(d => d.Staff)
                    .WithMany(p => p.Users)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasForeignKey(d => d.StaffId)
                    .HasConstraintName("fk_users_staff_staffid1");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
