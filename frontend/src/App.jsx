import { useState, useEffect } from 'react'
import './styles/App.css'
import Dashboard from './components/Dashboard'
import DataTable from './components/DataTable'
import DataChart from './components/DataChart'
import TokenDashboard from './components/TokenDashboard'
import Navbar from './components/Navbar'
import FilterPanel from './components/FilterPanel'
import { getSensors, getReadings } from './services/api'

function App() {
  const [activeTab, setActiveTab] = useState('history');
  const [sensors, setSensors] = useState([]);
  const [readings, setReadings] = useState([]);
  const [filters, setFilters] = useState({
    sensorId: '',
    sensorType: '',
    from: '',
    to: ''
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
      const apiFilters = { ...filters };
      if (filters.from) apiFilters.from = new Date(filters.from).getTime() / 1000;
      if (filters.to) apiFilters.to = new Date(filters.to).getTime() / 1000;

      const data = await getReadings(apiFilters);
      setReadings(data);
    };
    fetchReadings();
  }, [filters]);

  return (
    <div className="app-wrapper">
      <Navbar activeTab={activeTab} setActiveTab={setActiveTab} />

      <main className="main-content">
        {activeTab === 'dashboard' ? (
          <section className="dashboard-section">
            <Dashboard />
          </section>
        ) : activeTab === 'tokens' ? (
          <section className="tokens-section">
            <TokenDashboard />
          </section>
        ) : (
          <div className="history-view">
            <FilterPanel filters={filters} setFilters={setFilters} sensors={sensors} readings={readings} />

            <section className="chart-section">
              <DataChart filters={filters} sensors={sensors} readings={readings} />
            </section>

            <section className="table-section">
              <DataTable filters={filters} sensors={sensors} readings={readings} />
            </section>
          </div>
        )}
      </main>
    </div>
  )
}

export default App
