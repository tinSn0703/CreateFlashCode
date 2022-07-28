using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreateFlashCode
{
	public abstract class ApplicationAction
	{
		public abstract string Action(in Setting.Setting _setting);

		public abstract string ResultMessage { get; }

		public abstract string ActionName { get; }
	}
}
