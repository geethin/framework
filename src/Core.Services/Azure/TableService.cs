using Core.Services.Azure;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Azure
{
    public class TableService
    {
        private readonly AzureOptions _options;
        private readonly CloudTableClient _client;
        private static readonly string tableName = "YourTableName";

        public TableService(IOptions<AzureOptions> options)
        {
            _options = options.Value;
            var account = CloudStorageAccount.Parse(_options.BlobConnectionString);
            _client = account.CreateCloudTableClient();
        }

        /// <summary>
        /// 插入配置项
        /// </summary>
        /// <returns></returns>
        public async Task<List<SettingEntity>> InsertAndUpdateAsync(Dictionary<string, object> dic)
        {
            var result = new List<SettingEntity>();
            try
            {
                var table = _client.GetTableReference(tableName);
                var options = new TableBatchOperation();
                foreach (var item in dic)
                {
                    var entity = new SettingEntity
                    {
                        RowKey = item.Key,
                        Value = item.Value.ToString()
                    };
                    options.InsertOrMerge(entity);
                }
                var results = await table.ExecuteBatchAsync(options);
                result = results.Select(r => r.Result as SettingEntity).ToList();
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return default;
            }
        }
        /// <summary>
        /// 查询配置项
        /// </summary>
        /// <param name="partitionKey"></param>
        /// <returns></returns>
        public async Task<List<SettingEntity>> PartitionScanAsync(string partitionKey)
        {
            var result = new List<SettingEntity>();
            try
            {
                TableQuery<SettingEntity> partitionScanQuery =
                    new TableQuery<SettingEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey));

                TableContinuationToken token = null;
                var table = _client.GetTableReference(tableName);
                do
                {
                    TableQuerySegment<SettingEntity> segment = await table.ExecuteQuerySegmentedAsync(partitionScanQuery, token);
                    token = segment.ContinuationToken;
                    result.AddRange(segment);
                }
                while (token != null);
                return result;
            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                throw;
            }
        }
    }

    public class SettingEntity : TableEntity
    {
        public SettingEntity()
        {
            PartitionKey = "";
        }
        public string Value { get; set; }

    }
}
