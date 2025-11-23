import { useState } from 'react'
import './App.css'
import Dashboard from './components/Dashboard'
import DataTable from './components/DataTable'
import DataChart from './components/DataChart'
import TokenDashboard from './components/TokenDashboard'
import Navbar from './components/Navbar'

function App() {
  const [activeTab, setActiveTab] = useState('history');

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
            <section className="chart-section">
              <DataChart />
            </section>

            <section className="table-section">
              <DataTable />
            </section>
          </div>
        )}
      </main>
    </div>
  )
}

export default App
