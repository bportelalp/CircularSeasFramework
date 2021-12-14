# Importar librerías
import adafruit_bno055
import serial
import time


# Clase de inicialización y manejo del dispositivo BNO055 de adafruit conectado por I2C
class IMU:
    def __init__(self):
        """Configura el puerto UART para usar con el BNO055"""
        while True:
            try:
                self.uart = serial.Serial("/dev/serial0")
                self.sensor = adafruit_bno055.BNO055_UART(self.uart)
                break
            except RuntimeError as runtime:
                """En ocasiones salta error porque el dispositivo está ocupado, entonces
                se le da tiempo para obtener los datos"""
                self.uart.close()
                print(f'Fallo de ejecución {runtime.args}. BUS_OVER_RUN_ERROR, espere\n')
                time.sleep(1)
            except OSError as ComAccess:
                print(f'No se reconoce {ComAccess.args}')

    def statuscalibracion(self):
        """buleano indicando si el dispositivo está calibrado"""
        return self.sensor.calibration_status

    def medir(self):
        return self.sensor.euler


    def asistentecalibracion(self):
        """Guiado para calibrar"""
        print(f'Estado de calibración (sys, gyro, acel, mag): {self.statuscalibracion()}')
        if self.statuscalibracion() == (3, 3, 3, 3):
            return True
        else:
            return False
