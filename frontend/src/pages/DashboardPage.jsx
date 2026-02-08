import { useEffect } from 'react'
import { useNavigate } from 'react-router-dom'
import { Music, ListMusic, MapPin, TrendingUp } from 'lucide-react'
import useTrackStore from '../store/useTrackStore'
import usePlaylistStore from '../store/usePlaylistStore'
import useVenueStore from '../store/useVenueStore'
import useAuthStore from '../store/useAuthStore'
import { SkeletonCard } from '../components/common/LoadingSkeleton'
import './DashboardPage.css'

export default function DashboardPage() {
  const navigate = useNavigate()
  const user = useAuthStore((s) => s.user)
  const { tracks, fetchTracks, loading: tracksLoading } = useTrackStore()
  const { playlists, fetchPlaylists, loading: playlistsLoading } = usePlaylistStore()
  const { venues, fetchVenues, loading: venuesLoading } = useVenueStore()

  useEffect(() => {
    fetchTracks().catch(() => {})
    fetchPlaylists().catch(() => {})
    fetchVenues().catch(() => {})
  }, [fetchTracks, fetchPlaylists, fetchVenues])

  const isLoading = tracksLoading || playlistsLoading || venuesLoading

  const stats = [
    {
      label: 'Total Tracks',
      value: tracks.length,
      icon: Music,
      color: '#1db954',
      route: '/tracks',
    },
    {
      label: 'Playlists',
      value: playlists.length,
      icon: ListMusic,
      color: '#4a9eff',
      route: '/playlists',
    },
    {
      label: 'Venues',
      value: venues.length,
      icon: MapPin,
      color: '#ffa500',
      route: '/venues',
    },
    {
      label: 'Active',
      value: venues.filter((v) => v.subscriptionStatus === 'Paid').length,
      icon: TrendingUp,
      color: '#a855f7',
      route: '/venues',
    },
  ]

  return (
    <div className="dashboard-page">
      <div className="dashboard-page__header">
        <h1 className="dashboard-page__title">Dashboard</h1>
        <p className="dashboard-page__subtitle">
          Welcome back, {user?.fullName || 'there'}! Here's your overview.
        </p>
      </div>

      {/* Stats Grid */}
      <div className="stats-grid">
        {isLoading
          ? Array.from({ length: 4 }).map((_, i) => (
              <SkeletonCard key={i} />
            ))
          : stats.map((stat) => (
              <button
                key={stat.label}
                className="stat-card"
                onClick={() => navigate(stat.route)}
              >
                <div
                  className="stat-card__icon"
                  style={{ background: `${stat.color}20`, color: stat.color }}
                >
                  <stat.icon size={24} />
                </div>
                <div className="stat-card__info">
                  <span className="stat-card__value">{stat.value}</span>
                  <span className="stat-card__label">{stat.label}</span>
                </div>
              </button>
            ))}
      </div>

      {/* Recent Tracks */}
      <section className="dashboard-section">
        <div className="dashboard-section__header">
          <h2>Recent Tracks</h2>
          <button
            className="dashboard-section__link"
            onClick={() => navigate('/tracks')}
          >
            View all
          </button>
        </div>
        {tracksLoading ? (
          <div className="dashboard-section__skeleton">
            {Array.from({ length: 3 }).map((_, i) => (
              <SkeletonCard key={i} />
            ))}
          </div>
        ) : tracks.length === 0 ? (
          <p className="dashboard-section__empty">No tracks yet. Add your first track!</p>
        ) : (
          <div className="recent-tracks-list">
            {tracks.slice(0, 5).map((track) => (
              <div key={track.trackID} className="recent-track-item">
                <div className="recent-track-item__icon">
                  <Music size={18} />
                </div>
                <div className="recent-track-item__info">
                  <span className="recent-track-item__title">{track.title}</span>
                  <span className="recent-track-item__artist">{track.artist}</span>
                </div>
                <span className="recent-track-item__mood">{track.mood}</span>
              </div>
            ))}
          </div>
        )}
      </section>

      {/* Recent Playlists */}
      <section className="dashboard-section">
        <div className="dashboard-section__header">
          <h2>Recent Playlists</h2>
          <button
            className="dashboard-section__link"
            onClick={() => navigate('/playlists')}
          >
            View all
          </button>
        </div>
        {playlistsLoading ? (
          <div className="dashboard-section__skeleton">
            {Array.from({ length: 3 }).map((_, i) => (
              <SkeletonCard key={i} />
            ))}
          </div>
        ) : playlists.length === 0 ? (
          <p className="dashboard-section__empty">No playlists yet. Create your first playlist!</p>
        ) : (
          <div className="recent-playlists-grid">
            {playlists.slice(0, 4).map((pl) => (
              <div
                key={pl.playlistID}
                className="recent-playlist-card"
                onClick={() => navigate(`/playlists/${pl.playlistID}`)}
                role="button"
                tabIndex={0}
                onKeyDown={(e) =>
                  e.key === 'Enter' && navigate(`/playlists/${pl.playlistID}`)
                }
              >
                <div className="recent-playlist-card__icon">
                  <ListMusic size={24} />
                </div>
                <span className="recent-playlist-card__name">{pl.name}</span>
                <span className="recent-playlist-card__genre">{pl.vibeOrGenre}</span>
                <span className="recent-playlist-card__count">
                  {pl.trackIDs?.length || 0} tracks
                </span>
              </div>
            ))}
          </div>
        )}
      </section>
    </div>
  )
}
