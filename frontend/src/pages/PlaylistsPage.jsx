import { useEffect, useState } from 'react'
import { ListMusic, Plus, Search } from 'lucide-react'
import usePlaylistStore from '../store/usePlaylistStore'
import PlaylistList from '../components/playlists/PlaylistList'
import PlaylistForm from '../components/playlists/PlaylistForm'
import Modal from '../components/common/Modal'
import ConfirmDialog from '../components/common/ConfirmDialog'
import EmptyState from '../components/common/EmptyState'
import Button from '../components/common/Button'
import { useToast } from '../components/common/Toast'
import { SkeletonCard } from '../components/common/LoadingSkeleton'
import './PlaylistsPage.css'

export default function PlaylistsPage() {
  const toast = useToast()
  const {
    playlists,
    fetchPlaylists,
    createPlaylist,
    updatePlaylist,
    deletePlaylist,
    loading,
  } = usePlaylistStore()

  const [search, setSearch] = useState('')
  const [showForm, setShowForm] = useState(false)
  const [editingPlaylist, setEditingPlaylist] = useState(null)
  const [deletingPlaylist, setDeletingPlaylist] = useState(null)
  const [formLoading, setFormLoading] = useState(false)

  useEffect(() => {
    fetchPlaylists().catch(() => toast.error('Failed to load playlists'))
  }, [fetchPlaylists, toast])

  const filteredPlaylists = playlists.filter((p) => {
    if (!search) return true
    return (
      p.name?.toLowerCase().includes(search.toLowerCase()) ||
      p.vibeOrGenre?.toLowerCase().includes(search.toLowerCase())
    )
  })

  const handleCreate = async (data) => {
    setFormLoading(true)
    try {
      await createPlaylist(data)
      toast.success('Playlist created successfully')
      setShowForm(false)
    } catch {
      toast.error('Failed to create playlist')
    } finally {
      setFormLoading(false)
    }
  }

  const handleUpdate = async (data) => {
    if (!editingPlaylist) return
    setFormLoading(true)
    try {
      await updatePlaylist(editingPlaylist.playlistID, data)
      toast.success('Playlist updated successfully')
      setEditingPlaylist(null)
    } catch {
      toast.error('Failed to update playlist')
    } finally {
      setFormLoading(false)
    }
  }

  const handleDelete = async () => {
    if (!deletingPlaylist) return
    setFormLoading(true)
    try {
      await deletePlaylist(deletingPlaylist.playlistID)
      toast.success('Playlist deleted')
      setDeletingPlaylist(null)
    } catch {
      toast.error('Failed to delete playlist')
    } finally {
      setFormLoading(false)
    }
  }

  return (
    <div className="playlists-page">
      <div className="page-header">
        <div>
          <h1 className="page-header__title">Playlists</h1>
          <p className="page-header__subtitle">
            Manage your playlists ({playlists.length} playlists)
          </p>
        </div>
        <Button onClick={() => setShowForm(true)} icon={<Plus size={18} />}>
          Create Playlist
        </Button>
      </div>

      {/* Search */}
      <div className="tracks-filters">
        <div className="tracks-filters__search">
          <Search size={18} className="tracks-filters__search-icon" />
          <input
            type="text"
            placeholder="Search playlists..."
            value={search}
            onChange={(e) => setSearch(e.target.value)}
            className="tracks-filters__input"
          />
        </div>
      </div>

      {/* Content */}
      {loading && playlists.length === 0 ? (
        <div className="playlists-skeleton">
          {Array.from({ length: 6 }).map((_, i) => (
            <SkeletonCard key={i} />
          ))}
        </div>
      ) : filteredPlaylists.length === 0 ? (
        <EmptyState
          icon={<ListMusic size={48} />}
          title={search ? 'No matching playlists' : 'No playlists yet'}
          message={
            search
              ? 'Try adjusting your search.'
              : 'Create your first playlist to organize tracks by vibe.'
          }
          actionLabel={!search ? 'Create Playlist' : ''}
          onAction={() => setShowForm(true)}
        />
      ) : (
        <PlaylistList
          playlists={filteredPlaylists}
          onEdit={setEditingPlaylist}
          onDelete={setDeletingPlaylist}
        />
      )}

      {/* Create Modal */}
      <Modal
        isOpen={showForm}
        onClose={() => setShowForm(false)}
        title="Create Playlist"
        size="large"
      >
        <PlaylistForm onSubmit={handleCreate} loading={formLoading} />
      </Modal>

      {/* Edit Modal */}
      <Modal
        isOpen={!!editingPlaylist}
        onClose={() => setEditingPlaylist(null)}
        title="Edit Playlist"
        size="large"
      >
        <PlaylistForm
          initialData={editingPlaylist}
          onSubmit={handleUpdate}
          loading={formLoading}
        />
      </Modal>

      {/* Delete Confirmation */}
      <ConfirmDialog
        isOpen={!!deletingPlaylist}
        title="Delete Playlist"
        message={`Are you sure you want to delete "${deletingPlaylist?.name}"? This cannot be undone.`}
        onConfirm={handleDelete}
        onCancel={() => setDeletingPlaylist(null)}
        loading={formLoading}
      />
    </div>
  )
}
