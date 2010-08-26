using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Data;
using System.Data.Linq;
using System.Linq;
using Rainbow.Framework.Data.MsSql;
using Rainbow.Framework.Configuration;
using System.Configuration;
using Rainbow.Framework.Data.Providers.Geographic;

namespace Rainbow.Framework.Data.MsSql.Providers.Geographic {

    /// <summary>
    /// SQL implementation of the <code>GeographicProvider</code> API
    /// </summary>
    public class SqlGeographicProvider : GeographicProvider {
        /// <summary>
        /// Initializes the provider.
        /// </summary>
        /// <param name="name">The friendly name of the provider.</param>
        /// <param name="config">A collection of the name/value pairs representing the
        /// provider-specific attributes specified in the configuration for this provider.</param>
        /// <exception cref="T:System.ArgumentNullException">The name of the provider is null.</exception>
        /// <exception cref="T:System.ArgumentException">The name of the provider has a length of zero.</exception>
        /// <exception cref="T:System.InvalidOperationException">An attempt is made to call <see cref="M:System.Configuration.Provider.ProviderBase.Initialize(System.String,System.Collections.Specialized.NameValueCollection)"/> on a provider after the provider has already been initialized.</exception>
        public override void Initialize( string name, System.Collections.Specialized.NameValueCollection config ) {
            base.Initialize( name, config );

            // Initialize values from web.config.
            if ( config == null ) {
                throw new ArgumentNullException( "config" );
            }

            countriesFilter = GetConfigValue( config["countriesFilter"], string.Empty );
        }

        /// <summary>
        /// A helper function to retrieve config values from the configuration file.
        /// </summary>
        /// <param name="configValue">The config value.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        private string GetConfigValue( string configValue, string defaultValue ) {
            if ( String.IsNullOrEmpty( configValue ) )
                return defaultValue;

            return configValue;
        }

        /// <summary>
        /// Gets the list of countries
        /// </summary>
        /// <returns>
        /// a <code>IList&lt;Country&gt;</code> containing a list of all countries.
        /// This method ignores the CountriesFilter property.
        /// </returns>
        public override IList<Country> GetUnfilteredCountries() {
            return GetCountriesCore( string.Empty, string.Empty, CountryFields.CountryID );
        }

        /// <summary>
        /// 	<see cref="Rainbow.Framework.Providers.Geographic.GeographicProvider.GetCountries()"/>
        /// </summary>
        /// <returns>
        /// a <code>IList&lt;Country&gt;</code> containing a list of all countries.
        /// This method takes into account the CountriesFilter property.
        /// </returns>
        public override IList<Country> GetCountries() {
            return GetCountriesCore( countriesFilter, string.Empty, CountryFields.CountryID );
        }

        /// <summary>
        /// <see cref="Rainbow.Framework.Providers.Geographic.GeographicProvider.GetCountries( CountryFields )"/>
        /// </summary>
        public override IList<Country> GetCountries( CountryFields sortBY ) {
            return GetCountriesCore( countriesFilter, string.Empty, sortBY );
        }

        /// <summary>
        /// <see cref="Rainbow.Framework.Providers.Geographic.GeographicProvider.GetCountries( string )"/>
        /// </summary>
        public override IList<Country> GetCountries( string filter ) {
            return GetCountriesCore( countriesFilter, filter, CountryFields.CountryID );
        }

        /// <summary>
        /// <see cref="Rainbow.Framework.Providers.Geographic.GeographicProvider.GetCountries( string, CountryFields )"/>
        /// </summary>
        public override IList<Country> GetCountries( string filter, CountryFields sortBY ) {
            return GetCountriesCore( countriesFilter, filter, sortBY );
        }

        /// <summary>
        /// 	<see cref="Rainbow.Framework.Providers.Geographic.GeographicProvider.GetCountryStates( string )"/>
        /// </summary>
        /// <param name="countryID">The country code</param>
        /// <returns>
        /// The list of states for the specified country
        /// </returns>
        public override IList<State> GetCountryStates( string countryID ) {
            IList<State> result = new List<State>();

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "SELECT  StateID, NeutralName FROM rb_States WHERE CountryID = @CountryID ORDER BY NeutralName";
            cmd.Parameters.Add( "@CountryID", SqlDbType.NChar, 2 ).Value = countryID;

            cmd.Connection = ConnectionString;

            SqlDataReader reader = null;
            try {
                cmd.Connection.Open();

                reader = cmd.ExecuteReader();

                while ( reader.Read() ) {
                    result.Add( new State( reader.GetInt32( 0 ), countryID, reader.GetString( 1 ) ) );
                }
            }
            finally {
                if ( reader != null ) {
                    reader.Close();
                }
                cmd.Connection.Close();
            }

            return result;
        }

        /// <summary>
        /// 	<see cref="Rainbow.Framework.Providers.Geographic.GeographicProvider.GetCountryDisplayName( string, System.Globalization.CultureInfo )"/>
        /// </summary>
        /// <param name="countryID">The country's id</param>
        /// <param name="c">a <code>System.Globalization.CultureInfo</code> describing the language we want the name for</param>
        /// <returns>
        /// A <code>string</code> containing the localized name.
        /// </returns>
        /// <exception cref="CountryNotFoundException">If the country is not found</exception>
        public override string GetCountryDisplayName( string countryID, System.Globalization.CultureInfo c ) {
            string cacheKey = "COUNTRY_" + countryID + " - " + c.TwoLetterISOLanguageName;

            if ( CurrentCache.Get(cacheKey) == null ) {
                string result = GetLocalizedDisplayName( "COUNTRY_" + countryID, c );

                if ( string.IsNullOrEmpty( result ) ) {
                    result = this.GetCountry( countryID ).NeutralName;
                }

                CurrentCache.Insert( cacheKey, result );
            }

            return ( string )CurrentCache.Get( cacheKey );
        }

        /// <summary>
        /// 	<see cref="Rainbow.Framework.Providers.Geographic.GeographicProvider.GetStateDisplayName( int, System.Globalization.CultureInfo )"/>
        /// </summary>
        /// <param name="stateID"></param>
        /// <param name="c">a <code>System.Globalization.CultureInfo</code> describing the language we want the name for</param>
        /// <returns>
        /// A <code>string</code> containing the localized name.
        /// </returns>
        /// <exception cref="StateNotFoundException">If the state is not found</exception>
        public override string GetStateDisplayName( int stateID, System.Globalization.CultureInfo c ) {
            string cacheKey = "STATENAME_" + stateID + " - " + c.TwoLetterISOLanguageName;

            if ( CurrentCache.Get(cacheKey) == null ) {
                string result = GetLocalizedDisplayName( "STATENAME_" + stateID, c );

                if ( string.IsNullOrEmpty( result ) ) {
                    result = this.GetState( stateID ).NeutralName;
                }

                CurrentCache.Insert( cacheKey, result );
            }

            return ( string )CurrentCache.Get( cacheKey );
        }

        /// <summary>
        /// 	<see cref="Rainbow.Framework.Providers.Geographic.GeographicProvider.GetAdministrativeDivisionName( string, System.Globalization.CultureInfo )"/>
        /// </summary>
        /// <param name="administrativeDivisionName"></param>
        /// <param name="c">a <code>System.Globalization.CultureInfo</code> describing the language we want the name for</param>
        /// <returns>
        /// A <code>string</code> containing the localized name.
        /// </returns>
        public override string GetAdministrativeDivisionName( string administrativeDivisionName, System.Globalization.CultureInfo c ) {
            string cacheKey = "ADMINISTRATIVEDIVISIONNAME_" + administrativeDivisionName + " - " + c.TwoLetterISOLanguageName;

            if ( CurrentCache.Get(cacheKey) == null ) {
                string result = GetLocalizedDisplayName( "ADMINISTRATIVEDIVISIONNAME_" + administrativeDivisionName, c );

                if ( string.IsNullOrEmpty( result ) ) {
                    result = administrativeDivisionName;
                }

                CurrentCache.Insert( cacheKey, result );
            }

            return ( string )CurrentCache.Get( cacheKey );
        }

        /// <summary>
        /// 	<see cref="Rainbow.Framework.Providers.Geographic.GeographicProvider.GetCountry( string )"/>
        /// </summary>
        /// <param name="countryID">The country's id</param>
        /// <returns>
        /// A <code>Country</code> object containing the country info, or null if the country doesn't exist
        /// </returns>
        /// <exception cref="CountryNotFoundException">If the country is not found</exception>
        public override Country GetCountry( string countryID ) {
            Country result = null;

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "SELECT NeutralName, AdministrativeDivisionNeutralName FROM rb_Countries WHERE CountryID=@CountryID";
            cmd.Parameters.Add( "@CountryID", SqlDbType.NChar, 2 ).Value = countryID;

            cmd.Connection = ConnectionString;

            SqlDataReader reader = null;
            try {
                cmd.Connection.Open();

                reader = cmd.ExecuteReader();

                if ( !reader.Read() ) {
                    throw new CountryNotFoundException( string.Format( "The country with ID {0} was not found in the database", countryID ) );
                }
                result = new Country( countryID, reader.GetString( 0 ), reader.GetString( 1 ) );
            }
            finally {
                if ( reader != null ) {
                    reader.Close();
                }
                cmd.Connection.Close();
            }

            return result;
        }

        /// <summary>
        /// 	<see cref="Rainbow.Framework.Providers.Geographic.GeographicProvider.GetState( int )"/>
        /// </summary>
        /// <param name="stateID">The state's id</param>
        /// <returns>
        /// A <code>State</code> object containing the State info, or null if the state doesn't exist
        /// </returns>
        /// <exception cref="StateNotFoundException">If the state is not found</exception>
        public override State GetState( int stateID ) {
            State result = null;

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "SELECT CountryID, NeutralName FROM rb_States WHERE StateID=@StateID";
            cmd.Parameters.Add( "@StateID", SqlDbType.Int ).Value = stateID;

            cmd.Connection = ConnectionString;

            SqlDataReader reader = null;
            try {
                cmd.Connection.Open();

                reader = cmd.ExecuteReader();

                if ( !reader.Read() ) {
                    throw new StateNotFoundException( string.Format( "The state with ID {0} was not found in the database", stateID ) );
                }
                result = new State( stateID, reader.GetString( 0 ), reader.GetString( 1 ) );

            }
            finally {
                if ( reader != null ) {
                    reader.Close();
                }
                cmd.Connection.Close();
            }

            return result;
        }

        /// <summary>
        /// Gets the display name of the localized.
        /// </summary>
        /// <param name="textKey">The text key.</param>
        /// <param name="culture">The culture.</param>
        /// <returns></returns>
        private string GetLocalizedDisplayName( string textKey, CultureInfo culture ) {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "SELECT Description FROM rb_Localize WHERE Textkey=@TextKey AND CultureCode=@CultureCode";
            cmd.Parameters.Add( "@TextKey", SqlDbType.NVarChar, 255 ).Value = textKey;
            cmd.Parameters.Add( "@CultureCode", SqlDbType.NVarChar, 2 ).Value = culture.TwoLetterISOLanguageName;
            cmd.Connection = ConnectionString;

            SqlDataReader reader = null;
            try {
                cmd.Connection.Open();

                reader = cmd.ExecuteReader();

                if ( reader.Read() ) {
                    return reader.GetString( 0 );
                }
                else {
                    return null;
                }
            }
            finally {
                if ( reader != null ) {
                    reader.Close();
                }
                cmd.Connection.Close();
            }

        }

        /// <summary>
        /// Gets the countries core.
        /// </summary>
        /// <param name="configFilter">The config filter.</param>
        /// <param name="additionalFilter">The additional filter.</param>
        /// <param name="sortBY">The sort BY.</param>
        /// <returns></returns>
        private static IList<Country> GetCountriesCore( string configFilter, string additionalFilter, CountryFields sortBY ) {
            List<Country> result = new List<Country>();

            SqlCommand cmd = new SqlCommand();

            string filter = BuildCountriesFilter( configFilter, additionalFilter );

            if ( filter.Equals( string.Empty ) ) {
                cmd.CommandText = "rb_GetCountries";
            }
            else {
                cmd.CommandText = "rb_GetCountriesFiltered";
            }
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Connection = ConnectionString;

            if ( !filter.Equals( string.Empty ) ) {
                cmd.Parameters.Add( "@Filter", SqlDbType.NVarChar, 10000 ).Value = filter;
            }
            cmd.Parameters.Add( "@SortBy", SqlDbType.Int ).Value = ( int )sortBY;

            SqlDataReader reader = null;
            try {
                cmd.Connection.Open();

                reader = cmd.ExecuteReader();

                while ( reader.Read() ) {
                    result.Add( new Country( reader.GetString( 0 ), reader.GetString( 1 ), reader.GetString( 2 ) ) );
                }
            }
            finally {
                if ( reader != null ) {
                    reader.Close();
                }
                cmd.Connection.Close();
            }

            if ( sortBY == CountryFields.Name ) {
                result.Sort( new CountryNameComparer() );
            }

            return result;
        }

        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        protected static SqlConnection ConnectionString {
            get {

                SqlConnection result = null;
                try {
                    result = Config.SqlConnectionString;
                }
                catch {
                    // I'm in a test environment
                    result = new SqlConnection( ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString );
                }

                return result;
            }
        }

    }
}
