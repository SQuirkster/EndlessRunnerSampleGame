# Unity Integration Steps
This section describes the steps needed to integrate the Netflix Unity SDK into the unity game app. The Netflix SDK is provided as a unity package.
The following steps need to be done by the game developer to integrate the netflix sdk package.

* Import the Netflix Unity SDK as the package. E.g. Assets -> Import Package -> Custom Package and then point to the Netflix Unity SDK package file. 
* Netflix Unity SDK uses unity jar resolver aka external dependency manager for managing the Android and iOS dependencies. The current Netflix SDK uses the version 1.2.164 of the EDM which is included with the binary drop of the Netflix SDK. This also needs to be imported similar to the Netflix SDK.
* Android min api is 26 and target sdk api is 31. This should be set under the File -> Build Settings -> Player Settings.
* iOS Deployment Target version is 15.0.
* Internet permission should also be enabled under the Player Settings.
* The Netflix SDK includes the game object NetflixGameObject which needs to be included in the game application’s scene to allow the Netflix SDK to initialize the native iOS and Android SDKs.
* There is a sample code available under Runtime/SampleCode/. It has a working scene called NetflixTestScene which can be dropped into a new Unity Project to get the functioning app
* Please follow additional steps provided via the other documents including Netflix Unity SDK: User Guide, NGP Launch Icon Spec.
