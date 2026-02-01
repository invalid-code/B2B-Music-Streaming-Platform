import React from 'react'
import { Routes, Route, Link, useNavigate } from 'react-router-dom'
import logo from './assets/logo.svg'
import LoginPage from './pages/LoginPage'
import RegisterPage from './pages/RegisterPage'
import ErrorBoundary from './components/common/ErrorBoundary'
import { ToastProvider } from './components/common/Toast'
import useAuthStore from './store/useAuthStore'

export default function App() {
  const { user, logout } = useAuthStore()
  const navigate = useNavigate()

  const handleLogout = () => {
    logout()
    navigate('/')
  }

  return (
    <ErrorBoundary>
      <ToastProvider>
        <div className="app-root">
          <header className="app-header">
            <Link to="/">
              <img src={logo} alt="logo" height="40" />
            </Link>
            <nav>
              {user ? (
                <>
                  <span>Welcome, {user.fullName || user.email}</span>
                  {' | '}
                  <button onClick={handleLogout} style={{ background: 'none', border: 'none', color: 'inherit', cursor: 'pointer', textDecoration: 'underline' }}>
                    Logout
                  </button>
                </>
              ) : (
                <>
                  <Link to="/">Login</Link>
                  {' | '}
                  <Link to="/register">Register</Link>
                </>
              )}
            </nav>
          </header>

          <main>
            <Routes>
              <Route path="/" element={<LoginPage />} />
              <Route path="/register" element={<RegisterPage />} />
            </Routes>
          </main>
        </div>
      </ToastProvider>
    </ErrorBoundary>
  )
}
