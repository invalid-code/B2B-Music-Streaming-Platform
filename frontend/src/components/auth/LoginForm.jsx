import React, { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import useAuthStore from '../../store/useAuthStore'

export default function LoginForm() {
  const navigate = useNavigate()
  const { loginWithApi, loading, error } = useAuthStore()
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')

  const onSubmit = async (e) => {
    e.preventDefault()
    await loginWithApi({ email, password })
    // in a real app, check store state to decide navigation
    navigate('/')
  }

  return (
    <form className="auth-form auth-form--desktop" onSubmit={onSubmit}>
      <label>
        Email
        <input
          type="email"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          required
          autoFocus
        />
      </label>

      <label>
        Password
        <input
          type="password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          required
        />
      </label>

      <div className="auth-actions">
        <button type="submit" disabled={loading} className="btn-primary">
          {loading ? 'Signing in...' : 'Sign in'}
        </button>
      </div>

      {error && <div className="auth-error">{error}</div>}
    </form>
  )
}
