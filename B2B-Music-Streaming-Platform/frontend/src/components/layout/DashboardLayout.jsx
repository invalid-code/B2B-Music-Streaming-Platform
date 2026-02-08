import { NavLink, useNavigate } from 'react-router-dom'
import {
  LayoutDashboard,
  Music,
  ListMusic,
  MapPin,
  LogOut,
  Menu,
  X,
} from 'lucide-react'
import useAuthStore from '../../store/useAuthStore'
import { useState } from 'react'
import './DashboardLayout.css'

const NAV_ITEMS = [
  { to: '/dashboard', label: 'Dashboard', icon: LayoutDashboard },
  { to: '/tracks', label: 'Tracks', icon: Music },
  { to: '/playlists', label: 'Playlists', icon: ListMusic },
  { to: '/venues', label: 'Venues', icon: MapPin },
]

export default function DashboardLayout({ children }) {
  const { user, logout } = useAuthStore()
  const navigate = useNavigate()
  const [sidebarOpen, setSidebarOpen] = useState(false)

  const handleLogout = () => {
    logout()
    navigate('/login')
  }

  const closeSidebar = () => setSidebarOpen(false)

  return (
    <div className="dashboard-layout">
      {/* Mobile overlay */}
      {sidebarOpen && (
        <div className="sidebar-overlay" onClick={closeSidebar} />
      )}

      {/* Sidebar */}
      <aside className={`sidebar ${sidebarOpen ? 'sidebar--open' : ''}`}>
        <div className="sidebar__header">
          <h2 className="sidebar__brand">B2B Music</h2>
          <button
            className="sidebar__close"
            onClick={closeSidebar}
            aria-label="Close menu"
          >
            <X size={20} />
          </button>
        </div>

        <nav className="sidebar__nav">
          {NAV_ITEMS.map(({ to, label, icon: Icon }) => (
            <NavLink
              key={to}
              to={to}
              className={({ isActive }) =>
                `sidebar__link ${isActive ? 'sidebar__link--active' : ''}`
              }
              onClick={closeSidebar}
            >
              <Icon size={20} />
              <span>{label}</span>
            </NavLink>
          ))}
        </nav>

        <div className="sidebar__footer">
          <div className="sidebar__user">
            <div className="sidebar__avatar">
              {(user?.fullName || user?.email || 'U').charAt(0).toUpperCase()}
            </div>
            <div className="sidebar__user-info">
              <span className="sidebar__user-name">
                {user?.fullName || 'User'}
              </span>
              <span className="sidebar__user-email">{user?.email}</span>
            </div>
          </div>
          <button
            className="sidebar__logout"
            onClick={handleLogout}
            aria-label="Sign out"
          >
            <LogOut size={18} />
            <span>Sign out</span>
          </button>
        </div>
      </aside>

      {/* Main content area */}
      <div className="dashboard-main">
        <header className="topbar">
          <button
            className="topbar__menu"
            onClick={() => setSidebarOpen(true)}
            aria-label="Open menu"
          >
            <Menu size={24} />
          </button>
          <div className="topbar__spacer" />
          <span className="topbar__greeting">
            Welcome, {user?.fullName || user?.email || 'User'}
          </span>
        </header>

        <main className="dashboard-content">{children}</main>
      </div>
    </div>
  )
}
