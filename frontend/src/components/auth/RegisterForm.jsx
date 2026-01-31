import React, { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import useAuthStore from '../../store/useAuthStore'
import { validateEmail, validatePassword, validateRequired } from '../../utils/validation'

export default function RegisterForm() {
  const navigate = useNavigate()
  const { registerWithApi, loading, error } = useAuthStore()
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [fullName, setFullName] = useState('')
  const [venueName, setVenueName] = useState('')
  const [location, setLocation] = useState('')
  const [businessRegistrationNumber, setBusinessRegistrationNumber] = useState('')
  const [fieldErrors, setFieldErrors] = useState({})

  const validateForm = () => {
    const errors = {}
    errors.email = validateEmail(email)
    errors.password = validatePassword(password)
    errors.fullName = validateRequired(fullName, 'Full name')
    errors.venueName = validateRequired(venueName, 'Venue name')
    errors.location = validateRequired(location, 'Location')
    setFieldErrors(errors)
    return !Object.values(errors).some(err => err)
  }

  const onSubmit = async (e) => {
    e.preventDefault()
    if (!validateForm()) return
    await registerWithApi({ 
      email, 
      password, 
      fullName,
      venueName,
      location,
      businessRegistrationNumber: businessRegistrationNumber || undefined
    })
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
        <label className="form-label" htmlFor="reg-fullname">
          Full name
        </label>
        <input
          id="reg-fullname"
          type="text"
          value={fullName}
          onChange={(e) => setFullName(e.target.value)}
          required
          className="form-input"
          aria-describedby={fieldErrors.fullName ? "reg-fullname-error" : undefined}
          aria-invalid={!!fieldErrors.fullName}
        />
        {fieldErrors.fullName && <div id="reg-fullname-error" className="field-error" role="alert">{fieldErrors.fullName}</div>}
      </div>

      <div className="form-group">
        <label className="form-label" htmlFor="reg-venue">
          Venue name
        </label>
        <input
          id="reg-venue"
          type="text"
          value={venueName}
          onChange={(e) => setVenueName(e.target.value)}
          required
          className="form-input"
          aria-describedby={fieldErrors.venueName ? "reg-venue-error" : undefined}
          aria-invalid={!!fieldErrors.venueName}
        />
        {fieldErrors.venueName && <div id="reg-venue-error" className="field-error" role="alert">{fieldErrors.venueName}</div>}
      </div>

      <div className="form-group">
        <label className="form-label" htmlFor="reg-location">
          Location
        </label>
        <input
          id="reg-location"
          type="text"
          value={location}
          onChange={(e) => setLocation(e.target.value)}
          required
          className="form-input"
          aria-describedby={fieldErrors.location ? "reg-location-error" : undefined}
          aria-invalid={!!fieldErrors.location}
        />
        {fieldErrors.location && <div id="reg-location-error" className="field-error" role="alert">{fieldErrors.location}</div>}
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

      <div className="form-group">
        <label className="form-label" htmlFor="reg-business-reg">
          Business registration number (optional)
        </label>
        <input
          id="reg-business-reg"
          type="text"
          value={businessRegistrationNumber}
          onChange={(e) => setBusinessRegistrationNumber(e.target.value)}
          className="form-input"
        />
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
