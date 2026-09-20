namespace GeneralInfoManagement.Domain.BaseInfo.BranchArchiveAgg.FileUpload
{
    public interface IBranchArchiveFileUploader
    {
        string Upload(string sourceFilePath, long companyId, long branchId);
        void Delete(string relativeFilePath);
        string GetFullPath(string relativeFilePath);
    }
}
