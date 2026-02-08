import api from '../api/client'

export const authService = {
  async login(credentials) {
    return await api.post('/Auth/login', credentials)
  },

  async register(payload) {
    return await api.post('/Auth/register', payload)
  },
}

export default authService