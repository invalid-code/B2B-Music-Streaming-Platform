const API_BASE = import.meta.env.VITE_API_BASE || 'http://localhost:5269/api'

async function request(path, options = {}) {
  const url = `${API_BASE}${path}`
  
  try {
    const res = await fetch(url, options)
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
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(body),
  })
}

export default { post }
