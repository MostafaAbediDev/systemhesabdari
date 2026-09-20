using GeneralInfoManagement.Domain.BaseInfo.BranchArchiveAgg.FileUpload;

namespace GeneralInfoManagement.Infrastructure.EFCore.Repository.FileUpload
{
    public class BranchArchiveFileUploader : IBranchArchiveFileUploader
    {
        private readonly string _rootPath;

        public BranchArchiveFileUploader()
        {
            _rootPath = Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.CommonApplicationData),
                "TaadolSoftware");
        }
        public string Upload(string sourceFilePath, long companyId, long branchId)
        {
            if (string.IsNullOrWhiteSpace(sourceFilePath))
                throw new ArgumentException("مسیر فایل الزامی است.");

            if (!File.Exists(sourceFilePath))
                throw new FileNotFoundException(
                    "فایل مورد نظر پیدا نشد.",
                    sourceFilePath);

            var extension = Path.GetExtension(sourceFilePath)
                .TrimStart('.')
                .ToLowerInvariant();

            if (string.IsNullOrWhiteSpace(extension))
                extension = "unknown";

            var folderPath = Path.Combine(
                _rootPath,
                "Companies",
                companyId.ToString(),
                "Branches",
                branchId.ToString(),
                "Archives",
                extension);

            Directory.CreateDirectory(folderPath);

            var originalFileName = Path.GetFileName(sourceFilePath);

            var uniqueFileName =
                $"{Guid.NewGuid():N}_{originalFileName}";

            var destinationPath =
                Path.Combine(folderPath, uniqueFileName);

            File.Copy(
                sourceFilePath,
                destinationPath,
                overwrite: false);

            var relativePath = Path.Combine(
                "Companies",
                companyId.ToString(),
                "Branches",
                branchId.ToString(),
                "Archives",
                extension,
                uniqueFileName);

            return relativePath.Replace(
                Path.DirectorySeparatorChar,
                '/');
        }

        public void Delete(string relativeFilePath)
        {
            if (string.IsNullOrWhiteSpace(relativeFilePath))
                return;

            var fullPath = GetFullPath(relativeFilePath);

            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }

        public string GetFullPath(string relativeFilePath)
        {
            if (string.IsNullOrWhiteSpace(relativeFilePath))
                return string.Empty;

            var cleanPath = relativeFilePath
                .Replace('/', Path.DirectorySeparatorChar);

            return Path.Combine(_rootPath, cleanPath);
        }
    }
}
