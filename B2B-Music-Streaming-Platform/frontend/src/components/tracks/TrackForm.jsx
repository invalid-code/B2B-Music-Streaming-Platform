import { useState } from 'react'
import Input from '../common/Input'
import Button from '../common/Button'
import { validateRequired } from '../../utils/validation'

/**
 * Create / Edit form for a track.
 * If `initialData` is provided, the form pre-fills for editing.
 */
export default function TrackForm({ initialData = null, onSubmit, loading = false }) {
  const isEdit = !!initialData

  const [title, setTitle] = useState(initialData?.title || '')
  const [artist, setArtist] = useState(initialData?.artist || '')
  const [mood, setMood] = useState(initialData?.mood || '')
  const [cloudflareStorageKey, setCloudflareStorageKey] = useState(
    initialData?.cloudflareStorageKey || ''
  )
  const [errors, setErrors] = useState({})

  const validate = () => {
    const e = {}
    e.title = validateRequired(title, 'Title')
    e.artist = validateRequired(artist, 'Artist')
    e.mood = validateRequired(mood, 'Mood')
    setErrors(e)
    return !Object.values(e).some(Boolean)
  }

  const handleSubmit = (e) => {
    e.preventDefault()
    if (!validate()) return
    onSubmit({ title, artist, mood, cloudflareStorageKey })
  }

  return (
    <form className="track-form" onSubmit={handleSubmit}>
      <Input
        id="track-title"
        label="Title"
        value={title}
        onChange={(e) => setTitle(e.target.value)}
        error={errors.title}
        required
        autoFocus
      />
      <Input
        id="track-artist"
        label="Artist"
        value={artist}
        onChange={(e) => setArtist(e.target.value)}
        error={errors.artist}
        required
      />
      <Input
        id="track-mood"
        label="Mood / Genre"
        value={mood}
        onChange={(e) => setMood(e.target.value)}
        error={errors.mood}
        required
      />
      <Input
        id="track-storage-key"
        label="Storage Key (optional)"
        value={cloudflareStorageKey}
        onChange={(e) => setCloudflareStorageKey(e.target.value)}
      />

      <div className="track-form__actions">
        <Button type="submit" loading={loading}>
          {isEdit ? 'Save Changes' : 'Create Track'}
        </Button>
      </div>
    </form>
  )
}
