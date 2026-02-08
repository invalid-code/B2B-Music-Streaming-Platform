import { MapPin, Pencil, Trash2 } from 'lucide-react'

/**
 * Renders a list of venue cards with edit/delete actions.
 */
export default function VenueList({ venues, onEdit, onDelete }) {
  return (
    <div className="venue-list">
      {venues.map((venue) => {
        const isPaid = venue.subscriptionStatus === 'Paid'
        return (
          <div key={venue.venueID} className="venue-card">
            <div className="venue-card__icon">
              <MapPin size={22} />
            </div>

            <div className="venue-card__info">
              <div className="venue-card__name">{venue.businessName}</div>
              <div className="venue-card__location">
                <MapPin size={14} />
                {venue.location}
              </div>
            </div>

            <span
              className={`venue-card__badge ${isPaid ? 'venue-card__badge--paid' : 'venue-card__badge--trial'}`}
            >
              {venue.subscriptionStatus || 'Trial'}
            </span>

            <div className="venue-card__actions">
              <button
                className="venue-card__action-btn"
                onClick={() => onEdit(venue)}
                aria-label={`Edit ${venue.businessName}`}
              >
                <Pencil size={16} />
              </button>
              <button
                className="venue-card__action-btn venue-card__action-btn--danger"
                onClick={() => onDelete(venue)}
                aria-label={`Delete ${venue.businessName}`}
              >
                <Trash2 size={16} />
              </button>
            </div>
          </div>
        )
      })}
    </div>
  )
}
