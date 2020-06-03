from typing import List, Tuple
from random import randint
import gym
import numpy as np
import pickle
import json, sys, os
from os import path
import argparse
from Network import net

class BinaryActionLinearPolicy(object):
    def __init__(self, theta):
        self.w = theta[:-1]
        self.b = theta[-1]
    def act(self, ob):
        y = ob.dot(self.w) + self.b
        a = int(y < 0)
        return a

def cem(f, th_mean, batch_size, n_iter, elite_frac, initial_std=1.0):
    """
    Generic implementation of the cross-entropy method for maximizing a black-box function
    Args:
        f: a function mapping from vector -> scalar
        th_mean (np.array): initial mean over input distribution
        batch_size (int): number of samples of theta to evaluate per batch
        n_iter (int): number of batches
        elite_frac (float): each batch, select this fraction of the top-performing samples
        initial_std (float): initial standard deviation over parameter vectors
    returns:
        A generator of dicts. Subsequent dicts correspond to iterations of CEM algorithm.
        The dicts contain the following values:
        'ys' :  numpy array with values of function evaluated at current population
        'ys_mean': mean value of function over current population
        'theta_mean': mean value of the parameter vector over current population
    """
    n_elite = int(np.round(batch_size*elite_frac))
    th_std = np.ones_like(th_mean) * initial_std

    for _ in range(n_iter):
        ths = np.array([th_mean + dth for dth in  th_std[None,:]*np.random.randn(batch_size, th_mean.size)])
        ys = np.array([f(th) for th in ths])
        elite_inds = ys.argsort()[::-1][:n_elite]
        elite_ths = ths[elite_inds]
        th_mean = elite_ths.mean(axis=0)
        th_std = elite_ths.std(axis=0)
        yield {'ys' : ys, 'theta_mean' : th_mean, 'y_mean' : ys.mean()}

def do_rollout(agent, env, num_steps, render=False):
    total_rew = 0
    ob = env.reset()
    for t in range(num_steps):
        a = agent.act(ob)
        (ob, reward, done, _info) = env.step(a)
        total_rew += reward
        if render and t%3==0: env.render()
        if done: break
    return total_rew, t+1

class TrafficEnvironment:

        def __init__(self):
            pass

        def reset(self):
            obseravation = np.asarray(net.reset())
            return obseravation

        def step(self, action: List[int]):
            observation, reward, done, info = net.step(action)
            observation = np.asarray(observation)
            #print(observation, reward, done, info)
            return observation, reward, done, info


if __name__ == '__main__':
    env = TrafficEnvironment()
    params = dict(n_iter=100, batch_size=25, elite_frac=0.2)
    num_steps = 200 * 10
    
    def noisy_evaluation(theta):
        agent = BinaryActionLinearPolicy(theta)
        rew, T = do_rollout(agent, env, num_steps)
        return rew

    for (i, iterdata) in enumerate(cem(noisy_evaluation, np.zeros(4 + 1), **params)):
        print('Iteration %2i. Episode mean reward: %7.3f'%(i, iterdata['y_mean']))
        agent = BinaryActionLinearPolicy(iterdata['theta_mean'])
