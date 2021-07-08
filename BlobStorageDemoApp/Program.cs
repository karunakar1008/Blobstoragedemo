using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace BlobStorageDemoApp
{
    class Program
    {
        static string storageconnstring = "DefaultEndpointsProtocol=https;AccountName=storagaccountkpmgdemo;AccountKey=KfcmuPTw1xIjqsFpN9uefMxpTS82Jaen/LR3NPXk38joZXnW4vPs0URuQsBUesTFM20r28lDsIY2g4xgv1LcsA==;EndpointSuffix=core.windows.net";
        static string containerName = "images";
        static string filename = "DSC_0005.JPG";
        static string filepath = "E:\\demo blobs\\demo1.txt";
        static string downloadpath = "E:\\demo blobs\\Downloadedfilesfromazure";

        static void Main(string[] args)
        {
            Console.WriteLine("Learn Azure storage and how to work with different dot net classes");
            Console.WriteLine("Comment one by one and go through the code");

            //CreateContainer().Wait();
            //CreateBlob().Wait();
            //deleteblob().Wait();
            //Deletecontainer().Wait();

            //Uploadblobs().Wait();

            //GetBlobsNames().Wait();

            //GetAllBlobDownload().Wait();

            //GetSingleBlob().Wait();

            Console.WriteLine("Complete");

        }

        /// <summary>
        /// Create container
        /// </summary>
        /// <returns></returns>
        static async Task CreateContainer()
        {
            //storage account instance
            BlobServiceClient blobServiceClient = new BlobServiceClient(storageconnstring);

            //container
            BlobContainerClient containerClient = await blobServiceClient.CreateBlobContainerAsync(containerName);
        }

        /// <summary>
        /// Upoad blob
        /// </summary>
        /// <returns></returns>
        static async Task CreateBlob()
        {
            //reference to the storage account 
            BlobServiceClient blobServiceClient = new BlobServiceClient(storageconnstring);

            //getting the reference to the container 
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            containerClient.CreateIfNotExists();

            //getting the reference to the blob
            BlobClient blobClient = containerClient.GetBlobClient(filename); 


            using FileStream uploadFileStream = File.OpenRead(filepath); //reading the file content 

            await blobClient.UploadAsync(uploadFileStream, true);
            uploadFileStream.Close();
        }


        /// <summary>
        /// Delete blob
        /// </summary>
        /// <returns></returns>
        static async Task deleteblob()
        {

            BlobServiceClient blobServiceClient = new BlobServiceClient(storageconnstring);

            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);


            BlobClient blobClient = containerClient.GetBlobClient(filename); //getting the reference to the blob

            blobClient.DeleteIfExists();
        }

        /// <summary>
        /// Delete container
        /// </summary>
        /// <returns></returns>
        static async Task Deletecontainer()
        {

            BlobServiceClient blobServiceClient = new BlobServiceClient(storageconnstring);

            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            containerClient.DeleteIfExists();


        }


        /// <summary>
        /// Upload multile blobs
        /// </summary>
        /// <returns></returns>
        public static async Task Uploadblobs()
        {
            BlobServiceClient serviceclinet = new BlobServiceClient(storageconnstring);
            BlobContainerClient bc = serviceclinet.GetBlobContainerClient(containerName);
            bc.CreateIfNotExists();
            var files = Directory.GetFiles("E:\\demo blobs");

            foreach (string file in files)
            {

                BlobClient b = bc.GetBlobClient(Path.GetFileName(file));

                Dictionary<string, string> dict = new Dictionary<string, string>();
                dict.Add("CreatedBy", "Karuna");
                dict.Add("FileName", Path.GetFileName(file));
                dict.Add("FirstVersion", DateTime.Now.ToString());
                dict.Add("Department", "IT");
                dict.Add("ProjectName", "Internal");

                b.DeleteIfExists();

                bc.SetAccessPolicy(PublicAccessType.Blob);
                using FileStream filestream = File.OpenRead(file);
                await b.UploadAsync(filestream, true);
                await b.SetMetadataAsync(dict);
                //var leaseclinet = b.GetBlobLeaseClient();
                //TimeSpan leaseTime = TimeSpan.FromSeconds(60);
                //var b1 = await leaseclinet.AcquireAsync(leaseTime);

                //BlobProperties bp = b.GetProperties();

                //Console.WriteLine($"Blob state : {bp.LeaseState } , Lease Status : {bp.LeaseStatus }");
                //filestream.Close();
                Console.WriteLine("File Uplaoded" + file);

            }
           


           


        }

        /// <summary>
        /// Get all uploaded blobs names
        /// </summary>
        /// <returns></returns>
        static async Task GetBlobsNames()
        {

            BlobServiceClient blobServiceClient = new BlobServiceClient(storageconnstring);

            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);


            await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
            {
                Console.WriteLine("\t" + blobItem.Name);
            }

        }


        /// <summary>
        /// Download blobs
        /// </summary>
        /// <returns></returns>
        static async Task GetAllBlobDownload()
        {
            BlobServiceClient serviceclinet = new BlobServiceClient(storageconnstring);
            BlobContainerClient bc = serviceclinet.GetBlobContainerClient(containerName);

            await foreach (BlobItem item in bc.GetBlobsAsync())
            {
                BlobClient clinet = bc.GetBlobClient(item.Name);

                BlobDownloadInfo bd = await clinet.DownloadAsync();
                using (FileStream fs = File.OpenWrite(downloadpath + "\\" + item.Name))
                {
                    await bd.Content.CopyToAsync(fs);
                    fs.Close();
                }
            }


        }

        //Download single blob
         static async Task GetSingleBlob()
        {
            
            BlobServiceClient blobServiceClient = new BlobServiceClient(storageconnstring);
            
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            
            BlobClient blob = containerClient.GetBlobClient(filename);
            
            BlobDownloadInfo blobdata = await blob.DownloadAsync();
           
            using (FileStream fs = File.OpenWrite(downloadpath + "\\" + filename))
            {
                await blobdata.Content.CopyToAsync(fs);
                fs.Close();
            }

            // Read the new file
            using (FileStream downloadFileStream = File.OpenRead(downloadpath + "\\" + filename))
            {
                using var strreader = new StreamReader(downloadFileStream);
                string line;
                while ((line = strreader.ReadLine()) != null)
                {
                    Console.WriteLine(line);
                }
            }

        }

    }
}
