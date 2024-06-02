b081_1 :
    Stage 1
    Maze - Off
    Set Difficulty - Off
    SETTINGS:
        num_envs = 5
        run_id = 'b081_1'
        env = 'b081_1'
        init_from = 'null'
        exe = 'build'
        env_path = f'/content/drive/MyDrive/builds/{env}/{exe}.x86_64'
        hidden_units = 64
        hidden_units_r = 64
        num_layers = 3
        num_layers_r = 3
        rate = 0.0009
        beta = 0.005
        epsilon = 0.2
        lambd = 0.95
        num_epoch = 3
        max_steps = 8000000
        checkpoint_interval = 800000

b081_2 :
    Stage 2
    Maze - On
    Set Difficulty - On
    MazeDifficulty - Minimum
    SETTINGS:
        num_envs = 5
        run_id = 'b081_2'
        env = 'b081_2'
        init_from = 'null' # continue from stage 1

b081_3
    Stage 3
    Set Difficulty - Off
    MazeDifficulty - Minimum
    SETTINGS:
        num_envs = 5
        run_id = 'b081_3'
        env = 'b081_3'
        init_from = 'null' # continue from stage 2

b081_4
    Stage 4
    Set Difficulty - Off
    MazeDifficulty - Avg/High
    SETTINGS:
        num_envs = 5
        run_id = 'b081_4'
        env = 'b081_4'
        init_from = 'null' # continue from stage 3



from jinja2 import Template

# Define the YAML template
yaml_template = """
default_settings: null
behaviors:
  Dynamic Table:
    trainer_type: ppo
    hyperparameters:
      batch_size: 1024
      buffer_size: 10240
      learning_rate: {{rate}}
      beta: {{beta}}
      epsilon: {{epsilon}}
      lambd: {{lambd}}
      num_epoch: {{num_epoch}}
      shared_critic: false
      learning_rate_schedule: linear
      beta_schedule: linear
      epsilon_schedule: linear
    checkpoint_interval: {{checkpoint_interval}}
    network_settings:
      normalize: false
      hidden_units: {{hidden_units}}
      num_layers: {{num_layers}}
      vis_encode_type: simple
      memory: null
      goal_conditioning_type: hyper
      deterministic: false
    reward_signals:
      extrinsic:
        gamma: 0.99
        strength: 1.0
        network_settings:
          normalize: false
          hidden_units: {{hidden_units_r}}
          num_layers: {{num_layers_r}}
          vis_encode_type: simple
          memory: null
          goal_conditioning_type: hyper
          deterministic: false
    init_path: {{init_from}}
    keep_checkpoints: 15
    even_checkpoints: false
    max_steps: {{max_steps}}
    time_horizon: 64
    summary_freq: 10000
    threaded: false
    self_play: null
    behavioral_cloning: null
env_settings:
  env_path: {{env_path}}
  env_args: null
  base_port: 5004
  num_envs: {{num_envs}}
  num_areas: 1
  timeout_wait: 60
  seed: -1
  max_lifetime_restarts: 10
  restarts_rate_limit_n: 1
  restarts_rate_limit_period_s: 60
engine_settings:
  width: 84
  height: 84
  quality_level: 5
  time_scale: 20.0
  target_frame_rate: -1
  capture_frame_rate: 60
  no_graphics: true
environment_parameters: null
checkpoint_settings:
  run_id: {{run_id}}
  initialize_from: null
  load_model: false
  resume: false
  force: false
  train_model: false
  inference: false
  results_dir: /content/drive/MyDrive/results
torch_settings:
  device: cuda
debug: false
"""

# Render the YAML template with variables
yaml_rendered = Template(yaml_template).render(
    run_id=run_id,
    init_from=init_from,
    beta=beta,
    hidden_units_r=hidden_units_r,
    num_layers_r=num_layers_r,
    rate=rate,
    epsilon=epsilon,
    lambd=lambd,
    num_epoch=num_epoch,
    hidden_units=hidden_units,
    num_layers=num_layers,
    env_path=env_path,
    num_envs=num_envs,
    max_steps=max_steps,
    checkpoint_interval=checkpoint_interval
)

# Write the rendered YAML to a file
with open(f"config.yaml", "w+") as yaml_file:
    yaml_file.write(yaml_rendered)    