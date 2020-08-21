using Core.Services.Azure;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.File;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Services.Azure
{
    public class FileService
    {
        private readonly AzureOptions _options;
        private readonly CloudFileClient _client;

        public string ShareName { get; set; } = "YourShareName";
        public FileService(IOptions<AzureOptions> options)
        {
            _options = options.Value;
            var account = CloudStorageAccount.Parse(_options.BlobConnectionString);
            _client = account.CreateCloudFileClient();
        }

        /// <summary>
        /// 上传文件到根目录
        /// </summary>
        /// <param name="fileStream"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task<string> UploadFileAsync(Stream fileStream, string fileName)
        {
            var share = _client.GetShareReference(ShareName);
            var rootDir = share.GetRootDirectoryReference();
            var file = rootDir.GetFileReference(fileName);
            await file.UploadFromStreamAsync(fileStream);
            // 返回可访问的链接
            SharedAccessFilePolicy sasConstraints = new SharedAccessFilePolicy
            {
                SharedAccessExpiryTime = DateTime.UtcNow.AddHours(24),
                Permissions = SharedAccessFilePermissions.Read
            };

            var sas = file.GetSharedAccessSignature(sasConstraints);
            return file.Uri + sas;
        }

        /// <summary>
        /// 获取可访问链接
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public string GetSasUrl(string fileName)
        {
            var share = _client.GetShareReference(ShareName);
            var rootDir = share.GetRootDirectoryReference();
            var file = rootDir.GetFileReference(fileName);
            // 返回可访问的链接
            SharedAccessFilePolicy sasConstraints = new SharedAccessFilePolicy
            {
                SharedAccessExpiryTime = DateTime.UtcNow.AddHours(24),
                Permissions = SharedAccessFilePermissions.Read
            };
            var sas = file.GetSharedAccessSignature(sasConstraints);
            return file.Uri + sas;
        }

        public async Task Download(string fileName, string path)
        {
            var share = _client.GetShareReference(ShareName);
            var rootDir = share.GetRootDirectoryReference();
            var file = rootDir.GetFileReference(fileName);

            await file.DownloadToFileAsync(path, FileMode.OpenOrCreate);
        }
    }
}
