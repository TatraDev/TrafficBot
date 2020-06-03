import socket
import time

def step(action):
    host, port = "127.0.0.1", 25001
    sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    sock.connect((host,port))

    if (type(action) == list):
        send_data = str(action).strip('[]')
    elif (type(action) == str):
        send_data = action
    elif (type(action) == int):
        send_data = str(action)
            
    sock.sendall(send_data.encode("utf-8"))

    receive_data = sock.recv(1024)
    receive_data_list = receive_data.decode("utf-8").split(',')

    index = int(receive_data_list[0]) * 2
    observation = [float(receive_data_list[i]) for i in range(1,index + 1)]
    revard = int(receive_data_list[index + 1])
    done = bool(int(receive_data_list[index + 2])) 
    info = {}

    time.sleep(0.01)

    sock.close()
    
    return observation, revard, done, info

def reset():
    host, port = "127.0.0.1", 25001
    sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    sock.connect((host,port))

    send_data = "reset"
    sock.sendall(send_data.encode("utf-8"))

    receive_data = sock.recv(1024)
    receive_data_list = receive_data.decode("utf-8").split(',')

    time.sleep(0.01)
    
    index = int(receive_data_list[0]) * 2
    observation = [float(receive_data_list[i]) for i in range(1,index + 1)]

    sock.close()

    return observation
