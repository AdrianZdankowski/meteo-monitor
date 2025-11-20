import * as signalR from '@microsoft/signalr';

const HUB_URL = 'http://localhost:5252/dashboardHub';

let connection = null;

export const startConnection = async (onUpdate) => {
    connection = new signalR.HubConnectionBuilder()
        .withUrl(HUB_URL)
        .withAutomaticReconnect()
        .build();

    connection.on('ReceiveSensorUpdate', (sensorId, value, timestamp) => {
        onUpdate({ sensorId, value, timestamp });
    });

    try {
        await connection.start();
        console.log('SignalR Connected');
    } catch (err) {
        console.error('SignalR Connection Error: ', err);
        setTimeout(() => startConnection(onUpdate), 5000);
    }
};

export const stopConnection = async () => {
    if (connection) {
        await connection.stop();
    }
};
