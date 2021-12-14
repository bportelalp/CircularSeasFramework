# Onboard Manufacturing

Files related to Ultimaker 2+ TestBench using Corrective algorithm with Octoprint and Node-RED

Python files is executed on Raspberry pi 4 using Adafruit-BNO055 Sensor as IMU for measurements.

## Notes

Source python files requires the following dependencies

* Numpy

* Scipy

* paho-mqtt

* Adafruit-BNO055


# Folders

1. CorrectiveAlgorithm. Python source code to run into Raspberry Pi.

2. Nodered testbench dashboard. Node-RED flow to read and graph measurements and take control decisions.

3. UR5 Wave Simulator. Scripts for UR5 simulating oceanic disturbances. Not sure if it is the latest version.