import { FormEvent, useState } from 'react'
import { useNavigate, Link } from 'react-router-dom'
import api from '../api/client'

export default function LoginPage() {
  const [username, setUsername] = useState('')
  const [password, setPassword] = useState('')
  const [error, setError] = useState<string | null>(null)
  const navigate = useNavigate()

  async function onSubmit(e: FormEvent) {
    e.preventDefault()
    setError(null)
    try {
      const res = await api.post('/api/auth/login', { username, password })
      localStorage.setItem('token', res.data.token)
      localStorage.setItem('username', res.data.username)
      navigate('/')
    } catch (err: any) {
      setError(err?.response?.data?.message || 'Login failed')
    }
  }

  return (
    <div style={{ maxWidth: 400 }}>
      <h2>Login</h2>
      <form onSubmit={onSubmit} style={{ display: 'grid', gap: 8 }}>
        <input placeholder="Username" value={username} onChange={e => setUsername(e.target.value)} required />
        <input placeholder="Password" type="password" value={password} onChange={e => setPassword(e.target.value)} required />
        <button type="submit">Login</button>
        {error && <div style={{ color: 'red' }}>{error}</div>}
      </form>
      <p>New here? <Link to="/register">Register</Link></p>
    </div>
  )
}

