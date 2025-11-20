from .sensor import Sensor
import random


class TemperatureSensor(Sensor):
    """Temperature sensor - measurements in degrees Celsius"""

    def __init__(self, sensor_id: str, base_temp: float = 20.0):
        super().__init__(sensor_id)
        self.base_temp = base_temp
        self.current_temp = base_temp

    def read_value(self) -> dict:
        """Simulates temperature reading with small fluctuations"""
        trend = random.uniform(-0.5, 0.5)
        noise = random.uniform(-0.2, 0.2)
        self.current_temp = self.current_temp + trend + noise

        self.current_temp = max(-30, min(45, self.current_temp))
        return round(self.current_temp, 2)

    def get_sensor_type(self) -> str:
        return "temperature"
