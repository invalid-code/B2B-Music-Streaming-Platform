const API_BASE = import.meta.env.VITE_API_BASE

function getAuthToken() {
  try {
    const stored = localStorage.getItem('auth-storage')
    if (stored) {
      const parsed = JSON.parse(stored)
      return parsed?.state?.token || null
    }
  } catch (e) {
    return null
  }
  return null
}

async function request(path, options = {}) {
  const url = `${API_BASE}${path}`
  
  // Add auth token to headers if available
  const token = getAuthToken()
  const headers = {
    'Content-Type': 'application/json',
    ...options.headers,
  }
  if (token) {
    headers['Authorization'] = `Bearer ${token}`
  }
  
  try {
    const res = await fetch(url, { ...options, headers })
    let data = null
    
    try {
      data = await res.json()
    } catch (e) {
      // Response has no JSON body
    }
    
    if (!res.ok) {
      const msg = data?.message || data?.error || res.statusText || 'Request failed'
      const err = new Error(msg)
      err.response = res
      err.data = data
      err.status = res.status
      throw err
    }
    
    return data
  } catch (err) {
    // Network error or request failed
    if (!err.response) {
      err.message = 'Network error. Please check your connection.'
    }
    throw err
  }
}

export async function post(path, body) {
  return request(path, {
    method: 'POST',
    body: JSON.stringify(body),
  })
}

export async function get(path) {
  return request(path, {
    method: 'GET',
  })
}

export async function put(path, body) {
  return request(path, {
    method: 'PUT',
    body: JSON.stringify(body),
  })
}

export async function del(path) {
  return request(path, {
    method: 'DELETE',
  })
}

export default { post, get, put, del }
