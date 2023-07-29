# UnitySwift

A simple Unity plug-in for iOS designed to serve as a starting point for creating new plugins that bridge [Unity](http://unity3d.com/) and [Swift](https://swift.org/).

## Getting Started

Follow the instructions below to set up the plugin and integrate it into your Unity project.

### Prerequisites

* Unity 2019.x or later
* Xcode 11 or later

### Integration into Your Own Project

1. Clone or download this repository.
1. Open the Unity project where you want to integrate the Swift plugin.
1. Copy the entire `Assets/Plugins/iOS` folder from this repository into your Unity project's `Assets/Plugins` folder.

## Usage

### How to call Unity methods from Swift

To call Unity methods from Swift, use `UnitySendMessage` function like below:

```swift
// SwiftToUnity.swift
import Foundation

@objc public class SwiftToUnity: NSObject {
    @objc public static let shared = SwiftToUnity()

    /// Sends a "Hello World" message to the "Canvas" GameObject by calling the "OnMessageReceived" script method on that object with the "Hello World!" message.
    @objc public func swiftSendHelloWorldMessage() {
        // The UnitySendMessage function has three parameters: the name of the target GameObject, the script method to call on that object and the message string to pass to the called method.
        UnitySendMessage("Canvas", "OnMessageReceived", "Hello World!");
    }
}
```

See [SwiftToUnity.swift](Assets/Plugins/iOS/SwiftToUnity/Source/SwiftToUnity.swift).

### How to call Swift methods from Unity

#### **Step 1**: Create your Swift classes.

```swift
import Foundation

@objc public class SwiftToUnity: NSObject {
    @objc public static let shared = SwiftToUnity()

    /// Returns the "Hello, Swift!" string.
    ///
    /// - Returns: The "Hello, Swift!" string.
    @objc public func swiftHelloWorld() -> String {
        return "Hello, Swift!"
    }
}
```

#### **Step 2**: Create a `.mm` bridge file

```objc
// SwiftToUnityBridge.mm
#import <UnityFramework/UnityFramework-Swift.h>
#import "UnityInterface.h"

extern "C"
{
    char* cStringCopy(const char* string) {
        if (string == NULL) {
            return NULL;
        }

        size_t length = strlen(string) + 1;
        char* res = (char*) malloc(length);

        if (res != NULL) {
            memcpy(res, string, length);
        }

        return res;
    }

    char* cHelloWorld()
    {
        NSString *returnString = [[SwiftToUnity shared] swiftHelloWorld];
        return cStringCopy([returnString UTF8String]);
    }
}

```

#### **Step 3**: Add ``UnityFramework.modulemap``

```
framework module UnityFramework {
  umbrella header "UnityFramework.h"

  export *
  module * { export * }

  module UnityInterface {
      header "UnityInterface.h"

      export *
  }
}
```

#### **Step 4**: Add ``SwiftToUnityPostProcess.cs`` script

```csharp
#if UNITY_IOS
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

public static class SwiftToUnityPostProcess
{
    [PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string buildPath)
    {
        Debug.Log("OnPostProcessBuild: " + buildTarget);

        if (buildTarget == BuildTarget.iOS)
        {
            var projectPath = buildPath + "/Unity-iPhone.xcodeproj/project.pbxproj";

            var project = new PBXProject();
            project.ReadFromFile(projectPath);

            var unityFrameworkGuid = project.GetUnityFrameworkTargetGuid();

            // Modulemap
            project.AddBuildProperty(unityFrameworkGuid, "DEFINES_MODULE", "YES");

            var moduleFile = buildPath + "/UnityFramework/UnityFramework.modulemap";
            if (!File.Exists(moduleFile))
            {
                FileUtil.CopyFileOrDirectory("Assets/Plugins/iOS/SwiftToUnity/Source/UnityFramework.modulemap", moduleFile);
                project.AddFile(moduleFile, "UnityFramework/UnityFramework.modulemap");
                project.AddBuildProperty(unityFrameworkGuid, "MODULEMAP_FILE", "$(SRCROOT)/UnityFramework/UnityFramework.modulemap");
            }

            // Headers
            string unityInterfaceGuid = project.FindFileGuidByProjectPath("Classes/Unity/UnityInterface.h");
            project.AddPublicHeaderToBuild(unityFrameworkGuid, unityInterfaceGuid);

            string unityForwardDeclsGuid = project.FindFileGuidByProjectPath("Classes/Unity/UnityForwardDecls.h");
            project.AddPublicHeaderToBuild(unityFrameworkGuid, unityForwardDeclsGuid);

            string unityRenderingGuid = project.FindFileGuidByProjectPath("Classes/Unity/UnityRendering.h");
            project.AddPublicHeaderToBuild(unityFrameworkGuid, unityRenderingGuid);

            string unitySharedDeclsGuid = project.FindFileGuidByProjectPath("Classes/Unity/UnitySharedDecls.h");
            project.AddPublicHeaderToBuild(unityFrameworkGuid, unitySharedDeclsGuid);

            // Save project
            project.WriteToFile(projectPath);
        }

        Debug.Log("OnPostProcessBuild: Complete");
    }
}
#endif
```

Be sure to change the following line in ``SwiftPostProcess.cs`` script with the correct path to your `UnityFramework.modulemap` file:

```csharp
FileUtil.CopyFileOrDirectory("Assets/Plugins/iOS/SwiftToUnity/Source/UnityFramework.modulemap", moduleFile);
```

#### **Step 5**: Call your Swift methods from Unity

```csharp
using System.Runtime.InteropServices;
using UnityEngine;

public class CanvasScript : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern string cHelloWorld();

    private void Start()
    {
        cHelloWorld();
    }
}
```

## License

This project is licensed under the [MIT License](LICENSE).
