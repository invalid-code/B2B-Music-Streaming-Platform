import { Music, Pencil, Trash2 } from 'lucide-react'

/**
 * Renders a list of track cards with edit/delete actions.
 */
export default function TrackList({ tracks, onEdit, onDelete }) {
  return (
    <div className="track-list">
      {tracks.map((track) => (
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

          <div className="track-card__actions">
            <button
              className="track-card__action-btn"
              onClick={() => onEdit(track)}
              aria-label={`Edit ${track.title}`}
            >
              <Pencil size={16} />
            </button>
            <button
              className="track-card__action-btn track-card__action-btn--danger"
              onClick={() => onDelete(track)}
              aria-label={`Delete ${track.title}`}
            >
              <Trash2 size={16} />
            </button>
          </div>
        </div>
      ))}
    </div>
  )
}
