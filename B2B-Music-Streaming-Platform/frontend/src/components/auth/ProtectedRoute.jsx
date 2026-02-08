import { Navigate, useLocation } from 'react-router-dom'
import useAuthStore from '../../store/useAuthStore'

/**
 * Route guard that redirects unauthenticated users to /login.
 * Preserves the intended destination so we can redirect back after login.
 */
export default function ProtectedRoute({ children }) {
  const token = useAuthStore((state) => state.token)
  const location = useLocation()

  if (!token) {
    return <Navigate to="/login" state={{ from: location }} replace />
  }

  return children
}
