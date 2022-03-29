# CircularSeas Framework

CircularSeas Framework is an integrated service for manage recycled materials and use it with a cloud slicer based on Prusa Slicer

## Installation

Clone repository and open src/CircularSeasFramework/CircularSeasFramework.sln with Visual Studio 2022

## Usage

On the solution, there are three main executable projects


### Cloud Server
It is a Blazor WebAssembly hosted application, including REST API endpoints for user app.

### CircularSeas Manager Android
It is the Android version of CircularSeasManager. For deployment, adjust output for Release mode and right click over CircularSeasmanager.Android --> Archive and distribute as an Ad Hoc app (apk will be generate)

### CircularSeas Manager UWP
It is the Windows version of CircularSeasManager. For deployment, adjust output for Release mode and right click over CircularSeasManager.UWP/Publish/Generate application packages and test installation. Some Temporary key will be required so create one. It doesn't matter if it isn't certified by a trusted authority.