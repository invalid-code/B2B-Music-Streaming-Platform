import Button from './Button'
import './EmptyState.css'

/**
 * Empty state placeholder with optional action button.
 */
export default function EmptyState({
  icon,
  title = 'Nothing here yet',
  message = '',
  actionLabel = '',
  onAction = null,
}) {
  return (
    <div className="empty-state">
      {icon && <div className="empty-state__icon">{icon}</div>}
      <h3 className="empty-state__title">{title}</h3>
      {message && <p className="empty-state__message">{message}</p>}
      {actionLabel && onAction && (
        <Button onClick={onAction} size="small">
          {actionLabel}
        </Button>
      )}
    </div>
  )
}
