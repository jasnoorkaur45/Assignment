import { useEffect, useState } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import api from '../api/client'

interface TaskItem {
  id: number
  title: string
  dueDate?: string
  isCompleted: boolean
}

interface ProjectDetails {
  id: number
  title: string
  description?: string
  creationDate: string
  tasks: TaskItem[]
}

export default function ProjectDetailsPage() {
  const { id } = useParams()
  const projectId = Number(id)
  const [project, setProject] = useState<ProjectDetails | null>(null)
  const [error, setError] = useState<string | null>(null)
  const [taskTitle, setTaskTitle] = useState('')
  const [taskDue, setTaskDue] = useState('')
  const navigate = useNavigate()

  useEffect(() => {
    const token = localStorage.getItem('token')
    if (!token) {
      navigate('/login')
      return
    }
    ;(async () => {
      try {
        const res = await api.get(`/api/projects/${projectId}`)
        const data = res.data
        setProject({
          id: data.id,
          title: data.title,
          description: data.description,
          creationDate: data.creationDate,
          tasks: data.tasks,
        })
      } catch (err: any) {
        setError(err?.response?.data?.message || 'Failed to load project')
      }
    })()
  }, [projectId, navigate])

  async function addTask() {
    setError(null)
    try {
      const res = await api.post(`/api/projects/${projectId}/tasks`, {
        title: taskTitle,
        dueDate: taskDue ? new Date(taskDue) : null,
      })
      setProject(prev => prev ? { ...prev, tasks: [...prev.tasks, res.data] } : prev)
      setTaskTitle('')
      setTaskDue('')
    } catch (err: any) {
      setError(err?.response?.data?.message || 'Failed to add task')
    }
  }

  async function toggleTask(t: TaskItem) {
    try {
      const res = await api.put(`/api/tasks/${t.id}`, {
        title: t.title,
        dueDate: t.dueDate || null,
        isCompleted: !t.isCompleted,
      })
      setProject(prev => prev ? { ...prev, tasks: prev.tasks.map(x => x.id === t.id ? res.data : x) } : prev)
    } catch (err: any) {
      setError(err?.response?.data?.message || 'Failed to update task')
    }
  }

  async function deleteTask(taskId: number) {
    try {
      await api.delete(`/api/tasks/${taskId}`)
      setProject(prev => prev ? { ...prev, tasks: prev.tasks.filter(x => x.id !== taskId) } : prev)
    } catch (err: any) {
      setError(err?.response?.data?.message || 'Failed to delete task')
    }
  }

  if (!project) return <div>Loading...</div>

  return (
    <div>
      <h2>{project.title}</h2>
      {project.description && <p>{project.description}</p>}
      <div style={{ fontSize: 12, opacity: 0.6 }}>Created: {new Date(project.creationDate).toLocaleString()}</div>

      <section style={{ marginTop: 16 }}>
        <h3>Tasks</h3>
        <div style={{ display: 'flex', gap: 8, alignItems: 'center', marginBottom: 8 }}>
          <input placeholder="Task title" value={taskTitle} onChange={e => setTaskTitle(e.target.value)} />
          <input type="date" value={taskDue} onChange={e => setTaskDue(e.target.value)} />
          <button onClick={addTask} disabled={!taskTitle}>Add</button>
        </div>
        {error && <div style={{ color: 'red' }}>{error}</div>}
        <ul style={{ listStyle: 'none', padding: 0, display: 'grid', gap: 8 }}>
          {project.tasks.map(t => (
            <li key={t.id} style={{ border: '1px solid #ddd', padding: 8, borderRadius: 6, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
              <div>
                <div style={{ textDecoration: t.isCompleted ? 'line-through' : 'none' }}>{t.title}</div>
                {t.dueDate && <div style={{ fontSize: 12, opacity: 0.7 }}>Due: {new Date(t.dueDate).toLocaleDateString()}</div>}
              </div>
              <div style={{ display: 'flex', gap: 8 }}>
                <button onClick={() => toggleTask(t)}>{t.isCompleted ? 'Undo' : 'Complete'}</button>
                <button onClick={() => deleteTask(t.id)}>Delete</button>
              </div>
            </li>
          ))}
        </ul>
      </section>
    </div>
  )
}

