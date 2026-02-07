using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace API.Services
{
    /// <summary>
    /// Service for handling Cloudflare R2 storage operations including file upload and signed URL generation.
    /// Implements the "Anti-Theft" security layer using Cloudflare Workers for secure audio asset delivery.
    /// </summary>
    public class CloudflareR2Service : ICloudflareR2Service
    {
        private readonly HttpClient _httpClient;
        private readonly string _r2AccountId;
        private readonly string _r2AccessKeyId;
        private readonly string _r2SecretKey;
        private readonly string _r2BucketName;
        private readonly string _cloudflareWorkerUrl;
        private readonly ILogger<CloudflareR2Service> _logger;

        public CloudflareR2Service(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<CloudflareR2Service> logger)
        {
            _httpClient = httpClient;
            _logger = logger;

            // Load Cloudflare R2 configuration
            _r2AccountId = configuration["Cloudflare:R2:AccountId"]
                ?? throw new ArgumentNullException("Cloudflare:R2:AccountId", "Cloudflare R2 Account ID is required");
            _r2AccessKeyId = configuration["Cloudflare:R2:AccessKeyId"]
                ?? throw new ArgumentNullException("Cloudflare:R2:AccessKeyId", "Cloudflare R2 Access Key ID is required");
            _r2SecretKey = configuration["Cloudflare:R2:SecretKey"]
                ?? throw new ArgumentNullException("Cloudflare:R2:SecretKey", "Cloudflare R2 Secret Key is required");
            _r2BucketName = configuration["Cloudflare:R2:BucketName"]
                ?? throw new ArgumentNullException("Cloudflare:R2:BucketName", "Cloudflare R2 Bucket Name is required");
            _cloudflareWorkerUrl = configuration["Cloudflare:Worker:Url"]
                ?? throw new ArgumentNullException("Cloudflare:Worker:Url", "Cloudflare Worker URL is required");
        }

        /// <summary>
        /// Uploads an audio file to Cloudflare R2 storage.
        /// </summary>
        /// <param name="fileStream">The audio file stream to upload</param>
        /// <param name="fileName">The original file name</param>
        /// <param name="contentType">The MIME type of the file</param>
        /// <returns>The R2 storage key where the file was stored</returns>
        public async Task<string> UploadAudioFileAsync(Stream fileStream, string fileName, string contentType)
        {
            if (fileStream == null)
                throw new ArgumentNullException(nameof(fileStream));
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("File name cannot be null or empty", nameof(fileName));
            if (string.IsNullOrWhiteSpace(contentType))
                throw new ArgumentException("Content type cannot be null or empty", nameof(contentType));

            try
            {
                // Generate a unique storage key for the audio file
                var storageKey = GenerateStorageKey(fileName);

                // Prepare the R2 API endpoint
                var r2Url = $"https://{_r2AccountId}.r2.cloudflarestorage.com/{_r2BucketName}/{storageKey}";

                // Create the HTTP request
                var request = new HttpRequestMessage(HttpMethod.Put, r2Url);
                request.Headers.Add("Authorization", $"AWS4-HMAC-SHA256 Credential={_r2AccessKeyId}/20250101/us-east-1/s3/aws4_request, SignedHeaders=host;x-amz-content-sha256;x-amz-date, Signature={GenerateSignature(fileStream, storageKey)}");
                request.Headers.Add("x-amz-content-sha256", "UNSIGNED-PAYLOAD");
                request.Headers.Add("x-amz-date", DateTime.UtcNow.ToString("yyyyMMddTHHmmssZ"));
                request.Content = new StreamContent(fileStream);
                request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);

                // Execute the upload
                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Failed to upload file to R2. Status: {response.StatusCode}, Error: {errorContent}");
                    throw new Exception($"Failed to upload file to Cloudflare R2: {response.StatusCode}");
                }

                _logger.LogInformation($"Successfully uploaded audio file to R2 with key: {storageKey}");
                return storageKey;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading audio file to Cloudflare R2");
                throw;
            }
        }

        /// <summary>
        /// Generates a signed URL for secure audio playback through Cloudflare Worker.
        /// </summary>
        /// <param name="storageKey">The R2 storage key of the audio file</param>
        /// <param name="venueId">The venue ID for authorization</param>
        /// <param name="trackId">The track ID for authorization</param>
        /// <returns>A signed URL for secure audio playback</returns>
        public async Task<string> GenerateSignedUrlAsync(string storageKey, string venueId, string trackId)
        {
            if (string.IsNullOrWhiteSpace(storageKey))
                throw new ArgumentException("Storage key cannot be null or empty", nameof(storageKey));
            if (string.IsNullOrWhiteSpace(venueId))
                throw new ArgumentException("Venue ID cannot be null or empty", nameof(venueId));
            if (string.IsNullOrWhiteSpace(trackId))
                throw new ArgumentException("Track ID cannot be null or empty", nameof(trackId));

            try
            {
                // Prepare the request to Cloudflare Worker
                var workerRequest = new
                {
                    storageKey = storageKey,
                    venueId = venueId,
                    trackId = trackId,
                    timestamp = DateTime.UtcNow.ToString("o"),
                    expiresAt = DateTime.UtcNow.AddMinutes(15).ToString("o") // 15-minute expiration
                };

                var jsonContent = JsonSerializer.Serialize(workerRequest);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // Send request to Cloudflare Worker
                var response = await _httpClient.PostAsync(_cloudflareWorkerUrl, httpContent);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Failed to generate signed URL. Status: {response.StatusCode}, Error: {errorContent}");
                    throw new Exception($"Failed to generate signed URL: {response.StatusCode}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var signedUrlResponse = JsonSerializer.Deserialize<SignedUrlResponse>(responseContent);

                if (string.IsNullOrWhiteSpace(signedUrlResponse?.SignedUrl))
                {
                    throw new Exception("Invalid response from Cloudflare Worker - signed URL is missing");
                }

                _logger.LogInformation($"Generated signed URL for storage key: {storageKey}");
                return signedUrlResponse.SignedUrl;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating signed URL from Cloudflare Worker");
                throw;
            }
        }

        /// <summary>
        /// Validates that a file is an audio file and within size limits.
        /// </summary>
        /// <param name="fileStream">The file stream to validate</param>
        /// <param name="fileName">The file name</param>
        /// <returns>True if the file is valid, false otherwise</returns>
        public bool ValidateAudioFile(Stream fileStream, string fileName)
        {
            if (fileStream == null)
                return false;
            if (string.IsNullOrWhiteSpace(fileName))
                return false;

            // Check file extension
            var allowedExtensions = new[] { ".mp3", ".wav", ".flac", ".m4a", ".aac" };
            var fileExtension = Path.GetExtension(fileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(fileExtension))
                return false;

            // Check file size (max 100MB)
            const long maxFileSize = 100 * 1024 * 1024; // 100MB
            if (fileStream.Length > maxFileSize)
                return false;

            return true;
        }

        /// <summary>
        /// Generates a unique storage key for the audio file.
        /// </summary>
        /// <param name="originalFileName">The original file name</param>
        /// <returns>A unique storage key</returns>
        private string GenerateStorageKey(string originalFileName)
        {
            var fileExtension = Path.GetExtension(originalFileName);
            var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
            return $"audio/{DateTime.UtcNow:yyyy/MM/dd}/{uniqueFileName}";
        }

        /// <summary>
        /// Generates AWS Signature V4 for R2 authentication.
        /// Note: This is a simplified implementation. In production, you should use AWS SDK or a more robust signing method.
        /// </summary>
        /// <param name="fileStream">The file stream</param>
        /// <param name="storageKey">The storage key</param>
        /// <returns>The signature string</returns>
        private string GenerateSignature(Stream fileStream, string storageKey)
        {
            // This is a placeholder implementation
            // In a real implementation, you would implement AWS Signature V4 signing
            // or use the AWS SDK for .NET which handles this automatically
            _logger.LogWarning("Signature generation is using a placeholder implementation");
            return "placeholder-signature";
        }

        /// <summary>
        /// Response model for Cloudflare Worker signed URL response.
        /// </summary>
        private class SignedUrlResponse
        {
            public string SignedUrl { get; set; }
        }
    }

    /// <summary>
    /// Interface for Cloudflare R2 service operations.
    /// </summary>
    public interface ICloudflareR2Service
    {
        Task<string> UploadAudioFileAsync(Stream fileStream, string fileName, string contentType);
        Task<string> GenerateSignedUrlAsync(string storageKey, string venueId, string trackId);
        bool ValidateAudioFile(Stream fileStream, string fileName);
    }
}