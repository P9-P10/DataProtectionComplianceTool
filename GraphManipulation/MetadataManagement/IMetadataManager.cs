namespace GraphManipulation.MetadataManagement;

public interface IMetadataManager
{
    public void CreateMetadataTables();

    public void DropMetadataTables();

    public void MarkAsPersonalData(GDPRMetadata metadata);

    public void UpdateMetadataEntry(int entryId, GDPRMetadata value);

    public void DeleteMetadataEntry(int entryId);

    public IEnumerable<GDPRMetadata> GetAllMetadataEntries();

    public GDPRMetadata? GetMetadataEntry(int entryId);

    public IEnumerable<GDPRMetadata> GetMetadataWithNullValues();
}