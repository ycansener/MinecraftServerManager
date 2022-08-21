# MinecraftServerManager
API + Bootstrap Service to manage Minecraft Server via Web API

# How to use?

## Before continue
- Start a Redis server to be used for PubSub messaging (tested via Redis Docker, worked like a charm)
- Update necessary fields in the appsettings.json files in both projects

## 1. Register MinecraftServerManager.Bootstrap as a Windows Service
- Build the MinecraftServerManager.Bootstrap project
- Open up a terminal
- Locate to the output folder (bin/debug/net6.0/...)
- Run `.\MinecraftServerManager.Bootstrap.exe install`
- Hopefully you will see a success message in the terminal.
- Open up the Services screen in the server and look for a service named as MinecraftServerManager.Bootstrap...

## 2. Publish the Web API project on IIS or wherever you want.
- Do not forget, it should be able to connect to the same Redis instance that your service will consume.

## 3. Enjoy!
