# CircularSeas Extruder

Program repository in TwinCAT 3 for 3D printing filament extruder. Version 3.

## Previous versions.

* Extruder v1: Version made with a drill as extruder, controlled by Arduino. Extruder heater are made of nichrome thread.

* Extruder v2: Version made with a custom machined extruder. Controlled by Arduino and code in repository on LaboratorioRMA3 project TFG-TFM. This prototype has several problems related to temperature control and motion. Stepper motor has low power to achieve the extrusion and K Thermocouples shows an unstable measurement.

## About this prototype

This tries to improve the prototype v2, using a servo motor with a reducer to extrude and PT100 temperature sensors with industrial reading cards. It will be built with Beckhoff TwinCAT 3, including HMI in the development platform itself.