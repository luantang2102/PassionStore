using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using PassionStore.Core.Exceptions;
using PassionStore.Infrastructure.Settings;


namespace PassionStore.Infrastructure.Externals
{
    public class CloudinaryService
    {
        private readonly Cloudinary _cloudinary;
        private readonly CloudinaryOption _cloudinaryOption;

        public CloudinaryService(CloudinaryOption cloudinaryOption)
        {
            _cloudinaryOption = cloudinaryOption ?? throw new ArgumentNullException(nameof(cloudinaryOption));
            var account = new Account(
                _cloudinaryOption.CloudName,
                _cloudinaryOption.ApiKey,
                _cloudinaryOption.ApiSecret
            );
            _cloudinary = new Cloudinary(account);
        }

        public async Task<ImageUploadResult> AddImageAsync(IFormFile file)
        {
            var uploadResult = new ImageUploadResult();

            if (file.Length > 0)
            {
                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = "ecommerce-nash-app-images"
                };
                uploadResult = await _cloudinary.UploadAsync(uploadParams);
            }

            if (uploadResult.Error != null)
            {
                var attributes = new Dictionary<string, object>
                {
                    { "fileName", file.FileName },
                    { "error", uploadResult.Error }
                };
                throw new AppException(ErrorCode.CLOUDINARY_UPLOAD_FAILED, attributes);
            }

            return uploadResult;
        }

        public async Task<DeletionResult> DeleteImageAsync(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);

            var result = await _cloudinary.DestroyAsync(deleteParams);

            if (result.Error != null)
            {
                var attributes = new Dictionary<string, object>
                {
                    { "publicId", publicId },
                    { "error", result.Error }
                };
                throw new AppException(ErrorCode.CLOUDINARY_DELETE_FAILED, attributes);
            }

            return result;
        }
    }
}
