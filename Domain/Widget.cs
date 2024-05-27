using Domain.Conventions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain;

public class Widget : BaseTable
{
	public string Name { get; set; } = default!;
	public string? Description { get; set; }
	[Column(TypeName = "money")]
	public decimal Price { get; set; }
}

public class WidgetConfig : IEntityTypeConfiguration<Widget>
{
	public void Configure(EntityTypeBuilder<Widget> builder)
	{
		builder.HasAlternateKey(nameof(Widget.Name));
		builder.Property(e => e.Name).HasMaxLength(50).IsRequired();
		builder.Property(e => e.Description).HasMaxLength(255);
	}
}
