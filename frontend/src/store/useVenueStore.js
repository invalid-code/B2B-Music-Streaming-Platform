import { create } from 'zustand'
import venueService from '../services/venueService'

/**
 * Venue store — manages venue list, CRUD operations, and loading/error state.
 */
const useVenueStore = create((set) => ({
  venues: [],
  selectedVenue: null,
  loading: false,
  error: null,

  /** Fetch all venues */
  fetchVenues: async () => {
    set({ loading: true, error: null })
    try {
      const data = await venueService.getAll()
      // Backend wraps response in { venues: [...] }
      const list = data?.venues || (Array.isArray(data) ? data : [])
      set({ venues: list, loading: false })
    } catch (err) {
      set({ error: err.message, loading: false })
      throw err
    }
  },

  /** Fetch a single venue by ID */
  fetchVenueById: async (id) => {
    set({ loading: true, error: null })
    try {
      const data = await venueService.getById(id)
      set({ selectedVenue: data, loading: false })
      return data
    } catch (err) {
      set({ error: err.message, loading: false })
      throw err
    }
  },

  /** Create a new venue */
  createVenue: async (payload) => {
    set({ loading: true, error: null })
    try {
      const created = await venueService.create(payload)
      set((state) => ({
        venues: [...state.venues, created],
        loading: false,
      }))
      return created
    } catch (err) {
      set({ error: err.message, loading: false })
      throw err
    }
  },

  /** Update an existing venue */
  updateVenue: async (id, payload) => {
    set({ loading: true, error: null })
    try {
      await venueService.update(id, payload)
      set((state) => ({
        venues: state.venues.map((v) =>
          v.venueID === id ? { ...v, ...payload } : v
        ),
        loading: false,
      }))
    } catch (err) {
      set({ error: err.message, loading: false })
      throw err
    }
  },

  /** Delete a venue */
  deleteVenue: async (id) => {
    set({ loading: true, error: null })
    try {
      await venueService.delete(id)
      set((state) => ({
        venues: state.venues.filter((v) => v.venueID !== id),
        loading: false,
      }))
    } catch (err) {
      set({ error: err.message, loading: false })
      throw err
    }
  },

  clearError: () => set({ error: null }),
  clearSelectedVenue: () => set({ selectedVenue: null }),
}))

export default useVenueStore
