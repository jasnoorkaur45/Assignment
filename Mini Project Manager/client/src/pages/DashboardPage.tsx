import { useEffect, useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import api from '../api/client'

interface Project {
  id: number
  title: string
  description?: string
  creationDate: string
}

export default function DashboardPage() {
  const [projects, setProjects] = useState<Project[]>([])
  const [title, setTitle] = useState('')
  const [description, setDescription] = useState('')
  const [error, setError] = useState<string | null>(null)
  const navigate = useNavigate()

  const token = localStorage.getItem('token')
  useEffect(() => {
    if (!token) {
      navigate('/login')
      return
    }
    ;(async () => {
      try {
        const res = await api.get<Project[]>('/api/projects')
        setProjects(res.data)
      } catch (err: any) {
        setError(err?.response?.data?.message || 'Failed to load projects')
      }
    })()
  }, [token, navigate])

  async function createProject() {
    setError(null)
    try {
      const res = await api.post<Project>('/api/projects', { title, description })
      setProjects(prev => [res.data, ...prev])
      setTitle('')
      setDescription('')
    } catch (err: any) {
      setError(err?.response?.data?.message || 'Failed to create')
    }
  }

  async function deleteProject(id: number) {
    setError(null)
    try {
      await api.delete(`/api/projects/${id}`)
      setProjects(prev => prev.filter(p => p.id !== id))
    } catch (err: any) {
      setError(err?.response?.data?.message || 'Failed to delete')
    }
  }

  return (
    <div>
      <h2>Dashboard</h2>
      <section style={{ marginBottom: 16 }}>
        <h3>Create Project</h3>
        <div style={{ display: 'grid', gap: 8, maxWidth: 500 }}>
          <input placeholder="Title" value={title} onChange={e => setTitle(e.target.value)} />
          <textarea placeholder="Description (optional)" value={description} onChange={e => setDescription(e.target.value)} />
          <button onClick={createProject} disabled={!title || title.length < 3}>Create</button>
          {error && <div style={{ color: 'red' }}>{error}</div>}
        </div>
      </section>

      <section>
        <h3>Your Projects</h3>
        {projects.length === 0 && <div>No projects yet.</div>}
        <ul style={{ padding: 0, listStyle: 'none', display: 'grid', gap: 8 }}>
          {projects.map(p => (
            <li key={p.id} style={{ border: '1px solid #ddd', padding: 12, borderRadius: 6, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
              <div>
                <div style={{ fontWeight: 600 }}><Link to={`/projects/${p.id}`}>{p.title}</Link></div>
                {p.description && <div style={{ opacity: 0.8 }}>{p.description}</div>}
                <div style={{ fontSize: 12, opacity: 0.6 }}>Created: {new Date(p.creationDate).toLocaleString()}</div>
              </div>
              <button onClick={() => deleteProject(p.id)}>Delete</button>
            </li>
          ))}
        </ul>
      </section>
    </div>
  )
}

