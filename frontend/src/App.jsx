import { useState } from 'react'
import './App.css'
import Dashboard from './components/Dashboard'
import DataTable from './components/DataTable'
import DataChart from './components/DataChart'

function App() {
  return (
    <div className="container">
      <header className="header">
        <h1>Meteo Monitor</h1>
      </header>

      <main>
        <section>
          <Dashboard />
        </section>

        <section style={{ marginTop: '2rem' }}>
          <DataChart />
        </section>

        <section style={{ marginTop: '2rem' }}>
          <DataTable />
        </section>
      </main>
    </div>
  )
}

export default App
