using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFamily.Domain.PetManagment.Entities;
using PetFamily.Domain.PetManagment.ValueObjects;
using PetFamily.Domain.PetManagment.ValueObjects.Ids;
using PetFamily.Domain.Shared;
using Shared.Constants;

namespace PetFamily.Infrastructure.Configurations;

public class PetConfiguration : IEntityTypeConfiguration<Pet>
{
    public void Configure(EntityTypeBuilder<Pet> builder)
    {
        builder.ToTable("pets");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasConversion(
            id => id.Value,
            value => PetId.Create(value));

        builder.Property(p => p.Name)
            .HasColumnName("name")
            .HasMaxLength(Constants.MAX_MINOR_LENGTH)
            .IsRequired();

        builder.Property(p => p.Description)
            .HasColumnName("description")
            .HasMaxLength(Constants.MAX_DESCRIPTIOM_LENGTH)
            .IsRequired(false);

        builder.Property(p => p.Color)
             .HasColumnName("color")
             .IsRequired();

        builder.Property(p => p.HealthInfo)
            .HasColumnName("health_info")
            .HasMaxLength(Constants.MAX_INFO_LENGTH)
            .IsRequired();

        builder.ComplexProperty(p => p.PetAddress, address =>    //можно использовать OwnsOne> но Complex указывает на VO
        {
            address.Property(a => a.City).HasColumnName("city").IsRequired();
            address.Property(a => a.Street).HasColumnName("street").IsRequired();
            address.Property(a => a.House).HasColumnName("house").IsRequired();
            address.Property(a => a.Flat).HasColumnName("flat");
        });

        builder.Property(p => p.Weight)
            .HasColumnName("weight")
            .IsRequired(false);

        builder.Property(p => p.Height)
            .HasColumnName("height")
            .IsRequired(false);

        builder.OwnsOne(v => v.OwnerPhone, ownerPhone =>
        {
            ownerPhone.Property(p => p.Value)
                .HasColumnName("owner_phone");
        });

        builder.Property(p => p.Vaccinated)
                .HasColumnName("vaccinated")
                .HasColumnType("boolean")
                .IsRequired();

        builder.Property(p => p.Castrated)
            .HasColumnName("castrated")
            .HasColumnType("boolean")
            .IsRequired(false);

        builder.Property(p => p.Birthday)
            .HasColumnName("birthday")
            .HasColumnType("date")
            .IsRequired(false);

        builder.Property(p => p.PetStatus)
            .HasColumnName("pet_status")
            .HasDefaultValue(PetStatus.LookingTreatment)
            .IsRequired();

        builder.Property(p => p.Position)
            .HasColumnName("position")
            .HasConversion(
             Position => Position.Value,
             value => Position.Create(value).Value);

        builder.Property(p => p.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("date")
            .IsRequired();

        builder.OwnsOne(p => p.PetType, petType =>
        {
            petType.Property(pt => pt.SpeciesId)
                .HasColumnName("species_id")
                .HasColumnType("uuid")
                .IsRequired();

            petType.Property(pt => pt.BreedId)
                .HasColumnName("breed_id")
                .HasColumnType("uuid")
                .IsRequired();
        });

        builder.OwnsMany(p => p.PetRequisites, pr =>
        {
            pr.ToJson("pet_requisites");
            pr.Property(x => x.Title)
            .HasMaxLength(Constants.MAX_LOW_LENGTH);
            pr.Property(x => x.Value)
            .HasMaxLength(Constants.MAX_LOW_LENGTH);
        });


        builder.Property<bool>("_isDeleted")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("is_deleted")
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property<DateTime?>("DeletionDate")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("deletion_date")
            .IsRequired(false);


        builder.OwnsOne(p => p.Photos, pb =>
        {
            pb.ToJson("photos");
            pb.OwnsMany(x=> x.Values, pb =>
            {
                pb.Property(p => p.PathToStorage)
                    .HasConversion(
                    pts => pts.Path,
                    value => PhotoPath.Create(value).Value)
                .IsRequired()
                .HasMaxLength(Constants.MAX_MINOR_LENGTH);
            });
        });

    }
}
