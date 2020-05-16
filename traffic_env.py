from typing import List, Tuple
from random import randint
import importlib.util

class TrafficEnvironment:

        def __init__(self, 
                                vehicle_under_control_num: int = 1,
                                one_game_max_time: int = 100):
                self.one_game_max_time = one_game_max_time
                self.vehicle_under_control_num = vehicle_under_control_num

        def reset(self):
                pass
                # must reset game and environment variables and return initial observation

        def step(self, action: List[int]):
                observation, reward, done = net.step(action)
                info = {} # now is empty
                return observation, reward, done, info


class TrafficBot:

        def act(self, observation):
                # doesn't use observation now, just random speed
                vehicle_under_control_num = 1
                speed_change = [randint(0, 1) for i in range(vehicle_under_control_num)]
                return speed_change


if __name__ == '__main__':
        bot = TrafficBot()
        env = TrafficEnvironment()
        
        spec = importlib.util.spec_from_file_location("module.Net", "net.py")
        foo = importlib.util.module_from_spec(spec)
        spec.loader.exec_module(foo)
        net = foo.Net()
        
        for game_num in range(10):
                print('Game number: ', game_num)
                observation = env.reset()
                done = False
                while not done:
                        print(observation)
                        action = bot.act(observation)
                        print(action)
                        observation, reward, done, info = env.step(action)
                print("Game over, reward = ", reward)
