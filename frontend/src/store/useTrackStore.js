import { create } from 'zustand'
import trackService from '../services/trackService'

/**
 * Track store — manages track list, CRUD operations, and loading/error state.
 */
const useTrackStore = create((set, get) => ({
  tracks: [],
  selectedTrack: null,
  loading: false,
  error: null,

  /** Fetch all tracks from the API */
  fetchTracks: async () => {
    set({ loading: true, error: null })
    try {
      const data = await trackService.getAll()
      set({ tracks: Array.isArray(data) ? data : [], loading: false })
    } catch (err) {
      set({ error: err.message, loading: false })
      throw err
    }
  },

  /** Fetch a single track by ID */
  fetchTrackById: async (id) => {
    set({ loading: true, error: null })
    try {
      const data = await trackService.getById(id)
      set({ selectedTrack: data, loading: false })
      return data
    } catch (err) {
      set({ error: err.message, loading: false })
      throw err
    }
  },

  /** Create a new track and append it to the list */
  createTrack: async (payload) => {
    set({ loading: true, error: null })
    try {
      const created = await trackService.create(payload)
      set((state) => ({
        tracks: [...state.tracks, created],
        loading: false,
      }))
      return created
    } catch (err) {
      set({ error: err.message, loading: false })
      throw err
    }
  },

  /** Update an existing track in-place */
  updateTrack: async (id, payload) => {
    set({ loading: true, error: null })
    try {
      await trackService.update(id, payload)
      // Optimistic update — patch the local list
      set((state) => ({
        tracks: state.tracks.map((t) =>
          t.trackID === id ? { ...t, ...payload } : t
        ),
        loading: false,
      }))
    } catch (err) {
      set({ error: err.message, loading: false })
      throw err
    }
  },

  /** Delete a track by ID */
  deleteTrack: async (id) => {
    set({ loading: true, error: null })
    try {
      await trackService.delete(id)
      set((state) => ({
        tracks: state.tracks.filter((t) => t.trackID !== id),
        loading: false,
      }))
    } catch (err) {
      set({ error: err.message, loading: false })
      throw err
    }
  },

  /** Clear any error */
  clearError: () => set({ error: null }),

  /** Reset selected track */
  clearSelectedTrack: () => set({ selectedTrack: null }),
}))

export default useTrackStore
