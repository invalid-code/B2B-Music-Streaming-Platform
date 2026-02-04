import React from 'react'
import './LoadingSkeleton.css'

export function SkeletonBox({ width = '100%', height = '20px', borderRadius = '8px', className = '' }) {
  return (
    <div 
      className={`skeleton-box ${className}`}
      style={{ width, height, borderRadius }}
      aria-hidden="true"
    />
  )
}

export function SkeletonText({ lines = 3, width = '100%' }) {
  return (
    <div className="skeleton-text" style={{ width }}>
      {[...Array(lines)].map((_, i) => (
        <SkeletonBox 
          key={i} 
          height="16px" 
          width={i === lines - 1 ? '60%' : '100%'}
        />
      ))}
    </div>
  )
}

export function SkeletonCard() {
  return (
    <div className="skeleton-card">
      <SkeletonBox height="200px" borderRadius="12px" />
      <div className="skeleton-card-content">
        <SkeletonBox height="24px" width="70%" />
        <SkeletonText lines={2} />
      </div>
    </div>
  )
}

export function SkeletonForm() {
  return (
    <div className="skeleton-form">
      {[...Array(4)].map((_, i) => (
        <div key={i} className="skeleton-form-field">
          <SkeletonBox height="16px" width="30%" />
          <SkeletonBox height="48px" borderRadius="12px" />
        </div>
      ))}
      <SkeletonBox height="48px" width="100%" borderRadius="12px" />
    </div>
  )
}

export default SkeletonBox
