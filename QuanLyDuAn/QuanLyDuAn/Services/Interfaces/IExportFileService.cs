using QuanLyDuAn.Services.Exporting;

namespace QuanLyDuAn.Services.Interfaces
{
    public interface IExportFileService
    {
        ExportFileResult Export(ExportFileRequest request);
        ExportFileFormat ParseFormat(string? format);
    }
}
