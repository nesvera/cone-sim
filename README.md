### Cone-SIM

A simple self-driving car simulator built with Unity.

  - Easy to create new tracks
  - Save training data in txt files
  - Udp interface with autonomous mode

<p align="center">
<img src="images/pic_5.PNG" width="450" height="270">
</p>

### Precompiled builds

V0 - 1/19/18
  - [Linux](https://drive.google.com/open?id=1H92uKw3k1OTCE58SPFoQ247ly4FyhGGC)
  - [Windows](https://drive.google.com/open?id=13OlOGZzfnkzZZPYdxJ5oKztiSti_O4Sq)

### Training Mode

<p align="center">
<img src="images/pic_2.PNG" width="450" height="270">
</p>

In Table 1. is shown the data that is store which frame

Table 1

| Index | Data |
| ------ | ------ |
| 0 | Recording time |
| 1 | Throttle input |
| 2 | Brake input |
| 3 | Steering input |
| 4 | Handbrake input |
| 5 | Car speed |
| 6 | Acceleration x |
| 7 | Acceleration z |
| 8 | Position x |
| 9 | Position z |
|10 to 45 | 36 raycasts distance |

P.S.:
  - From the image below z is the vertical axis, and x is the horizontal axis
  - Raycast[0] is in the right side (90º) of the car

<p align="center">
<img src="images/pic_4.PNG" width="450" height="270">
</p>

## Autonomous Mode

Which frame a string is send throgh udp. This string contais the same data shown in Table 1.
The game waits for a csv string with the data shown in Table 2.

Table 2

| Index | Data |
| ------ | ------ |
| 0 | Throttle input |
| 1 | Brake input |
| 2 | Steering input |
| 3 | Handbrake input |

### Track Editor

<p align="center">
<img src="images/pic_3.PNG" width="450" height="270">
</p>

### To-do List
There is many things to be done
  - Improve joystick and racing wheel interface
  - Reinforcement Mode


Some bugs may apper!

