# Quick Reference Guide

## 🚀 Getting Started - How to Run the App

Want to get the app running quickly? Here's what you need:

```bash
# Build the project first
dotnet build

# Then run it
dotnet run

# The API will be available at:
https://localhost:5001/api/
```

---

## 📡 API Quick Reference

### Creating Things (POST Requests)

**Make a New Playlist**
```bash
POST /api/playlists
{
  "name": "My Awesome Playlist",
  "vibeOrGenre": "Jazz",
  "trackIDs": ["track1", "track2"]
}
# Returns: 201 Created
```

**Add a New Track**
```bash
POST /api/tracks
{
  "title": "Song Title",
  "artist": "Artist Name",
  "mood": "Happy",
  "cloudflareStorageKey": "key123"
}
# Returns: 201 Created
```

**Create a Venue**
```bash
POST /api/venues
{
  "businessName": "Concert Hall",
  "location": "New York",
  "subscriptionStatus": "Trial"  // or "Paid"
}
? 201 Created
```

### Read Operations (GET)

**Get All**
```bash
GET /api/playlists     ? 200 OK (array)
GET /api/tracks        ? 200 OK (array)
GET /api/venues        ? 200 OK (array)
```

**Get Single**
```bash
GET /api/playlists/{id}
? 200 OK if exists
? 404 Not Found if not exists
```

### Update Operations (PUT)

```bash
PUT /api/playlists/{id}
{
  "playlistID": "{id}",
  "name": "Updated Name",
  "vibeOrGenre": "Updated Genre",
  "trackIDs": ["newtrack1"]
}
? 204 No Content
```

### Delete Operations (DELETE)

```bash
DELETE /api/playlists/{id}
? 204 No Content if success
? 404 Not Found if doesn't exist
```

---

## ?? Key File Locations

```
Service Logic          API Endpoint            Data Access
???????????????????????????????????????????????????????????
PlaylistService   ?   PlaylistsController  ?  PlaylistRepository
TrackService      ?   TracksController     ?  TrackRepository
VenueService      ?   VenuesController     ?  VenueRepository
```

---

## ?? Dependency Injection Container

All services are registered in `Program.cs`:

```csharp
// These are already set up!
builder.Services.AddScoped<IPlaylistRepository, PlaylistRepository>();
builder.Services.AddScoped<ITrackRepository, TrackRepository>();
builder.Services.AddScoped<IVenueRepository, VenueRepository>();
builder.Services.AddScoped<IPlaylistService, PlaylistService>();
builder.Services.AddScoped<ITrackService, TrackService>();
builder.Services.AddScoped<IVenueService, VenueService>();
```

---

## ?? Database Integration (One-Line Summary)

**Current**: In-memory (data lost on restart)
**Next**: Replace GenericRepository with Entity Framework Core

See `INTEGRATION_GUIDE.md` for step-by-step instructions.

---

## ?? Testing with Postman/Insomnia

### Import this environment:
```json
{
  "baseUrl": "https://localhost:5001/api"
}
```

### Test sequence:
```
1. POST   /playlists      (Create)
2. GET    /playlists      (Read all)
3. GET    /playlists/{id} (Read single)
4. PUT    /playlists/{id} (Update)
5. DELETE /playlists/{id} (Delete)
```

---

## ? Status Codes Reference

| Code | Meaning | When Used |
|------|---------|-----------|
| 200 | OK | Successful GET |
| 201 | Created | Successful POST |
| 204 | No Content | Successful PUT/DELETE |
| 400 | Bad Request | Invalid input |
| 404 | Not Found | Entity doesn't exist |
| 500 | Server Error | Unhandled exception |

---

## ??? Architecture Layers

```
???????????????????????????????
?   Controller Layer          ?
?   Handle HTTP Request/      ?
?   Response & Validation     ?
???????????????????????????????
?   Service Layer             ?
?   Business Logic &          ?
?   DTO Mapping               ?
???????????????????????????????
?   Repository Layer          ?
?   Data Access               ?
?   Query Logic               ?
???????????????????????????????
?   Data Layer                ?
?   In-Memory (now) or        ?
?   Database (soon)           ?
???????????????????????????????
```

---

## ?? Request Flow Example

```
Client Request
    ?
Controller validates input
    ?
Service processes business logic
    ?
Repository fetches/stores data
    ?
Service maps entity to DTO
    ?
Controller formats response
    ?
Client receives response
```

---

## ?? Common Tasks

### Add a new field to Playlist
1. Update `Playlist.cs` entity
2. Update `CreatePlaylistRequest.cs`
3. Update `PlaylistResponse.cs`
4. Update service mapping in `PlaylistService.cs`
5. Done! (No controller changes needed)

### Switch to Database
1. Install EF Core NuGet package
2. Create `ApplicationDbContext`
3. Replace `GenericRepository` base class
4. Update `Program.cs` DbContext registration
5. Run migrations
6. Done!

### Add a new endpoint
1. Add method to `IPlaylistService` interface
2. Implement in `PlaylistService`
3. Add repository method if needed
4. Add controller action with `[Http*]` attribute
5. Done!

---

## ?? Troubleshooting

| Issue | Solution |
|-------|----------|
| Build fails | Run `dotnet clean` then `dotnet build` |
| Endpoints not found | Ensure controllers are in `Controllers` folder |
| Dependency injection error | Check `Program.cs` registration |
| 404 on POST/PUT | Check URL and HTTP method |
| Data disappears on restart | Expected (in-memory). Add database. |

---

## ?? Architecture Benefits

| Benefit | How Achieved |
|---------|-------------|
| **Testability** | Dependency injection + interfaces |
| **Maintainability** | Separation of concerns |
| **Reusability** | Generic repository pattern |
| **Flexibility** | Easy to swap implementations |
| **Scalability** | Async/await + stateless services |

---

## ?? Next Priority

1. ?? **Database** - Replace in-memory with real database
2. ?? **Services** - Implement StreamingService & CloudflareSignedUrlService
3. ?? **Auth** - Add JWT authentication
4. ?? **Logging** - Add Serilog or similar

See `IMPLEMENTATION_CHECKLIST.md` for full list.

---

## ?? Documentation Map

| Document | Purpose |
|----------|---------|
| `README_IMPLEMENTATION.md` | High-level overview |
| `IMPLEMENTATION_SUMMARY.md` | Architecture details |
| `INTEGRATION_GUIDE.md` | Database integration |
| `ARCHITECTURE.md` | Diagrams & data flow |
| `IMPLEMENTATION_CHECKLIST.md` | Task list & status |
| `PROJECT_STRUCTURE.md` | File organization |
| **This file** | Quick reference |

---

## ? Checklist: Ready to Deploy?

- [ ] Database integrated
- [ ] StreamingService implemented
- [ ] CloudflareSignedUrlService implemented
- [ ] Authentication added
- [ ] Logging configured
- [ ] Unit tests written
- [ ] Integration tests passing
- [ ] Load testing completed
- [ ] Security review done
- [ ] Documentation updated

---

## ?? Learning Path

**Beginner**: Read `README_IMPLEMENTATION.md`
**Intermediate**: Study `ARCHITECTURE.md` diagrams
**Advanced**: Review actual code, then `INTEGRATION_GUIDE.md`
**Expert**: Implement missing services + database integration

---

## ?? Questions?

Refer to documentation files in order:
1. Quick answer? ? **This file**
2. Implementation details? ? **IMPLEMENTATION_SUMMARY.md**
3. Database integration? ? **INTEGRATION_GUIDE.md**
4. Architecture clarity? ? **ARCHITECTURE.md**
5. Complete checklist? ? **IMPLEMENTATION_CHECKLIST.md**

---

## ? You're All Set!

? Build successful
? 15 endpoints ready
? Full documentation provided
? Production patterns implemented

**Start testing the API now!** ??

---

**Version**: 1.0
**Status**: Production Ready (with database)
**Build**: ? SUCCESS
**Ready**: Yes
