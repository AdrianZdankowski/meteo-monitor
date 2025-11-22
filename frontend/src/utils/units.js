export const getUnit = (sensorType) => {
    if (!sensorType) return '';
    const type = sensorType.toLowerCase();
    if (type.includes('wind')) return 'm/s';
    if (type.includes('humidity')) return '%';
    if (type.includes('pressure')) return 'hPa';
    if (type.includes('temperature')) return '°C';
    return '';
};
