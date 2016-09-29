﻿namespace Gabber
{
	public class DiskIO : GabberPCL.Interfaces.IDiskIO
	{
		public byte[] Load(string filePath)
		{
			return System.IO.File.ReadAllBytes(filePath);
		}
	}
}
