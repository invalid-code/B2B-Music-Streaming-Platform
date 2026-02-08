import { useEffect, useState } from 'react'
import { Music, Plus, Search } from 'lucide-react'
import useTrackStore from '../store/useTrackStore'
import TrackList from '../components/tracks/TrackList'
import TrackForm from '../components/tracks/TrackForm'
import Modal from '../components/common/Modal'
import ConfirmDialog from '../components/common/ConfirmDialog'
import EmptyState from '../components/common/EmptyState'
import Button from '../components/common/Button'
import { useToast } from '../components/common/Toast'
import { SkeletonCard } from '../components/common/LoadingSkeleton'
import './TracksPage.css'

export default function TracksPage() {
  const toast = useToast()
  const {
    tracks,
    fetchTracks,
    createTrack,
    updateTrack,
    deleteTrack,
    loading,
  } = useTrackStore()

  const [search, setSearch] = useState('')
  const [moodFilter, setMoodFilter] = useState('')
  const [showForm, setShowForm] = useState(false)
  const [editingTrack, setEditingTrack] = useState(null)
  const [deletingTrack, setDeletingTrack] = useState(null)
  const [formLoading, setFormLoading] = useState(false)

  useEffect(() => {
    fetchTracks().catch(() => toast.error('Failed to load tracks'))
  }, [fetchTracks, toast])

  /* --- Derived data --- */
  const moods = [...new Set(tracks.map((t) => t.mood).filter(Boolean))]
  const filteredTracks = tracks.filter((t) => {
    const matchesSearch =
      !search ||
      t.title?.toLowerCase().includes(search.toLowerCase()) ||
      t.artist?.toLowerCase().includes(search.toLowerCase())
    const matchesMood = !moodFilter || t.mood === moodFilter
    return matchesSearch && matchesMood
  })

  /* --- Handlers --- */
  const handleCreate = async (data) => {
    setFormLoading(true)
    try {
      await createTrack(data)
      toast.success('Track created successfully')
      setShowForm(false)
    } catch {
      toast.error('Failed to create track')
    } finally {
      setFormLoading(false)
    }
  }

  const handleUpdate = async (data) => {
    if (!editingTrack) return
    setFormLoading(true)
    try {
      await updateTrack(editingTrack.trackID, data)
      toast.success('Track updated successfully')
      setEditingTrack(null)
    } catch {
      toast.error('Failed to update track')
    } finally {
      setFormLoading(false)
    }
  }

  const handleDelete = async () => {
    if (!deletingTrack) return
    setFormLoading(true)
    try {
      await deleteTrack(deletingTrack.trackID)
      toast.success('Track deleted')
      setDeletingTrack(null)
    } catch {
      toast.error('Failed to delete track')
    } finally {
      setFormLoading(false)
    }
  }

  return (
    <div className="tracks-page">
      <div className="page-header">
        <div>
          <h1 className="page-header__title">Tracks</h1>
          <p className="page-header__subtitle">
            Manage your music library ({tracks.length} tracks)
          </p>
        </div>
        <Button onClick={() => setShowForm(true)} icon={<Plus size={18} />}>
          Add Track
        </Button>
      </div>

      {/* Filters */}
      <div className="tracks-filters">
        <div className="tracks-filters__search">
          <Search size={18} className="tracks-filters__search-icon" />
          <input
            type="text"
            placeholder="Search by title or artist..."
            value={search}
            onChange={(e) => setSearch(e.target.value)}
            className="tracks-filters__input"
          />
        </div>
        <select
          value={moodFilter}
          onChange={(e) => setMoodFilter(e.target.value)}
          className="tracks-filters__select"
        >
          <option value="">All moods</option>
          {moods.map((mood) => (
            <option key={mood} value={mood}>
              {mood}
            </option>
          ))}
        </select>
      </div>

      {/* Content */}
      {loading && tracks.length === 0 ? (
        <div className="tracks-skeleton">
          {Array.from({ length: 6 }).map((_, i) => (
            <SkeletonCard key={i} />
          ))}
        </div>
      ) : filteredTracks.length === 0 ? (
        <EmptyState
          icon={<Music size={48} />}
          title={search || moodFilter ? 'No matching tracks' : 'No tracks yet'}
          message={
            search || moodFilter
              ? 'Try adjusting your search or filters.'
              : 'Add your first track to get started.'
          }
          actionLabel={!search && !moodFilter ? 'Add Track' : ''}
          onAction={() => setShowForm(true)}
        />
      ) : (
        <TrackList
          tracks={filteredTracks}
          onEdit={setEditingTrack}
          onDelete={setDeletingTrack}
        />
      )}

      {/* Create Modal */}
      <Modal
        isOpen={showForm}
        onClose={() => setShowForm(false)}
        title="Add New Track"
      >
        <TrackForm onSubmit={handleCreate} loading={formLoading} />
      </Modal>

      {/* Edit Modal */}
      <Modal
        isOpen={!!editingTrack}
        onClose={() => setEditingTrack(null)}
        title="Edit Track"
      >
        <TrackForm
          initialData={editingTrack}
          onSubmit={handleUpdate}
          loading={formLoading}
        />
      </Modal>

      {/* Delete Confirmation */}
      <ConfirmDialog
        isOpen={!!deletingTrack}
        title="Delete Track"
        message={`Are you sure you want to delete "${deletingTrack?.title}"? This cannot be undone.`}
        onConfirm={handleDelete}
        onCancel={() => setDeletingTrack(null)}
        loading={formLoading}
      />
    </div>
  )
}
