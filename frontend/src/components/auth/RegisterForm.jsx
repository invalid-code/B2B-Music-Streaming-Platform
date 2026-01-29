import React, { useState } from 'react'
import { useNavigate, Link } from 'react-router-dom'
import useAuthStore from '../../store/useAuthStore'
import { validateEmail, validatePassword, validateRequired } from '../../utils/validation'

export default function RegisterForm() {
  const navigate = useNavigate()
  const { registerWithApi, loading, error } = useAuthStore()
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [company, setCompany] = useState('')
  const [fieldErrors, setFieldErrors] = useState({})

  const validateForm = () => {
    const errors = {}
    errors.email = validateEmail(email)
    errors.password = validatePassword(password)
    errors.company = validateRequired(company, 'Company name')
    setFieldErrors(errors)
    return !Object.values(errors).some(err => err)
  }

  const onSubmit = async (e) => {
    e.preventDefault()
    if (!validateForm()) return
    await registerWithApi({ email, password, company })
    // navigate to app or login after register
    navigate('/')
  }

  return (
    <form className="auth-form" onSubmit={onSubmit}>
      <div className="form-group">
        <label className="form-label" htmlFor="reg-email">
          Business email
        </label>
        <input
          id="reg-email"
          type="email"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          required
          className="form-input"
          aria-describedby={fieldErrors.email ? "reg-email-error" : undefined}
          aria-invalid={!!fieldErrors.email}
        />
        {fieldErrors.email && <div id="reg-email-error" className="field-error" role="alert">{fieldErrors.email}</div>}
      </div>

      <div className="form-group">
        <label className="form-label" htmlFor="reg-company">
          Company name
        </label>
        <input
          id="reg-company"
          type="text"
          value={company}
          onChange={(e) => setCompany(e.target.value)}
          className="form-input"
          aria-describedby={fieldErrors.company ? "reg-company-error" : undefined}
          aria-invalid={!!fieldErrors.company}
        />
        {fieldErrors.company && <div id="reg-company-error" className="field-error" role="alert">{fieldErrors.company}</div>}
      </div>

      <div className="form-group">
        <label className="form-label" htmlFor="reg-password">
          Password
        </label>
        <input
          id="reg-password"
          type="password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          required
          className="form-input"
          aria-describedby={fieldErrors.password ? "reg-password-error" : undefined}
          aria-invalid={!!fieldErrors.password}
        />
        {fieldErrors.password && <div id="reg-password-error" className="field-error" role="alert">{fieldErrors.password}</div>}
      </div>

      <div className="auth-actions">
        <button type="submit" disabled={loading} className="btn btn-primary">
          {loading ? 'Creating account...' : 'Create account'}
        </button>
      </div>

      {error && <div className="auth-error">{error}</div>}

      <div className="auth-links">
        <p>Already have an account? <Link to="/login">Sign in</Link></p>
      </div>
    </form>
  )
}
