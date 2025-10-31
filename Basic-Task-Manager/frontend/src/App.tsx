import React, { useEffect, useMemo, useState } from 'react';
import TaskItemRow from '@components/TaskItemRow';
import { addTask, deleteTask, fetchTasks, updateTask, type TaskItem } from '@api/tasks';

export default function App() {
  const [tasks, setTasks] = useState<TaskItem[]>([]);
  const [newTask, setNewTask] = useState('');
  const [loading, setLoading] = useState(true);
  const [busyIds, setBusyIds] = useState<Set<string>>(new Set());
  const canAdd = useMemo(() => newTask.trim().length > 0, [newTask]);

  useEffect(() => {
    let cancelled = false;
    (async () => {
      try {
        const data = await fetchTasks();
        if (!cancelled) setTasks(data);
      } finally {
        if (!cancelled) setLoading(false);
      }
    })();
    return () => { cancelled = true; };
  }, []);

  async function handleAddTask(e: React.FormEvent) {
    e.preventDefault();
    const description = newTask.trim();
    if (!description) return;

    setNewTask('');
    const optimistic: TaskItem = { id: crypto.randomUUID(), description, isCompleted: false };
    setTasks(prev => [optimistic, ...prev]);

    try {
      const created = await addTask(description);
      setTasks(prev => prev.map(t => (t.id === optimistic.id ? created : t)));
    } catch {
      setTasks(prev => prev.filter(t => t.id !== optimistic.id));
      alert('Failed to add task.');
    }
  }

  async function handleToggle(task: TaskItem) {
    setBusyIds(prev => new Set(prev).add(task.id));
    const original = tasks.find(t => t.id === task.id);
    setTasks(prev => prev.map(t => (t.id === task.id ? task : t)));
    try {
      const updated = await updateTask(task);
      setTasks(prev => prev.map(t => (t.id === task.id ? updated : t)));
    } catch {
      if (original) setTasks(prev => prev.map(t => (t.id === task.id ? original : t)));
      alert('Failed to update task.');
    } finally {
      setBusyIds(prev => { const n = new Set(prev); n.delete(task.id); return n; });
    }
  }

  async function handleDelete(id: string) {
    setBusyIds(prev => new Set(prev).add(id));
    const original = tasks;
    setTasks(prev => prev.filter(t => t.id !== id));
    try {
      await deleteTask(id);
    } catch {
      setTasks(original);
      alert('Failed to delete task.');
    } finally {
      setBusyIds(prev => { const n = new Set(prev); n.delete(id); return n; });
    }
  }

  return (
    <div className="mx-auto max-w-2xl p-4 sm:p-8">
      <header className="mb-6 sm:mb-8">
        <h1 className="text-2xl font-semibold text-gray-900 sm:text-3xl">Basic Task Manager</h1>
        <p className="mt-1 text-gray-500">Add, complete, or delete tasks.</p>
      </header>

      <form onSubmit={handleAddTask} className="mb-6 flex flex-col gap-3 sm:flex-row">
        <input
          type="text"
          placeholder="What do you need to do?"
          value={newTask}
          onChange={e => setNewTask(e.target.value)}
          className="w-full rounded border border-gray-300 px-3 py-2 outline-none ring-blue-500 focus:ring-2"
        />
        <button
          type="submit"
          disabled={!canAdd}
          className={`rounded px-4 py-2 font-medium text-white ${canAdd ? 'bg-blue-600 hover:bg-blue-700 active:bg-blue-800' : 'bg-blue-300 cursor-not-allowed'}`}
        >
          Add Task
        </button>
      </form>

      <section>
        {loading ? (
          <p className="text-gray-500">Loading tasks...</p>
        ) : tasks.length === 0 ? (
          <p className="text-gray-500">No tasks yet. Add your first one!</p>
        ) : (
          <ul className="flex flex-col gap-3">
            {tasks.map(task => (
              <div key={task.id} className={busyIds.has(task.id) ? 'opacity-60' : undefined}>
                <TaskItemRow task={task} onToggle={handleToggle} onDelete={handleDelete} />
              </div>
            ))}
          </ul>
        )}
      </section>
    </div>
  );
}
