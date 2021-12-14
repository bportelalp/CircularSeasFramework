# This is a sample Python script.

# Press Mayús+F10 to execute it or replace it with your code.
# Press Double Shift to search everywhere for classes, files, tool windows, actions, and settings.
from paho.mqtt import client as mqtt_client
import random
import time
import BNO
import numpy as np
from scipy.fft import rfft, rfftfreq

broker = '192.168.0.237'
port = 1883
client_id = 'python-runtime-raspberry'
username = 'laboratoriorma3'
password = 'RicardoMarinA3'
topicsSub = [('sensor/activar', 0),
             ('printer/medir', 0)]
estado = 'espera'

# Variables para Fourier
N = 100
t = []
y = []
cuentaFFT = 0
TempoFFT = 5
Ts = 0.5
SAMPLE_RATE = int(1/Ts)

def connect_mqtt():

    #Función en la conexión
    def on_connect(client, userdata, flags, rc):
        if rc == 0:
            print("Conectado a broker!")
        else:
            print("Error de conexión con código %d\n", rc)

    # Instanciar cliente, credenciais e funcións
    cliente = mqtt_client.Client(client_id)
    cliente.username_pw_set(username, password)
    cliente.on_connect = on_connect
    cliente.on_message = on_message
    # Conectarse a Bróker
    cliente.connect(broker, port)
    # Subscribirse a tódolos topics
    cliente.subscribe(topicsSub)
    return cliente


def on_message(client, userdata, msg):
    global estado, y, t
    if msg.topic == 'printer/medir':
        print(msg.payload)
        if int(msg.payload) == 1:
            estado = 'midiendo'
        elif int(msg.payload) == 0:
            estado = 'espera'
            t = []
            y = []


def publish(client, sensor):

    while True:
        if estado == 'espera':
            time.sleep(Ts)
        elif estado == 'midiendo':
            try:
                medida = sensor.medir()
            except:
                sensor = BNO.IMU()

            if medida is None:
                pass
            else:
                print(sensor.medir())
                result = client.publish('sensor/yaw', medida[0])
                result = client.publish('sensor/pitch', medida[1])
                result = client.publish('sensor/roll', medida[2])
            time.sleep(Ts)
        elif estado == 'error':
            pass


def fourier(tiempo, var):
    global N, t, y, cuentaFFT, TempoFFT, Ts, SAMPLE_RATE
    if len(t) >= N:
        del t[0]
        del y[0]
        cuentaFFT += 1
    t.append(tiempo)
    y.append(var)

    if cuentaFFT >= int(TempoFFT / Ts):
        cuentaFFT = 0
        xf = rfftfreq(N, 1 / SAMPLE_RATE)
        yf = rfft(y, N)
        periodo = 1 / xf[np.argmax(np.abs(yf))]
        rango = max(y) - min(y)
        print(f'Periodo: {periodo}, Rango: {rango}')
        return periodo
    else:
        return None


# Press the green button in the gutter to run the script.
if __name__ == '__main__':
    sensor = BNO.IMU()
    client = connect_mqtt()
    client.loop_start()
    publish(client, sensor)
