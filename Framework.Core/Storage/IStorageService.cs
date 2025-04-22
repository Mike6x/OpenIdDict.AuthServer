using Framework.Core.Storage.File;
using Framework.Core.Storage.File.Features;

namespace Framework.Core.Storage;

public interface IStorageService
{
    public Task<Uri> UploadAsync<T>(FileUploadCommand? request, FileType supportedFileType, CancellationToken cancellationToken = default)
        where T : class;

    public void Remove(Uri? path);

    public Uri? UnZip(Uri zipPath);

    public void RemoveFolder(string fullPath);

    public string GetLocalPathFromUri(Uri? path, bool isFullPath);
}

// Add from fsh