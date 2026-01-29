import React, { useState } from 'react'
import { useNavigate, Link } from 'react-router-dom'
import useAuthStore from '../../store/useAuthStore'
import { validateEmail, validatePassword } from '../../utils/validation'

export default function LoginForm() {
  const navigate = useNavigate()
  const { loginWithApi, loading, error } = useAuthStore()
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [fieldErrors, setFieldErrors] = useState({})

  const validateForm = () => {
    const errors = {}
    errors.email = validateEmail(email)
    errors.password = validatePassword(password)
    setFieldErrors(errors)
    return !Object.values(errors).some(err => err)
  }

  const onSubmit = async (e) => {
    e.preventDefault()
    if (!validateForm()) return
    await loginWithApi({ email, password })
    // in a real app, check store state to decide navigation
    navigate('/')
  }

  return (
    <form className="auth-form" onSubmit={onSubmit}>
      <div className="form-group">
        <label className="form-label" htmlFor="email">
          Email
        </label>
        <input
          id="email"
          type="email"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          required
          autoFocus
          className="form-input"
          aria-describedby={fieldErrors.email ? "email-error" : undefined}
          aria-invalid={!!fieldErrors.email}
        />
        {fieldErrors.email && <div id="email-error" className="field-error" role="alert">{fieldErrors.email}</div>}
      </div>

      <div className="form-group">
        <label className="form-label" htmlFor="password">
          Password
        </label>
        <input
          id="password"
          type="password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          required
          className="form-input"
          aria-describedby={fieldErrors.password ? "password-error" : undefined}
          aria-invalid={!!fieldErrors.password}
        />
        {fieldErrors.password && <div id="password-error" className="field-error" role="alert">{fieldErrors.password}</div>}
      </div>

      <div className="auth-actions">
        <button type="submit" disabled={loading} className="btn btn-primary">
          {loading ? 'Signing in...' : 'Sign in'}
        </button>
      </div>

      {error && <div className="auth-error">{error}</div>}

      <div className="auth-links">
        <p>Don't have an account? <Link to="/register">Sign up</Link></p>
      </div>
    </form>
  )
}
