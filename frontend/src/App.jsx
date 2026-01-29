import React from 'react'
import { Routes, Route, Link } from 'react-router-dom'
import logo from './assets/logo.svg'
import LoginPage from './pages/LoginPage'
import RegisterPage from './pages/RegisterPage'
import ErrorBoundary from './components/common/ErrorBoundary'

export default function App() {
  return (
    <ErrorBoundary>
      <div className="app-root">
        <header className="app-header">
          <Link to="/">
            <img src={logo} alt="logo" height="40" />
          </Link>
          <nav>
            <Link to="/login">Login</Link>
            {' | '}
            <Link to="/register">Register</Link>
          </nav>
        </header>

        <main>
          <Routes>
            <Route path="/" element={<LoginPage />} />
            <Route path="/login" element={<LoginPage />} />
            <Route path="/register" element={<RegisterPage />} />
          </Routes>
        </main>
      </div>
    </ErrorBoundary>
  )
}
