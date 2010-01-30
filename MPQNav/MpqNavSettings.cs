using System;
using System.Collections.Specialized;
using System.Configuration;

namespace MPQNav {
	public static class MpqNavSettings {
		private static readonly NameValueCollection _settings = ConfigurationManager.AppSettings;

		public static string MpqPath {
			get { return _settings["mpqPath"]; }
		}

		public static string DefaultContinent {
			get { return Maps.All[int.Parse(_settings["defaultContinent"])]; }
		}

		public static int DefaultMapX {
			get { return int.Parse(_settings["defaultMapX"]); }
		}

		public static int DefaultMapY {
			get { return int.Parse(_settings["defaultMapY"]); }
		}

	    public static bool UseMpq
	    {
	        get { return bool.Parse(_settings["useMpq"]); }
	    }
	}
}