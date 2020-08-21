using Core.Services.Azure;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Services.Azure
{
    /// <summary>
    /// blob 存储服务
    /// </summary>
    public class BlobService
    {
        private readonly AzureOptions _options;
        private readonly CloudBlobClient _client;
        private readonly IMemoryCache _cache;

        public string ContainerName { get; set; } = "YourContainerName";
        public BlobService(IOptions<AzureOptions> options, IMemoryCache memoryCache)
        {
            _options = options.Value;
            _cache = memoryCache;
            var account = CloudStorageAccount.Parse(_options.BlobConnectionString);
            _client = account.CreateCloudBlobClient();
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="fileStream"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task UploadFileStreamAsync(Stream fileStream, string fileName)
        {
            CloudBlobContainer container = _client.GetContainerReference(ContainerName);
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);
            await blockBlob.UploadFromStreamAsync(fileStream);
        }
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="filePath">完整路径</param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task UploadFileAsync(string filePath, string fileName)
        {
            CloudBlobContainer container = _client.GetContainerReference(ContainerName);
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);
            await blockBlob.UploadFromFileAsync(filePath);
        }
        /// <summary>
        /// 下载文件到stream
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        public async Task DownloadFileAsync(string filePath, Stream stream)
        {
            CloudBlobContainer container = _client.GetContainerReference(ContainerName);
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(filePath);
            //BlobRequestOptions options = new BlobRequestOptions
            //{
            //    DisableContentMD5Validation = true,
            //    StoreBlobContentMD5 = false
            //};
            await blockBlob.DownloadToStreamAsync(stream);
        }

        public string GetFileSAS(string filePath)
        {
            CloudBlobContainer container = _client.GetContainerReference(ContainerName);
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(filePath);

            var sas = blockBlob.GetSharedAccessSignature(new SharedAccessBlobPolicy
            {
                Permissions = SharedAccessBlobPermissions.Read,
                SharedAccessExpiryTime = DateTimeOffset.UtcNow.AddDays(100)
            });

            return blockBlob.Uri + sas;
        }

        /// <summary>
        /// 缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected double SaveProgress(string key, double value)
        {
            return _cache.Set(key, value);
        }
    }
}
