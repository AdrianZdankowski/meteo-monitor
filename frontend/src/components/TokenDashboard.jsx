import React, { useState, useEffect } from 'react';
import { getSensors } from '../services/api';
import { startConnection } from '../services/signalr';
import { Coins, Wallet } from 'lucide-react';
import './TokenDashboard.css';

const TokenDashboard = () => {
    const [sensors, setSensors] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    const fetchSensors = async () => {
        try {
            const data = await getSensors();
            setSensors(data);
            setLoading(false);
            setError(null);
        } catch (err) {
            console.error("Failed to fetch sensors:", err);
            setError("Failed to load sensor data");
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchSensors();

        // Refresh every 10 seconds
        const interval = setInterval(fetchSensors, 10000);

        // Listen for real-time updates via SignalR
        const handleUpdate = (update) => {
            // Refresh sensor data when updates occur
            fetchSensors();
        };

        startConnection(handleUpdate);

        return () => clearInterval(interval);
    }, []);

    const totalTokens = sensors.reduce((sum, sensor) => sum + (sensor.tokenBalance || 0), 0);

    if (loading) {
        return (
            <div className="token-dashboard">
                <h2>Sensor Token Rewards</h2>
                <p>Loading...</p>
            </div>
        );
    }

    if (error) {
        return (
            <div className="token-dashboard">
                <h2>Sensor Token Rewards</h2>
                <p className="error-message">{error}</p>
            </div>
        );
    }

    return (
        <div className="token-dashboard">
            <div className="token-header">
                <h2><Coins className="header-icon" /> Sensor Token Rewards</h2>
                <div className="total-tokens">
                    <span className="total-label">Total Distributed:</span>
                    <span className="total-value">{totalTokens.toFixed(2)} SENS</span>
                </div>
            </div>

            <div className="table-container">
                <table>
                    <thead>
                        <tr>
                            <th>Sensor ID</th>
                            <th>Type</th>
                            <th>Wallet Address</th>
                            <th>Token Balance (SENS)</th>
                        </tr>
                    </thead>
                    <tbody>
                        {sensors.map((sensor) => (
                            <tr key={sensor.id}>
                                <td>{sensor.sensorId}</td>
                                <td><span className="sensor-type-badge">{sensor.sensorType}</span></td>
                                <td>
                                    <div className="wallet-address">
                                        <Wallet className="wallet-icon" />
                                        <span className="address-text">
                                            {sensor.walletAddress || 'Not assigned'}
                                        </span>
                                    </div>
                                </td>
                                <td className="balance-cell">
                                    <span className="balance-value">
                                        {sensor.tokenBalance ? sensor.tokenBalance.toFixed(2) : '0.00'}
                                    </span>
                                </td>
                            </tr>
                        ))}
                        {sensors.length === 0 && (
                            <tr>
                                <td colSpan="4" className="empty-message">
                                    No sensors registered yet
                                </td>
                            </tr>
                        )}
                    </tbody>
                </table>
            </div>
        </div>
    );
};

export default TokenDashboard;
