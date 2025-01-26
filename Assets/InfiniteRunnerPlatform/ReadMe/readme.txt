Infinite Runner Platform Plugin is for UNITY_EDITOR_WIN, UNITY_EDITOR_OSX, UNITY_WEBPLAYER, UNITY_ANDROID, UNITY_IPHONE.

Infinite Runner Platform Plugin consists of mainly 3 sections:
    1. Controllers 
    2. Interfaces
    3. Services

Also some helper services are used for inner calculations & manipulation. 

1. Controllers are basically used to get inputs from unity inspector provided to the user.

2. Interfaces are helping in communication between two classes without creating objects. 

3. Services, with help of inherited interfaces are doing all the manipulation which are required for the output.

For example: We have "PlanePathController.cs" which passes all the inputs to "PlanePathService.cs" with the help of "IPlanePathService.cs" interface and are working respectively.


Infinite Runner Platform Plugin is licensed under Xeemu Studios. (c) copyright 2017, Xeemu Studios Co. / www.xeemu.com

Developed by Xeemu Studios Co. 
For Support Please contact "support@xeemu.com".
