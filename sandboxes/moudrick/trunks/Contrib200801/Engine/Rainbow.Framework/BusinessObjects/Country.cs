using System.Threading;
using Rainbow.Framework.Providers;

namespace Rainbow.Framework.BusinessObjects
{
    public class Country
    {
        string countryID;
        string neutralName;
        string administrativeDivisionNeutralName;

        public Country() : this(string.Empty, string.Empty, string.Empty)
        {
        }

        public Country(string countryID, string neutralName, 
            string administrativeDivisionNeutralName)
        {
            this.countryID = countryID;
            this.neutralName = neutralName;
            this.administrativeDivisionNeutralName = administrativeDivisionNeutralName;
        }


        /// <summary>
        /// Gets or sets the country ID.
        /// </summary>
        /// <value>The country ID.</value>
        public string CountryID
        {
            get { return countryID; }
            set { countryID = value; }
        }

        /// <summary>
        /// Gets or sets the neutral name.
        /// </summary>
        /// <value>The name of the neutral.</value>
        public string NeutralName
        {
            get { return neutralName; }
            set { neutralName = value; }
        }

        /// <summary>
        /// Gets or sets the name of the administrative division neutral name.
        /// </summary>
        /// <value>The name of the administrative division neutral.</value>
        public string AdministrativeDivisionNeutralName
        {
            get { return administrativeDivisionNeutralName; }
            set { administrativeDivisionNeutralName = value; }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get
            {
                return
                    GeographicProvider.Current.GetCountryDisplayName(countryID,
                                                                     Thread.CurrentThread.
                                                                         CurrentCulture);
            }
        }

        /// <summary>
        /// Gets the name of the administrative division.
        /// </summary>
        /// <value>The name of the administrative division.</value>
        public string AdministrativeDivisionName
        {
            get
            {
                return
                    GeographicProvider.Current.GetAdministrativeDivisionName(
                        administrativeDivisionNeutralName, Thread.CurrentThread.CurrentCulture);
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"></see> is equal to the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object"></see> to compare with the current <see cref="T:System.Object"></see>.</param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"></see> is equal to the current <see cref="T:System.Object"></see>; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            Country otherCountry = (Country) obj;
            return (countryID == otherCountry.countryID) &&
                   (neutralName == otherCountry.neutralName) &&
                   (administrativeDivisionNeutralName == otherCountry.administrativeDivisionNeutralName);
        }


        ///<summary>
        ///Serves as a hash function for a particular type. <see cref="M:System.Object.GetHashCode"></see> is suitable for use in hashing algorithms and data structures like a hash table.
        ///</summary>
        ///
        ///<returns>
        ///A hash code for the current <see cref="T:System.Object"></see>.
        ///</returns>
        ///<filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            return countryID.GetHashCode();
        }
    }
}
