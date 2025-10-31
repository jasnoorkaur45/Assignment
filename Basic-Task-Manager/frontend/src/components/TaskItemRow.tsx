import React from 'react';
import type { TaskItem } from '@api/tasks';

type Props = {
  task: TaskItem;
  onToggle: (task: TaskItem) => void;
  onDelete: (id: string) => void;
};

export default function TaskItemRow({ task, onToggle, onDelete }: Props) {
  return (
    <li className="flex items-center justify-between gap-3 rounded border border-gray-200 bg-white px-4 py-3 shadow-sm">
      <label className="flex items-center gap-3">
        <input
          type="checkbox"
          className="size-4 accent-blue-600"
          checked={task.isCompleted}
          onChange={() => onToggle({ ...task, isCompleted: !task.isCompleted })}
          aria-label={`Toggle ${task.description}`}
        />
        <span
          className={`text-sm sm:text-base ${
            task.isCompleted ? 'line-through text-gray-400' : 'text-gray-800'
          }`}
        >
          {task.description}
        </span>
      </label>
      <button
        onClick={() => onDelete(task.id)}
        className="rounded bg-red-500 px-3 py-1.5 text-sm font-medium text-white hover:bg-red-600 active:bg-red-700"
        aria-label={`Delete ${task.description}`}
      >
        Delete
      </button>
    </li>
  );
}
