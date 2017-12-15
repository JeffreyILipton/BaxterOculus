# cnn3d_grasping
Object grasping node with 3D CNN model

## Running
```
$ roslaunch cnn3d_grasping demo_grasping.launch
```
For Homunculus demo, 
```
$ roslaunch cnn3d_grasping demo_grasping.launch homunculus:=true
```


## Test

```
$ rosrun cnn3d_grasping test_srv.py
```
`test_srv.py` test all service calls and call `start` so that it can be used as a starting script.

To see the grasping confidence of Baxter, 
```
$ rostopic echo /demo_grasping/confidence
```
