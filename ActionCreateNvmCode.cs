using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreateFlashCode
{
	public class ActionCreateNvmCode : ApplicationAction
	{
		//--------------------------------------------------------------------------------------------------//
		// type
		//--------------------------------------------------------------------------------------------------//
		private struct NvmData
		{
			public short _address;
			public string _data;

			public NvmData(short _address, string _data) { this._address = _address; this._data = _data; }
		};

		//--------------------------------------------------------------------------------------------------//
		// field
		//--------------------------------------------------------------------------------------------------//

		private LinkedList<short> _listBlockBeginAddress;
		private LinkedList<NvmData> _listNvmData;
		private LinkedList<string> _listWriteCode;

		//--------------------------------------------------------------------------------------------------//
		// method
		//--------------------------------------------------------------------------------------------------//

		public override string Action(in Setting.Setting _setting)
		{
			if (_setting is null) throw new ArgumentNullException(nameof(_setting));
			if (!(_setting is SettingCreateNvmCode)) throw new ArgumentException("Invalid type inputed. Input type: [" + _setting.GetType().Name + "].", nameof(_setting));

			return this.Action(_setting as SettingCreateNvmCode);
		}

		//--------------------------------------------------------------------------------------------------//
		/// <summary></summary>
		/// <param name="_setting"></param>
		public string Action(in SettingCreateNvmCode _setting)
		{
			if (_setting is null) throw new ArgumentNullException(nameof(_setting));

			return "";
		}

		//--------------------------------------------------------------------------------------------------//
		public override string ResultMessage { get; }
		
		//--------------------------------------------------------------------------------------------------//
		public override string ActionName => "CreateNvmCode";

		//--------------------------------------------------------------------------------------------------//
		// private method
		//--------------------------------------------------------------------------------------------------//

		private bool IsSkipLine(in string _lineText)
		{
			return false;
		}

		//--------------------------------------------------------------------------------------------------//

		private LinkedList<string> Read(in SettingCreateNvmCode _setting)
		{
			LinkedList<string> _fileText = new LinkedList<string>();

			using (var _reader = new System.IO.StreamReader(_setting.NvmFilePath))
			{
				while (_reader.Peek() >= 0)
				{
					_fileText.AddLast(_reader.ReadLine());
				}
			}

			return _fileText;
		}
	}
}
