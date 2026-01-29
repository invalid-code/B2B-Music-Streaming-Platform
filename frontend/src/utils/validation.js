export const validateEmail = (email) => {
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
  return emailRegex.test(email) ? '' : 'Please enter a valid email address.'
}

export const validatePassword = (password) => {
  if (password.length < 6) {
    return 'Password must be at least 6 characters long.'
  }
  return ''
}

export const validateRequired = (value, fieldName) => {
  return value.trim() ? '' : `${fieldName} is required.`
}