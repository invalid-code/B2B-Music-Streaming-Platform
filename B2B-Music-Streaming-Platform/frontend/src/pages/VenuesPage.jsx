import { useEffect, useState } from 'react'
import { MapPin, Plus, Search } from 'lucide-react'
import useVenueStore from '../store/useVenueStore'
import VenueList from '../components/venues/VenueList'
import VenueForm from '../components/venues/VenueForm'
import Modal from '../components/common/Modal'
import ConfirmDialog from '../components/common/ConfirmDialog'
import EmptyState from '../components/common/EmptyState'
import Button from '../components/common/Button'
import { useToast } from '../components/common/Toast'
import { SkeletonCard } from '../components/common/LoadingSkeleton'
import './VenuesPage.css'

export default function VenuesPage() {
  const toast = useToast()
  const {
    venues,
    fetchVenues,
    createVenue,
    updateVenue,
    deleteVenue,
    loading,
  } = useVenueStore()

  const [search, setSearch] = useState('')
  const [statusFilter, setStatusFilter] = useState('')
  const [showForm, setShowForm] = useState(false)
  const [editingVenue, setEditingVenue] = useState(null)
  const [deletingVenue, setDeletingVenue] = useState(null)
  const [formLoading, setFormLoading] = useState(false)

  useEffect(() => {
    fetchVenues().catch(() => toast.error('Failed to load venues'))
  }, [fetchVenues, toast])

  const filteredVenues = venues.filter((v) => {
    const matchesSearch =
      !search ||
      v.businessName?.toLowerCase().includes(search.toLowerCase()) ||
      v.location?.toLowerCase().includes(search.toLowerCase())
    const matchesStatus =
      !statusFilter || v.subscriptionStatus === statusFilter
    return matchesSearch && matchesStatus
  })

  const handleCreate = async (data) => {
    setFormLoading(true)
    try {
      await createVenue(data)
      toast.success('Venue created successfully')
      setShowForm(false)
    } catch {
      toast.error('Failed to create venue')
    } finally {
      setFormLoading(false)
    }
  }

  const handleUpdate = async (data) => {
    if (!editingVenue) return
    setFormLoading(true)
    try {
      await updateVenue(editingVenue.venueID, data)
      toast.success('Venue updated successfully')
      setEditingVenue(null)
    } catch {
      toast.error('Failed to update venue')
    } finally {
      setFormLoading(false)
    }
  }

  const handleDelete = async () => {
    if (!deletingVenue) return
    setFormLoading(true)
    try {
      await deleteVenue(deletingVenue.venueID)
      toast.success('Venue deleted')
      setDeletingVenue(null)
    } catch {
      toast.error('Failed to delete venue')
    } finally {
      setFormLoading(false)
    }
  }

  return (
    <div className="venues-page">
      <div className="page-header">
        <div>
          <h1 className="page-header__title">Venues</h1>
          <p className="page-header__subtitle">
            Manage your business venues ({venues.length} venues)
          </p>
        </div>
        <Button onClick={() => setShowForm(true)} icon={<Plus size={18} />}>
          Add Venue
        </Button>
      </div>

      {/* Filters */}
      <div className="tracks-filters">
        <div className="tracks-filters__search">
          <Search size={18} className="tracks-filters__search-icon" />
          <input
            type="text"
            placeholder="Search by name or location..."
            value={search}
            onChange={(e) => setSearch(e.target.value)}
            className="tracks-filters__input"
          />
        </div>
        <select
          value={statusFilter}
          onChange={(e) => setStatusFilter(e.target.value)}
          className="tracks-filters__select"
        >
          <option value="">All statuses</option>
          <option value="Trial">Trial</option>
          <option value="Paid">Paid</option>
        </select>
      </div>

      {/* Content */}
      {loading && venues.length === 0 ? (
        <div className="venues-skeleton">
          {Array.from({ length: 4 }).map((_, i) => (
            <SkeletonCard key={i} />
          ))}
        </div>
      ) : filteredVenues.length === 0 ? (
        <EmptyState
          icon={<MapPin size={48} />}
          title={search || statusFilter ? 'No matching venues' : 'No venues yet'}
          message={
            search || statusFilter
              ? 'Try adjusting your search or filters.'
              : 'Add your first venue to start managing locations.'
          }
          actionLabel={!search && !statusFilter ? 'Add Venue' : ''}
          onAction={() => setShowForm(true)}
        />
      ) : (
        <VenueList
          venues={filteredVenues}
          onEdit={setEditingVenue}
          onDelete={setDeletingVenue}
        />
      )}

      {/* Create Modal */}
      <Modal
        isOpen={showForm}
        onClose={() => setShowForm(false)}
        title="Add New Venue"
      >
        <VenueForm onSubmit={handleCreate} loading={formLoading} />
      </Modal>

      {/* Edit Modal */}
      <Modal
        isOpen={!!editingVenue}
        onClose={() => setEditingVenue(null)}
        title="Edit Venue"
      >
        <VenueForm
          initialData={editingVenue}
          onSubmit={handleUpdate}
          loading={formLoading}
        />
      </Modal>

      {/* Delete Confirmation */}
      <ConfirmDialog
        isOpen={!!deletingVenue}
        title="Delete Venue"
        message={`Are you sure you want to delete "${deletingVenue?.businessName}"? This cannot be undone.`}
        onConfirm={handleDelete}
        onCancel={() => setDeletingVenue(null)}
        loading={formLoading}
      />
    </div>
  )
}
