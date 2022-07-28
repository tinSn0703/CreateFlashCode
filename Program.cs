using System;

namespace CreateFlashCode
{
	class Program
	{
		static void Main(string[] args)
		{
			var setting = new SettingCreateNvmCode();
			var settingFileXml = new Setting.SettingFileXmlController();
			//settingFileXml.Open(System.IO.Path.GetFullPath(settingFileXml.DefaultFileName));
			settingFileXml.Open(@"D:\workspace\作業場所_業務改善\Project\CreateFlashCode\setting.xml");
			//settingFileXml.ReadSetting(new SettingStreamXmlCreateFlashCode(setting));
			setting.NvmFilePath = @"C:\Users\nsowa.DKR\Downloads\P042D01000.txt";
			setting.OutputDirectory = System.IO.Path.GetDirectoryName(setting.NvmFilePath);
			settingFileXml.WriteSetting(new SettingStreamXmlCreateFlashCode(setting));
			settingFileXml.Save();
			settingFileXml.Close();
		}
	}
}
