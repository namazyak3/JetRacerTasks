from mlagents_envs.environment import UnityEnvironment
from mlagents_envs.envs.unity_gym_env import UnityToGymWrapper

unity_env = UnityEnvironment(
    file_name="Builds/Linux/JetRacerTasks.x86_64",
    base_port=5005,
    worker_id=0,
    seed=123,
    additional_args=["--no-graphics"]
)
env = UnityToGymWrapper(unity_env=unity_env, flatten_branched=True)

try:
    obs = env.reset()
    print(obs.shape)

except KeyboardInterrupt:
    env.close()
