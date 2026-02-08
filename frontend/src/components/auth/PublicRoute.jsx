import { Navigate } from 'react-router-dom'
import useAuthStore from '../../store/useAuthStore'

/**
 * Redirects authenticated users away from auth pages (login/register)
 * to the dashboard. Prevents logged-in users from seeing login forms.
 */
export default function PublicRoute({ children }) {
  const token = useAuthStore((state) => state.token)

  if (token) {
    return <Navigate to="/dashboard" replace />
  }

  return children
}
