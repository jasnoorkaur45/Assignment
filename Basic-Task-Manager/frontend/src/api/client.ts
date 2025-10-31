import axios from 'axios';

const baseURL = (import.meta as any).env?.VITE_API_URL || 'http://localhost:5005';

export const api = axios.create({
  baseURL,
  headers: {
    'Content-Type': 'application/json'
  }
});
