using ShairiStore.Enums;

namespace ShairiStore.Services;

public interface IExportService
{
    Task<byte[]> ExportAsync(ExportTypes exportType, IEnumerable<string>? columns = null, int? targetId = null);
}
