import socket
import time

def step(action):
    host, port = "127.0.0.1", 25001
    sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    sock.connect((host,port))

    if (type(action) == list):
        dataSend = str(action).strip('[]')
    elif (type(action) == str):
        dataSend = action
            
    sock.sendall(dataSend.encode("utf-8"))

    dataReceive = sock.recv(1024)

    stringData = dataReceive.decode("utf-8")
    stringDataList = stringData.split(',')
    print(stringDataList)

    index = int(stringDataList[0])

    observation = [float(stringDataList[i]) for i in range(1,index)]

    revard = int(stringDataList[index + 1])
    done = bool(int(stringDataList[index + 2]))
    
    info = {0:[float(stringDataList[i]) for i in range(index + 3,len(stringDataList))]}

    sock.close()
    time.sleep(0.5)
        
    return observation, revard, done, info

def send(sText):
    host, port = "127.0.0.1", 25001
    sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    sock.connect((host,port))

    if (type(sText) == str):
        dataSend = sText
        sock.sendall(dataSend.encode("utf-8"))
        sock.close()
    else:
        print("Not a string")
