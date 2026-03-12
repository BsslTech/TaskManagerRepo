using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BSSLTaskManagement.Models;

public partial class TaskDbContext : DbContext
{
    public TaskDbContext()
    {
    }

    public TaskDbContext(DbContextOptions<TaskDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AssignTaskTab> AssignTaskTabs { get; set; }

    public virtual DbSet<Curtab> Curtabs { get; set; }

    public virtual DbSet<LicenseTab> LicenseTabs { get; set; }

    public virtual DbSet<MainMenu> MainMenus { get; set; }

    public virtual DbSet<MenuAccessTab> MenuAccessTabs { get; set; }

    public virtual DbSet<MenusetupTab> MenusetupTabs { get; set; }

    public virtual DbSet<ModuleSetup> ModuleSetups { get; set; }

    public virtual DbSet<OtpDetail> OtpDetails { get; set; }

    public virtual DbSet<ProjectSubCategoryTab> ProjectSubCategoryTabs { get; set; }

    public virtual DbSet<ProjectTab> ProjectTabs { get; set; }

    public virtual DbSet<StaffTab> StaffTabs { get; set; }

    public virtual DbSet<SubMenusetupTab> SubMenusetupTabs { get; set; }

    public virtual DbSet<SubMenusetupTabBack> SubMenusetupTabBacks { get; set; }

    public virtual DbSet<SystemDefTab> SystemDefTabs { get; set; }

    public virtual DbSet<SystemMenuTab> SystemMenuTabs { get; set; }

    public virtual DbSet<SystemSubMenuTab> SystemSubMenuTabs { get; set; }

    public virtual DbSet<SystemTypeTab> SystemTypeTabs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost;Database=TaskPerformanceDb;integrated security=true;MultipleActiveResultSets=true;TrustServerCertificate=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AssignTaskTab>(entity =>
        {
            entity.ToTable("AssignTaskTab");

            entity.HasIndex(e => e.ProjectSubCategoryTabId, "IX_AssignTaskTab_ProjectSubCategoryTabId");

            entity.HasIndex(e => e.StaffTabId, "IX_AssignTaskTab_StaffTabId");

            entity.Property(e => e.Duration).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Remarks).HasMaxLength(100);
            entity.Property(e => e.TaskDescription).HasMaxLength(200);
            entity.Property(e => e.TaskReferenceNumber).HasMaxLength(100);

            entity.HasOne(d => d.ProjectSubCategoryTab).WithMany(p => p.AssignTaskTabs).HasForeignKey(d => d.ProjectSubCategoryTabId);

            entity.HasOne(d => d.StaffTab).WithMany(p => p.AssignTaskTabs).HasForeignKey(d => d.StaffTabId);
        });

        modelBuilder.Entity<Curtab>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("CURTAB");

            entity.Property(e => e.CurKoboName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CurNairaName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Curcode)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("CURCODE");
            entity.Property(e => e.Curname)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("CURNAME");
            entity.Property(e => e.Exequacct)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("EXEQUACCT");
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Isocode)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("ISOCODE");
            entity.Property(e => e.Ratedate)
                .HasColumnType("datetime")
                .HasColumnName("RATEDATE");
            entity.Property(e => e.Raten)
                .HasColumnType("numeric(9, 2)")
                .HasColumnName("RATEN");
            entity.Property(e => e.Symbol)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("SYMBOL");
        });

        modelBuilder.Entity<LicenseTab>(entity =>
        {
            entity.ToTable("LicenseTab");

            entity.Property(e => e.CompCode).HasMaxLength(10);
        });

        modelBuilder.Entity<MainMenu>(entity =>
        {
            entity.ToTable("MainMenu");

            entity.HasIndex(e => e.ModuleSetupId, "IX_MainMenu_ModuleSetupId");

            entity.HasOne(d => d.ModuleSetup).WithMany(p => p.MainMenus).HasForeignKey(d => d.ModuleSetupId);
        });

        modelBuilder.Entity<MenuAccessTab>(entity =>
        {
            entity.ToTable("MenuAccessTab");

            entity.HasIndex(e => e.SubMenusetupTabId, "IX_MenuAccessTab_SubMenusetupTabId");

            entity.HasOne(d => d.SubMenusetupTab).WithMany(p => p.MenuAccessTabs).HasForeignKey(d => d.SubMenusetupTabId);
        });

        modelBuilder.Entity<MenusetupTab>(entity =>
        {
            entity.ToTable("MenusetupTab");

            entity.HasIndex(e => e.MainMenuId, "IX_MenusetupTab_MainMenuId");

            entity.Property(e => e.MenuCode).HasMaxLength(50);
            entity.Property(e => e.MenuName).HasMaxLength(50);
            entity.Property(e => e.ModuleName).HasMaxLength(50);

            entity.HasOne(d => d.MainMenu).WithMany(p => p.MenusetupTabs)
                .HasForeignKey(d => d.MainMenuId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ModuleSetup>(entity =>
        {
            entity.ToTable("ModuleSetup");

            entity.HasIndex(e => e.SystemTypeTabId, "IX_ModuleSetup_SystemTypeTabId");

            entity.HasOne(d => d.SystemTypeTab).WithMany(p => p.ModuleSetups).HasForeignKey(d => d.SystemTypeTabId);
        });

        modelBuilder.Entity<ProjectSubCategoryTab>(entity =>
        {
            entity.ToTable("ProjectSubCategoryTab");

            entity.HasIndex(e => e.ProjectTabId, "IX_ProjectSubCategoryTab_ProjectTabId");

            entity.HasOne(d => d.ProjectTab).WithMany(p => p.ProjectSubCategoryTabs).HasForeignKey(d => d.ProjectTabId);
        });

        modelBuilder.Entity<ProjectTab>(entity =>
        {
            entity.ToTable("ProjectTab");
        });

        modelBuilder.Entity<StaffTab>(entity =>
        {
            entity.ToTable("StaffTab");

            entity.Property(e => e.StaffId).HasMaxLength(10);
            entity.Property(e => e.StaffName).HasMaxLength(200);
            entity.Property(e => e.StaffType).HasMaxLength(50);
        });

        modelBuilder.Entity<SubMenusetupTab>(entity =>
        {
            entity.ToTable("SubMenusetupTab");

            entity.HasIndex(e => e.MenuId, "IX_SubMenusetupTab_MenuId");

            entity.Property(e => e.FormId).HasMaxLength(20);
            entity.Property(e => e.IconImagename).HasMaxLength(50);
            entity.Property(e => e.PageUrl).HasMaxLength(100);
            entity.Property(e => e.ReportPageUrl).HasMaxLength(100);
            entity.Property(e => e.SubMenuCode).HasMaxLength(20);
            entity.Property(e => e.SubMenuName).HasMaxLength(250);

            entity.HasOne(d => d.Menu).WithMany(p => p.SubMenusetupTabs).HasForeignKey(d => d.MenuId);
        });

        modelBuilder.Entity<SubMenusetupTabBack>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("SubMenusetupTabBACK");

            entity.Property(e => e.FormId).HasMaxLength(20);
            entity.Property(e => e.IconImagename).HasMaxLength(50);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.PageUrl).HasMaxLength(100);
            entity.Property(e => e.ReportPageUrl).HasMaxLength(100);
            entity.Property(e => e.SubMenuCode).HasMaxLength(20);
            entity.Property(e => e.SubMenuName).HasMaxLength(250);
        });

        modelBuilder.Entity<SystemDefTab>(entity =>
        {
            entity.ToTable("SystemDefTab");

            entity.HasIndex(e => e.SystemSubMenuTabId, "IX_SystemDefTab_SystemSubMenuTabId");

            entity.HasOne(d => d.SystemSubMenuTab).WithMany(p => p.SystemDefTabs).HasForeignKey(d => d.SystemSubMenuTabId);
        });

        modelBuilder.Entity<SystemMenuTab>(entity =>
        {
            entity.ToTable("SystemMenuTab");

            entity.Property(e => e.Code).HasMaxLength(20);
            entity.Property(e => e.Desc).HasMaxLength(30);
        });

        modelBuilder.Entity<SystemSubMenuTab>(entity =>
        {
            entity.ToTable("SystemSubMenuTab");

            entity.HasIndex(e => e.SystemMenuTabId, "IX_SystemSubMenuTab_SystemMenuTabId");

            entity.Property(e => e.Code)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Desc).HasMaxLength(150);

            entity.HasOne(d => d.SystemMenuTab).WithMany(p => p.SystemSubMenuTabs).HasForeignKey(d => d.SystemMenuTabId);
        });

        modelBuilder.Entity<SystemTypeTab>(entity =>
        {
            entity.ToTable("SystemTypeTab");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
