import React, { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import useAuthStore from '../../store/useAuthStore'

export default function RegisterForm() {
  const navigate = useNavigate()
  const { registerWithApi, loading, error } = useAuthStore()
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [company, setCompany] = useState('')

  const onSubmit = async (e) => {
    e.preventDefault()
    await registerWithApi({ email, password, company })
    // navigate to app or login after register
    navigate('/')
  }

  return (
    <form className="auth-form auth-form--desktop" onSubmit={onSubmit}>
      <label>
        Business email
        <input
          type="email"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          required
        />
      </label>

      <label>
        Company name
        <input
          type="text"
          value={company}
          onChange={(e) => setCompany(e.target.value)}
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
          {loading ? 'Creating account...' : 'Create account'}
        </button>
      </div>

      {error && <div className="auth-error">{error}</div>}
    </form>
  )
}
