import socket
import time

class Net:
    def step(self, action):
        host, port = "127.0.0.1", 25001
        sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        sock.connect((host,port))

        dataSend = str(action).strip('[]')
        sock.sendall(dataSend.encode("utf-8"))

        dataReceive = sock.recv(1024)

        stringData = dataReceive.decode("utf-8")
        stringDataList = stringData.split(',')

        observation = [float(stringDataList[0]),float(stringDataList[1])]
        revard = int(stringDataList[2])
        done = bool(int(stringDataList[3]))

        sock.close()
        return observation, revard, done
