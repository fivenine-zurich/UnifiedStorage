# Unified Storage

Unified Storage provides a unified file IO API for various platforms.
It is based on the excellent [PCLStorage](https://github.com/dsplaisted/PCLStorage) library. UnifiedStorage tries to make testing your applications 
easier by providing interfaces for all classes and not using static factories.

## Supported Platforms
Currently the following platforms are supported:
 * Xamarin iOS (Universal API)
 * Xamarin Android
 * Windows Desktop (Classic .NET System.IO)
 * Windows 8.1 (Windows Store)
 * Windows Phone 8.1

## Usage
Make sure you install the NuGet package in every target platform project.

The starting point for using the UnifiedStorage library is the respective `IFileSystem` implementation. 
The `IFileSystem` enables retrieval of `IFile` and `IDirectory` objects which are used for working with the unified filesystem. 
If you are using a DI-Container in your project you may want to register the `IFileSystem` and its apropriate implementation in the container.

