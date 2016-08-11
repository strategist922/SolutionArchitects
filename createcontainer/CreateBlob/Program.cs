// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Microsoft">
//   Microsoft 2016
// </copyright>
// <summary>
//   Creates a container and a blob in the given Azure Storage Account.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;

namespace CreateBlob
{
    /// <summary>
    /// The main class.
    /// </summary>
    class Program
    {
        /*http://stackoverflow.com/questions/19552476/access-issue-when-uploading-file-to-cloudblockblob-windows-azure*/
        static void Main(string[] args)
        {
            const string FileName = ".\\Resources\\blob.txt";

            try
            {
                string storageAccountName = ConfigurationManager.AppSettings["StorageAccountName"];
                string storageAccountKey = ConfigurationManager.AppSettings["StorageKey"];
                string storageContainerName = ConfigurationManager.AppSettings["StorageContainerName"];
                string storageBlobName = ConfigurationManager.AppSettings["StorageBlobName"];

                Console.WriteLine(string.Format("Account: {0}, Container: {1}, Blob: {2}", storageAccountName, storageContainerName, storageBlobName));

                var storageCredentials = new StorageCredentials(storageAccountName, storageAccountKey);
                var storageAccount = new CloudStorageAccount(storageCredentials, true);
                var storageClient = storageAccount.CreateCloudBlobClient();
                var container = storageClient.GetContainerReference(storageContainerName);
                container.CreateIfNotExists(BlobContainerPublicAccessType.Blob);
                var blob = container.GetBlockBlobReference(storageBlobName);
                blob.UploadFromFile(FileName, System.IO.FileMode.Open);

                Console.WriteLine("Successfully created the blob!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                throw new Exception("Web job failed", ex);
            }
        }
    }
}
