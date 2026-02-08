import { create } from 'zustand'
import playlistService from '../services/playlistService'

/**
 * Playlist store — manages playlist list, CRUD operations, and loading/error state.
 */
const usePlaylistStore = create((set) => ({
  playlists: [],
  selectedPlaylist: null,
  loading: false,
  error: null,

  /** Fetch all playlists */
  fetchPlaylists: async () => {
    set({ loading: true, error: null })
    try {
      const data = await playlistService.getAll()
      set({ playlists: Array.isArray(data) ? data : [], loading: false })
    } catch (err) {
      set({ error: err.message, loading: false })
      throw err
    }
  },

  /** Fetch a single playlist by ID */
  fetchPlaylistById: async (id) => {
    set({ loading: true, error: null })
    try {
      const data = await playlistService.getById(id)
      set({ selectedPlaylist: data, loading: false })
      return data
    } catch (err) {
      set({ error: err.message, loading: false })
      throw err
    }
  },

  /** Create a new playlist */
  createPlaylist: async (payload) => {
    set({ loading: true, error: null })
    try {
      const created = await playlistService.create(payload)
      set((state) => ({
        playlists: [...state.playlists, created],
        loading: false,
      }))
      return created
    } catch (err) {
      set({ error: err.message, loading: false })
      throw err
    }
  },

  /** Update an existing playlist */
  updatePlaylist: async (id, payload) => {
    set({ loading: true, error: null })
    try {
      await playlistService.update(id, payload)
      set((state) => ({
        playlists: state.playlists.map((p) =>
          p.playlistID === id ? { ...p, ...payload } : p
        ),
        loading: false,
      }))
    } catch (err) {
      set({ error: err.message, loading: false })
      throw err
    }
  },

  /** Delete a playlist */
  deletePlaylist: async (id) => {
    set({ loading: true, error: null })
    try {
      await playlistService.delete(id)
      set((state) => ({
        playlists: state.playlists.filter((p) => p.playlistID !== id),
        loading: false,
      }))
    } catch (err) {
      set({ error: err.message, loading: false })
      throw err
    }
  },

  clearError: () => set({ error: null }),
  clearSelectedPlaylist: () => set({ selectedPlaylist: null }),
}))

export default usePlaylistStore
