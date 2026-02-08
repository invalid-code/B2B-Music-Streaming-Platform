using API.Data;
using API.Models.Core_Models;
using Microsoft.EntityFrameworkCore;

namespace API.Repository.Implementations
{
    public class TrackRepository : ITrackRepository
    {
        private readonly MusicStreamingDbContext _dbContext;

        public TrackRepository(MusicStreamingDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Track> GetByIdAsync(string id)
        {
            return await _dbContext.Tracks.FirstOrDefaultAsync(t => t.TrackID == id);
        }

        public async Task<List<Track>> GetAllAsync()
        {
            return await _dbContext.Tracks.ToListAsync();
        }

        public async Task<Track> AddAsync(Track entity)
        {
            await _dbContext.Tracks.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> UpdateAsync(Track entity)
        {
            var existingTrack = await _dbContext.Tracks.FirstOrDefaultAsync(t => t.TrackID == entity.TrackID);
            if (existingTrack == null)
                return false;

            existingTrack.Title = entity.Title;
            existingTrack.Artist = entity.Artist;
            existingTrack.Mood = entity.Mood;
            existingTrack.CloudflareStorageKey = entity.CloudflareStorageKey;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var track = await _dbContext.Tracks.FirstOrDefaultAsync(t => t.TrackID == id);
            if (track == null)
                return false;

            _dbContext.Tracks.Remove(track);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(string id)
        {
            return await _dbContext.Tracks.AnyAsync(t => t.TrackID == id);
        }

        public async Task<List<Track>> GetTracksByMoodAsync(string mood)
        {
            return await _dbContext.Tracks
                .Where(t => t.Mood.Equals(mood, StringComparison.OrdinalIgnoreCase))
                .ToListAsync();
        }

        public async Task<List<Track>> GetTracksByArtistAsync(string artist)
        {
            return await _dbContext.Tracks
                .Where(t => t.Artist.Equals(artist, StringComparison.OrdinalIgnoreCase))
                .ToListAsync();
        }
    }
}