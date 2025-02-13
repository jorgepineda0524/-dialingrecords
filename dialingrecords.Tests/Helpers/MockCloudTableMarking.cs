﻿using dialingrecords.Functions.Entities;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace dialingrecords.Tests.Helpers
{
    public class MockCloudTableMarking : CloudTable
    {
        public MockCloudTableMarking(Uri tableAddress) : base(tableAddress)
        {
        }

        public MockCloudTableMarking(Uri tableAbsoluteUri, StorageCredentials credentials) : base(tableAbsoluteUri, credentials)
        {
        }

        public MockCloudTableMarking(StorageUri tableAddress, StorageCredentials credentials) : base(tableAddress, credentials)
        {
        }

        public override async Task<TableResult> ExecuteAsync(TableOperation operation)
        {
            return await Task.FromResult(new TableResult
            {
                HttpStatusCode = 200,
                Result = TestFactory.GetMarkingsEntity()
            });
        }

        public override async Task<TableQuerySegment<ITableEntity>> ExecuteQuerySegmentedAsync<ITableEntity>(TableQuery<ITableEntity> query, TableContinuationToken token)
        {
            ConstructorInfo constructor = typeof(TableQuerySegment<ITableEntity>)
                   .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
                   .FirstOrDefault(c => c.GetParameters().Count() == 1);

            return await Task.FromResult(constructor.Invoke(new object[] { TestFactory.GetMarkingsEntity() }) as TableQuerySegment<ITableEntity>);
        }
    }
}
