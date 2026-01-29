# Quick Integration Guide

## Testing the APIs

### Using cURL or Postman

#### 1. Create a Playlist
```bash
POST /api/playlists
Content-Type: application/json

{
  "name": "Relaxing Jazz",
  "vibeOrGenre": "Jazz",
  "trackIDs": ["track1", "track2"]
}
```

#### 2. Get All Playlists
```bash
GET /api/playlists
```

#### 3. Get Playlist by ID
```bash
GET /api/playlists/{id}
```

#### 4. Update Playlist
```bash
PUT /api/playlists/{id}
Content-Type: application/json

{
  "playlistID": "{id}",
  "name": "Updated Name",
  "vibeOrGenre": "Updated Genre",
  "trackIDs": ["track1", "track2", "track3"]
}
```

#### 5. Delete Playlist
```bash
DELETE /api/playlists/{id}
```

---

### 1. Create a Track
```bash
POST /api/tracks
Content-Type: application/json

{
  "title": "Song Title",
  "artist": "Artist Name",
  "mood": "Happy",
  "cloudflareStorageKey": "storage-key-123"
}
```

#### 2. Get All Tracks
```bash
GET /api/tracks
```

#### 3. Get Track by ID
```bash
GET /api/tracks/{id}
```

#### 4. Update Track
```bash
PUT /api/tracks/{id}
Content-Type: application/json

{
  "trackID": "{id}",
  "title": "Updated Title",
  "artist": "Updated Artist",
  "mood": "Sad",
  "cloudflareStorageKey": "new-storage-key"
}
```

#### 5. Delete Track
```bash
DELETE /api/tracks/{id}
```

---

## Implementing Database Persistence

### Step 1: Add Entity Framework Core NuGet Package
```bash
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
# or for another database provider
```

### Step 2: Create DbContext
```csharp
using Microsoft.EntityFrameworkCore;
using API.Models.Core_Models;
using API.Models.Entities;

namespace API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Playlist> Playlists { get; set; }
        public DbSet<Track> Tracks { get; set; }
        public DbSet<Venue> Venues { get; set; }
    }
}
```

### Step 3: Update Program.cs
```csharp
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);
```

### Step 4: Create Database Repository Implementation
```csharp
public class PlaylistRepository : IPlaylistRepository
{
    private readonly ApplicationDbContext _context;

    public PlaylistRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Playlist> GetByIdAsync(string id)
    {
        return await _context.Playlists.FindAsync(id);
    }

    public async Task<List<Playlist>> GetAllAsync()
    {
        return await _context.Playlists.ToListAsync();
    }

    public async Task<Playlist> AddAsync(Playlist entity)
    {
        _context.Playlists.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> UpdateAsync(Playlist entity)
    {
        _context.Playlists.Update(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var playlist = await _context.Playlists.FindAsync(id);
        if (playlist == null) return false;
        
        _context.Playlists.Remove(playlist);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(string id)
    {
        return await _context.Playlists.AnyAsync(p => p.PlaylistID == id);
    }

    // Implement domain-specific methods...
}
```

## Project Structure Overview

```
API/
??? Controllers/
?   ??? PlaylistsController.cs ? NEW
?   ??? TracksController.cs ? NEW
?   ??? VenuesController.cs (existing)
??? Interface/
?   ??? IPlaylistService.cs (updated)
?   ??? ITrackService.cs
?   ??? IVenueService.cs
??? Services/ ? NEW
?   ??? PlaylistService.cs
?   ??? TrackService.cs
?   ??? VenueService.cs
??? Repository/ ? NEW
?   ??? IGenericRepository.cs
?   ??? IPlaylistRepository.cs
?   ??? ITrackRepository.cs
?   ??? IVenueRepository.cs
?   ??? Implementations/
?       ??? GenericRepository.cs
?       ??? PlaylistRepository.cs
?       ??? TrackRepository.cs
?       ??? VenueRepository.cs
??? Models/
?   ??? Entities/
?   ?   ??? Playlist.cs
?   ?   ??? Track.cs
?   ?   ??? Venue.cs
?   ??? DTOs/
?       ??? Requests/
?       ??? Response/
??? Program.cs (updated)
```

## Error Handling

All controllers include:
- Model validation checks
- Null checks for entity retrieval
- Appropriate HTTP status codes:
  - 200 OK - Successful GET
  - 201 Created - Successful POST
  - 204 No Content - Successful PUT/DELETE
  - 400 Bad Request - Invalid input
  - 404 Not Found - Entity doesn't exist
  - 500 Internal Server Error - Server error

## Next: Implement Remaining Services

1. **StreamingService** - Handle audio streaming logic
2. **CloudflareSignedUrlService** - Generate signed URLs for Cloudflare R2 storage

These services are referenced in Program.cs but need implementation.
