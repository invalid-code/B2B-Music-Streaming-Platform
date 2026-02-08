import api from '../api/client'

/**
 * Playlist API service — wraps all /playlists endpoints.
 */
export const playlistService = {
  getAll() {
    return api.get('/playlists')
  },

  getById(id) {
    return api.get(`/playlists/${id}`)
  },

  create(payload) {
    return api.post('/playlists', payload)
  },

  update(id, payload) {
    return api.put(`/playlists/${id}`, { playlistID: id, ...payload })
  },

  delete(id) {
    return api.del(`/playlists/${id}`)
  },
}

export default playlistService
