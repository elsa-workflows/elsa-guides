using System;
using Storage.Net;
using Storage.Net.Blobs;

namespace DocumentManagement.Core.Options
{
    public class DocumentStorageOptions
    {
        public Func<IBlobStorage> BlobStorageFactory { get; set; } = StorageFactory.Blobs.InMemory;
    }
}