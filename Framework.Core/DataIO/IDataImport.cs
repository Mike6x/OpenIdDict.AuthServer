using Framework.Core.Storage.File;
using Framework.Core.Storage.File.Features;

namespace Framework.Core.DataIO;

public interface IDataImport
{
    Task<IList<T>> ToListAsync<T>(FileUploadCommand request, FileType supportedFileType, string sheetName = "Sheet1");
}
