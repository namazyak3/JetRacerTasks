import sys
from typing import Optional

from mlagents_envs.environment import UnityEnvironment
from mlagents_envs.envs.unity_gym_env import UnityToGymWrapper

import gymnasium as gym

from sample_factory.cfg.arguments import parse_full_cfg, parse_sf_args
from sample_factory.envs.env_utils import register_env
from sample_factory.train import run_rl

def make_env_func(full_env_name, cfg=None, env_config=None, render_mode: Optional[str] = None):
    unity_env = UnityEnvironment(file_name="Builds/Linux/Shape8-144.x86_64", no_graphics=True)
    gym_env = UnityToGymWrapper(unity_env=unity_env, flatten_branched=True)
    return gym_env

def parse_custom_args(argv=None, evaluation=False):
    parser, cfg = parse_sf_args(argv=argv, evaluation=False)
    cfg = parse_full_cfg(parser=parser, argv=argv)
    return cfg

def main():
    register_env("Unity/Shape8-144", make_env_func=make_env_func, no_graphics=True)
    cfg = parse_custom_args()
    status = run_rl(cfg)
    return status

if __name__ == "__main__":
    sys.exit(main())
