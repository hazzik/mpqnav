using System;
using System.Collections.Specialized;
using System.Configuration;
using MPQNav.ADT;

namespace MPQNav {
	public static class MpqNavSettings {
		private static readonly NameValueCollection _settings = ConfigurationManager.AppSettings;

		public static string MpqPath {
			get { return _settings["mpqPath"]; }
		}

		public static ContinentType DefaultContinent {
			get {
				string continent = _settings["defaultContinent"];
				return (ContinentType)Enum.Parse(typeof(ContinentType), continent, true);
			}
		}

		public static int DefaultMapX {
			get { return int.Parse(_settings["defaultMapX"]); }
		}

		public static int DefaultMapY {
			get { return int.Parse(_settings["defaultMapY"]); }
		}
	}
}