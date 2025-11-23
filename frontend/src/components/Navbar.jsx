import React from 'react';
import { LayoutDashboard, History, Coins } from 'lucide-react';

const Navbar = ({ activeTab, setActiveTab }) => {
    return (
        <nav className="navbar">
            <div className="navbar-brand">
                <h1>Meteo Monitor</h1>
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
