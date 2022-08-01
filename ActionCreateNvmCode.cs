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
		// const
		//--------------------------------------------------------------------------------------------------//
		private const short READ_LINE_MIN_SIZE = 6;

		private const string COMMAND_REM_CODE = "14";

		private const string NVM_CODE_BLOCK = "#Block";

		private const short NUM_MATCH_ADDR_H = 1;
		private const short NUM_MATCH_ADDR_L = 2;
		private const short NUM_MATCH_WRITE_DATA = 3;

		private const string CODE_MATCH_ADDR_H = "addrH";
		private const string CODE_MATCH_ADDR_L = "addrL";
		private const string CODE_MATCH_WRITE_DATA = "data";

		private const short ADDR_DIGITS = 4;
		private const short SPLIT_ADDR_DIGITS = 2;
		private const short ADDR_H_SIDE_CHAR = 0;
		private const short ADDR_L_SIDE_CHAR = 2;

		//--------------------------------------------------------------------------------------------------//
		// type
		//--------------------------------------------------------------------------------------------------//
		/// <summary></summary>
		private struct FlashData
		{
			public int Addr { get; private set; }
			public string Data { get; private set; }

			public FlashData(int _address, string _data)
			{
				this.Addr = _address;
				this.Data = _data;
			}

			public FlashData(string _addrH, string _addrL, string _data)
			{
				this.Addr = Convert.ToInt32(_addrH + _addrL, 16); ;
				this.Data = _data;
			}
		};

		//--------------------------------------------------------------------------------------------------//
		// field
		//--------------------------------------------------------------------------------------------------//

		private string _resultMessage;

		//--------------------------------------------------------------------------------------------------//
		// public method
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

			List<string> _fileText = ReadFile(_setting);
			if (_fileText.Count < 1) return ReturnResultMessage("空のファイルです");

			var (_listBlockAddress, _listFlashData) = Read(_setting, _fileText);
			if (_listFlashData.Count < 1) return ReturnResultMessage("このNVMデータファイルから情報を読みだせませんでした。正しいファイルか確認してください。");

			var _listWriteCode = Create(_setting, _listBlockAddress, _listFlashData);

			this.Write(_setting, _listWriteCode);

			return ReturnResultMessage(
				"NVM検査データの作成に成功しました。生成されたファイルを確認してください。" + ((_listWriteCode.Count < 1) ? "\nこのNVMデータは、検査データでの書き込み不要です。" : ""));
		}

		//--------------------------------------------------------------------------------------------------//
		// private method
		//--------------------------------------------------------------------------------------------------//

		private string ReturnResultMessage(in string _message)
		{
			_resultMessage = _message;
			return _message;
		}

		//--------------------------------------------------------------------------------------------------//
		/// <summary></summary>
		/// <param name="_lineText"></param>
		/// <returns></returns>
		private bool IsSkipLine(in string _lineText)
		{
			if (_lineText.Length < READ_LINE_MIN_SIZE) return true;
			if ((_lineText[0] == NVM_CODE_BLOCK[0]) && (_lineText[1] != NVM_CODE_BLOCK[1]) && (_lineText[2] != NVM_CODE_BLOCK[2])) return true;
			if ((_lineText[0] == '$') && ((_lineText[1] != 'C') || (_lineText[2] != 'R'))) return true;

			return false;
		}

		//--------------------------------------------------------------------------------------------------//
		/// <summary></summary>
		/// <param name="_setting"></param>
		/// <returns></returns>
		private List<string> ReadFile(in SettingCreateNvmCode _setting)
		{
			var _fileText = new List<string>();

			using (var _reader = new System.IO.StreamReader(_setting.NvmFilePath))
			{
				while (_reader.Peek() >= 0)
				{
					string _lineText = _reader.ReadLine();
					if (this.IsSkipLine(_lineText)) continue;

					_fileText.Add(_lineText);
				}
			}

			return _fileText;
		}

		//--------------------------------------------------------------------------------------------------//
		/// <summary></summary>
		/// <param name="_setting"></param>
		/// <param name="_fileText"></param>
		/// <returns></returns>
		private (List<int>, List<FlashData>) Read(in SettingCreateNvmCode _setting, in IEnumerable<string> _fileText)
		{
			var _listBlockAddress = new List<int>();
			var _listFlashData = new List<FlashData>();

			bool _isBlockBegin = false;
			var _regex = new System.Text.RegularExpressions.Regex(@"^(?<addrH>[a-fA-F\d]{2}),(?<addrL>[a-fA-F\d]{2}),(?<data>[a-fA-F\d]{2})$");
			foreach(var _line in _fileText)
			{
				if ((_line[0] == '#') && (_line.IndexOf(NVM_CODE_BLOCK) == 0)) { _isBlockBegin = true; continue; }

				var _match = _regex.Match(_line);
				if (!_match.Success) continue;

				_listFlashData.Add(new FlashData(_match.Groups[NUM_MATCH_ADDR_H].Value, _match.Groups[NUM_MATCH_ADDR_L].Value, _match.Groups[NUM_MATCH_WRITE_DATA].Value));

				if (_isBlockBegin)
				{
					_listBlockAddress.Add(_listFlashData[_listFlashData.Count - 1].Addr);
					_isBlockBegin = false;
				}
			}

			if (_listFlashData.Count > 1) _listFlashData.Sort((a, b) => a.Addr - b.Addr);

			if (_listBlockAddress.Count < 1) _listBlockAddress.Add(_setting.MAX_WRITE_ADDRESS);
			else _listBlockAddress.Sort();

			return (_listBlockAddress, _listFlashData);
		}

		//--------------------------------------------------------------------------------------------------//
		private List<string> Create(in SettingCreateNvmCode _setting, in List<int> _listBlockAddress, in List<FlashData> _listFlashData)
		{
			var _listWriteCode = new List<string>();
			var _commandCode = COMMAND_REM_CODE + _setting.SendCommand + "," + _setting.RamCommand;
			var _commentCode = ",,,@" + _setting.Comment + "@@";

			for (int i = 0, j = 0; (i < _listFlashData.Count()) && (_listFlashData[i].Addr <= _setting.EndWriteAddress); i++)
			{
				if (_listFlashData[i].Addr < _setting.BeginWriteAddress) continue;

				string _addrStr = (_listFlashData[i].Addr / _setting.MinWriteByteNum).ToString("X4"); //
				string _addrStrH = _addrStr.Substring(ADDR_H_SIDE_CHAR, SPLIT_ADDR_DIGITS), _addrStrL = _addrStr.Substring(ADDR_L_SIDE_CHAR, SPLIT_ADDR_DIGITS);
				string _code = _setting.IsReverseAddress ? _addrStrL + _addrStrH : _addrStrH + _addrStrL;

				int _beforeAddr = _listFlashData[i].Addr - 1;

				for (int _countByte = 0, _countBlock = 0; 
					(i < _listFlashData.Count()) && (_countByte < _setting.MaxWriteByteNum) && (_countBlock < _setting.MaxWriteBlockNum) && (_listFlashData[i].Addr == _beforeAddr + 1) && (_listFlashData[i].Addr <= _setting.EndWriteAddress);
					_countByte++, i++)
				{
					if (_listBlockAddress[j] == _listFlashData[i].Addr)
					{
						_countBlock++;
						if (j < (_listBlockAddress.Count - 1)) j++;
					}

					_code += _listFlashData[i].Data;
					_beforeAddr = _listFlashData[i].Addr;
				}

				_listWriteCode.Add(_commandCode + _code + _commentCode);
			}

			return _listWriteCode;
		}

		//--------------------------------------------------------------------------------------------------//
		private void Write(in SettingCreateNvmCode _setting, in IEnumerable<string> _listWriteCode)
		{
			string _outputPath = _setting.OutputDirectory + "\\output_" + System.IO.Path.GetFileName(_setting.NvmFileName) + ".txt";
			using (var _writer = new System.IO.StreamWriter(_outputPath))
			{
				_writer.WriteLine(_setting.NvmFilePath + "\n");

				foreach (var _line in _listWriteCode)
				{
					_writer.WriteLine(_line);
				}
			}
		}
		
		//--------------------------------------------------------------------------------------------------//
		// public property
		//--------------------------------------------------------------------------------------------------//
		public override string ResultMessage => _resultMessage;

		//--------------------------------------------------------------------------------------------------//
		public override string ActionName => "CreateNvmCode";

		//--------------------------------------------------------------------------------------------------//
	}
}
