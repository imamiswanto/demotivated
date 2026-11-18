namespace Demotivated.Domain.Entities;

public class CollectionCategory
{
    public Guid CollectionId { get; set; }

    public Guid CategoryId { get; set; }

    // Navigation Properties
    public Collection? Collection { get; set; }

    public Category? Category { get; set; }
}