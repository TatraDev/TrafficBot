from websocket import create_connection
import time
ws = create_connection("ws://localhost:8000/")

def send(act):
    s = "AI"+act
    ws.send(s)

def receive():
    while(True):
        data = ws.recv()

        if(type(data) == bytes):
            data = data.decode("utf-8")
            
        try:
            data_list = data.split(',')
            index = int(data_list[0]) * 2
            observation = [float(data_list[i]) for i in range(1,index + 1)]
            revard = int(data_list[index + 1])
            done = bool(int(data_list[index + 2])) 
            info = {}
            
            return observation, revard, done, info
        except Exception:
            pass

