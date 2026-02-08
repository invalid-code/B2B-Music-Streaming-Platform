import { useEffect } from 'react'
import { useParams, useNavigate } from 'react-router-dom'
import { ArrowLeft, Music } from 'lucide-react'
import usePlaylistStore from '../store/usePlaylistStore'
import useTrackStore from '../store/useTrackStore'
import Button from '../components/common/Button'
import EmptyState from '../components/common/EmptyState'
import { SkeletonText } from '../components/common/LoadingSkeleton'
import './PlaylistsPage.css'

export default function PlaylistDetailPage() {
  const { id } = useParams()
  const navigate = useNavigate()
  const { selectedPlaylist, fetchPlaylistById, loading, clearSelectedPlaylist } =
    usePlaylistStore()
  const { tracks, fetchTracks } = useTrackStore()

  useEffect(() => {
    fetchPlaylistById(id).catch(() => navigate('/playlists'))
    return () => clearSelectedPlaylist()
  }, [id, fetchPlaylistById, navigate, clearSelectedPlaylist])

  useEffect(() => {
    if (tracks.length === 0) {
      fetchTracks().catch(() => {})
    }
  }, [tracks.length, fetchTracks])

  if (loading || !selectedPlaylist) {
    return (
      <div style={{ padding: '2rem' }}>
        <SkeletonText lines={5} />
      </div>
    )
  }

  const playlistTracks = tracks.filter((t) =>
    selectedPlaylist.trackIDs?.includes(t.trackID)
  )

  return (
    <div className="playlist-detail">
      <button
        className="playlist-detail__back-btn"
        onClick={() => navigate('/playlists')}
      >
        <ArrowLeft size={18} />
        <span>Back to Playlists</span>
      </button>

      <div className="playlist-detail__header">
        <div className="playlist-detail__info">
          <h1>{selectedPlaylist.name}</h1>
          {selectedPlaylist.vibeOrGenre && (
            <span className="playlist-detail__genre">
              {selectedPlaylist.vibeOrGenre}
            </span>
          )}
          <p className="playlist-detail__track-count">
            {playlistTracks.length} track{playlistTracks.length !== 1 ? 's' : ''}
          </p>
        </div>
        <Button variant="secondary" onClick={() => navigate('/playlists')}>
          Edit Playlist
        </Button>
      </div>

      {playlistTracks.length === 0 ? (
        <EmptyState
          icon={<Music size={48} />}
          title="No tracks assigned"
          message="Edit this playlist to add tracks."
        />
      ) : (
        <div className="track-list">
          {playlistTracks.map((track) => (
            <div key={track.trackID} className="track-card">
              <div className="track-card__icon">
                <Music size={20} />
              </div>
              <div className="track-card__info">
                <div className="track-card__title">{track.title}</div>
                <div className="track-card__artist">{track.artist}</div>
              </div>
              {track.mood && (
                <span className="track-card__mood">{track.mood}</span>
              )}
            </div>
          ))}
        </div>
      )}
    </div>
  )
}
