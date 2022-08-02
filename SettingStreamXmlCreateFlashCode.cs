using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.ComponentModel;

namespace CreateFlashCode
{
	public class SettingStreamXmlCreateFlashCode : Setting.SettingStreamXml
	{
		//--------------------------------------------------------------------------------------------------//
		// field
		//--------------------------------------------------------------------------------------------------//

		private const string SETTING_ELMENT_NAME_COMMAND = "Commnad";
		private const string SETTING_ELMENT_NAME_ADDRESS = "Adrress";

		private SettingCreateNvmCode _setting;

		private string _errMessage = "";

		//--------------------------------------------------------------------------------------------------//
		// public method
		//--------------------------------------------------------------------------------------------------//
		/// <summary>コンストラクタ</summary>
		public SettingStreamXmlCreateFlashCode()
		{
			this._setting = null;
		}

		//--------------------------------------------------------------------------------------------------//
		/// <summary>コンストラクタ</summary>
		/// <param name="_setting">本クラスで操作する設定データ</param>
		public SettingStreamXmlCreateFlashCode(in Setting.Setting _setting)
		{
			this.SetSetting(_setting ?? throw new ArgumentNullException(nameof(_setting)));
		}

		//--------------------------------------------------------------------------------------------------//
		/// <summary>コンストラクタ</summary>
		/// <param name="_setting">本クラスで操作する設定データ</param>
		public SettingStreamXmlCreateFlashCode(in SettingCreateNvmCode _setting)
		{
			this._setting = _setting ?? throw new ArgumentNullException(nameof(_setting));
		}

		//--------------------------------------------------------------------------------------------------//

		public override void SetSetting(in Setting.Setting _setting)
		{
			if (_setting is null) throw new ArgumentNullException(nameof(_setting));
			if (!(_setting is SettingCreateNvmCode)) throw new ArgumentException("Invalid type inputed. Input type: [" + _setting.GetType().Name + "].", nameof(_setting));

			this._setting = _setting as SettingCreateNvmCode;
		}

		//--------------------------------------------------------------------------------------------------//
		/// <summary></summary>
		/// <param name="_setting">本クラスで操作する設定データ</param>
		public void SetSetting(in SettingCreateNvmCode _setting)
		{
			this._setting = _setting ?? throw new ArgumentNullException(nameof(_setting));
		}

		//--------------------------------------------------------------------------------------------------//
		public override void Write()							{	throw new NotImplementedException("Enter XML elements as arguments");	}
		public override void Write(in Setting.Setting _setting)	{	throw new NotImplementedException("Enter XML elements as arguments");	}

		//--------------------------------------------------------------------------------------------------//
		/// <summary>XML形式の設定を、登録された設定オブジェクトへ書き込む</summary>
		/// <param name="_element">XML形式の設定</param>
		/// <exception cref="InvalidOperationException">オブジェクトの設定が未完了の状態で、関数が呼び出されました</exception>
		/// <exception cref="ArgumentNullException">引数がnullです</exception>
		/// <exception cref="NullReferenceException">設定の属性または要素が存在しませんでした</exception>
		/// <exception cref="Setting.InvalidSettingException">設定の形式が不正です</exception>
		public override void Write(in XElement _element)
		{
			if (_setting is null) throw new InvalidOperationException("[" + nameof(_setting) + "] is undefined.");
			if (_element is null) throw new ArgumentNullException(nameof(_element));

			_errMessage = "";

			try {
				_setting.NvmFilePath = ReadSetting(_element, nameof(_setting.NvmFilePath), (string x) => _setting.IsNvmFilePathExist(x), "File not found.");

				string _value = "";
				try { _value = ReadAttributeAsString(_element, nameof(_setting.OutputDirectory)); } catch (System.NullReferenceException) { }
				_setting.OutputDirectory = (_setting.IsOutputDirectoryExist(_value) ? _value : System.IO.Path.GetDirectoryName(_setting.NvmFilePath));
			}
			catch (System.NullReferenceException e) { _errMessage += e.Message + "\n"; }
			
			XElement _childElement;
			_childElement = _element.Element(SETTING_ELMENT_NAME_COMMAND);
			if (_childElement is null)	_errMessage += "Element [" + SETTING_ELMENT_NAME_COMMAND + "] didn't exist.\n";
			else						WriteCommandSetting(this._setting, _childElement);

			_childElement = _element.Element(SETTING_ELMENT_NAME_ADDRESS);
			if (_childElement is null)	_errMessage += "Element [" + SETTING_ELMENT_NAME_ADDRESS + "] didn't exist.\n";
			else						WriteAddressSetting(this._setting, _childElement);

			if (_errMessage != "") throw new NullReferenceException(_errMessage);
		}

		//--------------------------------------------------------------------------------------------------//
		/// <summary>設定オブジェクトを登録し、XML形式の設定から内容を書き込む</summary>
		/// <param name="_setting">設定オブジェクト</param>
		/// <param name="_element">XML形式の設定</param>
		/// <exception cref="InvalidOperationException">オブジェクトの設定が未完了の状態で、関数が呼び出されました</exception>
		/// <exception cref="ArgumentNullException">引数がnullです</exception>
		/// <exception cref="NullReferenceException">設定の属性または要素が存在しませんでした</exception>
		/// <exception cref="Setting.InvalidSettingException">設定の形式が不正です</exception>
		public override void Write(in Setting.Setting _setting, in XElement _element)
		{
			this.SetSetting(_setting);
			this.Write(_element);
		}

		//--------------------------------------------------------------------------------------------------//
		/// <summary>設定オブジェクトを登録し、XML形式の設定から内容を書き込む</summary>
		/// <param name="_setting">設定オブジェクト</param>
		/// <param name="_element">XML形式の設定</param>
		public void Write(in SettingCreateNvmCode _setting, in XElement _element)
		{
			this.SetSetting(_setting);
			this.Write(_element);
		}

		//--------------------------------------------------------------------------------------------------//
		public override void Read()								{	throw new NotImplementedException("Enter XML elements as arguments");	}
		public override void Read(in Setting.Setting _setting)	{	throw new NotImplementedException("Enter XML elements as arguments");	}

		//--------------------------------------------------------------------------------------------------//
		/// <summary>登録された設定オブジェクトから、XML形式の設定を読み出す</summary>
		/// <param name="_element">XML形式の設定の出力</param>
		public override void Read(out XElement _element)
		{
			if (_setting is null) throw new InvalidOperationException("[" + nameof(_setting) + "] is undefined.");

			//var myAttribute = (DescriptionAttribute)TypeDescriptor.GetProperties(_setting)[nameof(_setting.NvmFilePath)].Attributes[typeof(DescriptionAttribute)];

			_element = 
				new XElement(this.ElementName,
				new XAttribute(nameof(_setting.NvmFilePath), _setting.NvmFilePath),
				new XAttribute(nameof(_setting.OutputDirectory), _setting.OutputDirectory),
				new XComment("説明"),
				new XElement(SETTING_ELMENT_NAME_COMMAND,
					new XAttribute(nameof(_setting.SendCommand), _setting.SendCommand),
					new XAttribute(nameof(_setting.RamCommand), _setting.RamCommand),
					new XAttribute(nameof(_setting.IsReverseAddress), _setting.IsReverseAddress),
					new XAttribute(nameof(_setting.MinWriteByteNum), _setting.MinWriteByteNum),
					new XAttribute(nameof(_setting.MaxWriteByteNum), _setting.MaxWriteByteNum),
					new XAttribute(nameof(_setting.MaxWriteBlockNum), _setting.MaxWriteBlockNum)
				),
				new XElement(SETTING_ELMENT_NAME_ADDRESS,
					new XAttribute(nameof(_setting.BeginWriteAddress), _setting.BeginWriteAddress),
					new XAttribute(nameof(_setting.EndWriteAddress), _setting.EndWriteAddress),
					new XAttribute(nameof(_setting.Comment), _setting.Comment)
				)
			);
		}

		//--------------------------------------------------------------------------------------------------//
		/// <summary>設定オブジェクト登録し、XML形式の設定を読み出す</summary>
		/// <param name="_setting">設定オブジェクト</param>
		/// <param name="_element">XML形式の設定の出力</param>
		public override void Read(in Setting.Setting _setting, out XElement _element)
		{
			this.SetSetting(_setting);
			this.Read(out _element);
		}

		//--------------------------------------------------------------------------------------------------//
		/// <summary>設定オブジェクト登録し、XML形式の設定を読み出す</summary>
		/// <param name="_setting">設定オブジェクト</param>
		/// <param name="_element">XML形式の設定の出力</param>
		public void Read(in SettingCreateNvmCode _setting, out XElement _element)
		{
			this.SetSetting(_setting);
			this.Read(out _element);
		}

		//--------------------------------------------------------------------------------------------------//
		// public method
		//--------------------------------------------------------------------------------------------------//
		/// <summary>xml要素から設定を読み出す</summary>
		/// <param name="_element">xml要素</param>
		/// <param name="_name">設定名</param>
		/// <param name="_is">設定の形式をチェックするメソッド</param>
		/// <param name="_errMessage">_is == false時のエラーメッセージ</param>
		/// <returns>読みだした設定</returns>
		private int ReadSetting(in XElement _element, in string _name, Func<int, bool> _is, in string _errMessage = "")
		{
			int _value = ReadAttributeAsInt(_element, _name);
			return (_is(_value) ? _value : throw new Setting.InvalidSettingException(_name, _errMessage));
		}

		//--------------------------------------------------------------------------------------------------//
		/// <summary>xml要素から設定を読み出す</summary>
		/// <param name="_element">xml要素</param>
		/// <param name="_name">設定名</param>
		/// <param name="_is">設定の形式をチェックするメソッド</param>
		/// <param name="_errMessage">_is == false時のエラーメッセージ</param>
		/// <returns>読みだした設定</returns>
		private short ReadSetting(in XElement _element, in string _name, Func<short, bool> _is, in string _errMessage = "")
		{
			short _value = ReadAttributeAsShort(_element, _name);
			return (_is(_value) ? _value : throw new Setting.InvalidSettingException(_name, _errMessage));
		}

		//--------------------------------------------------------------------------------------------------//
		/// <summary>xml要素から設定を読み出す</summary>
		/// <param name="_element">xml要素</param>
		/// <param name="_name">設定名</param>
		/// <param name="_is">設定の形式をチェックするメソッド</param>
		/// <param name="_errMessage">_is == false時のエラーメッセージ</param>
		/// <returns>読みだした設定</returns>
		private string ReadSetting(in XElement _element, in string _name, Func<string, bool> _is, in string _errMessage = "")
		{
			string _value = ReadAttributeAsString(_element, _name);
			return (_is(_value) ? _value : throw new Setting.InvalidSettingException(_name, _errMessage));
		}

		//--------------------------------------------------------------------------------------------------//
		/// <summary>xml要素内のアドレスの設定をオブジェクトに書き込む</summary>
		/// <param name="_setting">設定オブジェクト</param>
		/// <param name="_element">xml要素</param>
		/// <exception cref="Setting.InvalidSettingException">設定の形式が不正です</exception>
		private void WriteAddressSetting(SettingCreateNvmCode _setting, in XElement _element)
		{
			try { _setting.BeginWriteAddress = ReadSetting(_element, nameof(_setting.BeginWriteAddress), (int x) => _setting.IsBeginWriteAddressInRange(x)); }
			catch (System.NullReferenceException e) { _errMessage += e.Message + "\n"; }
			
			try { _setting.EndWriteAddress = ReadSetting(_element, nameof(_setting.EndWriteAddress), (int x) => _setting.IsEndWriteAddressInRange(x)); }
			catch (System.NullReferenceException e) { _errMessage += e.Message + "\n"; }

			try { _setting.Comment = ReadSetting(_element, nameof(_setting.Comment), (string x) => _setting.IsCommentCorrect(x));}
			catch (System.NullReferenceException e) { _errMessage += e.Message + "\n"; }
}

		//--------------------------------------------------------------------------------------------------//
		/// <summary>xml要素内のコマンドの設定をオブジェクトに書き込む</summary>
		/// <param name="_setting">設定オブジェクト</param>
		/// <param name="_element">xml要素</param>
		/// <exception cref="Setting.InvalidSettingException">設定の形式が不正です</exception>
		private void WriteCommandSetting(SettingCreateNvmCode _setting, in XElement _element)
		{
			try { _setting.SendCommand = ReadSetting(_element, nameof(_setting.SendCommand), (string x) => _setting.IsSendCommandCorrect(x)); }
			catch (System.NullReferenceException e) { _errMessage += e.Message + "\n"; }

			try { _setting.RamCommand = ReadSetting(_element, nameof(_setting.RamCommand), (string x) => _setting.IsRamCommandCorrect(x)); }
			catch (System.NullReferenceException e) { _errMessage += e.Message + "\n"; }

			try { _setting.IsReverseAddress = ReadAttributeAsBool(_element, nameof(_setting.IsReverseAddress)); }
			catch (System.NullReferenceException e) { _errMessage += e.Message + "\n"; }

			try { _setting.MaxWriteBlockNum = ReadSetting(_element, nameof(_setting.MaxWriteBlockNum), (int x) => _setting.IsMaxWriteBlockNumInRange(x)); }
			catch (System.NullReferenceException e) { _errMessage += e.Message + "\n"; }

			try { _setting.MinWriteByteNum = ReadSetting(_element, nameof(_setting.MinWriteByteNum), (short x) => _setting.IsMinWriteByteNumInRange(x)); }
			catch (System.NullReferenceException e) { _errMessage += e.Message + "\n"; }

			try { _setting.MaxWriteByteNum = ReadSetting(_element, nameof(_setting.MaxWriteByteNum), (short x) => _setting.IsMaxWriteByteNumInRange(x)); }
			catch (System.NullReferenceException e) { _errMessage += e.Message + "\n"; }
		}

		//--------------------------------------------------------------------------------------------------//
		// property
		//--------------------------------------------------------------------------------------------------//
		/// <summary></summary>
		public override Setting.Setting Setting => _setting;

		/// <summary></summary>
		public override string SettingName => _setting.SettingName;

		/// <summary></summary>
		public override string ElementName => this.SettingName;
	}
}
