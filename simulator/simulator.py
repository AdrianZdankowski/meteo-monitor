import logging
import paho.mqtt.client as mqtt
import json
import time
import argparse
import json as _json
import threading
from sensors import TemperatureSensor, HumiditySensor, PressureSensor, WindSensor

logging.basicConfig(
    level=logging.INFO,
    format="%(asctime)s [%(levelname)s] %(message)s",
)


class WeatherStationSimulator:
    """Weather station simulator with multiple sensors"""

    def __init__(self, broker_host="localhost", broker_port=1883):
        self.broker_host = broker_host
        self.broker_port = broker_port
        self.sensors = []
        self.client = None
        self.running = False

    def on_connect(self, client, userdata, flags, rc, properties=None):
        """Callback invoked after connecting to MQTT broker"""
        if rc == 0:
            logging.info(
                "Connected to MQTT broker: %s:%s", self.broker_host, self.broker_port
            )
        else:
            logging.error("Failed to connect to MQTT broker. Code: %s", rc)

    def on_publish(self, client, userdata, mid, rc=None, properties=None):
        """Callback invoked after publishing a message"""
        pass

    def setup_sensors(self):
        """Configures 16 sensor instances with different intervals"""

        self.sensors.append(TemperatureSensor("TEMP_001", 22.0))
        self.sensors.append(TemperatureSensor("TEMP_002", 20.0))
        self.sensors.append(TemperatureSensor("TEMP_003", 23.0))
        self.sensors.append(TemperatureSensor("TEMP_004", 24.0))

        self.sensors.append(HumiditySensor("HUM_001", 55.0))
        self.sensors.append(HumiditySensor("HUM_002", 60.0))
        self.sensors.append(HumiditySensor("HUM_003", 65.0))
        self.sensors.append(HumiditySensor("HUM_004", 70.0))

        self.sensors.append(PressureSensor("PRESS_001", 1013.25))
        self.sensors.append(PressureSensor("PRESS_002", 1012.0))
        self.sensors.append(PressureSensor("PRESS_003", 1014.0))
        self.sensors.append(PressureSensor("PRESS_004", 1011.0))

        self.sensors.append(WindSensor("WIND_001", 5.0))
        self.sensors.append(WindSensor("WIND_002", 3.0))
        self.sensors.append(WindSensor("WIND_003", 4.0))
        self.sensors.append(WindSensor("WIND_004", 2.0))

        logging.info("Configured %s sensors:", len(self.sensors))
        for sensor in self.sensors:
            interval_display = (
                f"{sensor.interval}s"
                if sensor.interval is not None
                else "random(0.5-1.5s)"
            )
            logging.info(
                "  - %s (%s) (interval: %s)",
                sensor.sensor_id,
                sensor.get_sensor_type(),
                interval_display,
            )

    def connect_mqtt(self):
        """Establishes connection to MQTT broker"""
        self.client = mqtt.Client(
            mqtt.CallbackAPIVersion.VERSION2, client_id="weather_simulator"
        )
        self.client.on_connect = self.on_connect
        self.client.on_publish = self.on_publish

        try:
            logging.info(
                "Connecting to MQTT broker %s:%s...", self.broker_host, self.broker_port
            )
            self.client.connect(self.broker_host, self.broker_port, 60)
            self.client.loop_start()
            time.sleep(1)
            return True
        except Exception as e:
            logging.error("Cannot connect to MQTT broker: %s", e)
            logging.error("Make sure MQTT broker is running on localhost:1883")
            return False

    def publish_sensor_data(self, sensor):
        """Publishes sensor data to MQTT"""
        try:
            data = sensor.get_data()
            topic = sensor.get_mqtt_topic()
            payload = json.dumps(data)

            result = self.client.publish(topic, payload, qos=1)

            if result.rc == mqtt.MQTT_ERR_SUCCESS:
                logging.info("[%s] -> %s", sensor.sensor_id, topic)
                logging.info("Data: %s", json.dumps(data["value"], indent=2))
            else:
                logging.error("Error publishing data from %s", sensor.sensor_id)

        except Exception as e:
            logging.exception("Error during data publication: %s", e)

    def sensor_loop(self, sensor):
        """Loop for a single sensor"""
        while self.running:
            if sensor.should_read():
                self.publish_sensor_data(sensor)
            time.sleep(0.1)

    def start(self):
        """Starts the simulator"""
        logging.info("%s", "\n" + "=" * 60)
        logging.info("WEATHER STATION SIMULATOR")
        logging.info("%s", "=" * 60)

        self.setup_sensors()

        if not self.connect_mqtt():
            return

        self.running = True

        threads = []
        for sensor in self.sensors:
            thread = threading.Thread(
                target=self.sensor_loop, args=(sensor,), daemon=True
            )
            thread.start()
            threads.append(thread)

        logging.info("%s", "\n" + "=" * 60)
        logging.info("Simulator started! Press Ctrl+C to stop.")
        logging.info("%s", "=" * 60 + "\n")

        try:
            while True:
                time.sleep(1)
        except KeyboardInterrupt:
            logging.info("\n\nStopping simulator...")
            self.stop()

    def stop(self):
        """Stops the simulator"""
        self.running = False
        if self.client:
            self.client.loop_stop()
            self.client.disconnect()
        logging.info("Simulator stopped.")


def main():
    """Main function"""
    parser = argparse.ArgumentParser(description="Weather station simulator")
    parser.add_argument(
        "--single",
        action="store_true",
        help="Send single payload from one sensor and exit",
    )
    parser.add_argument(
        "--sensor",
        type=str,
        help="Sensor ID to use for single-send mode (e.g. TEMP_001)",
    )
    parser.add_argument(
        "--value",
        type=str,
        help="Value to send as JSON for single-send mode (e.g. '{\"temperature\":22}')",
    )
    args = parser.parse_args()

    simulator = WeatherStationSimulator()

    if args.single:
        if not args.sensor or args.value is None:
            parser.error("--single requires --sensor and --value")

        simulator.setup_sensors()

        selected = next(
            (s for s in simulator.sensors if s.sensor_id == args.sensor), None
        )
        if not selected:
            logging.error("Sensor '%s' not found. Available sensors:", args.sensor)
            for s in simulator.sensors:
                logging.info(" - %s (%s)", s.sensor_id, s.get_sensor_type())
            return

        try:
            payload_value = _json.loads(args.value)
        except Exception:
            payload_value = args.value

        payload = {
            "sensor_id": selected.sensor_id,
            "sensor_type": selected.get_sensor_type(),
            "timestamp": time.time(),
            "value": payload_value,
        }

        if not simulator.connect_mqtt():
            return

        topic = selected.get_mqtt_topic()
        sim_payload = _json.dumps(payload)
        result = simulator.client.publish(topic, sim_payload, qos=1)
        if result.rc == mqtt.MQTT_ERR_SUCCESS:
            logging.info("Published single payload to %s:", topic)
            logging.info("%s", _json.dumps(payload, indent=2))
        else:
            logging.error("Failed to publish single payload")

        simulator.stop()
        return

    simulator.start()


if __name__ == "__main__":
    main()
