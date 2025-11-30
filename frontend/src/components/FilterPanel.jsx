import React from 'react';
import { Download } from 'lucide-react';
import '../styles/FilterPanel.css';

const FilterPanel = ({ filters, setFilters, sensors, readings }) => {
    const handleFilterChange = (e) => {
        setFilters({ ...filters, [e.target.name]: e.target.value });
    };

    const downloadData = (format) => {
        const dataToExport = readings.map(r => ({
            SensorId: r.sensorId,
            SensorType: r.sensorType,
            Value: r.value,
            Timestamp: new Date(r.timestamp * 1000).toISOString()
        }));

        if (format === 'json') {
            const blob = new Blob([JSON.stringify(dataToExport, null, 2)], { type: 'application/json' });
            const url = URL.createObjectURL(blob);
            const a = document.createElement('a');
            a.href = url;
            a.download = 'readings.json';
            a.click();
        } else if (format === 'csv') {
            const headers = ['SensorId', 'SensorType', 'Value', 'Timestamp'];
            const csvContent = [
                headers.join(','),
                ...dataToExport.map(row => headers.map(header => row[header]).join(','))
            ].join('\n');
            const blob = new Blob([csvContent], { type: 'text/csv' });
            const url = URL.createObjectURL(blob);
            const a = document.createElement('a');
            a.href = url;
            a.download = 'readings.csv';
            a.click();
        }
    };

    // Unique sensor types
    const sensorTypes = [...new Set(sensors.map(s => s.sensorType))];

    return (
        <div className="filter-panel">
            <div className="filter-group">
                <label>Sensor Type</label>
                <select
                    name="sensorType"
                    value={filters.sensorType}
                    onChange={(e) => setFilters({ ...filters, sensorType: e.target.value, sensorId: '' })}
                    className="filter-input"
                >
                    <option value="">All</option>
                    {sensorTypes.map(type => <option key={type} value={type}>{type}</option>)}
                </select>
            </div>
            <div className="filter-group">
                <label>Sensor</label>
                <select
                    name="sensorId"
                    value={filters.sensorId}
                    onChange={handleFilterChange}
                    className="filter-input"
                >
                    <option value="">All</option>
                    {sensors
                        .filter(s => !filters.sensorType || s.sensorType === filters.sensorType)
                        .map(s => (
                            <option key={s.sensorId} value={s.sensorId}>{s.sensorId}</option>
                        ))
                    }
                </select>
            </div>
            <div className="filter-group">
                <label>From</label>
                <input
                    type="datetime-local"
                    name="from"
                    value={filters.from}
                    onChange={handleFilterChange}
                    className="filter-input"
                />
            </div>
            <div className="filter-group">
                <label>To</label>
                <input
                    type="datetime-local"
                    name="to"
                    value={filters.to}
                    onChange={handleFilterChange}
                    className="filter-input"
                />
            </div>
            <div className="filter-actions">
                <button onClick={() => downloadData('csv')} className="btn btn-green" style={{ borderRadius: '12px', padding: '0.75rem 1.5rem', fontSize: '0.9rem' }}>
                    <Download size={16} style={{ marginRight: '8px' }} /> CSV
                </button>
                <button onClick={() => downloadData('json')} className="btn btn-blue" style={{ borderRadius: '12px', padding: '0.75rem 1.5rem', fontSize: '0.9rem' }}>
                    <Download size={16} style={{ marginRight: '8px' }} /> JSON
                </button>
            </div>
        </div>
    );
};

export default FilterPanel;
