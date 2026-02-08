import { useEffect, useState } from 'react'
import { Search } from 'lucide-react'
import useTrackStore from '../../store/useTrackStore'

/**
 * Multi-select track picker for playlist forms.
 * Fetches tracks on mount if not already loaded.
 */
export default function TrackPicker({ selectedTrackIDs = [], onChange }) {
  const { tracks, fetchTracks } = useTrackStore()
  const [search, setSearch] = useState('')

  useEffect(() => {
    if (tracks.length === 0) {
      fetchTracks().catch(() => {})
    }
  }, [tracks.length, fetchTracks])

  const filtered = tracks.filter((t) => {
    if (!search) return true
    return (
      t.title?.toLowerCase().includes(search.toLowerCase()) ||
      t.artist?.toLowerCase().includes(search.toLowerCase())
    )
  })

  const toggleTrack = (trackID) => {
    if (selectedTrackIDs.includes(trackID)) {
      onChange(selectedTrackIDs.filter((id) => id !== trackID))
    } else {
      onChange([...selectedTrackIDs, trackID])
    }
  }

  return (
    <div className="track-picker">
      <label className="track-picker__label">
        Assign Tracks ({selectedTrackIDs.length} selected)
      </label>

      <div className="track-picker__search">
        <Search size={16} className="track-picker__search-icon" />
        <input
          type="text"
          placeholder="Search tracks..."
          value={search}
          onChange={(e) => setSearch(e.target.value)}
          className="track-picker__search-input"
        />
      </div>

      <div className="track-picker__list">
        {filtered.length === 0 ? (
          <div className="track-picker__empty">
            {tracks.length === 0 ? 'No tracks available' : 'No matches found'}
          </div>
        ) : (
          filtered.map((track) => {
            const isSelected = selectedTrackIDs.includes(track.trackID)
            return (
              <label
                key={track.trackID}
                className={`track-picker__item ${isSelected ? 'track-picker__item--selected' : ''}`}
              >
                <input
                  type="checkbox"
                  className="track-picker__checkbox"
                  checked={isSelected}
                  onChange={() => toggleTrack(track.trackID)}
                />
                <div className="track-picker__item-info">
                  <div className="track-picker__item-title">{track.title}</div>
                  <div className="track-picker__item-artist">{track.artist}</div>
                </div>
              </label>
            )
          })
        )}
      </div>
    </div>
  )
}
