import React, { useState, useEffect } from 'react';
import { getReadings, getSensors } from '../services/api';
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer } from 'recharts';

const DataChart = () => {
    const [readings, setReadings] = useState([]);
    const [sensors, setSensors] = useState([]);
    const [selectedSensorId, setSelectedSensorId] = useState('');

    useEffect(() => {
        const fetchSensors = async () => {
            const data = await getSensors();
            setSensors(data);
            if (data.length > 0) setSelectedSensorId(data[0].sensorId);
        };
        fetchSensors();
    }, []);

    useEffect(() => {
        if (!selectedSensorId) return;

        const fetchReadings = async () => {
            // Get last 50 readings for the chart
            const data = await getReadings({ sensorId: selectedSensorId, sort: 'asc' }); // We want asc for chart
            // API default is desc, so we might need to reverse or request asc.
            // Let's assume we request 'asc' or just reverse client side.
            // Actually my API impl supports sort.

            // Wait, my API impl for getReadings defaults to desc.
            // Let's pass sort: 'asc' to getReadings.
            // But wait, getReadings in api.js passes sort param.
            // And ReadingsController handles it.
            // So I should pass sort: 'asc'.

            // However, if I want the *latest* 50 readings but in chronological order,
            // I should get them desc (latest first), take 50, then reverse them.
            // Because if I get asc, I get the *oldest* readings.

            const latestData = await getReadings({ sensorId: selectedSensorId, sort: 'desc' });
            const chartData = latestData.slice(0, 50).reverse().map(r => ({
                ...r,
                formattedTime: new Date(r.timestamp * 1000).toLocaleTimeString()
            }));
            setReadings(chartData);
        };
        fetchReadings();
    }, [selectedSensorId]);

    return (
        <div className="chart-container">
            <div className="control-group" style={{ marginBottom: '1rem' }}>
                <label>Select Sensor for Graph</label>
                <select
                    value={selectedSensorId}
                    onChange={(e) => setSelectedSensorId(e.target.value)}
                    className="control-input"
                    style={{ maxWidth: '300px' }}
                >
                    {sensors.map(s => (
                        <option key={s.sensorId} value={s.sensorId}>{s.sensorId} ({s.sensorType})</option>
                    ))}
                </select>
            </div>

            <div style={{ width: '100%', height: '300px' }}>
                <ResponsiveContainer width="100%" height="100%">
                    <LineChart data={readings}>
                        <CartesianGrid strokeDasharray="3 3" />
                        <XAxis dataKey="formattedTime" />
                        <YAxis />
                        <Tooltip />
                        <Legend />
                        <Line type="monotone" dataKey="value" stroke="#8884d8" activeDot={{ r: 8 }} />
                    </LineChart>
                </ResponsiveContainer>
            </div>
        </div>
    );
};

export default DataChart;
