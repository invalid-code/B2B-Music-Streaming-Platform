import React from 'react'
import './Button.css'

export default function Button({ 
  children, 
  type = 'button',
  variant = 'primary',
  size = 'medium',
  loading = false,
  disabled = false,
  fullWidth = false,
  icon,
  onClick,
  ...props 
}) {
  const classNames = [
    'modern-btn',
    `btn-${variant}`,
    `btn-${size}`,
    loading && 'btn-loading',
    fullWidth && 'btn-full-width',
    disabled && 'btn-disabled'
  ].filter(Boolean).join(' ')

  return (
    <button
      type={type}
      className={classNames}
      disabled={disabled || loading}
      onClick={onClick}
      {...props}
    >
      {loading && (
        <span className="btn-spinner" aria-hidden="true">
          <svg className="spinner-icon" viewBox="0 0 24 24">
            <circle cx="12" cy="12" r="10" fill="none" strokeWidth="3" />
          </svg>
        </span>
      )}
      {icon && !loading && <span className="btn-icon">{icon}</span>}
      <span className={loading ? 'btn-text-loading' : ''}>{children}</span>
    </button>
  )
}
