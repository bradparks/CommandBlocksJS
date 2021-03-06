﻿using CommandblocksJS.Cmd;
using Noesis.Javascript;
using System;
using System.IO;
using System.Text;

namespace CommandBlocksJS.Cmd
{
	public class JsScriptExecutor
	{
		public void Run(string coreJsPath, string scriptPath, string worldDirectory, IntVector3 position, bool isSchematic)
		{
			string coreCode = File.ReadAllText(coreJsPath);
			string usercode = File.ReadAllText(scriptPath);
			JavascriptContext jsContext = new JavascriptContext();

			if (!isSchematic)
				jsContext.SetParameter("api", new JsApi(worldDirectory));
			else
				jsContext.SetParameter("api", new JsSchematicApi(worldDirectory));
#if DEBUG
			jsContext.Run(coreCode);
			jsContext.Run("var startPosition = new Util.Vector3(" + position.x + ", " + position.y + ", " + position.z + ");");
#else
			try
			{
				jsContext.Run(coreCode);
				jsContext.Run(string.Format("var startPosition = new Util.Vector3({0}, {1}, {2});", position.x, position.y, position.z));
			}
			catch (JavascriptException e)
			{
				string error = string.Format("Javascripterror: '{0}' at Line {1} Column {2} to {3}", e.Message, e.Line, e.StartColumn, e.EndColumn);
				throw new SystemException("Error in CommandblockJS Core Javascript code! Please make sure you are using the latest build!\n\n" + error);
			}
#endif

#if DEBUG
			jsContext.Run(usercode + "\n cbjsWorker();");
#else
			try
			{
				jsContext.Run(usercode + "\n cbjsWorker();");
			}
			catch(JavascriptException e)
			{
				string message = string.Format("Javascripterror: '{0}' at Line {1} Column {2} to {3}", e.Message, e.Line, e.StartColumn, e.EndColumn);
				throw new ApplicationException(message);
			}
#endif
		}
	}
}