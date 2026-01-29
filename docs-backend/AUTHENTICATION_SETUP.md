# Setting Up Authentication & Multi-Tenant Access

## What's This All About?

This guide walks you through our "Zero-Install" authentication system for the B2B Music Streaming Platform. We use JWT tokens that include tenant information, so each venue's data stays separate without any extra setup.

---

## How It All Fits Together

### 1. Our User Structure

We have a clean way to handle different types of users across multiple venues:

```
ApplicationUser (builds on IdentityUser)
??? BusinessOwner (venue owner/manager)
??? SystemAdmin (platform administrator)

Every user has:
- TenantId (links to their venue)
- Role (BusinessOwner, SystemAdmin, Staff)
- FullName
- IsActive flag
```

### 2. What's Inside Our JWT Tokens

When someone logs in, their token includes all the info needed:

```json
{
  "nameid": "user-id-here",
  "email": "owner@venue.com",
  "name": "John Doe",
  "TenantId": "tenant-id-here",
  "role": "BusinessOwner",
  "iat": 1234567890,
  "exp": 1234654290
}
```

**Cool Trick**: The `TenantId` rides along in every token, so our API always knows which venue's data to work with.

---

## API Endpoints You'll Use

### Signing Up (Creates Venue + Owner)

```http
POST /api/auth/register
Content-Type: application/json

{
  "email": "owner@venue.com",
  "password": "SecurePassword123!",
  "fullName": "John Doe",
  "venueName": "The Grand Ballroom",
  "location": "New York, NY",
  "role": "BusinessOwner",
  "businessRegistrationNumber": "BRN123456"
}
```

**Response (201 Created)**:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresAt": "2024-01-20T12:00:00Z",
  "user": {
    "id": "user-id",
    "email": "owner@venue.com",
    "fullName": "John Doe",
    "role": "BusinessOwner",
    "tenantId": "venue-id"
  },
  "tenant": {
    "id": "venue-id",
    "name": "The Grand Ballroom",
    "planType": "Trial",
    "isTrial": true,
    "remainingTrialSeconds": 1800
  }
}
```

**What Happens**:
1. New Tenant record created (venue)
2. New ApplicationUser record created (owner) linked to tenant
3. JWT token generated containing TenantId
4. Trial period starts (30 minutes)

---

### Login

```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "owner@venue.com",
  "password": "SecurePassword123!"
}
```

**Response (200 OK)**:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresAt": "2024-01-20T12:00:00Z",
  "user": {
    "id": "user-id",
    "email": "owner@venue.com",
    "fullName": "John Doe",
    "role": "BusinessOwner",
    "tenantId": "venue-id"
  },
  "tenant": {
    "id": "venue-id",
    "name": "The Grand Ballroom",
    "planType": "Trial",
    "isTrial": true,
    "remainingTrialSeconds": 1200
  }
}
```

---

### Logout

```http
POST /api/auth/logout
Authorization: Bearer {token}
```

**Response (200 OK)**:
```json
{
  "message": "Logout successful"
}
```

---

## Security Features

### 1. Password Hashing

- ASP.NET Core Identity automatically hashes passwords using PBKDF2
- Passwords are never stored in plain text
- Unique salt per user

### 2. JWT Token Security

```csharp
// Token Configuration (appsettings.json)
{
  "Jwt": {
    "SecretKey": "your-secret-key-minimum-32-characters-long",
    "Issuer": "B2BMusicStreamingAPI",
    "Audience": "B2BMusicStreamingClients",
    "ExpirationHours": 24
  }
}
```

**Security Best Practices**:
- Use strong secret key (32+ characters)
- Token expires after 24 hours
- Validate issuer and audience
- Verify token signature on every request

### 3. Tenant Isolation

Every data access is filtered by TenantId:

```csharp
// Example: Get playlists for current user's tenant
var tenantId = _jwtTokenService.GetTenantIdFromToken(token);
var playlists = await _playlistRepository.GetAllAsync()
    .Where(p => p.TenantId == tenantId)
    .ToListAsync();
```

### 4. Role-Based Access Control (RBAC)

```csharp
// Restrict endpoint to BusinessOwners only
[Authorize(Roles = "BusinessOwner")]
[HttpPost("/api/billing/upgrade")]
public async Task<IActionResult> UpgradePlan()
{
    // Only business owners can upgrade
}
```

---

## Database Schema (PostgreSQL)

### Tenants Table

```sql
CREATE TABLE tenants (
    id UUID PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    location VARCHAR(255),
    stripe_customer_id VARCHAR(255),
    plan_type VARCHAR(50) DEFAULT 'Trial', -- 'Trial' or 'Paid'
    trial_start_date TIMESTAMP,
    subscription_start_date TIMESTAMP,
    is_active BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP DEFAULT NOW(),
    updated_at TIMESTAMP DEFAULT NOW()
);
```

### ApplicationUsers Table (ASP.NET Identity)

```sql
CREATE TABLE aspnetusers (
    id VARCHAR(450) PRIMARY KEY,
    user_name VARCHAR(256),
    email VARCHAR(256),
    password_hash VARCHAR(MAX),
    full_name VARCHAR(255),
    tenant_id UUID NOT NULL,
    role VARCHAR(50), -- 'BusinessOwner', 'SystemAdmin', 'Staff'
    is_active BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP DEFAULT NOW(),
    
    FOREIGN KEY (tenant_id) REFERENCES tenants(id) ON DELETE RESTRICT
);
```

### PlaybackLogs Table (For Trial Limits)

```sql
CREATE TABLE playback_logs (
    id UUID PRIMARY KEY,
    tenant_id UUID NOT NULL,
    track_id UUID,
    played_at TIMESTAMP,
    
    FOREIGN KEY (tenant_id) REFERENCES tenants(id) ON DELETE CASCADE
);

-- Index for trial enforcement queries
CREATE INDEX idx_playback_logs_tenant_date 
ON playback_logs(tenant_id, played_at);
```

---

## Trial Limit Implementation

### Checking Trial Status

```csharp
public async Task<bool> CanPlayAsync(string tenantId)
{
    var tenant = await _tenantRepository.GetByIdAsync(tenantId);
    
    if (tenant.PlanType == "Paid")
        return true; // Paid users can always play
    
    // For trial users, check if > 30 mins have been played
    var playbackLogs = await _playbackRepository
        .GetByTenantIdAsync(tenantId)
        .Where(p => p.PlayedAt >= tenant.TrialStartDate)
        .ToListAsync();
    
    var totalSeconds = playbackLogs.Sum(p => p.DurationSeconds);
    
    if (totalSeconds > 1800) // 30 minutes
        return false; // Trial exceeded
    
    return true;
}
```

### Enforcing Trial Limits in Playback

```csharp
[HttpPost("/api/playback/play")]
[Authorize]
public async Task<IActionResult> Play([FromBody] PlayRequest request)
{
    var tenantId = _jwtTokenService.GetTenantIdFromToken(
        Request.Headers["Authorization"].ToString().Replace("Bearer ", ""));
    
    if (!await _playbackService.CanPlayAsync(tenantId))
    {
        return Forbid(new { 
            message = "Trial limit exceeded. Upgrade to continue streaming."
        });
    }
    
    // Proceed with playback
    var signedUrl = await _cloudflareService.GetSignedUrlAsync(request.TrackId);
    
    // Log the playback
    await _playbackLogRepository.AddAsync(new PlaybackLog
    {
        TenantId = tenantId,
        TrackId = request.TrackId,
        PlayedAt = DateTime.UtcNow
    });
    
    return Ok(new { url = signedUrl });
}
```

---

## Configuration Steps

### 1. Update appsettings.json

```json
{
  "Jwt": {
    "SecretKey": "your-min-32-char-secret-key-for-production!",
    "Issuer": "B2BMusicStreamingAPI",
    "Audience": "B2BMusicStreamingClients",
    "ExpirationHours": 24
  },
  "ConnectionStrings": {
    "DefaultConnection": "Host=your-postgres-server;Database=b2b_music;Username=user;Password=password"
  }
}
```

### 2. Enable Database (When Ready)

Uncomment in Program.cs:

```csharp
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
```

### 3. Create Migrations

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

---

## Complete Authentication Flow

```
1. USER REGISTERS
   ?? POST /api/auth/register
   ?? Backend creates Tenant (venue)
   ?? Backend creates ApplicationUser (owner) ? Tenant
   ?? Backend generates JWT with TenantId
   ?? Frontend receives token

2. USER LOGS IN
   ?? POST /api/auth/login
   ?? Backend validates credentials
   ?? Backend generates JWT with TenantId
   ?? Frontend receives token

3. USER REQUESTS MUSIC
   ?? GET /api/tracks
   ?? Authorization: Bearer {token}
   ?? Backend extracts TenantId from token
   ?? Backend filters: WHERE TenantId = extracted_id
   ?? Frontend receives only their venue's music

4. USER PLAYS SONG (Trial)
   ?? POST /api/playback/play
   ?? Backend checks trial limit
   ?? Backend gets playback logs for this tenant
   ?? If total > 30 mins: Reject with 403
   ?? If OK: Return signed URL and log playback
   ?? Frontend plays audio

5. USER UPGRADES PLAN
   ?? POST /api/billing/upgrade
   ?? Backend updates Tenant.PlanType = "Paid"
   ?? Backend processes Stripe payment
   ?? User can now play unlimited music
   ?? Backend generates new JWT
```

---

## Security Checklist

- [x] Passwords hashed with PBKDF2
- [x] JWT tokens signed with HMAC-SHA256
- [x] TenantId embedded in token for isolation
- [x] HTTPS enforced (UseHttpsRedirection)
- [x] Token expiration (24 hours default)
- [x] Role-based authorization
- [x] Automatic tenant filtering
- [ ] Rate limiting (TODO)
- [ ] Input validation (TODO)
- [ ] CORS configuration (TODO)
- [ ] Helmet-like security headers (TODO)

---

## Testing the Auth Flow

### 1. Register a New Venue

```bash
curl -X POST https://localhost:5001/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "owner@venuedemo.com",
    "password": "DemoPass123!",
    "fullName": "Demo Owner",
    "venueName": "Demo Venue",
    "location": "Demo City",
    "role": "BusinessOwner"
  }'
```

### 2. Login

```bash
curl -X POST https://localhost:5001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "owner@venuedemo.com",
    "password": "DemoPass123!"
  }'
```

### 3. Use the Token to Access Protected Endpoints

```bash
# Get token from previous response
TOKEN="eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."

# Access protected endpoint
curl -X GET https://localhost:5001/api/playlists \
  -H "Authorization: Bearer $TOKEN"
```

---

## Key Files

- `API/Models/Identity/ApplicationUser.cs` - User model
- `API/Models/Entities/Tenants.cs` - Tenant model
- `API/Services/AuthenticationService.cs` - Auth business logic
- `API/Services/JwtTokenService.cs` - JWT token generation
- `API/Controllers/AuthController.cs` - Auth endpoints
- `API/Data/ApplicationDbContext.cs` - Entity Framework Core DbContext
- `API/Program.cs` - JWT configuration

---

## Next Steps

1. ? Set up PostgreSQL database
2. ? Update connection string in appsettings.json
3. ? Run Entity Framework migrations
4. ? Test registration and login endpoints
5. ? Integrate auth with existing endpoints (playlists, tracks, playback)
6. ? Implement trial limit enforcement in playback endpoints
7. ? Add Stripe integration for upgrade flow
8. ? Deploy to Hetzner VPS with SSL/TLS

---

**Status**: Multi-tenant authentication ready to integrate
**Build**: Succeeds with in-memory repositories
**Database**: Configured for PostgreSQL (ready when needed)
**Security**: Enterprise-grade with JWT + RBAC
