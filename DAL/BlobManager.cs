using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.StorageClient;
using System.Configuration;
using System.IO;
using System.Data.SqlClient;
using System.Data;


namespace AJSolutions.DAL
{

    public class BlobManager
    {

        public string DefaultUrl()
        {
            // Retrieve storage account from connection string.
            Microsoft.WindowsAzure.Storage.CloudStorageAccount StorageAccount = Microsoft.WindowsAzure.Storage.CloudStorageAccount.Parse(Convert.ToString(ConfigurationManager.AppSettings["StorageConnectionString"]));

            return Convert.ToString(StorageAccount.BlobStorageUri.PrimaryUri);
        }

        //Retrieve Blob Container
        public Microsoft.WindowsAzure.Storage.Blob.CloudBlobContainer GetCloudBlobContainer(string ContainerName, Boolean PublicAccess = false)
        {
            // Retrieve storage account from connection string.
            Microsoft.WindowsAzure.Storage.CloudStorageAccount StorageAccount = Microsoft.WindowsAzure.Storage.CloudStorageAccount.Parse(Convert.ToString(ConfigurationManager.AppSettings["StorageConnectionString"]));

            // Create the blob client.
            Microsoft.WindowsAzure.Storage.Blob.CloudBlobClient blobClient = StorageAccount.CreateCloudBlobClient();

            // Retrieve a reference to a container.
            Microsoft.WindowsAzure.Storage.Blob.CloudBlobContainer container = blobClient.GetContainerReference(ContainerName);

            // Create the container if it doesn't exist.
            container.CreateIfNotExists();


            //Set permission to public
            if (PublicAccess)
                container.SetPermissions(
                    new Microsoft.WindowsAzure.Storage.Blob.BlobContainerPermissions
                    {
                        PublicAccess =
                          Microsoft.WindowsAzure.Storage.Blob.BlobContainerPublicAccessType.Blob
                    });
            else
                container.SetPermissions(
                   new Microsoft.WindowsAzure.Storage.Blob.BlobContainerPermissions
                   {
                       PublicAccess =
                         Microsoft.WindowsAzure.Storage.Blob.BlobContainerPublicAccessType.Off
                   });

            return container;
        }

        // Upload a Blob File
        public void UploadBlob(string UserId, string BlobName, HttpPostedFileBase image, int timeout = 60)
        {

            // Retrieve a reference to a container.
            Microsoft.WindowsAzure.Storage.Blob.CloudBlobContainer container = GetCloudBlobContainer(UserId);

            // Retrieve reference to a blob named 
            Microsoft.WindowsAzure.Storage.Blob.CloudBlockBlob blockBlob = container.GetBlockBlobReference(BlobName);

            blockBlob.Properties.ContentType = image.ContentType;
            blockBlob.UploadFromStream(image.InputStream);
        }

        //Upload a Blob File
        public void UploadByteBlob(string UserId, string BlobName, string ContentType, byte[] image)
        {

            // Retrieve a reference to a container.
            Microsoft.WindowsAzure.Storage.Blob.CloudBlobContainer container = GetCloudBlobContainer(UserId);

            // Retrieve reference to a blob named 
            Microsoft.WindowsAzure.Storage.Blob.CloudBlockBlob blockBlob = container.GetBlockBlobReference(BlobName);

            blockBlob.Properties.ContentType = ContentType;
            blockBlob.UploadFromByteArray(image, 0, image.Length);
        }

        // Get url of blob
        public string DownloadBlob(string UserId, string BlobName)
        {
            // Retrieve a reference to a container.
            Microsoft.WindowsAzure.Storage.Blob.CloudBlobContainer container = GetCloudBlobContainer(UserId);

            // Retrieve reference to a blob named   

            Microsoft.WindowsAzure.Storage.Blob.CloudBlockBlob blockBlob = container.GetBlockBlobReference(BlobName);

            //var sasToken = blockBlob.GetSharedAccessSignature(new SharedAccessBlobPolicy()
            //{
            //    Permissions = SharedAccessBlobPermissions.Read,
            //    SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(10),//assuming the blob can be downloaded in 10 miinutes
            //}, new SharedAccessBlobHeaders()
            //{
            //    ContentDisposition = "attachment; filename=file-name"
            //});

            //return string.Format("{0}{1}", blockBlob.Uri, sasToken);

            return blockBlob.Uri.ToString();
        }

        public string DownloadPublicBlob(string UserId, string BlobName, bool PublicAccess = true)
        {
            //Retrieve a reference to a container.
            //Retrieve storage account from connection string.
            Microsoft.WindowsAzure.Storage.CloudStorageAccount StorageAccount = Microsoft.WindowsAzure.Storage.CloudStorageAccount.Parse(Convert.ToString(ConfigurationManager.AppSettings["StorageConnectionString"]));

            // Create the blob client.
            Microsoft.WindowsAzure.Storage.Blob.CloudBlobClient blobClient = StorageAccount.CreateCloudBlobClient();

            // Retrieve a reference to a container.
            Microsoft.WindowsAzure.Storage.Blob.CloudBlobContainer container = blobClient.GetContainerReference(UserId);

            // Create the container if it doesn't exist.
            container.CreateIfNotExists();


            //Set permission to public
            if (PublicAccess)
                container.SetPermissions(
                    new Microsoft.WindowsAzure.Storage.Blob.BlobContainerPermissions
                    {
                        PublicAccess =
                          Microsoft.WindowsAzure.Storage.Blob.BlobContainerPublicAccessType.Blob
                    });
            else
                container.SetPermissions(
                   new Microsoft.WindowsAzure.Storage.Blob.BlobContainerPermissions
                   {
                       PublicAccess =
                         Microsoft.WindowsAzure.Storage.Blob.BlobContainerPublicAccessType.Off
                   });



            // Retrieve reference to a blob named   

            Microsoft.WindowsAzure.Storage.Blob.CloudBlockBlob blockBlob = container.GetBlockBlobReference(BlobName);

            //var sasToken = blockBlob.GetSharedAccessSignature(new SharedAccessBlobPolicy()
            //{
            //    Permissions = SharedAccessBlobPermissions.Read,
            //    SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(10),//assuming the blob can be downloaded in 10 miinutes
            //}, new SharedAccessBlobHeaders()
            //{
            //    ContentDisposition = "attachment; filename=file-name"
            //});

            //return string.Format("{0}{1}", blockBlob.Uri, sasToken);

            return blockBlob.Uri.ToString();
        }

        public string DownloadSharedBlob(string UserId, string BlobName)
        {

            //Retrieve storage account from connection string.
            Microsoft.WindowsAzure.Storage.CloudStorageAccount StorageAccount = Microsoft.WindowsAzure.Storage.CloudStorageAccount.Parse(Convert.ToString(ConfigurationManager.AppSettings["StorageConnectionString"]));

            // Create the blob client.
            Microsoft.WindowsAzure.Storage.Blob.CloudBlobClient blobClient = StorageAccount.CreateCloudBlobClient();

            // Retrieve a reference to a container.
            Microsoft.WindowsAzure.Storage.Blob.CloudBlobContainer container = blobClient.GetContainerReference(UserId);

            var blob = container.GetBlockBlobReference(BlobName);
            var sasToken = blob.GetSharedAccessSignature(new SharedAccessBlobPolicy()
            {
                Permissions = SharedAccessBlobPermissions.Read,
                SharedAccessExpiryTime = DateTimeOffset.UtcNow.AddMinutes(15),
            }, new SharedAccessBlobHeaders()
            {
                ContentDisposition = "attachment; filename=\"somefile.pdf\"",
            });
            var downloadUrl = string.Format("{0}{1}", blob.Uri.AbsoluteUri, sasToken);
            return downloadUrl;
        }

        //This function is using storage client ddl of blob 
        public byte[] DownloadBlobClient(string UserId, string BlobName)
        {
            // Retrieve storage account from connection string.
            Microsoft.WindowsAzure.CloudStorageAccount StorageAccount = Microsoft.WindowsAzure.CloudStorageAccount.Parse(Convert.ToString(ConfigurationManager.AppSettings["StorageConnectionString"]));

            // Create the blob client.
            Microsoft.WindowsAzure.StorageClient.CloudBlobClient blobClient = StorageAccount.CreateCloudBlobClient();

            // Retrieve a reference to a container.
            Microsoft.WindowsAzure.StorageClient.CloudBlobContainer container = blobClient.GetContainerReference(UserId);

            // Create the container if it doesn't exist.
            container.CreateIfNotExist();


            //Set permission to public           
            container.SetPermissions(
                new Microsoft.WindowsAzure.StorageClient.BlobContainerPermissions
                {
                    PublicAccess =
                      Microsoft.WindowsAzure.StorageClient.BlobContainerPublicAccessType.Off
                });


            // Retrieve reference to a blob named   

            Microsoft.WindowsAzure.StorageClient.CloudBlockBlob blockBlob = container.GetBlockBlobReference(BlobName);


            return blockBlob.DownloadByteArray();

        }



        // Access multiple blob at a time
        public List<string> DownloadBlobList(string UserId, string UserTable = null)
        {
            Microsoft.WindowsAzure.Storage.Blob.CloudBlobContainer container = GetCloudBlobContainer(UserId);

            List<string> blobs = new List<string>();

            if (UserTable != null)
            {
                foreach (var blobItem in container.ListBlobs(UserTable))
                {
                    blobs.Add(Convert.ToString(blobItem.Uri));
                }
            }
            else
            {
                foreach (var blobItem in container.ListBlobs(UserId))
                {
                    blobs.Add(Convert.ToString(blobItem.Uri));
                }

            }
            return blobs;
        }

        //Delete a blob file
        public void DeleteBlob(string UserId, string BlobName)
        {
            Microsoft.WindowsAzure.Storage.Blob.CloudBlobContainer container = GetCloudBlobContainer(UserId);

            Microsoft.WindowsAzure.Storage.Blob.CloudBlockBlob blockBlob = container.GetBlockBlobReference(BlobName);
            if (blockBlob.Exists())
                blockBlob.Delete();
        }

    }
}