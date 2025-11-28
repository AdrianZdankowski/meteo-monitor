import React, { useEffect, useState } from 'react';
import { getDashboardData } from '../services/api';
import { startConnection } from '../services/signalr';
import { Activity, Thermometer, Droplets, Wind } from 'lucide-react';
import { getUnit } from '../utils/units';
import '../styles/Dashboard.css';

const Dashboard = () => {
    const [data, setData] = useState([]);

    useEffect(() => {
        const fetchData = async () => {
            try {
                const result = await getDashboardData();
                setData(result);
            } catch (error) {
                console.error("Failed to fetch dashboard data", error);
            }
        };

        fetchData();

        const handleUpdate = (update) => {
            fetchData();
        };

        startConnection(handleUpdate);

        return () => {
        };
    }, []);

    const getIcon = (type) => {
        switch (type.toLowerCase()) {
            case 'temperature': return <Thermometer className="w-6 h-6 text-red-500" />;
            case 'humidity': return <Droplets className="w-6 h-6 text-blue-500" />;
            case 'wind': return <Wind className="w-6 h-6 text-gray-500" />;
            default: return <Activity className="w-6 h-6 text-green-500" />;
        }
    };

    return (
        <div className="dashboard-grid">
            {data.map((sensor) => (
                <div key={sensor.sensorId} className="sensor-card">
                    <div className="sensor-icon">
                        {getIcon(sensor.sensorType)}
                    </div>
                    <div className="sensor-info">
                        <h3>{sensor.sensorId}</h3>
                        <p className="sensor-type">{sensor.sensorType}</p>
                        <p className="sensor-value">
                            {sensor.lastValue.toFixed(2)}
                            <span style={{ fontSize: '0.6em', marginLeft: '4px', color: '#6b7280' }}>
                                {getUnit(sensor.sensorType)}
                            </span>
                        </p>
                        <p className="sensor-meta">Last: {new Date(sensor.lastTimestamp * 1000).toLocaleString()}</p>
                        <p className="sensor-meta">Avg (last 100): {sensor.averageLast100.toFixed(2)} {getUnit(sensor.sensorType)}</p>
                    </div>
                </div>
            ))}
        </div>
    );
};

export default Dashboard;
