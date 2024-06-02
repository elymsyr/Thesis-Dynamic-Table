# # b075 Wall Creation Diffuculty Function


# import matplotlib.pyplot as plt
# from random import randint
# from math import sqrt

# def get_random():
#     return randint(200,600)

# def function(value,multiplier,hardness = 40):
#     if randint(0,100) < hardness and value > 0.3: # unsuccesful env creation situations
#         value = (pow(value,multiplier) * value) * 0.77
#     else:
#         value = (pow(value,multiplier) * value) + (multiplier*0.6)
#     return value

# def function2(value,hardness):
#     if randint(0,100) > randint(10,int(hardness)):
#         value = value - value*value*0.000001
#         hardness -= .1
#         if hardness < 11:
#             hardness = 11
#     else:
#         value = value + value*value*0.000001
#         hardness += .1
#     return value,hardness

# def create_xy(hardness, value):
#     multiplier = 0.0001
#     total_steps = 0
#     step_count = 10000000
    
#     x = []
#     y = []
#     game = 0
#     while step_count > 0:
#         game+=1
#         steps = get_random()
#         value = function(value,multiplier,hardness)
#         step_count -= steps
#         total_steps += steps
#         x.append(total_steps)
#         y.append(value)
#     print(game)
#     return x,y

# def create_freq(hardness, value=1000):
#     total_steps = 0
#     step_count = 10000000
    
#     x = []
#     y = []
#     game = 0
#     while step_count > 0:
#         game+=1
#         steps = get_random()
#         value,hardness = function2(value,hardness)
#         step_count -= steps
#         total_steps += steps
#         x.append(total_steps)
#         y.append(value)
#     print(game)
#     return x,y



# # x1,y1 = create_xy(10,0.1)
# # x2,y2 = create_xy(30,0.01)
# # x3,y3 = create_xy(50,0.005)
# x1,y1 = create_freq(20)
# x2,y2 = create_freq(50)
# x3,y3 = create_freq(80)

# plt.ylim(0,2000)
# plt.xlim(0,10000000)
# plt.plot(x1, y1)
# plt.plot(x2, y2)
# plt.plot(x3, y3)

# plt.xlabel('Steps')
# plt.ylabel('value')
# plt.title('Create Wall Possibilty')
# plt.show()

# -----------------------------------------------------------------------------------------

# import matplotlib.pyplot as plt
# from random import randint
# m=0.0001
# d=0.09
# X=[]
# Y=[]
# def increase(n):
#   newn=(n/m)**m * n *1.05
#   return newn
# def decrease(n):
#   newn=n**m * n * 0.99
#   return newn
# for n in range(1000):
#   X.append(n)
#   Y.append(d)
#   if randint(0,100) > 20:
#     d=increase(d)
#   else:
#     d=decrease(d)
#   if d > 0.6:
#     d=0.6
# plt.plot(X, Y)
# plt.show()

# -----------------------------------------------------------------------------------------

import matplotlib.pyplot as plt
from random import randint
X=[]
Y=[]
n = 0.05
def decrease(n):
  newn = n + n * 0.05
  return newn
for d in range(5000):
  X.append(d)
  Y.append(n)
  n = decrease(n)
plt.plot(X, Y)
plt.show()