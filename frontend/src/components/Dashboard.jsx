import React, { useEffect, useState } from 'react';
import { getDashboardData } from '../services/api';
import { startConnection } from '../services/signalr';
import { Activity, Thermometer, Droplets, Wind } from 'lucide-react';

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
            setData(prevData => {
                const index = prevData.findIndex(d => d.sensorId === update.sensorId);
                if (index > -1) {
                    const newData = [...prevData];
                    // Update last value and re-calculate average (simplified for now, ideally backend sends new avg)
                    // For this requirement: "values refresh themselves after a new value is registered"
                    // We update the LastValue. The average might need a refresh or we can just keep it as is until page refresh
                    // or we can update it if we track the count. For now, let's just update LastValue.
                    newData[index] = {
                        ...newData[index],
                        lastValue: update.value,
                        lastTimestamp: update.timestamp
                    };
                    return newData;
                }
                return prevData;
            });
        };

        startConnection(handleUpdate);

        return () => {
            // Cleanup signalR if needed, but usually we keep it open for the app lifetime
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
                        <p className="sensor-value">{sensor.lastValue.toFixed(2)}</p>
                        <p className="sensor-meta">Last: {new Date(sensor.lastTimestamp * 1000).toLocaleString()}</p>
                        <p className="sensor-meta">Avg (last 100): {sensor.averageLast100.toFixed(2)}</p>
                    </div>
                </div>
            ))}
        </div>
    );
};

export default Dashboard;
