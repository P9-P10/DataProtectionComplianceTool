using GraphManipulation.Logging;
using GraphManipulation.MetadataManagement;

namespace GraphManipulation.Decorators;

public class LoggingMetadataManager : IMetadataManager
{
    private readonly IMetadataManager _metadataManager;
    private readonly ILogger _logger;

    public LoggingMetadataManager(IMetadataManager metadataManager, ILogger logger)
    {
        _metadataManager = metadataManager;
        _logger = logger;
    }

    public void CreateMetadataTables()
    {
        _metadataManager.CreateMetadataTables();
        _logger.Append(new MutableLog(LogType.Metadata, LogMessageFormat.Plaintext, "Created metadata tables"));
    }

    public void DropMetadataTables()
    {
        _metadataManager.DropMetadataTables();
        _logger.Append(new MutableLog(LogType.Metadata, LogMessageFormat.Plaintext, "Dropped metadata tables"));
    }

    public void MarkAsPersonalData(GDPRMetadata metadata)
    {
        _metadataManager.MarkAsPersonalData(metadata);
        var logMessage =
            $"{metadata.TargetTable}, {metadata.TargetColumn} marked as personal data{CreateEndOfLogMessage(metadata)}";

        _logger.Append(new MutableLog(LogType.Metadata, LogMessageFormat.Plaintext, logMessage));
    }

    private static string CreateEndOfLogMessage(GDPRMetadata metadata)
    {
        var filledDataString = CreateFilledDataString(metadata);
        var missingDataString = CreateMissingDataString(metadata);

        return (string.IsNullOrEmpty(filledDataString) ? "." : filledDataString) +
               (string.IsNullOrEmpty(missingDataString) ? "" : missingDataString);
    }

    private static string CreateFilledDataString(GDPRMetadata metadata)
    {
        var filledDataFields = GetNotNullMetadataFieldValues(metadata);

        if (filledDataFields.Count == 0)
        {
            return "";
        }

        return
            $" and has the following metadata: {string.Join(", ", filledDataFields.Select(pair => $"{pair.Key} = {pair.Value}"))}.";
    }

    private static string CreateMissingDataString(GDPRMetadata metadata)
    {
        var nullFields = GetNullMetadataFields(metadata);

        if (nullFields.Count == 0)
        {
            return "";
        }

        return $" The following metadata is missing: {string.Join(", ", nullFields)}";
    }

    private static List<string> GetNullMetadataFields(GDPRMetadata metadata)
    {
        var result = new List<string>();

        if (string.IsNullOrEmpty(metadata.Purpose))
        {
            result.Add("purpose");
        }

        if (string.IsNullOrEmpty(metadata.Origin))
        {
            result.Add("origin");
        }

        if (metadata.LegallyRequired == null)
        {
            result.Add("legally_required");
        }

        return result;
    }

    private static Dictionary<string, string> GetNotNullMetadataFieldValues(GDPRMetadata metadata)
    {
        var result = new Dictionary<string, string>();

        if (!string.IsNullOrEmpty(metadata.Purpose))
        {
            result.Add("purpose", metadata.Purpose);
        }

        if (!string.IsNullOrEmpty(metadata.Origin))
        {
            result.Add("origin", metadata.Origin);
        }

        if (metadata.LegallyRequired != null)
        {
            result.Add("legally_required", metadata.LegallyRequired.ToString()!);
        }

        return result;
    }

    public void UpdateMetadataEntry(int entryId, GDPRMetadata value)
    {
        _metadataManager.UpdateMetadataEntry(entryId, value);

        var updatedMetadata = _metadataManager.GetMetadataEntry(entryId);
        
        var logMessage =
            $"Metadata for {updatedMetadata.TargetTable}, {updatedMetadata.TargetColumn} updated{CreateEndOfLogMessage(updatedMetadata)}";

        _logger.Append(new MutableLog(LogType.Metadata, LogMessageFormat.Plaintext, logMessage));
    }

    public void DeleteMetadataEntry(int entryId)
    {
        throw new NotImplementedException();
    }

    public void LinkVacuumingRuleToMetadata(int ruleId, int metadataId)
    {
        throw new NotImplementedException();
    }
    
    public IEnumerable<GDPRMetadata> GetAllMetadataEntries()
    {
        throw new NotImplementedException();
    }

    public GDPRMetadata? GetMetadataEntry(int entryId)
    {
        return _metadataManager.GetMetadataEntry(entryId);
    }

    public IEnumerable<GDPRMetadata> GetMetadataWithNullValues()
    {
        return _metadataManager.GetMetadataWithNullValues();
    }
}