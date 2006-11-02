using System;

namespace Rainbow.DesktopModules
{
    /// <summary>
    /// Placeable Registration module
    /// </summary>
    public class Register : RegisterFull
    {
		public override Guid GuidID 
		{
			get
			{
				return new Guid("{09C7351B-C9A1-454e-953F-E17E6E6EF092}");
			}
		}
    }
}