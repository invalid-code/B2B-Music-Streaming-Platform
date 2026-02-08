import { useNavigate } from 'react-router-dom'
import { ListMusic, Pencil, Trash2 } from 'lucide-react'

/**
 * Renders a grid of playlist cards.
 */
export default function PlaylistList({ playlists, onEdit, onDelete }) {
  const navigate = useNavigate()

  return (
    <div className="playlist-grid">
      {playlists.map((pl) => (
        <div key={pl.playlistID} className="playlist-card">
          <div className="playlist-card__header">
            <div className="playlist-card__icon">
              <ListMusic size={22} />
            </div>
            <div className="playlist-card__actions">
              <button
                className="playlist-card__action-btn"
                onClick={() => onEdit(pl)}
                aria-label={`Edit ${pl.name}`}
              >
                <Pencil size={16} />
              </button>
              <button
                className="playlist-card__action-btn playlist-card__action-btn--danger"
                onClick={() => onDelete(pl)}
                aria-label={`Delete ${pl.name}`}
              >
                <Trash2 size={16} />
              </button>
            </div>
          </div>

          <span
            className="playlist-card__name"
            onClick={() => navigate(`/playlists/${pl.playlistID}`)}
            role="link"
            tabIndex={0}
            onKeyDown={(e) =>
              e.key === 'Enter' && navigate(`/playlists/${pl.playlistID}`)
            }
          >
            {pl.name}
          </span>

          <div className="playlist-card__meta">
            {pl.vibeOrGenre && (
              <span className="playlist-card__genre">{pl.vibeOrGenre}</span>
            )}
            <span>{pl.trackIDs?.length || 0} tracks</span>
          </div>
        </div>
      ))}
    </div>
  )
}
