using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace CreateFlashCode
{
	public class SettingCreateNvmCode : Setting.Setting
	{
		//--------------------------------------------------------------------------------------------------//
		// field
		//--------------------------------------------------------------------------------------------------//
		private string nvmFilePass;

		//--------------------------------------------------------------------------------------------------//
		// public method
		//--------------------------------------------------------------------------------------------------//
		/// <summary>コンストラクタ</summary>
		public SettingCreateNvmCode() { this.SetDafultSetting(); }

		//--------------------------------------------------------------------------------------------------//
		/// <summary>設定データにデフォルトの設定を入力する</summary>
		public override void SetDafultSetting()
		{
			this.NvmFilePath = "";
			this.OutputDirectory = "";
			this.SendCommand = "MFTX";
			this.RamCommand = "K06";
			this.Comment = "[Flash] write";
			this.BeginWriteAddress = MIN_WRITE_ADDRESS;
			this.EndWriteAddress = MAX_WRITE_ADDRESS;
			this.MinWriteByteNum = 0;
			this.MaxWriteByteNum = 64;
			this.MaxWriteBlockNum = 4;
			this.IsReverseAddress = true;
		}

		//--------------------------------------------------------------------------------------------------//
		/// <summary>NVMデータファイルのパスが有効であるか?</summary>
		/// <param name="_path"></param>
		/// <returns></returns>
		public bool IsNvmFilePathExist(in string _path)
		{
			if (string.IsNullOrWhiteSpace(_path)) return false;
			return System.IO.File.Exists(_path);
		}

		//--------------------------------------------------------------------------------------------------//
		/// <summary>結果を保存するファイルへのディレクトリが有効であるか?</summary>
		/// <param name="_path"></param>
		/// <returns></returns>
		public bool IsOutputDirectoryExist(in string _path)
		{
			if (string.IsNullOrWhiteSpace(_path)) return false;
			return System.IO.Directory.Exists(_path);
		}

		//--------------------------------------------------------------------------------------------------//
		/// <summary>NVMデータ書き込みを行うRSコマンドの正しい形式であるか?</summary>
		public bool IsSendCommandCorrect(in string _command)
		{
			if (string.IsNullOrWhiteSpace(_command)) return false;
			return ((_command.Length <= this.MAX_COMMAND_SIZE) && IsStrSingleByteAlphanumeric(_command));
		}

		//--------------------------------------------------------------------------------------------------//
		/// <summary>NVMデータ書き込みを行うRAMモニタコマンドの正しい形式であるか?</summary>
		public bool IsRamCommandCorrect(in string _command)
		{
			if (string.IsNullOrWhiteSpace(_command)) return false;
			return ((_command.Length <= this.MAX_COMMAND_SIZE) && IsStrSingleByteAlphanumeric(_command));
		}

		//--------------------------------------------------------------------------------------------------//
		/// <summary>コメントの正しい形式であるか?</summary>
		public bool IsCommentCorrect(in string _comment)
		{
			return (_comment.IndexOfAny(new char[] { ',', '@' }) < 0);
		}

		//--------------------------------------------------------------------------------------------------//
		/// <summary>書き込みを行う先頭アドレス番号(0x0000~0xffffまで)の範囲内であるか?</summary>
		public bool IsBeginWriteAddressInRange(in int _num)
		{
			return ((MIN_WRITE_ADDRESS <= _num) && (_num <= MAX_WRITE_ADDRESS));
		}

		//--------------------------------------------------------------------------------------------------//
		/// <summary>書き込みを行う末尾アドレス番号(0x0000~0xffffまで)の範囲内であるか?</summary>
		public bool IsEndWriteAddressInRange(in int _num)
		{
			return ((MIN_WRITE_ADDRESS <= _num) && (_num <= MAX_WRITE_ADDRESS));
		}

		//--------------------------------------------------------------------------------------------------//
		/// <summary>書き込みコマンドで一度に書き込めるアドレスの最小バイト数の範囲内であるか?</summary>
		public bool IsMinWriteByteNumInRange(in short _num)
		{
			return ((MIN_WRITE_ADDRESS < _num) && (_num <= MAX_WRITE_ADDRESS));
		}

		//--------------------------------------------------------------------------------------------------//
		/// <summary>書き込みコマンドで一度に書き込めるアドレスの最大バイト数の範囲内であるか?</summary>
		public bool IsMaxWriteByteNumInRange(in short _num)
		{
			return ((MIN_WRITE_ADDRESS < _num) && (_num <= MAX_WRITE_ADDRESS));
		}

		//--------------------------------------------------------------------------------------------------//
		/// <summary>書き込みコマンドを実行できる最大ブロック数が範囲内であるか?</summary>
		public bool IsMaxWriteBlockNumInRange(in int _num)
		{
			return ((MIN_WRITE_ADDRESS < _num) && (_num <= (MAX_WRITE_ADDRESS + 1)));
		}

		//--------------------------------------------------------------------------------------------------//
		// private method
		//--------------------------------------------------------------------------------------------------//
		/// <summary>文字列が半角英数字のみで構成されているか?</summary>
		/// <param name="_str"></param>
		/// <returns>Yes/No</returns>
		private bool IsStrSingleByteAlphanumeric(in string _str)
		{
			return System.Text.RegularExpressions.Regex.IsMatch(_str, @"^[A-Za-z0-9]+$");
		}

		//--------------------------------------------------------------------------------------------------//
		// property
		//--------------------------------------------------------------------------------------------------//
		/// <summaryフォーム上の設定の要素名</summary>
		public override string SettingName => "CreateNvmCode";

		/// <summary>NVMデータファイル名</summary>
		public string NvmFileName { get; private set; }

		[Description("NVMデータファイルへのパス"), Category("Appearance")]
		/// <summary>NVMデータファイルのパス</summary>
		public string NvmFilePath { get => nvmFilePass; set { NvmFileName = System.IO.Path.GetFileName(value); nvmFilePass = value; }}

		[Description("生成した検査コードが書き込まれたファイルを保存するディレクトリ"), Category("Appearance")]
		/// <summary>結果を保存するファイルへのディレクトリ</summary>
		public string OutputDirectory { get; set; }

		[Description("NVMデータ書き込みを行うRSコマンド 半角英数字のみであること"), Category("Appearance")]
		/// <summary>NVMデータ書き込みを行うRSコマンド</summary>
		public string SendCommand { get; set; }

		[Description("NVMデータ書き込みを行うRAMモニタコマンド 半角英数字のみであること"), Category("Appearance")]
		/// <summary>NVMデータ書き込みを行うRAMモニタコマンド</summary>
		public string RamCommand { get; set; }

		[Description("検査データ後に付加されるコメント 使用不可文字[,@]が含まれていないこと"), Category("Appearance")]
		/// <summary>コメント</summary>
		public string Comment { get; set; }

		[Description("書き込みを行う先頭アドレス番号 0~65535(0x0000~0xFFFF)まで"), Category("Appearance")]
		/// <summary>書き込みを行う先頭アドレス番号 0~65535(0x0000~0xFFFF)まで)</summary>
		public int BeginWriteAddress { get; set; }

		[Description("書き込みを行う末尾アドレス番号 0~65535(0x0000~0xFFFF)まで"), Category("Appearance")]
		/// <summary>書き込みを行う末尾アドレス番号 0~65535(0x0000~0xFFFF)まで</summary>
		public int EndWriteAddress { get; set; }

		[Description("書き込みコマンドで一度に書き込めるアドレスの最小バイト数 1~65535まで"), Category("Appearance")]
		/// <summary>書き込みコマンドで一度に書き込めるアドレスの最小バイト数</summary>
		public short MinWriteByteNum { get; set; }

		[Description("書き込みコマンドで一度に書き込めるアドレスの最大バイト数 1~65535まで"), Category("Appearance")]
		/// <summary>書き込みコマンドで一度に書き込めるアドレスの最大バイト数</summary>
		public short MaxWriteByteNum { get; set; }

		[Description("書き込みコマンドを実行できる最大ブロック数 1~65536まで"), Category("Appearance")]
		/// <summary>書き込みコマンドを実行できる最大ブロック数</summary>
		public int MaxWriteBlockNum { get; set; }

		[Description("コマンドのアドレス指定で、H/Lを反転させるか。 true/falseで指定する"), Category("Appearance")]
		/// <summary>書き込みコマンドでのアドレス指定時、H/Lを反転させますか?</summary>
		public bool IsReverseAddress { get; set; }

		/// <summary></summary>
		public short MAX_COMMAND_SIZE => 10;

		/// <summary></summary>
		public int MIN_WRITE_ADDRESS => 0;

		/// <summary></summary>
		public int MAX_WRITE_ADDRESS => 65535;
	}
}
