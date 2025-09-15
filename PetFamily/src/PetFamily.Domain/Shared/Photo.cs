namespace PetFamily.Domain.Shared;

public  class Photo
{
    public Guid Id { get; private set; }
    public string FileName { get; private set; } = string.Empty;
    public string ContentType { get; private set; } = string.Empty;
    public string BucketName { get; private set;} = string.Empty;
    public long Size { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool IsMain { get; private set; }
    public int Order { get; private set; }

    // Для Entity Framework
    private Photo() { }

    public Photo(
        string fileName,
        string contentType,
        string bucketName,
        long size,
        bool isMain = false,
        int order = 0)
    {
        Id = Guid.NewGuid();
        FileName = fileName;
        ContentType = contentType;
        BucketName = bucketName;
        Size = size;
        CreatedAt = DateTime.UtcNow;
        IsMain = isMain;
        Order = order;
    }

    public void MarkAsMain() => IsMain = true;
    public void RemoveAsMain() => IsMain = false;
    public void UpdateOrder(int order) => Order = order;
    public static string GenerateObjectName(Guid id, string fileName) => $"{id}_{fileName}";
}
