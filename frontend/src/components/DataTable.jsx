import React, { useState, useEffect } from 'react';
import { getReadings, getSensors } from '../services/api';
import { ArrowUp, ArrowDown, Download } from 'lucide-react';

const DataTable = () => {
    const [readings, setReadings] = useState([]);
    const [sensors, setSensors] = useState([]);
    const [filters, setFilters] = useState({
        sensorId: '',
        sensorType: '',
        from: '',
        to: '',
        sort: 'desc'
    });

    useEffect(() => {
        const fetchSensors = async () => {
            const data = await getSensors();
            setSensors(data);
        };
        fetchSensors();
    }, []);

    useEffect(() => {
        const fetchReadings = async () => {
            // Convert dates to unix timestamp if present
            const apiFilters = { ...filters };
            if (filters.from) apiFilters.from = new Date(filters.from).getTime() / 1000;
            if (filters.to) apiFilters.to = new Date(filters.to).getTime() / 1000;

            const data = await getReadings(apiFilters);
            setReadings(data);
        };
        fetchReadings();
    }, [filters]);

    const handleFilterChange = (e) => {
        setFilters({ ...filters, [e.target.name]: e.target.value });
    };

    const toggleSort = () => {
        setFilters(prev => ({ ...prev, sort: prev.sort === 'asc' ? 'desc' : 'asc' }));
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
        <div>
            <div className="controls">
                <div className="control-group">
                    <label>Sensor Type</label>
                    <select name="sensorType" value={filters.sensorType} onChange={handleFilterChange} className="control-input">
                        <option value="">All</option>
                        {sensorTypes.map(type => <option key={type} value={type}>{type}</option>)}
                    </select>
                </div>
                <div className="control-group">
                    <label>Sensor ID</label>
                    <select name="sensorId" value={filters.sensorId} onChange={handleFilterChange} className="control-input">
                        <option value="">All</option>
                        {sensors.filter(s => !filters.sensorType || s.sensorType === filters.sensorType).map(s => (
                            <option key={s.sensorId} value={s.sensorId}>{s.sensorId}</option>
                        ))}
                    </select>
                </div>
                <div className="control-group">
                    <label>From</label>
                    <input type="datetime-local" name="from" value={filters.from} onChange={handleFilterChange} className="control-input" />
                </div>
                <div className="control-group">
                    <label>To</label>
                    <input type="datetime-local" name="to" value={filters.to} onChange={handleFilterChange} className="control-input" />
                </div>
                <div className="button-group">
                    <button onClick={() => downloadData('csv')} className="btn btn-green">
                        <Download size={16} style={{ marginRight: '8px' }} /> CSV
                    </button>
                    <button onClick={() => downloadData('json')} className="btn btn-blue">
                        <Download size={16} style={{ marginRight: '8px' }} /> JSON
                    </button>
                </div>
            </div>

            <div className="table-container">
                <table className="data-table">
                    <thead>
                        <tr>
                            <th>Sensor ID</th>
                            <th>Type</th>
                            <th>Value</th>
                            <th onClick={toggleSort} className="sortable-header">
                                Timestamp
                                {filters.sort === 'asc' ? <ArrowUp size={16} /> : <ArrowDown size={16} />}
                            </th>
                        </tr>
                    </thead>
                    <tbody>
                        {readings.map((reading) => (
                            <tr key={reading.id || Math.random()}>
                                <td>{reading.sensorId}</td>
                                <td>{reading.sensorType}</td>
                                <td>{reading.value.toFixed(2)}</td>
                                <td>{new Date(reading.timestamp * 1000).toLocaleString()}</td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>
        </div>
    );
};

export default DataTable;
