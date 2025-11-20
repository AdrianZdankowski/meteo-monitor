from .sensor import Sensor
import random


class PressureSensor(Sensor):
    """Atmospheric pressure sensor - measurements in hPa"""

    def __init__(self, sensor_id: str, base_pressure: float = 1013.25):
        super().__init__(sensor_id)
        self.base_pressure = base_pressure
        self.current_pressure = base_pressure

    def read_value(self) -> float:
        """Simulates atmospheric pressure reading and returns numeric hPa"""
        change = random.uniform(-1, 1)
        self.current_pressure = self.current_pressure + change

        self.current_pressure = max(950, min(1050, self.current_pressure))

        return round(self.current_pressure, 2)

    def get_sensor_type(self) -> str:
        return "pressure"
