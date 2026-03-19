using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using TaskManagement.Models;
using static TaskManagement.IdentityLib;

namespace TaskManagement
{
    public class TaskDbContext: IdentityDbContext
    {
        public TaskDbContext()
        {
        }

        public TaskDbContext(DbContextOptions<TaskDbContext> options)
            : base(options)
        {
        }
        public DbSet<TaskIdentityUser> TaskIdentityUsers { get; set; }
        public DbSet<TaskIdentityRole> TaskIdentityRoles { get; set; }
        public  DbSet<AssignTaskTab> AssignTaskTab { get; set; }

        public  DbSet<Curtab> Curtab { get; set; }

        public  DbSet<LicenseTab> LicenseTab { get; set; }

        public  DbSet<MainMenu> MainMenu { get; set; }

        public  DbSet<MenuAccessTab> MenuAccessTab { get; set; }

        public  DbSet<MenusetupTab> MenusetupTab { get; set; }

        public  DbSet<ModuleSetup> ModuleSetup { get; set; }

        public  DbSet<OtpDetail> OtpDetails { get; set; }

        public  DbSet<ProjectSubCategoryTab> ProjectSubCategoryTabs { get; set; }

        public  DbSet<ProjectTab> ProjectTabs { get; set; }

        public  DbSet<StaffTab> StaffTab { get; set; }

        public  DbSet<SubMenusetupTab> SubMenusetupTab { get; set; }

        public  DbSet<SubMenusetupTabBack> SubMenusetupTabBacks { get; set; }

        public  DbSet<SystemDefTab> SystemDefTab { get; set; }

        public  DbSet<SystemMenuTab> SystemMenuTab { get; set; }

        public  DbSet<SystemSubMenuTab> SystemSubMenuTab { get; set; }

        public  DbSet<SystemTypeTab> SystemTypeTab { get; set; }
        public  DbSet<ClientTab> ClientTab { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<AssignTaskTab>(entity =>
        //    {
        //        entity.ToTable("AssignTaskTab");

        //        entity.HasIndex(e => e.ProjectSubCategoryTabId, "IX_AssignTaskTab_ProjectSubCategoryTabId");

        //        entity.HasIndex(e => e.StaffTabId, "IX_AssignTaskTab_StaffTabId");

        //        entity.Property(e => e.Duration).HasColumnType("decimal(18, 2)");
        //        entity.Property(e => e.Remarks).HasMaxLength(100);
        //        entity.Property(e => e.TaskDescription).HasMaxLength(200);
        //        entity.Property(e => e.TaskReferenceNumber).HasMaxLength(100);

        //        entity.HasOne(d => d.ProjectSubCategoryTab).WithMany(p => p.AssignTaskTabs).HasForeignKey(d => d.ProjectSubCategoryTabId);

        //        entity.HasOne(d => d.StaffTab).WithMany(p => p.AssignTaskTabs).HasForeignKey(d => d.StaffTabId);
        //    });

        //    modelBuilder.Entity<Curtab>(entity =>
        //    {
        //        entity
        //            .HasNoKey()
        //            .ToTable("CURTAB");

        //        entity.Property(e => e.CurKoboName)
        //            .HasMaxLength(50)
        //            .IsUnicode(false);
        //        entity.Property(e => e.CurNairaName)
        //            .HasMaxLength(50)
        //            .IsUnicode(false);
        //        entity.Property(e => e.Curcode)
        //            .HasMaxLength(10)
        //            .IsUnicode(false)
        //            .HasColumnName("CURCODE");
        //        entity.Property(e => e.Curname)
        //            .HasMaxLength(250)
        //            .IsUnicode(false)
        //            .HasColumnName("CURNAME");
        //        entity.Property(e => e.Exequacct)
        //            .HasMaxLength(30)
        //            .IsUnicode(false)
        //            .HasColumnName("EXEQUACCT");
        //        entity.Property(e => e.Id).ValueGeneratedOnAdd();
        //        entity.Property(e => e.Isocode)
        //            .HasMaxLength(20)
        //            .IsUnicode(false)
        //            .HasColumnName("ISOCODE");
        //        entity.Property(e => e.Ratedate)
        //            .HasColumnType("datetime")
        //            .HasColumnName("RATEDATE");
        //        entity.Property(e => e.Raten)
        //            .HasColumnType("numeric(9, 2)")
        //            .HasColumnName("RATEN");
        //        entity.Property(e => e.Symbol)
        //            .HasMaxLength(5)
        //            .IsUnicode(false)
        //            .HasColumnName("SYMBOL");
        //    });

        //    modelBuilder.Entity<LicenseTab>(entity =>
        //    {
        //        entity.ToTable("LicenseTab");

        //        entity.Property(e => e.CompCode).HasMaxLength(10);
        //    });

        //    modelBuilder.Entity<MainMenu>(entity =>
        //    {
        //        entity.ToTable("MainMenu");

        //        entity.HasIndex(e => e.ModuleSetupId, "IX_MainMenu_ModuleSetupId");

        //        entity.HasOne(d => d.ModuleSetup).WithMany(p => p.MainMenus).HasForeignKey(d => d.ModuleSetupId);
        //    });

        //    modelBuilder.Entity<MenuAccessTab>(entity =>
        //    {
        //        entity.ToTable("MenuAccessTab");

        //        entity.HasIndex(e => e.SubMenusetupTabId, "IX_MenuAccessTab_SubMenusetupTabId");

        //        entity.HasOne(d => d.SubMenusetupTab).WithMany(p => p.MenuAccessTabs).HasForeignKey(d => d.SubMenusetupTabId);
        //    });

        //    modelBuilder.Entity<MenusetupTab>(entity =>
        //    {
        //        entity.ToTable("MenusetupTab");

        //        entity.HasIndex(e => e.MainMenuId, "IX_MenusetupTab_MainMenuId");

        //        entity.Property(e => e.MenuCode).HasMaxLength(50);
        //        entity.Property(e => e.MenuName).HasMaxLength(50);
        //        entity.Property(e => e.ModuleName).HasMaxLength(50);

        //        entity.HasOne(d => d.MainMenu).WithMany(p => p.MenusetupTabs)
        //            .HasForeignKey(d => d.MainMenuId)
        //            .OnDelete(DeleteBehavior.Cascade);
        //    });

        //    modelBuilder.Entity<ModuleSetup>(entity =>
        //    {
        //        entity.ToTable("ModuleSetup");

        //        entity.HasIndex(e => e.SystemTypeTabId, "IX_ModuleSetup_SystemTypeTabId");

        //        entity.HasOne(d => d.SystemTypeTab).WithMany(p => p.ModuleSetups).HasForeignKey(d => d.SystemTypeTabId);
        //    });

        //    modelBuilder.Entity<ProjectSubCategoryTab>(entity =>
        //    {
        //        entity.ToTable("ProjectSubCategoryTab");

        //        entity.HasIndex(e => e.ProjectTabId, "IX_ProjectSubCategoryTab_ProjectTabId");

        //        entity.HasOne(d => d.ProjectTab).WithMany(p => p.ProjectSubCategoryTabs).HasForeignKey(d => d.ProjectTabId);
        //    });

        //    modelBuilder.Entity<ProjectTab>(entity =>
        //    {
        //        entity.ToTable("ProjectTab");
        //    });

        //    modelBuilder.Entity<StaffTab>(entity =>
        //    {
        //        entity.ToTable("StaffTab");

        //        entity.Property(e => e.StaffId).HasMaxLength(10);
        //        entity.Property(e => e.StaffName).HasMaxLength(200);
        //        entity.Property(e => e.StaffType).HasMaxLength(50);
        //    });

        //    modelBuilder.Entity<SubMenusetupTab>(entity =>
        //    {
        //        entity.ToTable("SubMenusetupTab");

        //        entity.HasIndex(e => e.MenuId, "IX_SubMenusetupTab_MenuId");

        //        entity.Property(e => e.FormId).HasMaxLength(20);
        //        entity.Property(e => e.IconImagename).HasMaxLength(50);
        //        entity.Property(e => e.PageUrl).HasMaxLength(100);
        //        entity.Property(e => e.ReportPageUrl).HasMaxLength(100);
        //        entity.Property(e => e.SubMenuCode).HasMaxLength(20);
        //        entity.Property(e => e.SubMenuName).HasMaxLength(250);

        //        entity.HasOne(d => d.Menu).WithMany(p => p.SubMenusetupTabs).HasForeignKey(d => d.MenuId);
        //    });

        //    modelBuilder.Entity<SubMenusetupTabBack>(entity =>
        //    {
        //        entity
        //            .HasNoKey()
        //            .ToTable("SubMenusetupTabBACK");

        //        entity.Property(e => e.FormId).HasMaxLength(20);
        //        entity.Property(e => e.IconImagename).HasMaxLength(50);
        //        entity.Property(e => e.Id).ValueGeneratedOnAdd();
        //        entity.Property(e => e.PageUrl).HasMaxLength(100);
        //        entity.Property(e => e.ReportPageUrl).HasMaxLength(100);
        //        entity.Property(e => e.SubMenuCode).HasMaxLength(20);
        //        entity.Property(e => e.SubMenuName).HasMaxLength(250);
        //    });

        //    modelBuilder.Entity<SystemDefTab>(entity =>
        //    {
        //        entity.ToTable("SystemDefTab");

        //        entity.HasIndex(e => e.SystemSubMenuTabId, "IX_SystemDefTab_SystemSubMenuTabId");

        //        entity.HasOne(d => d.SystemSubMenuTab).WithMany(p => p.SystemDefTabs).HasForeignKey(d => d.SystemSubMenuTabId);
        //    });

        //    modelBuilder.Entity<SystemMenuTab>(entity =>
        //    {
        //        entity.ToTable("SystemMenuTab");

        //        entity.Property(e => e.Code).HasMaxLength(20);
        //        entity.Property(e => e.Desc).HasMaxLength(30);
        //    });

        //    modelBuilder.Entity<SystemSubMenuTab>(entity =>
        //    {
        //        entity.ToTable("SystemSubMenuTab");

        //        entity.HasIndex(e => e.SystemMenuTabId, "IX_SystemSubMenuTab_SystemMenuTabId");

        //        entity.Property(e => e.Code)
        //            .HasMaxLength(200)
        //            .IsUnicode(false);
        //        entity.Property(e => e.Desc).HasMaxLength(150);

        //        entity.HasOne(d => d.SystemMenuTab).WithMany(p => p.SystemSubMenuTabs).HasForeignKey(d => d.SystemMenuTabId);
        //    });

        //    modelBuilder.Entity<SystemTypeTab>(entity =>
        //    {
        //        entity.ToTable("SystemTypeTab");
        //    });

        //    OnModelCreatingPartial(modelBuilder);
        //}

        //partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
