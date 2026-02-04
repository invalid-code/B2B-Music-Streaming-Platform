#!/usr/bin/env bash
set -e

echo "Creating folders and files for auth UI (desktop-optimized)..."

mkdir -p src/components/auth src/store src/styles src/assets

# Create component skeletons
cat > src/components/auth/LoginForm.jsx <<'JS'
import React from 'react';
import '../../styles/Auth.css';

export default function LoginForm() {
  return (
    <form className="auth-form auth-form--desktop">
      {/* Desktop-optimized LoginForm placeholder */}
    </form>
  );
}
JS

cat > src/components/auth/RegisterForm.jsx <<'JS'
import React from 'react';
import '../../styles/Auth.css';

export default function RegisterForm() {
  return (
    <form className="auth-form auth-form--desktop">
      {/* Desktop-optimized RegisterForm placeholder */}
    </form>
  );
}
JS

# Create Zustand store file
cat > src/store/useAuthStore.js <<'JS'
import create from 'zustand';

export const useAuthStore = create((set) => ({
  user: null,
  token: null,
  login: (user, token) => set({ user, token }),
  logout: () => set({ user: null, token: null }),
}));

export default useAuthStore;
JS

# Styles and asset placeholder
cat > src/styles/Auth.css <<'CSS'
/* Desktop-optimized auth styles */
.auth-form { max-width: 480px; margin: 0 auto; }
.auth-form--desktop { padding: 24px; }
CSS

cat > src/assets/logo.svg <<'SVG'
<!-- placeholder branding icon -->
<svg xmlns="http://www.w3.org/2000/svg" width="120" height="40" viewBox="0 0 120 40"></svg>
SVG

echo "Auth scaffold created."

echo "If you're on Windows (PowerShell), run equivalent commands or use Git Bash/WSL to run this script."
