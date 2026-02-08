import api from '../api/client'

/**
 * Track API service — wraps all /tracks endpoints.
 */
export const trackService = {
  getAll() {
    return api.get('/tracks')
  },

  getById(id) {
    return api.get(`/tracks/${id}`)
  },

  create(payload) {
    return api.post('/tracks', payload)
  },

  update(id, payload) {
    return api.put(`/tracks/${id}`, { trackID: id, ...payload })
  },

  delete(id) {
    return api.del(`/tracks/${id}`)
  },
}

export default trackService
