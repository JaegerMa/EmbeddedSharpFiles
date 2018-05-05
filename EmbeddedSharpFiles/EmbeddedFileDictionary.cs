using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmbeddedSharpFiles
{
	public delegate void EmbedDictionaryGetFileBeforeHandler(GetFileBeforeEventArgs e);
	public delegate void EmbedDictionaryGetFileAfterHandler(GetFileAfterEventArgs e);
	public delegate void EmbedDictionarySetFileHandler(SetFileEventArgs e);

	public static class EmbeddedFileDictionary
	{
		public static event EmbedDictionaryGetFileBeforeHandler FileGetBefore;
		public static event EmbedDictionaryGetFileAfterHandler FileGetAfter;
		public static event EmbedDictionarySetFileHandler FileSet;

		static Dictionary<string, EmbeddedFile> files;
		static EmbeddedFileDictionary()
		{
			files = new Dictionary<string, EmbeddedFile>();
		}

		public static EmbeddedFile GetFile(string id)
		{
			var getBeforeEventArgs = RaiseGetFileBefore(id);
			id = getBeforeEventArgs.id;

			if(!files.ContainsKey(id))
				return null;

			var file = files[id];

			var getAfterEventArgs = RaiseGetFileAfter(id, file);
			file = getAfterEventArgs.file;

			return file;
		}
		public static EmbeddedFile SetFile(string id, EmbeddedFile file)
		{
			var previous = GetFile(id);

			var setFileEventArgs = RaiseSetFile(id, file, previous);
			file = setFileEventArgs.file;
			previous = setFileEventArgs.previousFile;
			if(setFileEventArgs.cancelled)
				return previous;

			files[id] = file;

			return previous;
		}


		private static GetFileBeforeEventArgs RaiseGetFileBefore(string id)
		{
			var eventArgs = new GetFileBeforeEventArgs(id);
			FileGetBefore?.Invoke(eventArgs);

			return eventArgs;
		}
		private static GetFileAfterEventArgs RaiseGetFileAfter(string id, EmbeddedFile file)
		{
			var eventArgs = new GetFileAfterEventArgs(id, file);
			FileGetAfter?.Invoke(eventArgs);

			return eventArgs;
		}
		private static SetFileEventArgs RaiseSetFile(string id, EmbeddedFile file, EmbeddedFile previousFile)
		{
			var eventArgs = new SetFileEventArgs(id, file, previousFile);
			FileSet?.Invoke(eventArgs);

			return eventArgs;
		}
	}
}
