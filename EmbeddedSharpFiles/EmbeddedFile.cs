using System;
using System.IO;
using System.Reflection;
using static EmbeddedSharpFiles.EmbeddedSharpFiles;

namespace EmbeddedSharpFiles
{
	public class EmbeddedFile
	{
		public string FileName { get; set; }
		public string ResourceName { get; set; }
		public Assembly ResourceAssembly { get; set; }
		public string ResourceNamespace { get; set; }
		

		public Type ReferenceType
		{
			set => SetReferenceType(value);
		}
		public string ResourceString => $"{this.ResourceNamespace}.{this.ResourceName}";
		public Stream ContentStream => GetContentStream();
		public string ContentString => GetContentString();

		public EmbeddedFile()
		{ }
		public EmbeddedFile(string resourceName, Type referenceType, string fileName = null)
			: this(resourceName, referenceType?.Assembly, referenceType?.Namespace, fileName)
		{ }
		public EmbeddedFile(string resourceName, Assembly resourceAssembly, string resourceNamespace, string fileName = null)
		{
			this.ResourceName = resourceName;
			this.FileName = fileName ?? resourceName;
			this.ResourceAssembly = resourceAssembly;
			this.ResourceNamespace = resourceNamespace;
		}
		public EmbeddedFile(EmbeddedFile embeddedFile)
			: this(embeddedFile.ResourceName, embeddedFile.ResourceAssembly, embeddedFile.ResourceNamespace, embeddedFile.FileName)
		{ }

		public virtual void SetReferenceType(Type referenceType)
		{
			this.ResourceAssembly = referenceType.Assembly;
			this.ResourceNamespace = referenceType.Namespace;
		}

		public virtual string GetAbsolutePath(string directory)
		{
			if(directory == null)
				throw new ArgumentException("Base directory mustn't be null", nameof(directory));
			if(!Path.IsPathRooted(directory))
				throw new ArgumentException("Base directory must be absolute", nameof(directory));

			return Path.Combine(directory, this.FileName);
		}

		public virtual Stream GetContentStream()
		{
			return this.ResourceAssembly.GetManifestResourceStream(this.ResourceString);
		}
		public virtual string GetContentString()
		{
			using(var stream = this.ContentStream)
			using(var reader = new StreamReader(stream, true))
				return reader.ReadToEnd();
		}

		public virtual void ExtractTo(string path)
		{
			Log($"Extracting '{this.ResourceString}' to '{path}'", LogLevel.DEBUG);
			CreateDirectory(path);

			using(var resourceStream = GetContentStream())
			using(var writeStream = new FileStream(path, FileMode.Create))
				resourceStream.CopyTo(writeStream);
		}
		public virtual bool Extract(string directory, string fileName = null, bool skipIfExisting = false)
		{
			fileName = fileName ?? this.FileName ?? this.ResourceName;

			if(skipIfExisting && Exists(directory, fileName))
			{
				Log($"Won't extract '{this.ResourceString}' to directory '{directory}', file '{fileName}' as it already exists", LogLevel.DEBUG);
				return false;
			}

			var filePath = Path.Combine(directory, fileName);
			ExtractTo(filePath);
			return true;
		}
		public virtual bool TryExtract(string directory, string fileName = null, bool skipIfExisting = false)
		{
			Log($"Trying to extract '{this.ResourceString}' to directory '{directory}', file '{fileName}'", LogLevel.DEBUG);
			try
			{
				Extract(directory, fileName: fileName, skipIfExisting: skipIfExisting);
				return true;
			}
			catch(Exception x)
			{
				Log($"Error while extracting '{this.ResourceString}' to directory '{directory}', file '{fileName}': {x.ToString()}", LogLevel.ERROR);
				return false;
			}
		}

		public virtual bool Exists(string directory, string fileName = null)
		{
			fileName = fileName ?? this.FileName ?? this.ResourceName;
			var filePath = Path.Combine(directory, fileName);

			Log($"Checking if file '{filePath}' exists", LogLevel.DEBUG);
			var fileExists = File.Exists(filePath);
			Log($"File '{filePath}' {(fileExists ? "exists" : "doesn't exist")}", LogLevel.DEBUG);

			return fileExists;
		}


		protected virtual void CreateDirectory(string path)
		{
			var directory = Path.GetDirectoryName(path);
			if(Directory.Exists(directory))
				return;

			Log($"Trying to create parent directory '{directory}'", LogLevel.DEBUG);
			Directory.CreateDirectory(directory);
			Log($"Directory '{directory}' created", LogLevel.DEBUG);
		}
	}
}
