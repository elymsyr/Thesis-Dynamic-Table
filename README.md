# Dynamic Table

## Table of Contents

- [About](#about)
- [Usage](#usage)
  - [Prerequisites](#prerequisites)
  - [Training and Running Models](#train)
- [Files and Folders](#files)
- [Methods](#methods)
  - [Dynamic Table](#table)
  - [Unity ML-Agents](#mlagents)
  - [PPO Learning](#ppo)
  - [A* Path Finding](#astar)
- [Results and Observations](#results)

## <a name = "abstrach">About</a>
This study explores the application of deep reinforcement learning to the 'Dynamic Table' model, designed for efficient manipulation of objects. Reinforcement learning, a field that has received significant attention within machine learning and AI, enables an agent to learn optimal strategies for sequential decision-making tasks through trial and error. The Dynamic Table, trained using these advanced techniques, shows promise in various domains, including logistics, industrial production, gaming, andhome automation. The model's performance was thoroughly assessed, demonstratingrobustness in object placement and movement  capabilities within constrainedenvironments. The results underline the potential of the model to improve robotic manipulation tasks, laying a solid foundation for future research and practical implementations. Further studies could extend these findings to more complex real- world scenarios, emphasizing the practical applications of the Dynamic Table model in diverse fields.

## Usage <a name = "usage">Usage</a>

### <a name = "prerequisites">Prerequisites</a>
Download ML Agents package from Unity Package Manager.
Create a  conda (with python version 3.9.13) venv.
Install [requirements.txt](Docs/requirements.txt).
Upgrade and add packages to conda venv:
`pip install --upgrade setuptools pip wheel`
`pip install nvidia-pyindex`
`conda install cuda -c nvidia/label/cuda-11.8.0`
Download Cuda11.8 and Visual Studio (I use 2019. 2022 does not work.).
To test cuda, open cmd, type python, import torch, `print(torch.cuda.is_available())`, expect true
Test the mlagents by writing on cmd `mlagents-learn -h` after activating the environment.


### <a name = "train">Training and Running Models</a>

Open cmd and activate the venv. Type `mlagents-learn --run-id TestID`. Start training by pressing the Play button in the Unity Editor.
Or just press the Play button in the Unity Editor to use heuristics.

## <a name = "abstrach">Files and Folders</a>
 - [Thesis.pdf](Thesis.pdf): Thesis paper, including the references.
 - [Notes](/Notes/): Nots taken during the process.
 - [results](/results/): Training results including onnx models.


## <a name = "methods">Methods</a>
### <a name = "table">Dyanmic Table</a>
Dynamic Table has a structure consisting of different numbers of rectangular prism parts that can move up and down independently of each other.
### <a name = "mlagents">Unity ML-Agents</a>
ML-Agents can be considered one of the new researches in this matter. It is a toolkit that was first released in 2017. ML-Agents was released to create a suitable environment for researchers and developers to transform games and simulations into environments where intelligent agents can be trained with the help of machine learning algorithms.
### <a name = "ppo">PPO Learning</a>
Proximal policy optimization, or PPO for short, is a reinforcement learning algorithm used in agent training and is classified as a policy gradient method. The algorithm was introduced by OpenAI in 2017 and now can be considered state-of-the- art in Reinforcement Learning Algorithms, in OpenAI's words. The biggest reasonwhy the PPO algorithm has achieved such good performance is that the previous algorithms were not scalable, inefficient with data, or too complicated. Meanwhile, PPO design focuses on 3 things: simple implementation, sample efficiency, and stability.
### <a name = "astar">A* Path Finding</a>
A* is a graph traversal and path-finding algorithm which is used to find the shortest path in graph-based search problems. It searches for the path with the lowest total cost by calculating the cost to the current node called g cost and the estimatedcost from the current node to the destination called h cost.

## <a name = "results">Resulst and Observations</a>
A comparative analysis of the different model versions reveals key improvements and areas for further enhancement. The table above highlights the differences between versions in terms of performance and features. Notably, versions 078 and 090 demonstrate the highest test points, indicating superior performance interms of accuracy. However, the importance of version 090 among 078 is explainedwith the other key observations below. Observations There is a positive relationship between longer episode lengths and a higher number of completed episodes, which tends to result in improved performance measures. For example, versions that have episode lengths of approximately 50 andhave completed millions of episodes demonstrate strong performance. The presence of a configuration consisting of 64 units and 3 hidden layers is often observed in the high-performing versions, suggesting that it represents an optimal balance for the model's architecture. Comparing 090 and 078 Version 078 obtained a test score of around 98.7, surpassing all other tested versions. This indicates significant improvements in managing complex tasks usingthe dynamic table model. Version 090 demonstrated outstanding performance, achieving a test score of approximately 98. It consistently performed effectively across many setups. In contrast, to achieve the difference between the versions 078 and 090, It is important to note that the operational mechanisms of maze systems, with the exception of the 090 version, are as follows: The occurrences of these events are unpredictable, and their levels of difficulty fluctuate dynamically throughout the training process. As the model achieves greater success, its challenges multiply. Theincrease in complexity is accomplished by growing the quantity of randomly generated walls. Various iterations of mazes have been attempted, with detailed modifications to the difficulty level in each iteration. However, none of these attempts had successful outcomes. When comparing the 090 and 078 versions, the 078 version achieved a higher score and also facilitated faster product delivery. However, in contrast to the 090 version, the other versions lacked the use of an A* algorithm, which made theminsufficient in situations when there was an obstacle blocking the path between the product and the target. As a result, these versions rarely achieved a successful arrival of the product to the target. Despite scoring 88.5, even the 076 version failed to achieve adequate outcomes when trained for these scenarios. Among models trainedfor the maze system, it achieved the highest score of 45.6, second only to the 090 version. The primary factor that significantly influenced this score, beyond initial expectations, was an error in the adjustment of maze difficulty. Specifically, the difficulty wasn't increasing at an enough rapid rate, whereas the main issue was that it 17 decreased too swiftly. The 090 version successfully achieves the desired results inboth pre-prepared mazes and 