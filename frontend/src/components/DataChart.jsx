import React, { useState, useEffect } from 'react';
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer } from 'recharts';
import { getUnit } from '../utils/units';
import '../styles/DataChart.css';

const COLORS = ['#8884d8', '#82ca9d', '#ffc658', '#ff7300', '#0088fe', '#00c49f', '#ffbb28', '#ff8042', '#a4de6c', '#d0ed57'];

const DataChart = ({ filters, sensors, readings }) => {
    const [chartData, setChartData] = useState([]);
    const [activeSensors, setActiveSensors] = useState([]);

    useEffect(() => {
        // If no sensor type is selected, show the placeholder
        if (!filters.sensorType && !filters.sensorId) {
            setChartData([]);
            return;
        }

        const uniqueSensorIds = [...new Set(readings.map(r => r.sensorId))];
        setActiveSensors(uniqueSensorIds);

        const pivotMap = new Map();

        readings.forEach(r => {
            const key = r.timestamp;
            if (!pivotMap.has(key)) {
                pivotMap.set(key, {
                    timestamp: r.timestamp,
                    formattedTime: new Date(r.timestamp * 1000).toLocaleTimeString(),
                });
            }
            const entry = pivotMap.get(key);
            entry[r.sensorId] = r.value;
        });

        const pivotedData = Array.from(pivotMap.values()).sort((a, b) => a.timestamp - b.timestamp);

        if (!filters.from && !filters.to && pivotedData.length > 50) {
            setChartData(pivotedData.slice(-50));
        } else {
            setChartData(pivotedData);
        }

    }, [readings, filters]);

    if (!filters.sensorType && !filters.sensorId) {
        return (
            <div className="chart-container" style={{ display: 'flex', alignItems: 'center', justifyContent: 'center', minHeight: '400px' }}>
                <div style={{ textAlign: 'center', color: '#64748b' }}>
                    <h3 style={{ margin: 0, fontSize: '1.2rem', fontWeight: 600 }}>No Data Selected</h3>
                    <p style={{ margin: '0.5rem 0 0', opacity: 0.8 }}>Please select a Sensor Type or specific Sensor to view the chart.</p>
                </div>
            </div>
        );
    }

    return (
        <div className="chart-container" style={{ display: 'flex', flexDirection: 'column', alignItems: 'center', justifyContent: 'center', minHeight: '400px', padding: '20px' }}>
            <div style={{ width: '100%', height: '350px', display: 'flex', justifyContent: 'center' }}>
                <ResponsiveContainer width="95%" height="100%">
                    <LineChart data={chartData} margin={{ top: 20, right: 30, left: 20, bottom: 5 }}>
                        <CartesianGrid strokeDasharray="3 3" stroke="#475569" strokeOpacity={0.3} />
                        <XAxis
                            dataKey="formattedTime"
                            stroke="#475569"
                            fontSize={12}
                            tickLine={false}
                            axisLine={{ stroke: '#475569' }}
                            tick={{ fill: '#475569' }}
                        />
                        <YAxis
                            domain={['auto', 'auto']}
                            stroke="#475569"
                            fontSize={12}
                            tickLine={false}
                            axisLine={{ stroke: '#475569' }}
                            tick={{ fill: '#475569' }}
                        />
                        <Tooltip
                            contentStyle={{ backgroundColor: 'rgba(255, 255, 255, 0.95)', borderRadius: '12px', border: '1px solid #e2e8f0', boxShadow: '0 4px 12px rgba(0,0,0,0.1)', color: '#1e293b' }}
                            itemStyle={{ color: '#1e293b' }}
                            labelStyle={{ color: '#64748b', marginBottom: '0.5rem' }}
                        />
                        <Legend wrapperStyle={{ paddingTop: '20px', color: '#475569' }} />
                        {activeSensors.map((sensorId, index) => (
                            <Line
                                key={sensorId}
                                type="monotone"
                                dataKey={sensorId}
                                name={sensorId}
                                stroke={COLORS[index % COLORS.length]}
                                strokeWidth={3}
                                activeDot={{ r: 6, strokeWidth: 0 }}
                                dot={false}
                                connectNulls
                            />
                        ))}
                    </LineChart>
                </ResponsiveContainer>
            </div>
        </div>
    );
};

export default DataChart;
