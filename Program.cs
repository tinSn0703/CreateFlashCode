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
			catch(Setting.InvalidSettingException e)
			{
				Console.WriteLine(e.Message);
				Console.WriteLine("上記に示す設定に正しい形式の値を入力し、再度実行してください。");

				goto EXIT_MAIN;
			}
			catch (System.Xml.XmlException e)
			{
				Console.WriteLine(e.Message);
				Console.WriteLine("設定ファイル [" + _setting_file.SettingFilePath + "]に、本アプリケーション用の要素 ["　+ _setting.SettingName + "]が存在しませんでした。");
				Console.WriteLine("設定ファイルに、要素[" + _setting.SettingName + "]を追加します。値を入力し、再度実行してください。");

				goto EXIT_MAIN_AND_WRITE_SETTING;
			}
			catch(NullReferenceException e)
			{
				Console.WriteLine(e.Message);
				Console.WriteLine("上記項目が設定ファイルに存在しませんでした。");
				Console.WriteLine("上記項目を追加した設定ファイルを再生成します。値を入力し、再度実行してください。");

				goto EXIT_MAIN_AND_WRITE_SETTING;
			}

			var _app = new ActionCreateNvmCode();
			var _result = _app.Action(_setting);
			Console.WriteLine(_result);
			
			EXIT_MAIN_AND_WRITE_SETTING:
			
			_setting_file.WriteSetting(new SettingStreamXmlCreateFlashCode(_setting));
			_setting_file.Save();
			_setting_file.Close();

			EXIT_MAIN:

			Console.WriteLine("終了するには何かキーを押してください ...");
			Console.ReadKey();
		}
	}
}
