from .sensor import Sensor
import random


class WindSensor(Sensor):
    """Wind sensor - provides only wind speed"""

    def __init__(self, sensor_id: str, base_speed: float = 3.0):
        super().__init__(sensor_id)
        self.base_speed = base_speed
        self.current_speed = base_speed

    def read_value(self) -> float:
        """Simulates wind speed reading and returns numeric speed"""
        speed_change = random.uniform(-2, 2)
        self.current_speed = max(0, self.current_speed + speed_change)
        self.current_speed = min(40, self.current_speed)

        return round(self.current_speed, 2)

    def get_sensor_type(self) -> str:
        return "wind"
