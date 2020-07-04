import requests


unity_data_URL = 'http://127.0.0.1:8000/TrafficBot/unity'
action_URL = 'http://127.0.0.1:8000/TrafficBot/bot'


def send(act):
    act = {'bot_data': str(act)}
    requests.post(action_URL, data=act)


def receive():
    while True:
        data = requests.get(unity_data_URL)
        data = data.text

        try:
            data_list = data.split(',')
            index = int(data_list[0])
            observation = [float(data_list[i]) for i in range(1,index + 1)]
            revard = int(data_list[index + 1])
            done = bool(int(data_list[index + 2])) 
            info = {}
            
            return observation, revard, done, info

        except Exception:
            pass
