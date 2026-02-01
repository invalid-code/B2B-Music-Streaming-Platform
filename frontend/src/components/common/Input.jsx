import React, { useState } from 'react'
import { Eye, EyeOff } from 'lucide-react'
import './Input.css'

export default function Input({ 
  id,
  type = 'text',
  label,
  value,
  onChange,
  error,
  required = false,
  placeholder,
  autoFocus = false,
  disabled = false,
  ...props 
}) {
  const [showPassword, setShowPassword] = useState(false)
  const [isFocused, setIsFocused] = useState(false)
  
  const isPassword = type === 'password'
  const inputType = isPassword && showPassword ? 'text' : type
  const hasValue = value && value.length > 0

  return (
    <div className={`input-wrapper ${error ? 'has-error' : ''} ${disabled ? 'is-disabled' : ''}`}>
      <div className={`input-container ${isFocused || hasValue ? 'is-active' : ''}`}>
        <input
          id={id}
          type={inputType}
          value={value}
          onChange={onChange}
          onFocus={() => setIsFocused(true)}
          onBlur={() => setIsFocused(false)}
          required={required}
          placeholder={placeholder}
          autoFocus={autoFocus}
          disabled={disabled}
          className="modern-input"
          aria-describedby={error ? `${id}-error` : undefined}
          aria-invalid={!!error}
          {...props}
        />
        <label htmlFor={id} className="floating-label">
          {label} {required && <span className="required-star">*</span>}
        </label>
        
        {isPassword && (
          <button
            type="button"
            onClick={() => setShowPassword(!showPassword)}
            className="password-toggle"
            aria-label={showPassword ? 'Hide password' : 'Show password'}
            tabIndex={-1}
          >
            {showPassword ? <EyeOff size={20} /> : <Eye size={20} />}
          </button>
        )}
      </div>
      
      {error && (
        <div id={`${id}-error`} className="input-error" role="alert">
          {error}
        </div>
      )}
    </div>
  )
}
