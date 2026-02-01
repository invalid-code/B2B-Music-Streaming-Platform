# Frontend (Vite + React)

This folder contains a minimal Vite + React frontend scaffold tailored for the B2B Music Streaming Platform's auth flows.

Quick start (from `frontend/`):

```bash
npm install
npm run dev
```

Notes:
- The frontend follows an API-First pattern and expects auth endpoints at `/api/auth/login` and `/api/auth/register`.
- Use the `useAuthStore` (Zustand) methods `loginWithApi` and `registerWithApi` to integrate with the backend.
- The app is desktop-optimized and uses `react-router-dom` for `/login` and `/register` pages.
