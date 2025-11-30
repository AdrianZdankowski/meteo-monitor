import React, { useState, useEffect } from 'react';
import { ArrowUp, ArrowDown } from 'lucide-react';
import { getUnit } from '../utils/units';
import '../styles/DataTable.css';

const DataTable = ({ filters, sensors, readings }) => {
    const [localSort, setLocalSort] = useState({
        sortBy: 'timestamp',
        sortOrder: 'desc'
    });
    const [visibleCount, setVisibleCount] = useState(50);

    // Reset visible count when filters change
    useEffect(() => {
        setVisibleCount(50);
    }, [filters, readings]);

    const toggleSort = (field) => {
        setLocalSort(prev => ({
            sortBy: field,
            sortOrder: prev.sortBy === field && prev.sortOrder === 'asc' ? 'desc' : 'asc'
        }));
    };

    const handleShowMore = () => {
        setVisibleCount(prev => prev + 50);
    };

    const sortedReadings = readings.sort((a, b) => {
        const factor = localSort.sortOrder === 'asc' ? 1 : -1;
        if (localSort.sortBy === 'value') {
            return (a.value - b.value) * factor;
        }
        return (a.timestamp - b.timestamp) * factor;
    });

    const visibleReadings = sortedReadings.slice(0, visibleCount);

    return (
        <div>
            <div className="table-container">
                <table className="data-table">
                    <thead>
                        <tr>
                            <th>Sensor ID</th>
                            <th>Type</th>
                            <th onClick={() => toggleSort('value')} className="cursor-pointer">
                                <div className="sortable-header-content">
                                    Value
                                    {localSort.sortBy === 'value' && (localSort.sortOrder === 'asc' ? <ArrowUp size={16} /> : <ArrowDown size={16} />)}
                                </div>
                            </th>
                            <th onClick={() => toggleSort('timestamp')} className="cursor-pointer">
                                <div className="sortable-header-content">
                                    Timestamp
                                    {localSort.sortBy === 'timestamp' && (localSort.sortOrder === 'asc' ? <ArrowUp size={16} /> : <ArrowDown size={16} />)}
                                </div>
                            </th>
                        </tr>
                    </thead>
                    <tbody>
                        {visibleReadings.map((reading) => (
                            <tr key={reading.id || Math.random()}>
                                <td>{reading.sensorId}</td>
                                <td>{reading.sensorType}</td>
                                <td>{reading.value.toFixed(2)} {getUnit(reading.sensorType)}</td>
                                <td>{new Date(reading.timestamp * 1000).toLocaleString()}</td>
                            </tr>
                        ))}
                    </tbody>
                </table>
                {visibleCount < readings.length && (
                    <div style={{ display: 'flex', justifyContent: 'center', padding: '1rem' }}>
                        <button
                            onClick={handleShowMore}
                            className="btn btn-blue"
                            style={{ padding: '0.5rem 2rem', borderRadius: '20px' }}
                        >
                            Show More
                        </button>
                    </div>
                )}
            </div>
        </div>
    );
};

export default DataTable;
