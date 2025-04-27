using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Framework.Core.Origin;
using Framework.Core.Storage;
using Framework.Core.Storage.File;
using Framework.Core.Storage.File.Features;
using Framework.Infrastructure.Common.Extensions;
using Microsoft.Extensions.Options;

namespace Framework.Infrastructure.Storage.Files;

public class LocalFileStorageService(IOptions<OriginOptions> originSettings) : IStorageService
{
    public async Task<Uri> UploadAsync<T>(FileUploadCommand? request, FileType supportedFileType, CancellationToken cancellationToken = default)
        where T : class
    {
        if (request?.Data == null)
        {
            return null!;
        }

        if (request?.Extension is null || !supportedFileType.GetDescriptionList().Contains(request.Extension.ToLower(System.Globalization.CultureInfo.CurrentCulture)))
            throw new InvalidOperationException("File Format Not Supported.");
        if (request.Name is null)
            throw new InvalidOperationException("Name is required.");

        // string base64Data = Regex.Match(request.Data, "data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value
        string base64Data = Regex.Match(request.Data,
            $"data:{supportedFileType.ToString().ToLower()}/(?<type>.+?),(?<data>.+)").Groups["data"].Value;

        var streamData = new MemoryStream(Convert.FromBase64String(base64Data));
        if (streamData.Length > 0)
        {
            string folder = typeof(T).Name;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                folder = folder.Replace(@"\", "/", StringComparison.Ordinal);
            }

            string folderName = supportedFileType switch
            {
                FileType.Image => Path.Combine("assets", "images", folder),
                FileType.QuizMedia => Path.Combine("assets", "quizs", folder),
                FileType.Document => Path.Combine("assets", "documents", folder),
                _ => Path.Combine("assets", "others", folder),
            };
            string pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
            Directory.CreateDirectory(pathToSave);

            string fileName = request.Name.Trim('"');
            fileName = RemoveSpecialCharacters(fileName);
            fileName = fileName.ReplaceWhitespace("-");
            fileName += request.Extension.Trim();
            string fullPath = Path.Combine(pathToSave, fileName);
            string dbPath = Path.Combine(folderName, fileName);
            if (System.IO.File.Exists(dbPath))
            {
                dbPath = NextAvailableFilename(dbPath);
                fullPath = NextAvailableFilename(fullPath);
            }

            using var stream = new FileStream(fullPath, FileMode.Create);
            await streamData.CopyToAsync(stream, cancellationToken);
            var path = dbPath.Replace("\\", "/", StringComparison.Ordinal);
            var imageUri = new Uri(originSettings.Value.OriginUrl!, path);
            return imageUri;
        }
        else
        {
            return null!;
        }
    }

    public static string RemoveSpecialCharacters(string str)
    {
        return Regex.Replace(str, "[^a-zA-Z0-9_.]+", string.Empty, RegexOptions.Compiled);
    }

    public void Remove(Uri? path)
    {
        // var pathString = path!.ToString()
        var pathString = GetLocalPathFromUri(path!, true);
        if (System.IO.File.Exists(pathString))
        {
            System.IO.File.Delete(pathString);
        }
    }

    private const string NumberPattern = "-{0}";

    private static string NextAvailableFilename(string path)
    {
        if (!System.IO.File.Exists(path))
        {
            return path;
        }

        if (Path.HasExtension(path))
        {
            return GetNextFilename(path.Insert(path.LastIndexOf(Path.GetExtension(path), StringComparison.Ordinal), NumberPattern));
        }

        return GetNextFilename(path + NumberPattern);
    }

    private static string GetNextFilename(string pattern)
    {
        string tmp = string.Format(pattern, 1);

        if (!System.IO.File.Exists(tmp))
        {
            return tmp;
        }

        int min = 1, max = 2;

        while (System.IO.File.Exists(string.Format(pattern, max)))
        {
            min = max;
            max *= 2;
        }

        while (max != min + 1)
        {
            int pivot = (max + min) / 2;
            if (System.IO.File.Exists(string.Format(pattern, pivot)))
            {
                min = pivot;
            }
            else
            {
                max = pivot;
            }
        }

        return string.Format(pattern, max);
    }

    public void RemoveFolder(string fullPath)
    {

        if (Directory.Exists(fullPath))
            Directory.Delete(fullPath, true);
    }

    public Uri? UnZip(Uri? zipPath)
    {
        if (zipPath == null) return null;

        var zipFullPath = GetLocalPathFromUri(zipPath, true);

        if (!System.IO.File.Exists(zipFullPath)) return null;

        string unzipFullPath = Regex.Replace(zipFullPath, ".zip", string.Empty);

        ZipFile.ExtractToDirectory(zipFullPath, unzipFullPath, true);

        string unzipRelativePath = Regex.Replace(GetLocalPathFromUri(zipPath, false), ".zip", string.Empty);

        return new Uri(originSettings.Value.OriginUrl!, unzipRelativePath);
    }

    /// <summary>
    /// isFullPass = true, return full local path
    /// isFullPass = false, return relative Path
    /// </summary>
    /// <param name="path"></param>
    /// <param name="isFullPath"></param>
    /// <returns></returns>
    public string GetLocalPathFromUri(Uri? path, bool isFullPath)
    {
        if (path == null) return string.Empty;

        string relativePath = Regex.Replace(path.AbsolutePath, "/assets/", "assets/");

        return isFullPath ? Path.Combine(Directory.GetCurrentDirectory(), relativePath) : relativePath;
    }


}
