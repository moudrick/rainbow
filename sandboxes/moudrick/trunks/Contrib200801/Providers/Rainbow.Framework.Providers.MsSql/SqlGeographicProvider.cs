using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Data;
using Rainbow.Framework.BusinessObjects;
using Rainbow.Framework.Providers;
using Rainbow.Framework.Providers.Geographic;
using Rainbow.Framework.Settings;
using System.Configuration;

namespace Rainbow.Framework.Providers.MsSql
{
    /// <summary>
    /// SQL implementation of the <code>GeographicProvider</code> API
    /// </summary>
    public class SqlGeographicProvider : GeographicProvider
    {
        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        protected static SqlConnection ConnectionString
        {
            get
            {
                SqlConnection result;
                try
                {
                    result = Config.SqlConnectionString;
                }
                catch
                {
                    // I'm in a test environment
                    result =
                        new SqlConnection(
                            ConfigurationManager.ConnectionStrings["ConnectionString"].
                                ConnectionString);
                }
                return result;
            }
        }

        internal class CountryNameComparer : IComparer<Country>
        {
            public int Compare(Country x, Country y)
            {
                CaseInsensitiveComparer comparer = new CaseInsensitiveComparer(CultureInfo.CurrentUICulture);
                return comparer.Compare(x.Name, y.Name);
            }
        }

        /// <summary>
        /// Initializes the provider.
        /// </summary>
        /// <param name="name">The friendly name of the provider.</param>
        /// <param name="config">A collection of the name/value pairs representing the 
        /// provider-specific attributes specified in the configuration for this provider.</param>
        public override void Initialize(string name,
                                        System.Collections.Specialized.NameValueCollection config)
        {
            base.Initialize(name, config);

            // Initialize values from web.config.
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            countriesFilter = GetConfigValue(config["countriesFilter"], string.Empty);
        }

        public override IList<Country> GetUnfilteredCountries()
        {
            return GetCountriesCore(string.Empty, string.Empty, CountryFields.CountryID);
        }

        /// <summary>
        /// <see cref="GeographicProvider.GetCountries()"/>
        /// </summary>
        public override IList<Country> GetCountries()
        {
            return GetCountriesCore(countriesFilter, string.Empty, CountryFields.CountryID);
        }

        /// <summary>
        /// <see cref="GeographicProvider.GetCountries( CountryFields )"/>
        /// </summary>
        public override IList<Country> GetCountries(CountryFields sortBy)
        {
            return GetCountriesCore(countriesFilter, string.Empty, sortBy);
        }

        /// <summary>
        /// <see cref="GeographicProvider.GetCountries( string )"/>
        /// </summary>
        public override IList<Country> GetCountries(string filter)
        {
            return GetCountriesCore(countriesFilter, filter, CountryFields.CountryID);
        }

        /// <summary>
        /// <see cref="GeographicProvider.GetCountries( string, CountryFields )"/>
        /// </summary>
        public override IList<Country> GetCountries(string filter, CountryFields sortBY)
        {
            return GetCountriesCore(countriesFilter, filter, sortBY);
        }

        /// <summary>
        /// <see cref="GeographicProvider.GetCountryStates( string )"/>
        /// </summary>
        public override IList<State> GetCountryStates(string countryID)
        {
            IList<State> result = new List<State>();

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText =
                "SELECT  StateID, NeutralName FROM rb_States WHERE CountryID = @CountryID ORDER BY NeutralName";
            cmd.Parameters.Add("@CountryID", SqlDbType.NChar, 2).Value = countryID;

            cmd.Connection = ConnectionString;

            SqlDataReader reader = null;
            try
            {
                cmd.Connection.Open();

                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    result.Add(new State(reader.GetInt32(0), countryID, reader.GetString(1)));
                }
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                cmd.Connection.Close();
            }

            return result;
        }

        /// <summary>
        /// <see cref="GeographicProvider.GetCountryDisplayName( string, System.Globalization.CultureInfo )"/>
        /// </summary>
        public override string GetCountryDisplayName(string countryID,
                                                     CultureInfo cultureInfo)
        {
            string cacheKey = "COUNTRY_" + countryID + " - " + cultureInfo.TwoLetterISOLanguageName;

            if (CurrentCache.Get(cacheKey) == null)
            {
                string result = GetLocalizedDisplayName("COUNTRY_" + countryID, cultureInfo);

                if (string.IsNullOrEmpty(result))
                {
                    result = GetCountry(countryID).NeutralName;
                }

                CurrentCache.Insert(cacheKey, result);
            }

            return (string) CurrentCache.Get(cacheKey);
        }

        /// <summary>
        /// <see cref="GeographicProvider.GetStateDisplayName( int, System.Globalization.CultureInfo )"/>
        /// </summary>
        public override string GetStateDisplayName(int stateID, CultureInfo cultureInfo)
        {
            string cacheKey = "STATENAME_" + stateID + " - " + cultureInfo.TwoLetterISOLanguageName;

            if (CurrentCache.Get(cacheKey) == null)
            {
                string result = GetLocalizedDisplayName("STATENAME_" + stateID, cultureInfo);

                if (string.IsNullOrEmpty(result))
                {
                    result = GetState(stateID).NeutralName;
                }

                CurrentCache.Insert(cacheKey, result);
            }

            return (string) CurrentCache.Get(cacheKey);
        }

        /// <summary>
        /// <see cref="GeographicProvider.GetAdministrativeDivisionName( string, System.Globalization.CultureInfo )"/>
        /// </summary>
        public override string GetAdministrativeDivisionName(string administrativeDivisionName,
                                                             CultureInfo cultureInfo)
        {
            string cacheKey = "ADMINISTRATIVEDIVISIONNAME_" + administrativeDivisionName + " - " +
                              cultureInfo.TwoLetterISOLanguageName;

            if (CurrentCache.Get(cacheKey) == null)
            {
                string result =
                    GetLocalizedDisplayName(
                        "ADMINISTRATIVEDIVISIONNAME_" + administrativeDivisionName, cultureInfo);

                if (string.IsNullOrEmpty(result))
                {
                    result = administrativeDivisionName;
                }

                CurrentCache.Insert(cacheKey, result);
            }

            return (string) CurrentCache.Get(cacheKey);
        }

        /// <summary>
        /// <see cref="GeographicProvider.GetCountry( string )"/>
        /// </summary>
        public override Country GetCountry(string countryID)
        {
            Country result;

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText =
                "SELECT NeutralName, AdministrativeDivisionNeutralName FROM rb_Countries WHERE CountryID=@CountryID";
            cmd.Parameters.Add("@CountryID", SqlDbType.NChar, 2).Value = countryID;

            cmd.Connection = ConnectionString;

            SqlDataReader reader = null;
            try
            {
                cmd.Connection.Open();

                reader = cmd.ExecuteReader();

                if (!reader.Read())
                {
                    throw new CountryNotFoundException(
                        string.Format("The country with ID {0} was not found in the database",
                                      countryID));
                }
                result = new Country(countryID, reader.GetString(0), reader.GetString(1));
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                cmd.Connection.Close();
            }

            return result;
        }

        /// <summary>
        /// <see cref="GeographicProvider.GetState( int )"/>
        /// </summary>
        public override State GetState(int stateID)
        {
            State result;

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "SELECT CountryID, NeutralName FROM rb_States WHERE StateID=@StateID";
            cmd.Parameters.Add("@StateID", SqlDbType.Int).Value = stateID;

            cmd.Connection = ConnectionString;

            SqlDataReader reader = null;
            try
            {
                cmd.Connection.Open();

                reader = cmd.ExecuteReader();

                if (!reader.Read())
                {
                    throw new StateNotFoundException(
                        string.Format("The state with ID {0} was not found in the database", stateID));
                }
                result = new State(stateID, reader.GetString(0), reader.GetString(1));
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                cmd.Connection.Close();
            }

            return result;
        }

        /// <summary>
        /// A helper function to retrieve config values from the configuration file. 
        /// </summary>
        /// <param name="configValue"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        static string GetConfigValue(string configValue, string defaultValue)
        {
            if (String.IsNullOrEmpty(configValue))
            {
                return defaultValue;
            }
            return configValue;
        }

        static string GetLocalizedDisplayName(string textKey, CultureInfo culture)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText =
                "SELECT Description FROM rb_Localize WHERE Textkey=@TextKey AND CultureCode=@CultureCode";
            cmd.Parameters.Add("@TextKey", SqlDbType.NVarChar, 255).Value = textKey;
            cmd.Parameters.Add("@CultureCode", SqlDbType.NVarChar, 2).Value =
                culture.TwoLetterISOLanguageName;
            cmd.Connection = ConnectionString;

            SqlDataReader reader = null;
            try
            {
                cmd.Connection.Open();

                reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    return reader.GetString(0);
                }
                else
                {
                    return null;
                }
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                cmd.Connection.Close();
            }
        }

        static IList<Country> GetCountriesCore(string configFilter,
                                               string additionalFilter,
                                               CountryFields sortBY)
        {
            List<Country> result = new List<Country>();

            SqlCommand cmd = new SqlCommand();

            string filter = BuildCountriesFilter(configFilter, additionalFilter);

            if (filter.Equals(string.Empty))
            {
                cmd.CommandText = "rb_GetCountries";
            }
            else
            {
                cmd.CommandText = "rb_GetCountriesFiltered";
            }
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Connection = ConnectionString;

            if (!filter.Equals(string.Empty))
            {
                cmd.Parameters.Add("@Filter", SqlDbType.NVarChar, 10000).Value = filter;
            }
            cmd.Parameters.Add("@SortBy", SqlDbType.Int).Value = (int) sortBY;

            SqlDataReader reader = null;
            try
            {
                cmd.Connection.Open();

                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    result.Add(
                        new Country(reader.GetString(0), reader.GetString(1), reader.GetString(2)));
                }
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                cmd.Connection.Close();
            }

            if (sortBY == CountryFields.Name)
            {
                result.Sort(new CountryNameComparer());
            }

            return result;
        }

       
    }
}
