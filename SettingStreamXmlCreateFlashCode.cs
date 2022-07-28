using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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
		public override void Write(in XElement _element)
		{
			if (_setting is null) throw new InvalidOperationException("[" + nameof(_setting) + "] is undefined.");
			if (_element is null) throw new ArgumentNullException(nameof(_element));

			var _value = _element.Attribute(nameof(_setting.NvmFilePath)).Value;
			_setting.NvmFilePath = (_setting.IsNvmFilePathExist(_value) ? _value : throw new Setting.InvalidSettingException(nameof(_setting.NvmFilePath), "File not found."));

			_value = _element.Attribute(nameof(_setting.OutputDirectory)).Value;
			_setting.OutputDirectory = (_setting.IsOutputDirectoryExist(_value) ? _value : "");

			WriteCommandSetting(this._setting, _element.Element(SETTING_ELMENT_NAME_COMMAND));
			WriteAddressSetting(this._setting, _element.Element(SETTING_ELMENT_NAME_ADDRESS));
		}

		//--------------------------------------------------------------------------------------------------//
		/// <summary>設定オブジェクトを登録し、XML形式の設定から内容を書き込む</summary>
		/// <param name="_setting">設定オブジェクト</param>
		/// <param name="_element">XML形式の設定</param>
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

			_element = new XElement(this.ElementName,
				new XAttribute(nameof(_setting.NvmFilePath), _setting.NvmFilePath),
				new XAttribute(nameof(_setting.OutputDirectory), _setting.OutputDirectory),
				new XElement(SETTING_ELMENT_NAME_COMMAND,
					new XAttribute(nameof(_setting.SendCommand), _setting.SendCommand),
					new XAttribute(nameof(_setting.RamCommand), _setting.RamCommand),
					new XAttribute(nameof(_setting.Comment), _setting.Comment)
				),
				new XElement(SETTING_ELMENT_NAME_ADDRESS,
					new XAttribute(nameof(_setting.BeginWriteAddress), _setting.BeginWriteAddress),
					new XAttribute(nameof(_setting.EndWriteAddress), _setting.EndWriteAddress),
					new XAttribute(nameof(_setting.IsReverseAddress), _setting.IsReverseAddress),
					new XAttribute(nameof(_setting.MinWriteByteNum), _setting.MinWriteByteNum),
					new XAttribute(nameof(_setting.MaxWriteByteNum), _setting.MaxWriteByteNum),
					new XAttribute(nameof(_setting.MaxWriteBlockNum), _setting.MaxWriteBlockNum)
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
		private void WriteAddressSetting(in SettingCreateNvmCode _setting, in XElement _element)
		{
			int _value_int = (int)_element.Attribute(nameof(_setting.BeginWriteAddress));
			_setting.BeginWriteAddress = (_setting.IsBeginWriteAddressInRange(_value_int) ? _value_int : throw new Setting.InvalidSettingException(nameof(_setting.BeginWriteAddress)));

			_value_int = (int)_element.Attribute(nameof(_setting.EndWriteAddress));
			_setting.EndWriteAddress = (_setting.IsEndWriteAddressInRange(_value_int) ? _value_int : throw new Setting.InvalidSettingException(nameof(_setting.EndWriteAddress)));

			short _value_short = (short)_element.Attribute(nameof(_setting.MinWriteByteNum));
			_setting.MinWriteByteNum = (_setting.IsMinWriteByteNumInRange(_value_short) ? _value_short : throw new Setting.InvalidSettingException(nameof(_setting.MinWriteByteNum)));

			_value_short = (short)_element.Attribute(nameof(_setting.MaxWriteByteNum));
			_setting.MaxWriteByteNum = (_setting.IsMaxWriteByteNumInRange(_value_short) ? _value_short : throw new Setting.InvalidSettingException(nameof(_setting.MaxWriteByteNum)));

			_value_short = (short)_element.Attribute(nameof(_setting.MaxWriteBlockNum));
			_setting.MaxWriteBlockNum = (_setting.IsMaxWriteBlockNumInRange(_value_short) ? _value_short : throw new Setting.InvalidSettingException(nameof(_setting.MaxWriteBlockNum)));

			bool _value_bool = (bool)_element.Attribute(nameof(_setting.IsReverseAddress));
		}

		//--------------------------------------------------------------------------------------------------//
		private void WriteCommandSetting(in SettingCreateNvmCode _setting, in XElement _element)
		{
			var _value = _element.Attribute(nameof(_setting.SendCommand)).Value;
			_setting.SendCommand = (_setting.IsSendCommandCorrect(_value) ? _value : throw new Setting.InvalidSettingException(nameof(_setting.SendCommand)));

			_value = _element.Attribute(nameof(_setting.RamCommand)).Value;
			_setting.RamCommand = (_setting.IsRamCommandCorrect(_value) ? _value : throw new Setting.InvalidSettingException(nameof(_setting.RamCommand)));

			_value = _element.Attribute(nameof(_setting.Comment)).Value;
			_setting.Comment = (_setting.IsCommentCorrect(_value) ? _value : throw new Setting.InvalidSettingException(nameof(_setting.Comment)));
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
