import { Link, Outlet, useLocation, useNavigate } from 'react-router-dom'

function App() {
  const location = useLocation()
  const navigate = useNavigate()
  const token = localStorage.getItem('token')
  const username = localStorage.getItem('username')

  function logout() {
    localStorage.removeItem('token')
    localStorage.removeItem('username')
    navigate('/login')
  }

  return (
    <div style={{ fontFamily: 'system-ui', maxWidth: 900, margin: '0 auto', padding: 16 }}>
      <header style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 16 }}>
        <nav style={{ display: 'flex', gap: 12 }}>
          <Link to="/">Dashboard</Link>
          {!token && <Link to="/login">Login</Link>}
          {!token && <Link to="/register">Register</Link>}
        </nav>
        {token && (
          <div style={{ display: 'flex', gap: 12, alignItems: 'center' }}>
            <span>Hi, {username}</span>
            <button onClick={logout}>Logout</button>
          </div>
        )}
      </header>
      <main>
        <Outlet />
      </main>
      <footer style={{ marginTop: 32, fontSize: 12, opacity: 0.6 }}>Mini Project Manager</footer>
    </div>
  )
}

export default App

