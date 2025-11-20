from .sensor import Sensor
import random


class HumiditySensor(Sensor):
    """Humidity sensor - measurements in percent"""

    def __init__(self, sensor_id: str, base_humidity: float = 60.0):
        super().__init__(sensor_id)
        self.base_humidity = base_humidity
        self.current_humidity = base_humidity

    def read_value(self) -> float:
        """Simulates humidity reading and returns numeric percent"""
        change = random.uniform(-2, 2)
        self.current_humidity = self.current_humidity + change

        self.current_humidity = max(0, min(100, self.current_humidity))

        return round(self.current_humidity, 2)

    def get_sensor_type(self) -> str:
        return "humidity"
