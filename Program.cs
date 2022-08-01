using System;

namespace CreateFlashCode
{
	class Program
	{
		static void Main(string[] args)
		{
			var _setting = new SettingCreateNvmCode();
			var _setting_file = new Setting.SettingFileXmlController();

			_setting_file.Open((args.Length > 0) ? args[0] : System.IO.Path.GetFullPath(_setting_file.DefaultFileName));

			try { _setting_file.ReadSetting(new SettingStreamXmlCreateFlashCode(_setting)); }
			catch (System.Xml.XmlException e)
			{
				_setting_file.WriteSetting(new SettingStreamXmlCreateFlashCode(_setting));
				_setting_file.Save();
				_setting_file.Close();

				Console.WriteLine(e.Message);
				Console.WriteLine("設定ファイル[" + _setting_file.SettingFilePath + "]に、要素["　+ _setting.SettingName + "]が存在しませんでした。");
				Console.WriteLine("設定ファイルに、要素[" + _setting.SettingName + "]を追加しました。設定を入力してください。");
				return;
			}
			catch(Setting.InvalidSettingException e)
			{
				Console.WriteLine(e.Message);
				Console.WriteLine("正しい形式の設定を入力してください。");
				return;
			}

			var _app = new ActionCreateNvmCode();
			var _result = _app.Action(_setting);
			Console.WriteLine(_result);

			_setting_file.WriteSetting(new SettingStreamXmlCreateFlashCode(_setting));
			_setting_file.Save();
			_setting_file.Close();
		}
	}
}
