import api from '../api/client'

export const authService = {
  async login(credentials) {
    return await api.post('/auth/login', credentials)
  },

  async register(payload) {
    return await api.post('/auth/register', payload)
  },
}

export default authService