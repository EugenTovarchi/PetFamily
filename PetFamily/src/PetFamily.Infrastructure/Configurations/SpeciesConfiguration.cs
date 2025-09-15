using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFamily.Domain.PetManagment.AggregateRoot;
using PetFamily.Domain.PetManagment.ValueObjects.Ids;
using Shared.Constants;

namespace PetFamily.Infrastructure.Configurations;

public class SpeciesConfiguration : IEntityTypeConfiguration<Species>
{
    public void Configure(EntityTypeBuilder<Species> builder)
    {
        builder.ToTable("species");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .HasConversion(
                id => id.Value,          
                value => SpeciesId.Create(value)  
            )
            .HasColumnName("id"); 

        builder.Property(s => s.Title)
            .IsRequired()
            .HasMaxLength(Constants.MAX_LOW_LENGTH)
            .HasColumnName("title");

        builder.OwnsMany(s => s.Breeds, breed =>
        {
            breed.ToJson("breeds"); 

            breed.Property(b => b.Id)
                .HasConversion(
                    id => id.Value,
                    value => BreedId.Create(value))
                .HasColumnName("id");

            breed.Property(b => b.Title)
                .IsRequired()
                .HasMaxLength(Constants.MAX_LOW_LENGTH)
                .HasColumnName("title");
        });
    }
}

