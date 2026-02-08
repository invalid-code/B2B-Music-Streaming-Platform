import { useState, useEffect } from 'react'
import Input from '../common/Input'
import Button from '../common/Button'
import TrackPicker from './TrackPicker'
import { validateRequired } from '../../utils/validation'

/**
 * Create / Edit form for a playlist — includes a track picker.
 */
export default function PlaylistForm({ initialData = null, onSubmit, loading = false }) {
  const isEdit = !!initialData

  const [name, setName] = useState(initialData?.name || '')
  const [vibeOrGenre, setVibeOrGenre] = useState(initialData?.vibeOrGenre || '')
  const [selectedTrackIDs, setSelectedTrackIDs] = useState(
    initialData?.trackIDs || []
  )
  const [errors, setErrors] = useState({})

  /* Reset form if initialData changes (e.g. switching from edit to create) */
  useEffect(() => {
    setName(initialData?.name || '')
    setVibeOrGenre(initialData?.vibeOrGenre || '')
    setSelectedTrackIDs(initialData?.trackIDs || [])
    setErrors({})
  }, [initialData])

  const validate = () => {
    const e = {}
    e.name = validateRequired(name, 'Name')
    e.vibeOrGenre = validateRequired(vibeOrGenre, 'Vibe / Genre')
    setErrors(e)
    return !Object.values(e).some(Boolean)
  }

  const handleSubmit = (e) => {
    e.preventDefault()
    if (!validate()) return
    onSubmit({ name, vibeOrGenre, trackIDs: selectedTrackIDs })
  }

  return (
    <form className="playlist-form" onSubmit={handleSubmit}>
      <Input
        id="playlist-name"
        label="Playlist Name"
        value={name}
        onChange={(e) => setName(e.target.value)}
        error={errors.name}
        required
        autoFocus
      />
      <Input
        id="playlist-vibe"
        label="Vibe / Genre"
        value={vibeOrGenre}
        onChange={(e) => setVibeOrGenre(e.target.value)}
        error={errors.vibeOrGenre}
        required
      />

      <TrackPicker
        selectedTrackIDs={selectedTrackIDs}
        onChange={setSelectedTrackIDs}
      />

      <div className="playlist-form__actions">
        <Button type="submit" loading={loading}>
          {isEdit ? 'Save Changes' : 'Create Playlist'}
        </Button>
      </div>
    </form>
  )
}
