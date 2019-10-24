This package is used to support controllers(Nreal Light or Nreal Phone Controller) in the original way. It matches with the NrealSDKForUnity(After 1.0 version).

Why need this package?
If you're using the old android system we released, the controller will not run correctly. Because the controller service has totally changed. So you would need this package to run controller in the original way.

How to use?
First make sure your project already contains the NrealSDKForUnity sdk. Then import this package to your project. Find the "ReplaceChontrollerProvider" prefab, and drag it into your scene which contains a "NRInput" prefab, and save the scene. Now the controller would be supported in the original way by the android system.

How does it work?
The "ReplaceChontrollerProvider" prefab would check if the current android system is old version, if true,  it would make sdk to use the original way to update controller data. If false, it would do nothing.