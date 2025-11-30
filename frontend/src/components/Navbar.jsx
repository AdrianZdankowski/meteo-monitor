import React from 'react';
import { LayoutDashboard, History, Coins, CloudSun } from 'lucide-react';
import '../styles/Navbar.css';

const Navbar = ({ activeTab, setActiveTab }) => {
    return (
        <nav className="navbar">
            <div className="navbar-brand" style={{ display: 'flex', alignItems: 'center', gap: '0.75rem' }}>
                <CloudSun size={32} color="#3b82f6" />
                <h1 style={{ background: 'none', WebkitTextFillColor: 'initial', color: '#1e293b', fontSize: '1.5rem' }}>Meteo Monitor</h1>
            </div>
            <div className="navbar-menu">
                <button
                    className={`nav-item ${activeTab === 'history' ? 'active' : ''}`}
                    onClick={() => setActiveTab('history')}
                >
                    <History size={20} />
                    <span>Historical Data</span>
                </button>
                <button
                    className={`nav-item ${activeTab === 'dashboard' ? 'active' : ''}`}
                    onClick={() => setActiveTab('dashboard')}
                >
                    <LayoutDashboard size={20} />
                    <span>Live Dashboard</span>
                </button>
                <button
                    className={`nav-item ${activeTab === 'tokens' ? 'active' : ''}`}
                    onClick={() => setActiveTab('tokens')}
                >
                    <Coins size={20} />
                    <span>Token Rewards</span>
                </button>
            </div>
        </nav>
    );
};

export default Navbar;
