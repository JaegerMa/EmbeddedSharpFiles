using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmbeddedSharpFiles
{
	public class GetFileBeforeEventArgs
	{
		public string id;

		public GetFileBeforeEventArgs(string id)
		{
			this.id = id;
		}
	}

	public class GetFileAfterEventArgs
	{
		public readonly string id;
		public EmbeddedFile file;

		public GetFileAfterEventArgs(string id, EmbeddedFile file)
		{
			this.id = id;
			this.file = file;
		}
	}

	public class SetFileEventArgs
	{
		public string id;
		public bool cancelled;
		public EmbeddedFile file;
		public EmbeddedFile previousFile;

		public SetFileEventArgs(string id, EmbeddedFile file, EmbeddedFile previousFile)
		{
			this.id = id;
			this.file = file;
			this.previousFile = previousFile;
		}
	}
}
