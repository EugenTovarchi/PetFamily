using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFamily.Domain.PetManagment.AggregateRoot;
using Shared.Constants;

namespace PetFamily.Infrastructure.Configurations;

public class VolunteerConfiguration : IEntityTypeConfiguration<Volunteer>
{
    public void Configure(EntityTypeBuilder<Volunteer> builder)
    {
        builder.ToTable("volunteers");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.Id)       //доп настройка для бд ключа
            .HasConversion(
            id => id.Value,
            value => VolunteerId.Create(value));

        builder.OwnsOne(v => v.VolunteerFullName,
            name =>
        {
            name.Property(n => n.FirstName).HasColumnName("first_name").IsRequired();
            name.Property(n => n.LastName).HasColumnName("last_name").IsRequired();
            name.Property(n => n.MiddleName).HasColumnName("middle_name");
        });

        builder.OwnsOne(v => v.Email, email =>
        {
            email.Property(e => e.Value)
                .HasColumnName("email")
                .HasMaxLength(Constants.MAX_MINOR_LENGTH)
                .IsRequired(); // NOT NULL
        });

        builder.OwnsOne(v => v.Phone,
            phone =>
        {
            phone.Property(p => p.Value)
                .HasColumnName("phone")
                .IsRequired();
        });

        builder.Property(v => v.VolunteerInfo)
            .HasColumnName("volunteer_info")
           .HasMaxLength(Constants.MAX_INFO_LENGTH);

        builder.OwnsMany(v => v.VolunteerSocialMedias,
            sm =>
        {
            sm.ToJson("volunteer_social_media");
            sm.Property(x => x.Title)
            .HasMaxLength(Constants.MAX_LOW_LENGTH);
            sm.Property(x => x.Url)
            .HasMaxLength(Constants.MAX_LOW_LENGTH);
        });

        builder.OwnsMany(v => v.Requisites,
            vr =>
        {
            vr.ToJson("volunteer_requisites");
            vr.Property(x => x.Title)
            .HasMaxLength(Constants.MAX_LOW_LENGTH);
            vr.Property(x => x.Value)
            .HasMaxLength(Constants.MAX_LOW_LENGTH);
        });

        builder.HasMany(v => v.Pets)
            .WithOne()
            .HasForeignKey("volunteer_id")
            .OnDelete(DeleteBehavior.NoAction);

        builder.Property(v => v.ExperienceYears)
            .HasColumnName("volunteer_exp_years")
           .HasMaxLength(Constants.MAX_MINOR_LENGTH)
           .IsRequired();

        builder.Property<bool>("_isDeleted")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("is_deleted");
    }
}
