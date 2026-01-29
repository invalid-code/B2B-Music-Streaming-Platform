import create from 'zustand'
import api from '../api/client'

export const useAuthStore = create((set) => ({
  user: null,
  token: null,
  loading: false,
  error: null,
  login: (user, token) => set({ user, token, error: null }),
  logout: () => set({ user: null, token: null }),
  // API-first placeholder: performs network call to API gateway
  loginWithApi: async (credentials) => {
    set({ loading: true, error: null })
    try {
      const data = await api.post('/auth/login', credentials)
      set({ user: data.user || { email: credentials.email }, token: data.token, loading: false })
    } catch (err) {
      set({ error: err.message || 'Network error', loading: false })
    }
  },
  registerWithApi: async (payload) => {
    set({ loading: true, error: null })
    try {
      const data = await api.post('/auth/register', payload)
      set({ user: data.user || { email: payload.email }, token: data.token, loading: false })
    } catch (err) {
      set({ error: err.message || 'Network error', loading: false })
    }
  },
}))

export default useAuthStore
