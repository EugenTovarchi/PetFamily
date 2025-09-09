using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFamily.Domain.PetManagment.Entities;
using PetFamily.Domain.PetManagment.ValueObjects.Ids;
using Shared.Constants;

namespace PetFamily.Infrastructure.Configurations;

//public class BreedConfiguration : IEntityTypeConfiguration<Breed>
//{
//    public void Configure(EntityTypeBuilder<Breed> builder)
//    {
//        builder.ToTable("breeds");

//        builder.HasKey(s => s.Id);
//        builder.Property(v => v.Id)       
//            .HasConversion(
//            id => id.Value,
//            value => BreedId.Create(value));

//        builder.Property(s => s.Title)
//            .IsRequired()
//            .HasMaxLength(Constants.MAX_LOW_LENGTH)
//            .HasColumnName("title");

//        builder.Property(b => b.SpeciesId)
//            .IsRequired()
//            .HasColumnName("species_id")
//            .HasConversion(  
//                id => id.Value,
//                value => SpeciesId.Create(value));

//        builder.HasOne<Species>()  
//            .WithMany(s => s.Breeds)  
//            .HasForeignKey(b => b.SpeciesId)  
//            .OnDelete(DeleteBehavior.Cascade);  
//    }
//}

