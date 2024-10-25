using Amazon.S3.Transfer;
using Amazon.S3;
using Amazon.S3.Model;

namespace WebAPI.Services
{
    public class S3Service
    {
        private readonly string _bucketName;
        private readonly IAmazonS3 _s3Client;

        public S3Service(IConfiguration configuration)
        {
            _bucketName = configuration["AWS:BucketName"];
            _s3Client = new AmazonS3Client(configuration["AWS:AccessKey"], configuration["AWS:SecretKey"], Amazon.RegionEndpoint.GetBySystemName(configuration["AWS:Region"]));
        }

        public async Task UploadFileAsync(IFormFile file)
        {
            using (var newMemoryStream = new MemoryStream())
            {
                file.CopyTo(newMemoryStream);
                var uploadRequest = new TransferUtilityUploadRequest
                {
                    InputStream = newMemoryStream,
                    Key = file.FileName,
                    BucketName = _bucketName,
                    //CannedACL = S3CannedACL.PublicRead // Makes the file publicly accessible
                };

                var fileTransferUtility = new TransferUtility(_s3Client);
                await fileTransferUtility.UploadAsync(uploadRequest);
            }
        }

        public async Task DeleteFileAsync(string fileName, string bucketName)
        {
            var deleteRequest = new DeleteObjectRequest
            {
                BucketName = bucketName,
                Key = fileName
            };
            await _s3Client.DeleteObjectAsync(deleteRequest);
        }

    }
}
