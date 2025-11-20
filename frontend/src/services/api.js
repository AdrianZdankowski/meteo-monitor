import axios from 'axios';

const API_URL = 'http://localhost:5252/api';

const api = axios.create({
    baseURL: API_URL,
    headers: {
        'Content-Type': 'application/json',
    },
});

export const getSensors = async () => {
    const response = await api.get('/sensors');
    return response.data;
};

export const getReadings = async (filters = {}) => {
    const params = new URLSearchParams();
    if (filters.sensorId) params.append('sensorId', filters.sensorId);
    if (filters.sensorType) params.append('sensorType', filters.sensorType);
    if (filters.from) params.append('from', filters.from);
    if (filters.to) params.append('to', filters.to);
    if (filters.sort) params.append('sort', filters.sort);

    const response = await api.get(`/readings?${params.toString()}`);
    return response.data;
};

export const getDashboardData = async () => {
    const response = await api.get('/dashboard');
    return response.data;
};

export default api;
