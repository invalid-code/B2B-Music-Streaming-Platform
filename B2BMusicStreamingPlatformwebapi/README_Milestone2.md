# B2B Music Streaming Platform - Milestone 2 Implementation

## Overview

This document details the implementation of **Milestone 2: The Core Streaming Engine** for the B2B Music Streaming Platform. This milestone focuses on implementing the "Zero-Install" player experience and the secure delivery of audio assets using Cloudflare R2 and Signed URLs.

## Features Implemented

### 1. Media Upload Pipeline (Task C.1)

**Endpoint**: `POST /api/tracks/upload`

**Functionality**:

- Accepts multipart form data with track metadata and audio file
- Validates audio files (supported formats: MP3, WAV, FLAC, M4A, AAC)
- Enforces file size limit (100MB maximum)
- Uploads audio files to Cloudflare R2 storage
- Creates track records in PostgreSQL database
- Returns upload status and track details

**Request Format**:

```json
{
  "title": "Track Title",
  "artist": "Artist Name",
  "mood": "Track Mood/Genre",
  "audioFile": "multipart/form-data file"
}
```

**Response Format**:

```json
{
  "trackID": "string",
  "title": "string",
  "artist": "string",
  "mood": "string",
  "cloudflareStorageKey": "string",
  "uploadedAt": "2025-01-01T00:00:00Z",
  "success": true,
  "errorMessage": null
}
```

### 2. Stream Authorization Endpoint (Task C.2)

**Endpoint**: `POST /api/tracks/authorize-stream`

**Functionality**:

- Validates track existence in database
- Generates time-limited signed URLs via Cloudflare Worker
- Implements "Anti-Theft" security layer
- Returns signed URL for secure audio playback
- Default expiration: 15 minutes

**Request Format**:

```json
{
  "trackId": "string",
  "venueId": "string",
  "playbackDuration": 900
}
```

**Response Format**:

```json
{
  "trackId": "string",
  "venueId": "string",
  "signedUrl": "https://...",
  "expiresAt": "2025-01-01T00:15:00Z",
  "authorizedDuration": 900,
  "success": true,
  "errorMessage": null
}
```

## Architecture Components

### Cloudflare R2 Service (`CloudflareR2Service.cs`)

**Key Features**:

- File upload to Cloudflare R2 with AWS Signature V4
- Signed URL generation via Cloudflare Worker
- Audio file validation (format and size)
- Comprehensive error handling and logging
- Scalable for future enhancements

**Configuration**:

```json
{
  "Cloudflare": {
    "R2": {
      "AccountId": "your-r2-account-id",
      "AccessKeyId": "your-r2-access-key-id",
      "SecretKey": "your-r2-secret-key",
      "BucketName": "your-r2-bucket-name"
    },
    "Worker": {
      "Url": "https://your-worker.your-account.workers.dev/signed-url"
    }
  }
}
```

### Enhanced Track Service (`TrackService.cs`)

**New Methods**:

- `UploadTrackAsync()`: Handles complete upload workflow
- `AuthorizeStreamAsync()`: Manages stream authorization

**Error Handling**:

- Comprehensive validation and error responses
- Structured error messages for frontend consumption
- Logging for debugging and monitoring

### Updated Track Controller (`TracksController.cs`)

**New Endpoints**:

- `POST /api/tracks/upload`: File upload endpoint
- `POST /api/tracks/authorize-stream`: Stream authorization endpoint

**Features**:

- Model validation with DataAnnotations
- Proper HTTP status codes
- Integration with dependency injection
- Comprehensive documentation

## Security Features

### Anti-Theft Protection

- **Signed URLs**: Time-limited URLs that expire after 15 minutes
- **Direct URL Protection**: Direct R2 URLs return 403 Forbidden
- **Worker Validation**: Cloudflare Worker validates session tokens
- **Venue Authorization**: Only authorized venues can access tracks

### File Validation

- **Format Validation**: Only supports audio formats (MP3, WAV, FLAC, M4A, AAC)
- **Size Limits**: Maximum 100MB per file
- **Content Type**: Validates MIME types

## Scalability Considerations

### Cloud Infrastructure

- **Cloudflare R2**: Object storage with global CDN
- **Cloudflare Workers**: Edge compute for signed URL generation
- **PostgreSQL**: Relational database for metadata
- **ASP.NET Core**: High-performance web API

### Performance Optimizations

- **Async Operations**: All I/O operations are asynchronous
- **Streaming Uploads**: Files are streamed to R2 without loading into memory
- **Connection Pooling**: HTTP client reuse for R2 operations
- **Caching**: Cloudflare CDN for fast content delivery

### Future Enhancements

- **AWS SDK Integration**: Replace manual signature generation
- **File Processing**: Audio analysis and metadata extraction
- **Batch Operations**: Bulk upload and processing
- **Monitoring**: Metrics and alerting for production

## Testing Considerations

### Unit Tests (Recommended)

```csharp
// Test file upload validation
[Fact]
public async Task UploadTrackAsync_InvalidFile_ReturnsError()
{
    // Test implementation
}

// Test signed URL generation
[Fact]
public async Task AuthorizeStreamAsync_ValidRequest_ReturnsSignedUrl()
{
    // Test implementation
}
```

### Integration Tests

- End-to-end upload workflow
- Stream authorization flow
- Error handling scenarios
- Performance testing

### Manual Testing

1. **File Upload**: Test with various audio formats and sizes
2. **Stream Authorization**: Verify signed URL generation
3. **Security**: Confirm direct URLs are blocked
4. **Error Handling**: Test validation and error responses

## Deployment Notes

### Prerequisites

1. **Cloudflare Account**: R2 bucket and Worker setup
2. **Database**: PostgreSQL connection string
3. **Configuration**: App settings with Cloudflare credentials

### Environment Variables

```bash
# Database
PostgreSQLDbConnStr="your-connection-string"

# JWT Authentication
JwtIssuer="your-issuer"
JwtAudience="your-audience"
JwtSecretKey="your-secret-key"

# Cloudflare R2
Cloudflare__R2__AccountId="your-account-id"
Cloudflare__R2__AccessKeyId="your-access-key"
Cloudflare__R2__SecretKey="your-secret-key"
Cloudflare__R2__BucketName="your-bucket-name"

# Cloudflare Worker
Cloudflare__Worker__Url="https://your-worker.your-account.workers.dev/signed-url"
```

### Cloudflare Setup

1. **Create R2 Bucket**: Set up bucket for audio storage
2. **Configure Worker**: Deploy worker for signed URL generation
3. **Set CORS**: Configure CORS policies for frontend access
4. **Set Permissions**: Configure IAM permissions for R2 access

## API Documentation

### Swagger Integration

The API includes comprehensive Swagger documentation accessible at:

- **Development**: `https://localhost:5001/swagger`
- **Production**: `https://your-domain.com/swagger`

### API Endpoints

- `GET /api/tracks` - List all tracks
- `GET /api/tracks/{id}` - Get track by ID
- `POST /api/tracks` - Create track (metadata only)
- `PUT /api/tracks/{id}` - Update track
- `DELETE /api/tracks/{id}` - Delete track
- `POST /api/tracks/upload` - Upload track with audio file
- `POST /api/tracks/authorize-stream` - Get signed URL for streaming

## Error Handling

### Common Errors

- **400 Bad Request**: Invalid request data or validation errors
- **404 Not Found**: Track or venue not found
- **500 Internal Server Error**: Server-side errors
- **403 Forbidden**: Authentication/authorization failures

### Error Response Format

```json
{
  "message": "Error description",
  "details": "Additional error details (optional)"
}
```

## Next Steps

### Milestone 3 Preparation

- Frontend React application for track management
- Media Session API integration for hardware controls
- Persistent player state management
- Background playback functionality

### Production Considerations

- **Monitoring**: Application insights and logging
- **Security**: HTTPS enforcement and security headers
- **Performance**: Load testing and optimization
- **Backup**: Database and file backup strategies

## Support

For issues or questions related to this implementation:

1. Check the API documentation in Swagger
2. Review the error logs for detailed information
3. Verify Cloudflare configuration and credentials
4. Test with sample requests using tools like Postman

## License

This implementation is part of the B2B Music Streaming Platform project.
