import api from '../api/client'

/**
 * Venue API service — wraps all /venues endpoints.
 */
export const venueService = {
  getAll() {
    return api.get('/venues')
  },

  getById(id) {
    return api.get(`/venues/${id}`)
  },

  create(payload) {
    return api.post('/venues', payload)
  },

  update(id, payload) {
    return api.put(`/venues/${id}`, { venueID: id, ...payload })
  },

  delete(id) {
    return api.del(`/venues/${id}`)
  },
}

export default venueService
