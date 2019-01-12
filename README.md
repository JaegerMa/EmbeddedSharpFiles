# EmbeddedSharpFiles

EmbeddedSharpFiles is a .NET Standard 2.0 compatible library to manage embedded files in loaded assemblies.

## Usage
**Note**: EmbeddedSharpFiles is not able to automatically read all embedded files

First, we have to create an `EmbeddedFile` instance referencing our embedded file. We have to pass both the containing assembly and the namespace. As both are contained in the `Type` class you can also pass a `Type` instance
```csharp
var embeddedFile = new EmbeddedFile("myembedded-file-name.bin", typeof(SomeClassInTheNamespace));
```

If the resource lies in a subfolder, the folder is part of the name:
```csharp
var embeddedFile = new EmbeddedFile("myResourceFolder.myembedded-file-name.bin", typeof(SomeClassInTheNamespace));
```


Now this instance can be used to either get the stream to the content, the string contained in the embedded file, or it can directly be extracted to a certain file.
The content string is created using UTF-8.
```csharp
using(var contentStream = embeddedFile.ContentStream)
{
	//...
}
```
```csharp
var content = embeddedFile.ContentString;
```
```csharp
embeddedFile.ExtractTo(@"C:\Foo\Bar\myextractedfile.bin");

//If no file name is stored in the EmbeddedFile instance and no
//file name is passed to .ExtractTo, the resource name is used.
embeddedFile.ExtractToDirectory(@"C:\Foo\Bar"); // -> Extracts to C:\Foo\Bar\myebedded-file-name.bin

//While .ExtractTo and .ExtractToDirectory will throw an exception if something goes wrong,
//.TryExtractTo and .TryExtractToDirectory will catch exceptions and return a bool indicating
//whether extraction was successfull
embeddedFile.TryExtractToDirectory(@"C:\Foo\Bar", "myextractedfile.bin");
```

### EmbeddedFileDictionary
To store and globally access your embedded files from different classes, instances or modules, there's the `EmbeddedFileDictionary`.
Basically, it's just a .NET `Dictionary<string, EmbeddedFile>` with some managing code around it.

To store and retrieve embedded files, use methods `.GetFile` and `.SetFile`. If a requested entry isn't found, `null` is returned.
```csharp
EmbeddedDictionary.SetFile("myembedded-file", embeddedFile);
/* ... */

var file = EmbeddedFileDictionary.GetFile("myembedded-file");
```

#### Events
`EmbeddedFileDictionary` provides some events to inject into get- and set-calls, but these currently aren't documented.

## Logging
EmbeddedSharpFiles provides a logging interface so the main application can log actions done by this library.

It is as simple as the static class `GenericSharpLoading` provides the event `OnLog` which is fired every time the library would log something.

Along with the message, a log level is given, which is just a string containing the word `DEBUG`, `INFO`, `WARNING` or `ERROR`. These strings will probably always be the same, but if you want to process it, I would recommend to use the class `GenericSharpLoading.LogLevel` (nested class) and its four constants `DEBUG`, `INFO`, `WARNING` or `ERROR` which names are guaranteed to be never changed.

### Example
```csharp
class MyClass
{
	void Init()
	{
		GenericSharpLoading.GenericSharpLoading.OnLog += this.Log;

		//...
	}

	void Log(string message, string logLevel)
	{
		Console.WriteLine($"[MyApplication][{logLevel}] {message}");
	}
}

```


## License
EmbeddedSharpFiles is licensed under the MIT License
