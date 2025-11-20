from abc import ABC, abstractmethod
import time
import random


class Sensor(ABC):
    """Abstract base class for all sensors"""

    def __init__(self, sensor_id: str):
        """
        Args:
            sensor_id: Unique sensor identifier
        """
        self.sensor_id = sensor_id
        self.interval = None
        self.last_read_time = 0
        self._next_interval = self._random_interval()

    def _random_interval(self) -> float:
        return random.uniform(0.5, 1.5)

    @abstractmethod
    def read_value(self):
        """Abstract method to read value from sensor"""
        pass

    @abstractmethod
    def get_sensor_type(self) -> str:
        """Returns the sensor type"""
        pass

    def should_read(self) -> bool:
        """Checks if it's time to read. Uses either fixed `interval` or a randomized one."""
        current_time = time.time()
        if current_time - self.last_read_time >= self._next_interval:
            self.last_read_time = current_time
            self._next_interval = self.interval or self._random_interval()
            return True
        return False

    def get_data(self) -> dict:
        """Returns sensor data in dictionary format"""
        return {
            "sensor_id": self.sensor_id,
            "sensor_type": self.get_sensor_type(),
            "timestamp": time.time(),
            "value": self.read_value(),
        }

    def get_mqtt_topic(self) -> str:
        """Returns MQTT topic for the sensor"""
        return f"sensors/{self.get_sensor_type()}/{self.sensor_id}"
