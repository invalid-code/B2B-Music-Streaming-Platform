import React from 'react'

class ErrorBoundary extends React.Component {
  constructor(props) {
    super(props)
    this.state = { hasError: false, error: null }
  }

  static getDerivedStateFromError(error) {
    return { hasError: true, error }
  }

  componentDidCatch(error, errorInfo) {
    console.error('ErrorBoundary caught an error:', error, errorInfo)
  }

  render() {
    if (this.state.hasError) {
      return (
        <div style={{ padding: '2rem', textAlign: 'center', color: '#fff', background: '#121212' }}>
          <h2>Something went wrong.</h2>
          <p>Please try refreshing the page.</p>
          <button onClick={() => window.location.reload()} style={{ padding: '0.5rem 1rem', background: '#1db954', color: '#fff', border: 'none', borderRadius: '4px' }}>
            Refresh Page
          </button>
        </div>
      )
    }

    return this.props.children
  }
}

export default ErrorBoundary