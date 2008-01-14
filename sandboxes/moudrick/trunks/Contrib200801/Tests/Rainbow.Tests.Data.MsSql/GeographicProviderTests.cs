using System;
using System.Collections.Generic;
using NUnit.Framework;
using System.Globalization;
using Rainbow.Framework.BusinessObjects;
using Rainbow.Framework.Providers;
using Rainbow.Framework.Providers.Geographic;

namespace Rainbow.Tests.Data.MsSql
{
   [TestFixture]
   public class GeographicProviderTests : BaseProviderTestFixture
   {
        [SetUp]
        public void Init() 
        {
            GeographicProvider.Instance.CountriesFilter = "AR,BR,CL,UY";
        }

        [Test]
        public void CountryCtor1() 
        {
            Country c = new Country();
            Assert.AreEqual( string.Empty, c.CountryID );
            Assert.AreEqual( string.Empty, c.NeutralName );
            Assert.AreEqual( string.Empty, c.AdministrativeDivisionNeutralName );
        }

        [Test]
        public void CountryCtor2() 
        {
            Country c = new Country( "BR", "Brazil", "State" );
            Assert.AreEqual( "BR", c.CountryID );
            Assert.AreEqual( "Brazil", c.NeutralName );
            Assert.AreEqual( "State", c.AdministrativeDivisionNeutralName );
        }

        [Test]
        public void StateCtor1() 
        {
            State s = new State();
            Assert.AreEqual( string.Empty, s.CountryID );
            Assert.AreEqual( string.Empty, s.NeutralName );
            Assert.AreEqual( 0, s.StateID );
        }

        [Test]
        public void StateCtor2() 
        {
            State s = new State( 1000, "UY", "aName" );
            Assert.AreEqual( "UY", s.CountryID );
            Assert.AreEqual( "aName", s.NeutralName );
            Assert.AreEqual( 1000, s.StateID );
        }

        [Test]
        public void ProviderInitialize() 
        {
            string filteredCountries = GeographicProvider.Instance.CountriesFilter;
            Assert.AreEqual( "AR,BR,CL,UY", filteredCountries );
        }

        [Test]
        public void GetCountries1() 
        {
            IList<Country> countries = GeographicProvider.Instance.GetCountries();
            Assert.AreEqual( countries.Count, 4 );  // AR, BR, UY and CL
        }

        [Test]
        public void GetCountriesLocalization1()
        {
            IList<Country> countries = GeographicProvider.Instance.GetCountries();

            CultureInfo currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo( "de-DE" );

            Assert.AreEqual( "Argentinien", countries[0].Name );
            Assert.AreEqual( "Brasilien", countries[1].Name );
            Assert.AreEqual( "Chile", countries[2].Name );
            Assert.AreEqual( "Uruguay", countries[3].Name );

            // again so we can test cached names
            Assert.AreEqual( "Argentinien", countries[0].Name );
            Assert.AreEqual( "Brasilien", countries[1].Name );
            Assert.AreEqual( "Chile", countries[2].Name );
            Assert.AreEqual( "Uruguay", countries[3].Name );

            System.Threading.Thread.CurrentThread.CurrentCulture = currentCulture;
        }

        [Test]
        public void GetSortedCountries1()
        {
            IList<Country> countries = GeographicProvider.Instance.GetCountries( CountryFields.CountryID );
            Assert.AreEqual( countries.Count, 4 );  // AR, BR, UY and CL
        }

        [Test]
        public void GetSortedCountries2()
        {
            IList<Country> countries = GeographicProvider.Instance.GetCountries( CountryFields.CountryID );

            Assert.AreEqual( "AR", countries[0].CountryID );
            Assert.AreEqual( "BR", countries[1].CountryID );
            Assert.AreEqual( "CL", countries[2].CountryID );
            Assert.AreEqual( "UY", countries[3].CountryID );

            Assert.AreEqual( "Argentina", countries[0].NeutralName );
            Assert.AreEqual( "Brazil", countries[1].NeutralName );
            Assert.AreEqual( "Chile", countries[2].NeutralName );
            Assert.AreEqual( "Uruguay", countries[3].NeutralName );
        }

        [Test]
        public void GetSortedCountries3() 
        {
            IList<Country> countries = GeographicProvider.Instance.GetCountries( CountryFields.NeutralName );

            Assert.AreEqual( "AR", countries[0].CountryID );
            Assert.AreEqual( "BR", countries[1].CountryID );
            Assert.AreEqual( "CL", countries[2].CountryID );
            Assert.AreEqual( "UY", countries[3].CountryID );

            Assert.AreEqual( "Argentina", countries[0].NeutralName );
            Assert.AreEqual( "Brazil", countries[1].NeutralName );
            Assert.AreEqual( "Chile", countries[2].NeutralName );
            Assert.AreEqual( "Uruguay", countries[3].NeutralName );
        }

        [Test]
        public void GetSortedCountries4()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo( "es-ES" );
            IList<Country> countries = GeographicProvider.Instance.GetCountries( CountryFields.Name );

            Assert.AreEqual( "AR", countries[0].CountryID );
            Assert.AreEqual( "BR", countries[1].CountryID );
            Assert.AreEqual( "CL", countries[2].CountryID );
            Assert.AreEqual( "UY", countries[3].CountryID );

            Assert.AreEqual( "Argentina", countries[0].NeutralName );
            Assert.AreEqual( "Brazil", countries[1].NeutralName );
            Assert.AreEqual( "Chile", countries[2].NeutralName );
            Assert.AreEqual( "Uruguay", countries[3].NeutralName );
        }

        [Test]
        public void GetSortedCountries5() {
            IList<Country> countries = GeographicProvider.Instance.GetCountries( CountryFields.None );

            Assert.AreEqual( "AR", countries[0].CountryID );
            Assert.AreEqual( "BR", countries[1].CountryID );
            Assert.AreEqual( "CL", countries[2].CountryID );
            Assert.AreEqual( "UY", countries[3].CountryID );

            Assert.AreEqual( "Argentina", countries[0].NeutralName );
            Assert.AreEqual( "Brazil", countries[1].NeutralName );
            Assert.AreEqual( "Chile", countries[2].NeutralName );
            Assert.AreEqual( "Uruguay", countries[3].NeutralName );
        }

        [Test]
        public void GetCountriesFiltered1()
        {
            IList<Country> countries = GeographicProvider.Instance.GetCountries( "AR,UY" );
            Assert.AreEqual( countries.Count, 2 );  // AR, and UY 

            Assert.AreEqual( "AR", countries[0].CountryID );
            Assert.AreEqual( "UY", countries[1].CountryID );
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void GetCountriesFiltered2() 
        {
            GeographicProvider.Instance.GetCountries( "LongFilter" );
        }

        [Test]
        public void GetFilteredSortedCountries1() 
        {
            IList<Country> countries = GeographicProvider.Instance.GetCountries( "AR,UY", CountryFields.CountryID );
            Assert.AreEqual( countries.Count, 2 );  // AR, UY 
        }

        [Test]
        public void GetFilteredSortedCountries2()
        {
            IList<Country> countries = GeographicProvider.Instance.GetCountries( "AR,UY", CountryFields.CountryID );

            Assert.AreEqual( "AR", countries[0].CountryID );
            Assert.AreEqual( "UY", countries[1].CountryID );

            Assert.AreEqual( "Argentina", countries[0].NeutralName );
            Assert.AreEqual( "Uruguay", countries[1].NeutralName );
        }

        [Test]
        public void GetFilteredSortedCountries3() 
        {
            IList<Country> countries = GeographicProvider.Instance.GetCountries( "AR,UY", CountryFields.NeutralName );

            Assert.AreEqual( "AR", countries[0].CountryID );
            Assert.AreEqual( "UY", countries[1].CountryID );

            Assert.AreEqual( "Argentina", countries[0].NeutralName );
            Assert.AreEqual( "Uruguay", countries[1].NeutralName );
        }

        [Test]
        public void GetFilteredSortedCountries4() 
        {
            IList<Country> countries = GeographicProvider.Instance.GetCountries( "AR,UY", CountryFields.CountryID );

            Assert.AreEqual( "AR", countries[0].CountryID );
            Assert.AreEqual( "UY", countries[1].CountryID );

            Assert.AreEqual( "Argentina", countries[0].NeutralName );
            Assert.AreEqual( "Uruguay", countries[1].NeutralName );
        }

        [Test]
        public void GetFilteredSortedCountries5() 
        {
            IList<Country> countries = GeographicProvider.Instance.GetCountries( "AR,UY", CountryFields.None );

            Assert.AreEqual( "AR", countries[0].CountryID );
            Assert.AreEqual( "UY", countries[1].CountryID );

            Assert.AreEqual( "Argentina", countries[0].NeutralName );
            Assert.AreEqual( "Uruguay", countries[1].NeutralName );
        }

        [Test]
        public void GetSortedCountriesNoCountryFilter1() 
        {
            GeographicProvider.Instance.CountriesFilter = string.Empty;
            IList<Country> countries = GeographicProvider.Instance.GetCountries( CountryFields.CountryID );
            Assert.AreEqual( countries.Count, 237 );  
        }

        [Test]
        public void GetSortedCountriesNoCountryFilter2() 
        {
            GeographicProvider.Instance.CountriesFilter = string.Empty;
            IList<Country> countries = GeographicProvider.Instance.GetCountries( CountryFields.CountryID );

            Assert.AreEqual( "AD", countries[0].CountryID );
            Assert.AreEqual( "AE", countries[1].CountryID );
            Assert.AreEqual( "AF", countries[2].CountryID );
            Assert.AreEqual( "ZW", countries[236].CountryID );

            Assert.AreEqual( "Andorra", countries[0].NeutralName );
            Assert.AreEqual( "United Arab Emirates", countries[1].NeutralName );
            Assert.AreEqual( "Afghanistan", countries[2].NeutralName );
            Assert.AreEqual( "Zimbabwe", countries[236].NeutralName );
        }

        [Test]
        public void GetSortedCountriesNoCountryFilter3() 
        {
            GeographicProvider.Instance.CountriesFilter = string.Empty;
            IList<Country> countries = GeographicProvider.Instance.GetCountries( CountryFields.NeutralName );

            Assert.AreEqual( "AF", countries[0].CountryID );
            Assert.AreEqual( "AL", countries[1].CountryID );
            Assert.AreEqual( "ZM", countries[235].CountryID );
            Assert.AreEqual( "ZW", countries[236].CountryID );

            Assert.AreEqual( "Afghanistan", countries[0].NeutralName );
            Assert.AreEqual( "Albania", countries[1].NeutralName );
            Assert.AreEqual( "Zambia", countries[235].NeutralName );
            Assert.AreEqual( "Zimbabwe", countries[236].NeutralName );
        }

        [Test]
        public void GetSortedCountriesNoCountryFilter4()
        {
            GeographicProvider.Instance.CountriesFilter = string.Empty;
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo( "es-ES" );
            IList<Country> countries = GeographicProvider.Instance.GetCountries( CountryFields.Name );

            Assert.AreEqual( "AF", countries[0].CountryID );
            Assert.AreEqual( "AL", countries[1].CountryID );
            Assert.AreEqual( "ZM", countries[235].CountryID );
            Assert.AreEqual( "ZW", countries[236].CountryID );

            Assert.AreEqual( "Afghanistan", countries[0].NeutralName );
            Assert.AreEqual( "Albania", countries[1].NeutralName );
            Assert.AreEqual( "Zambia", countries[235].NeutralName );
            Assert.AreEqual( "Zimbabwe", countries[236].NeutralName );
        }

        [Test]
        public void GetSortedCountriesNoCountryFilter5()
        {
            GeographicProvider.Instance.CountriesFilter = string.Empty;
            IList<Country> countries = GeographicProvider.Instance.GetCountries( CountryFields.None );

            Assert.AreEqual( "AD", countries[0].CountryID );
            Assert.AreEqual( "AE", countries[1].CountryID );
            Assert.AreEqual( "ZM", countries[235].CountryID );
            Assert.AreEqual( "ZW", countries[236].CountryID );

            Assert.AreEqual( "Andorra", countries[0].NeutralName );
            Assert.AreEqual( "United Arab Emirates", countries[1].NeutralName );
            Assert.AreEqual( "Zambia", countries[235].NeutralName );
            Assert.AreEqual( "Zimbabwe", countries[236].NeutralName );
        }

        [Test]
        public void GetCountriesFilteredNoCountryFilter1()
        {
            GeographicProvider.Instance.CountriesFilter = string.Empty;
            IList<Country> countries = GeographicProvider.Instance.GetCountries( "AR,UY" );
            Assert.AreEqual( countries.Count, 2 );  // AR, and UY 

            Assert.AreEqual( "AR", countries[0].CountryID );
            Assert.AreEqual( "UY", countries[1].CountryID );
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void GetCountriesFilteredNoCountryFilter2() 
        {
            GeographicProvider.Instance.CountriesFilter = string.Empty;
            GeographicProvider.Instance.GetCountries( "LongFilter" );
        }

        [Test]
        public void GetCountriesSortedNoCountryFilter1()
        {
            GeographicProvider.Instance.CountriesFilter = string.Empty;
            IList<Country> countries = GeographicProvider.Instance.GetCountries( CountryFields.CountryID );
            Assert.AreEqual( countries.Count, 237 );  // AR, BR, UY and CL
        }

        [Test]
        public void GetFilteredSortedCountriesNoCountryFilter1() 
        {
            GeographicProvider.Instance.CountriesFilter = string.Empty;
            IList<Country> countries = GeographicProvider.Instance.GetCountries( "AR,UY", CountryFields.CountryID );
            Assert.AreEqual( countries.Count, 2 );  // AR, UY 
        }

        [Test]
        public void GetFilteredSortedCountriesNoCountryFilter2()
        {
            GeographicProvider.Instance.CountriesFilter = string.Empty;
            IList<Country> countries = GeographicProvider.Instance.GetCountries( "AR,UY", CountryFields.CountryID );

            Assert.AreEqual( "AR", countries[0].CountryID );
            Assert.AreEqual( "UY", countries[1].CountryID );

            Assert.AreEqual( "Argentina", countries[0].NeutralName );
            Assert.AreEqual( "Uruguay", countries[1].NeutralName );
        }

        [Test]
        public void GetFilteredSortedCountriesNoCountryFilter3()
        {
            GeographicProvider.Instance.CountriesFilter = string.Empty;
            IList<Country> countries = GeographicProvider.Instance.GetCountries( "AR,UY", CountryFields.NeutralName );

            Assert.AreEqual( "AR", countries[0].CountryID );
            Assert.AreEqual( "UY", countries[1].CountryID );

            Assert.AreEqual( "Argentina", countries[0].NeutralName );
            Assert.AreEqual( "Uruguay", countries[1].NeutralName );
        }

        [Test]
        public void GetFilteredSortedCountriesNoCountryFilter4()
        {
            GeographicProvider.Instance.CountriesFilter = string.Empty;
            IList<Country> countries = GeographicProvider.Instance.GetCountries( "AR,UY", CountryFields.CountryID );

            Assert.AreEqual( "AR", countries[0].CountryID );
            Assert.AreEqual( "UY", countries[1].CountryID );

            Assert.AreEqual( "Argentina", countries[0].NeutralName );
            Assert.AreEqual( "Uruguay", countries[1].NeutralName );
        }

        [Test]
        public void GetFilteredSortedCountriesNoCountryFilter5() 
        {
            GeographicProvider.Instance.CountriesFilter = string.Empty;
            IList<Country> countries = GeographicProvider.Instance.GetCountries( "AR,UY", CountryFields.None );

            Assert.AreEqual( "AR", countries[0].CountryID );
            Assert.AreEqual( "UY", countries[1].CountryID );

            Assert.AreEqual( "Argentina", countries[0].NeutralName );
            Assert.AreEqual( "Uruguay", countries[1].NeutralName );
        }

        [Test]
        public void GetUnfilteredCountries1() 
        {
            IList<Country> allCountries = GeographicProvider.Instance.GetUnfilteredCountries();
            Assert.AreEqual( 237, allCountries.Count );
        }

        [Test]
        public void GetCountryStates1()
        {
            IList<State> states = GeographicProvider.Instance.GetCountryStates( "AE" );

            Assert.AreEqual( 4, states.Count );

            foreach ( State s in states ) {
                switch ( s.StateID ) {
                    case 599:
                        Assert.AreEqual( "Abu Dhabi", s.NeutralName );
                        Assert.AreEqual( "AE", s.CountryID );
                        break;
                    case 2082:
                        Assert.AreEqual( "Ash Shariqah", s.NeutralName );
                        Assert.AreEqual( "AE", s.CountryID );
                        break;
                    case 9470:
                        Assert.AreEqual( "Dubai", s.NeutralName );
                        Assert.AreEqual( "AE", s.CountryID );
                        break;
                    case 9217877:
                        Assert.AreEqual( "Al l'Ayn", s.NeutralName );
                        Assert.AreEqual( "AE", s.CountryID );
                        break;
                    default:
                        Assert.Fail();
                        break;
                }
            }
        }

        [Test]
        public void GetCountryStates2()
        {
            IList<State> states = GeographicProvider.Instance.GetCountryStates( "PP" );
            Assert.AreEqual( 0, states.Count );
        }

        [Test]
        public void GetCountryDisplayName1()
        {
            string displayName = GeographicProvider.Instance.GetCountryDisplayName( "BR", new CultureInfo( "es-ES" ) );
            Assert.AreEqual( "Brasil", displayName );
        }

        [Test]
        [ExpectedException(typeof(CountryNotFoundException))]
        public void GetCountryDisplayName2() 
        {
            GeographicProvider.Instance.GetCountryDisplayName( "ZZ", new CultureInfo( "es-ES" ) );
        }

        [Test]
        public void GetStateDisplayName1() 
        {
            string displayName = GeographicProvider.Instance.GetStateDisplayName( 1003, new CultureInfo( "en-US" ) );
            Assert.AreEqual( "Alabama", displayName );
        }

        [Test]
       [ExpectedException(typeof(StateNotFoundException))]
        public void GetStateDisplayName2() 
        {
            GeographicProvider.Instance.GetStateDisplayName( -40, new CultureInfo( "en-US" ) );
        }

        [Test]
        public void GetAdministrativeDivisionName1() 
        {
            string displayName = GeographicProvider.Instance.GetAdministrativeDivisionName( "Department", new CultureInfo( "es-ES" ) );
            Assert.AreEqual( "Department", displayName );
        }

        [Test]
        public void GetCountry1()
        {
            Country c = GeographicProvider.Instance.GetCountry( "US" );

            Assert.AreEqual( "US", c.CountryID );
            Assert.AreEqual( "United States", c.NeutralName );
        }

        [Test]
        public void GetCountry2()
        {
            Country c = GeographicProvider.Instance.GetCountry( "US" );

            CultureInfo currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo( "de-DE" );
            Assert.AreEqual( "Vereinigte Staaten von Amerika", c.Name );
            System.Threading.Thread.CurrentThread.CurrentCulture = currentCulture;
        }

        [Test]
        [ExpectedException(typeof(CountryNotFoundException))]
        public void GetCountry3() 
        {
            GeographicProvider.Instance.GetCountry( ".." );
        }

        [Test]
        public void GetState1() 
        {
            State state = GeographicProvider.Instance.GetState( 1003 );
            Assert.AreEqual( "Alabama", state.NeutralName );
            Assert.AreEqual( 1003, state.StateID );
            Assert.AreEqual( "US", state.CountryID );
        }

        [Test]
        [ExpectedException(typeof(StateNotFoundException))]
        public void GetState2()
        {
            GeographicProvider.Instance.GetState( -100 );
        }

        [Test]
        public void CurrentCountry1()
        {
            Country actual = GeographicProvider.Instance.CurrentCountry;
            Country expected = GeographicProvider.Instance.GetCountry( RegionInfo.CurrentRegion.Name );
            Assert.AreEqual( expected, actual );
        }
    }
}
