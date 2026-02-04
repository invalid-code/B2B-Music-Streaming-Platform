import React, { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import useAuthStore from '../../store/useAuthStore'
import { validateEmail, validatePassword, validateRequired } from '../../utils/validation'
import Input from '../common/Input'
import Button from '../common/Button'
import { useToast } from '../common/Toast'

export default function RegisterForm() {
  const navigate = useNavigate()
  const { registerWithApi, loading, error } = useAuthStore()
  const toast = useToast()
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
    if (!validateForm()) {
      toast.error('Please fix the form errors')
      return
    }
    try {
      await registerWithApi({ 
        email, 
        password, 
        fullName,
        venueName,
        location,
        businessRegistrationNumber: businessRegistrationNumber || undefined
      })
      toast.success('Account created successfully!')
      navigate('/')
    } catch (err) {
      toast.error(err.message || 'Registration failed')
    }
  }

  return (
    <form className="auth-form" onSubmit={onSubmit}>
      <Input
        id="reg-email"
        type="email"
        label="Business email"
        value={email}
        onChange={(e) => setEmail(e.target.value)}
        error={fieldErrors.email}
        required
      />

      <Input
        id="reg-fullname"
        type="text"
        label="Full name"
        value={fullName}
        onChange={(e) => setFullName(e.target.value)}
        error={fieldErrors.fullName}
        required
      />

      <Input
        id="reg-venue"
        type="text"
        label="Venue name"
        value={venueName}
        onChange={(e) => setVenueName(e.target.value)}
        error={fieldErrors.venueName}
        required
      />

      <Input
        id="reg-location"
        type="text"
        label="Location"
        value={location}
        onChange={(e) => setLocation(e.target.value)}
        error={fieldErrors.location}
        required
      />

      <Input
        id="reg-password"
        type="password"
        label="Password"
        value={password}
        onChange={(e) => setPassword(e.target.value)}
        error={fieldErrors.password}
        required
      />

      <Input
        id="reg-business-reg"
        type="text"
        label="Business registration number (optional)"
        value={businessRegistrationNumber}
        onChange={(e) => setBusinessRegistrationNumber(e.target.value)}
      />

      <div className="auth-actions">
        <Button 
          type="submit" 
          loading={loading}
          fullWidth
        >
          Create account
        </Button>
      </div>

      <div className="auth-links">
        <p>Already have an account? <Link to="/">Sign in</Link></p>
      </div>
    </form>
  )
}
