import React, { useState } from 'react'
import { useNavigate, Link } from 'react-router-dom'
import useAuthStore from '../../store/useAuthStore'
import { validateEmail, validatePassword } from '../../utils/validation'
import Input from '../common/Input'
import Button from '../common/Button'
import { useToast } from '../common/Toast'

export default function LoginForm() {
  const navigate = useNavigate()
  const { loginWithApi, loading } = useAuthStore()
  const toast = useToast()
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
    if (!validateForm()) {
      toast.error('Please fix the form errors')
      return
    }
    try {
      await loginWithApi({ email, password })
      toast.success('Welcome back!')
      navigate('/dashboard')
    } catch (err) {
      toast.error(err.message || 'Login failed')
    }
  }

  return (
    <form className="auth-form" onSubmit={onSubmit}>
      <Input
        id="email"
        type="email"
        label="Email"
        value={email}
        onChange={(e) => setEmail(e.target.value)}
        error={fieldErrors.email}
        required
        autoFocus
      />

      <Input
        id="password"
        type="password"
        label="Password"
        value={password}
        onChange={(e) => setPassword(e.target.value)}
        error={fieldErrors.password}
        required
      />

      <div className="auth-actions">
        <Button 
          type="submit" 
          loading={loading}
          fullWidth
        >
          Sign in
        </Button>
      </div>

      <div className="auth-links">
        <p>Don&apos;t have an account? <Link to="/register">Sign up</Link></p>
      </div>
    </form>
  )
}
