const API_BASE = import.meta.env.VITE_API_BASE || '/api'

async function request(path, options = {}) {
  const url = `${API_BASE}${path}`
  const res = await fetch(url, options)
  let data = null
  try {
    data = await res.json()
  } catch (e) {
    // no json
  }
  if (!res.ok) {
    const msg = data?.message || res.statusText || 'Request failed'
    const err = new Error(msg)
    err.response = res
    err.data = data
    throw err
  }
  return data
}

export async function post(path, body) {
  return request(path, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(body),
  })
}

export default { post }
