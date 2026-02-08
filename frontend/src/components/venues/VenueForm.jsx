import { useState, useEffect } from 'react'
import Input from '../common/Input'
import Button from '../common/Button'
import { validateRequired } from '../../utils/validation'

/**
 * Create / Edit form for a venue.
 */
export default function VenueForm({ initialData = null, onSubmit, loading = false }) {
  const isEdit = !!initialData

  const [businessName, setBusinessName] = useState(initialData?.businessName || '')
  const [location, setLocation] = useState(initialData?.location || '')
  const [subscriptionStatus, setSubscriptionStatus] = useState(
    initialData?.subscriptionStatus || 'Trial'
  )
  const [errors, setErrors] = useState({})

  useEffect(() => {
    setBusinessName(initialData?.businessName || '')
    setLocation(initialData?.location || '')
    setSubscriptionStatus(initialData?.subscriptionStatus || 'Trial')
    setErrors({})
  }, [initialData])

  const validate = () => {
    const e = {}
    e.businessName = validateRequired(businessName, 'Business name')
    e.location = validateRequired(location, 'Location')
    setErrors(e)
    return !Object.values(e).some(Boolean)
  }

  const handleSubmit = (e) => {
    e.preventDefault()
    if (!validate()) return
    onSubmit({ businessName, location, subscriptionStatus })
  }

  return (
    <form className="venue-form" onSubmit={handleSubmit}>
      <Input
        id="venue-name"
        label="Business Name"
        value={businessName}
        onChange={(e) => setBusinessName(e.target.value)}
        error={errors.businessName}
        required
        autoFocus
      />
      <Input
        id="venue-location"
        label="Location"
        value={location}
        onChange={(e) => setLocation(e.target.value)}
        error={errors.location}
        required
      />

      <div className="venue-form__select-wrapper">
        <label className="venue-form__select-label" htmlFor="venue-status">
          Subscription Status
        </label>
        <select
          id="venue-status"
          className="venue-form__select"
          value={subscriptionStatus}
          onChange={(e) => setSubscriptionStatus(e.target.value)}
        >
          <option value="Trial">Trial</option>
          <option value="Paid">Paid</option>
        </select>
      </div>

      <div className="venue-form__actions">
        <Button type="submit" loading={loading}>
          {isEdit ? 'Save Changes' : 'Create Venue'}
        </Button>
      </div>
    </form>
  )
}
