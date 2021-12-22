# CircularSeasExtrude

# Instructions.

Packages required.

* TwinCAT 3.1

* TF6420 TwinCAT 3 Database Server.

To execute project, be sure to select target device with Twincat 3 XAR Runtime.

Into CircularSeasExtruder Project/Real time, by default XAR use Target device with 4 cores, where Core3 was isolated and specific for Runtime. Adjust it at your convenience.

WARNING: This information about cores will be commited using git. Be sure to check that it is correct every pull action.

CircularSeasExtruder is prepared to run with specific Hardware with runtime into PC-dev using Ethernet RT driver. To run without hardware without problems, disable "Device 2 (EtherCAT). To use with hardware, be sure into "Device 2 (EtherCAT)" that "Device name" is your Ehernet RT Adapter.