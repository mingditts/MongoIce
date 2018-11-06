using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace MongoIce.Test
{
	public static class AssertHelper
	{
		/// <summary>
		/// Are equals
		/// </summary>
		/// <param name="o1"></param>
		/// <param name="o2"></param>
		/// <returns></returns>
		public static void AreEquals(object o1, object o2)
		{
			if (o1 == null && o2 == null)
			{
				return;
			}

			if (o1 != null && o2 == null)
			{
				throw new System.Exception("Assert failed!");
			}

			if (o1 == null && o2 != null)
			{
				throw new System.Exception("Assert failed!");
			}

			Assert.AreEqual(JsonConvert.SerializeObject(o1).Equals(JsonConvert.SerializeObject(o2)), true);
		}
	}
}
