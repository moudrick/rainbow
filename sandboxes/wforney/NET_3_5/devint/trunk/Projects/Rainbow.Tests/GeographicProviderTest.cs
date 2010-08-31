namespace Rainbow.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Threading;

    using NUnit.Framework;

    using Rainbow.Framework.Data.Providers.Geographic;

    /// <summary>
    /// The geographic provider test.
    /// </summary>
    [TestFixture]
    public class GeographicProviderTest
    {
        #region Public Methods

        /// <summary>
        /// The country ctor test 1.
        /// </summary>
        [Test]
        public void CountryCtorTest1()
        {
            var c = new Country();
            Assert.AreEqual(string.Empty, c.CountryID);
            Assert.AreEqual(string.Empty, c.NeutralName);
            Assert.AreEqual(string.Empty, c.AdministrativeDivisionNeutralName);
        }

        /// <summary>
        /// The country ctor test 2.
        /// </summary>
        [Test]
        public void CountryCtorTest2()
        {
            var c = new Country("BR", "Brazil", "State");
            Assert.AreEqual("BR", c.CountryID);
            Assert.AreEqual("Brazil", c.NeutralName);
            Assert.AreEqual("State", c.AdministrativeDivisionNeutralName);
        }

        /// <summary>
        /// The current country test 1.
        /// </summary>
        [Test]
        public void CurrentCountryTest1()
        {
            Country actual = GeographicProvider.Current.CurrentCountry;
            Country expected = GeographicProvider.Current.GetCountry(RegionInfo.CurrentRegion.Name);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// The fixture set up.
        /// </summary>
        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            // Set up initial database environment for testing purposes
            TestHelper.TearDownDb();
            TestHelper.RecreateDbSchema();
        }

        /// <summary>
        /// The get administrative division name test 1.
        /// </summary>
        [Test]
        public void GetAdministrativeDivisionNameTest1()
        {
            string displayName = GeographicProvider.Current.GetAdministrativeDivisionName(
                "Department", new CultureInfo("es-ES"));
            Assert.AreEqual("Department", displayName);
        }

        /// <summary>
        /// The get countries filtered no country filter test 1.
        /// </summary>
        [Test]
        public void GetCountriesFilteredNoCountryFilterTest1()
        {
            GeographicProvider.Current.CountriesFilter = string.Empty;
            IList<Country> countries = GeographicProvider.Current.GetCountries("AR,UY");
            Assert.AreEqual(countries.Count, 2); // AR, and UY 

            Assert.AreEqual("AR", countries[0].CountryID);
            Assert.AreEqual("UY", countries[1].CountryID);
        }

        /// <summary>
        /// The get countries filtered no country filter test 2.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void GetCountriesFilteredNoCountryFilterTest2()
        {
            GeographicProvider.Current.CountriesFilter = string.Empty;
            IList<Country> countries = GeographicProvider.Current.GetCountries("LongFilter");
        }

        /// <summary>
        /// The get countries filtered test 1.
        /// </summary>
        [Test]
        public void GetCountriesFilteredTest1()
        {
            IList<Country> countries = GeographicProvider.Current.GetCountries("AR,UY");
            Assert.AreEqual(countries.Count, 2); // AR, and UY 

            Assert.AreEqual("AR", countries[0].CountryID);
            Assert.AreEqual("UY", countries[1].CountryID);
        }

        /// <summary>
        /// The get countries filtered test 2.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void GetCountriesFilteredTest2()
        {
            GeographicProvider.Current.GetCountries("LongFilter");
        }

        /// <summary>
        /// The get countries localization test 1.
        /// </summary>
        [Test]
        public void GetCountriesLocalizationTest1()
        {
            IList<Country> countries = GeographicProvider.Current.GetCountries();

            var currentCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = new CultureInfo("de-DE");

            Assert.AreEqual("Argentinien", countries[0].Name);
            Assert.AreEqual("Brasilien", countries[1].Name);
            Assert.AreEqual("Chile", countries[2].Name);
            Assert.AreEqual("Uruguay", countries[3].Name);

            // again so we can test cached names
            Assert.AreEqual("Argentinien", countries[0].Name);
            Assert.AreEqual("Brasilien", countries[1].Name);
            Assert.AreEqual("Chile", countries[2].Name);
            Assert.AreEqual("Uruguay", countries[3].Name);

            Thread.CurrentThread.CurrentCulture = currentCulture;
        }

        /// <summary>
        /// The get countries sorted no country filter test 1.
        /// </summary>
        [Test]
        public void GetCountriesSortedNoCountryFilterTest1()
        {
            GeographicProvider.Current.CountriesFilter = string.Empty;
            IList<Country> countries = GeographicProvider.Current.GetCountries(CountryFields.CountryID);
            Assert.AreEqual(countries.Count, 237); // AR, BR, UY and CL
        }

        /// <summary>
        /// The get countries test 1.
        /// </summary>
        [Test]
        public void GetCountriesTest1()
        {
            IList<Country> countries = GeographicProvider.Current.GetCountries();
            Assert.AreEqual(countries.Count, 4); // AR, BR, UY and CL
        }

        /// <summary>
        /// The get country display name test 1.
        /// </summary>
        [Test]
        public void GetCountryDisplayNameTest1()
        {
            string displayName = GeographicProvider.Current.GetCountryDisplayName("BR", new CultureInfo("es-ES"));
            Assert.AreEqual("Brasil", displayName);
        }

        /// <summary>
        /// The get country display name test 2.
        /// </summary>
        [Test]
        [ExpectedException(typeof(CountryNotFoundException))]
        public void GetCountryDisplayNameTest2()
        {
            GeographicProvider.Current.GetCountryDisplayName("ZZ", new CultureInfo("es-ES"));
        }

        /// <summary>
        /// The get country states test 1.
        /// </summary>
        [Test]
        public void GetCountryStatesTest1()
        {
            IList<State> states = GeographicProvider.Current.GetCountryStates("AE");

            Assert.AreEqual(4, states.Count);

            foreach (var s in states)
            {
                switch (s.StateID)
                {
                    case 599:
                        Assert.AreEqual("Abu Dhabi", s.NeutralName);
                        Assert.AreEqual("AE", s.CountryID);
                        break;
                    case 2082:
                        Assert.AreEqual("Ash Shariqah", s.NeutralName);
                        Assert.AreEqual("AE", s.CountryID);
                        break;
                    case 9470:
                        Assert.AreEqual("Dubai", s.NeutralName);
                        Assert.AreEqual("AE", s.CountryID);
                        break;
                    case 9217877:
                        Assert.AreEqual("Al l'Ayn", s.NeutralName);
                        Assert.AreEqual("AE", s.CountryID);
                        break;
                    default:
                        Assert.Fail();
                        break;
                }
            }
        }

        /// <summary>
        /// The get country states test 2.
        /// </summary>
        [Test]
        public void GetCountryStatesTest2()
        {
            IList<State> states = GeographicProvider.Current.GetCountryStates("PP");
            Assert.AreEqual(0, states.Count);
        }

        /// <summary>
        /// The get country test 1.
        /// </summary>
        [Test]
        public void GetCountryTest1()
        {
            Country c = GeographicProvider.Current.GetCountry("US");

            Assert.AreEqual("US", c.CountryID);
            Assert.AreEqual("United States", c.NeutralName);
        }

        /// <summary>
        /// The get country test 2.
        /// </summary>
        [Test]
        public void GetCountryTest2()
        {
            Country c = GeographicProvider.Current.GetCountry("US");

            var currentCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = new CultureInfo("de-DE");
            Assert.AreEqual("Vereinigte Staaten von Amerika", c.Name);
            Thread.CurrentThread.CurrentCulture = currentCulture;
        }

        /// <summary>
        /// The get country test 3.
        /// </summary>
        [Test]
        [ExpectedException(typeof(CountryNotFoundException))]
        public void GetCountryTest3()
        {
            GeographicProvider.Current.GetCountry("..");
        }

        /// <summary>
        /// The get filtered sorted countries no country filter test 1.
        /// </summary>
        [Test]
        public void GetFilteredSortedCountriesNoCountryFilterTest1()
        {
            GeographicProvider.Current.CountriesFilter = string.Empty;
            IList<Country> countries = GeographicProvider.Current.GetCountries("AR,UY", CountryFields.CountryID);
            Assert.AreEqual(countries.Count, 2); // AR, UY 
        }

        /// <summary>
        /// The get filtered sorted countries no country filter test 2.
        /// </summary>
        [Test]
        public void GetFilteredSortedCountriesNoCountryFilterTest2()
        {
            GeographicProvider.Current.CountriesFilter = string.Empty;
            IList<Country> countries = GeographicProvider.Current.GetCountries("AR,UY", CountryFields.CountryID);

            Assert.AreEqual("AR", countries[0].CountryID);
            Assert.AreEqual("UY", countries[1].CountryID);

            Assert.AreEqual("Argentina", countries[0].NeutralName);
            Assert.AreEqual("Uruguay", countries[1].NeutralName);
        }

        /// <summary>
        /// The get filtered sorted countries no country filter test 3.
        /// </summary>
        [Test]
        public void GetFilteredSortedCountriesNoCountryFilterTest3()
        {
            GeographicProvider.Current.CountriesFilter = string.Empty;
            IList<Country> countries = GeographicProvider.Current.GetCountries("AR,UY", CountryFields.NeutralName);

            Assert.AreEqual("AR", countries[0].CountryID);
            Assert.AreEqual("UY", countries[1].CountryID);

            Assert.AreEqual("Argentina", countries[0].NeutralName);
            Assert.AreEqual("Uruguay", countries[1].NeutralName);
        }

        /// <summary>
        /// The get filtered sorted countries no country filter test 4.
        /// </summary>
        [Test]
        public void GetFilteredSortedCountriesNoCountryFilterTest4()
        {
            GeographicProvider.Current.CountriesFilter = string.Empty;
            IList<Country> countries = GeographicProvider.Current.GetCountries("AR,UY", CountryFields.CountryID);

            Assert.AreEqual("AR", countries[0].CountryID);
            Assert.AreEqual("UY", countries[1].CountryID);

            Assert.AreEqual("Argentina", countries[0].NeutralName);
            Assert.AreEqual("Uruguay", countries[1].NeutralName);
        }

        /// <summary>
        /// The get filtered sorted countries no country filter test 5.
        /// </summary>
        [Test]
        public void GetFilteredSortedCountriesNoCountryFilterTest5()
        {
            GeographicProvider.Current.CountriesFilter = string.Empty;
            IList<Country> countries = GeographicProvider.Current.GetCountries("AR,UY", CountryFields.None);

            Assert.AreEqual("AR", countries[0].CountryID);
            Assert.AreEqual("UY", countries[1].CountryID);

            Assert.AreEqual("Argentina", countries[0].NeutralName);
            Assert.AreEqual("Uruguay", countries[1].NeutralName);
        }

        /// <summary>
        /// The get filtered sorted countries test 1.
        /// </summary>
        [Test]
        public void GetFilteredSortedCountriesTest1()
        {
            IList<Country> countries = GeographicProvider.Current.GetCountries("AR,UY", CountryFields.CountryID);
            Assert.AreEqual(countries.Count, 2); // AR, UY 
        }

        /// <summary>
        /// The get filtered sorted countries test 2.
        /// </summary>
        [Test]
        public void GetFilteredSortedCountriesTest2()
        {
            IList<Country> countries = GeographicProvider.Current.GetCountries("AR,UY", CountryFields.CountryID);

            Assert.AreEqual("AR", countries[0].CountryID);
            Assert.AreEqual("UY", countries[1].CountryID);

            Assert.AreEqual("Argentina", countries[0].NeutralName);
            Assert.AreEqual("Uruguay", countries[1].NeutralName);
        }

        /// <summary>
        /// The get filtered sorted countries test 3.
        /// </summary>
        [Test]
        public void GetFilteredSortedCountriesTest3()
        {
            IList<Country> countries = GeographicProvider.Current.GetCountries("AR,UY", CountryFields.NeutralName);

            Assert.AreEqual("AR", countries[0].CountryID);
            Assert.AreEqual("UY", countries[1].CountryID);

            Assert.AreEqual("Argentina", countries[0].NeutralName);
            Assert.AreEqual("Uruguay", countries[1].NeutralName);
        }

        /// <summary>
        /// The get filtered sorted countries test 4.
        /// </summary>
        [Test]
        public void GetFilteredSortedCountriesTest4()
        {
            IList<Country> countries = GeographicProvider.Current.GetCountries("AR,UY", CountryFields.CountryID);

            Assert.AreEqual("AR", countries[0].CountryID);
            Assert.AreEqual("UY", countries[1].CountryID);

            Assert.AreEqual("Argentina", countries[0].NeutralName);
            Assert.AreEqual("Uruguay", countries[1].NeutralName);
        }

        /// <summary>
        /// The get filtered sorted countries test 5.
        /// </summary>
        [Test]
        public void GetFilteredSortedCountriesTest5()
        {
            IList<Country> countries = GeographicProvider.Current.GetCountries("AR,UY", CountryFields.None);

            Assert.AreEqual("AR", countries[0].CountryID);
            Assert.AreEqual("UY", countries[1].CountryID);

            Assert.AreEqual("Argentina", countries[0].NeutralName);
            Assert.AreEqual("Uruguay", countries[1].NeutralName);
        }

        /// <summary>
        /// The get sorted countries no country filter test 1.
        /// </summary>
        [Test]
        public void GetSortedCountriesNoCountryFilterTest1()
        {
            GeographicProvider.Current.CountriesFilter = string.Empty;
            IList<Country> countries = GeographicProvider.Current.GetCountries(CountryFields.CountryID);
            Assert.AreEqual(countries.Count, 237);
        }

        /// <summary>
        /// The get sorted countries no country filter test 2.
        /// </summary>
        [Test]
        public void GetSortedCountriesNoCountryFilterTest2()
        {
            GeographicProvider.Current.CountriesFilter = string.Empty;
            IList<Country> countries = GeographicProvider.Current.GetCountries(CountryFields.CountryID);

            Assert.AreEqual("AD", countries[0].CountryID);
            Assert.AreEqual("AE", countries[1].CountryID);
            Assert.AreEqual("AF", countries[2].CountryID);
            Assert.AreEqual("ZW", countries[236].CountryID);

            Assert.AreEqual("Andorra", countries[0].NeutralName);
            Assert.AreEqual("United Arab Emirates", countries[1].NeutralName);
            Assert.AreEqual("Afghanistan", countries[2].NeutralName);
            Assert.AreEqual("Zimbabwe", countries[236].NeutralName);
        }

        /// <summary>
        /// The get sorted countries no country filter test 3.
        /// </summary>
        [Test]
        public void GetSortedCountriesNoCountryFilterTest3()
        {
            GeographicProvider.Current.CountriesFilter = string.Empty;
            IList<Country> countries = GeographicProvider.Current.GetCountries(CountryFields.NeutralName);

            Assert.AreEqual("AF", countries[0].CountryID);
            Assert.AreEqual("AL", countries[1].CountryID);
            Assert.AreEqual("ZM", countries[235].CountryID);
            Assert.AreEqual("ZW", countries[236].CountryID);

            Assert.AreEqual("Afghanistan", countries[0].NeutralName);
            Assert.AreEqual("Albania", countries[1].NeutralName);
            Assert.AreEqual("Zambia", countries[235].NeutralName);
            Assert.AreEqual("Zimbabwe", countries[236].NeutralName);
        }

        /// <summary>
        /// The get sorted countries no country filter test 4.
        /// </summary>
        [Test]
        public void GetSortedCountriesNoCountryFilterTest4()
        {
            GeographicProvider.Current.CountriesFilter = string.Empty;
            Thread.CurrentThread.CurrentCulture = new CultureInfo("es-ES");
            IList<Country> countries = GeographicProvider.Current.GetCountries(CountryFields.Name);

            Assert.AreEqual("AF", countries[0].CountryID);
            Assert.AreEqual("AL", countries[1].CountryID);
            Assert.AreEqual("ZM", countries[235].CountryID);
            Assert.AreEqual("ZW", countries[236].CountryID);

            Assert.AreEqual("Afghanistan", countries[0].NeutralName);
            Assert.AreEqual("Albania", countries[1].NeutralName);
            Assert.AreEqual("Zambia", countries[235].NeutralName);
            Assert.AreEqual("Zimbabwe", countries[236].NeutralName);
        }

        /// <summary>
        /// The get sorted countries no country filter test 5.
        /// </summary>
        [Test]
        public void GetSortedCountriesNoCountryFilterTest5()
        {
            GeographicProvider.Current.CountriesFilter = string.Empty;
            IList<Country> countries = GeographicProvider.Current.GetCountries(CountryFields.None);

            Assert.AreEqual("AD", countries[0].CountryID);
            Assert.AreEqual("AE", countries[1].CountryID);
            Assert.AreEqual("ZM", countries[235].CountryID);
            Assert.AreEqual("ZW", countries[236].CountryID);

            Assert.AreEqual("Andorra", countries[0].NeutralName);
            Assert.AreEqual("United Arab Emirates", countries[1].NeutralName);
            Assert.AreEqual("Zambia", countries[235].NeutralName);
            Assert.AreEqual("Zimbabwe", countries[236].NeutralName);
        }

        /// <summary>
        /// The get sorted countries test 1.
        /// </summary>
        [Test]
        public void GetSortedCountriesTest1()
        {
            IList<Country> countries = GeographicProvider.Current.GetCountries(CountryFields.CountryID);
            Assert.AreEqual(countries.Count, 4); // AR, BR, UY and CL
        }

        /// <summary>
        /// The get sorted countries test 2.
        /// </summary>
        [Test]
        public void GetSortedCountriesTest2()
        {
            IList<Country> countries = GeographicProvider.Current.GetCountries(CountryFields.CountryID);

            Assert.AreEqual("AR", countries[0].CountryID);
            Assert.AreEqual("BR", countries[1].CountryID);
            Assert.AreEqual("CL", countries[2].CountryID);
            Assert.AreEqual("UY", countries[3].CountryID);

            Assert.AreEqual("Argentina", countries[0].NeutralName);
            Assert.AreEqual("Brazil", countries[1].NeutralName);
            Assert.AreEqual("Chile", countries[2].NeutralName);
            Assert.AreEqual("Uruguay", countries[3].NeutralName);
        }

        /// <summary>
        /// The get sorted countries test 3.
        /// </summary>
        [Test]
        public void GetSortedCountriesTest3()
        {
            IList<Country> countries = GeographicProvider.Current.GetCountries(CountryFields.NeutralName);

            Assert.AreEqual("AR", countries[0].CountryID);
            Assert.AreEqual("BR", countries[1].CountryID);
            Assert.AreEqual("CL", countries[2].CountryID);
            Assert.AreEqual("UY", countries[3].CountryID);

            Assert.AreEqual("Argentina", countries[0].NeutralName);
            Assert.AreEqual("Brazil", countries[1].NeutralName);
            Assert.AreEqual("Chile", countries[2].NeutralName);
            Assert.AreEqual("Uruguay", countries[3].NeutralName);
        }

        /// <summary>
        /// The get sorted countries test 4.
        /// </summary>
        [Test]
        public void GetSortedCountriesTest4()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("es-ES");
            IList<Country> countries = GeographicProvider.Current.GetCountries(CountryFields.Name);

            Assert.AreEqual("AR", countries[0].CountryID);
            Assert.AreEqual("BR", countries[1].CountryID);
            Assert.AreEqual("CL", countries[2].CountryID);
            Assert.AreEqual("UY", countries[3].CountryID);

            Assert.AreEqual("Argentina", countries[0].NeutralName);
            Assert.AreEqual("Brazil", countries[1].NeutralName);
            Assert.AreEqual("Chile", countries[2].NeutralName);
            Assert.AreEqual("Uruguay", countries[3].NeutralName);
        }

        /// <summary>
        /// The get sorted countries test 5.
        /// </summary>
        [Test]
        public void GetSortedCountriesTest5()
        {
            IList<Country> countries = GeographicProvider.Current.GetCountries(CountryFields.None);

            Assert.AreEqual("AR", countries[0].CountryID);
            Assert.AreEqual("BR", countries[1].CountryID);
            Assert.AreEqual("CL", countries[2].CountryID);
            Assert.AreEqual("UY", countries[3].CountryID);

            Assert.AreEqual("Argentina", countries[0].NeutralName);
            Assert.AreEqual("Brazil", countries[1].NeutralName);
            Assert.AreEqual("Chile", countries[2].NeutralName);
            Assert.AreEqual("Uruguay", countries[3].NeutralName);
        }

        /// <summary>
        /// The get state display name test 1.
        /// </summary>
        [Test]
        public void GetStateDisplayNameTest1()
        {
            string displayName = GeographicProvider.Current.GetStateDisplayName(1003, new CultureInfo("en-US"));
            Assert.AreEqual("Alabama", displayName);
        }

        /// <summary>
        /// The get state display name test 2.
        /// </summary>
        [Test]
        [ExpectedException(typeof(StateNotFoundException))]
        public void GetStateDisplayNameTest2()
        {
            GeographicProvider.Current.GetStateDisplayName(-40, new CultureInfo("en-US"));
        }

        /// <summary>
        /// The get state test 1.
        /// </summary>
        [Test]
        public void GetStateTest1()
        {
            State state = GeographicProvider.Current.GetState(1003);
            Assert.AreEqual("Alabama", state.NeutralName);
            Assert.AreEqual(1003, state.StateID);
            Assert.AreEqual("US", state.CountryID);
        }

        /// <summary>
        /// The get state test 2.
        /// </summary>
        [Test]
        [ExpectedException(typeof(StateNotFoundException))]
        public void GetStateTest2()
        {
            GeographicProvider.Current.GetState(-100);
        }

        /// <summary>
        /// The get unfiltered countries test 1.
        /// </summary>
        [Test]
        public void GetUnfilteredCountriesTest1()
        {
            IList<Country> allCountries = GeographicProvider.Current.GetUnfilteredCountries();
            Assert.AreEqual(237, allCountries.Count);
        }

        /// <summary>
        /// The init.
        /// </summary>
        [SetUp]
        public void Init()
        {
            GeographicProvider.Current.CountriesFilter = "AR,BR,CL,UY";
        }

        /// <summary>
        /// The provider initialize test.
        /// </summary>
        [Test]
        public void ProviderInitializeTest()
        {
            string filteredCountries = GeographicProvider.Current.CountriesFilter;
            Assert.AreEqual("AR,BR,CL,UY", filteredCountries);
        }

        /// <summary>
        /// The state ctor test 1.
        /// </summary>
        [Test]
        public void StateCtorTest1()
        {
            var s = new State();
            Assert.AreEqual(string.Empty, s.CountryID);
            Assert.AreEqual(string.Empty, s.NeutralName);
            Assert.AreEqual(0, s.StateID);
        }

        /// <summary>
        /// The state ctor test 2.
        /// </summary>
        [Test]
        public void StateCtorTest2()
        {
            var s = new State(1000, "UY", "aName");
            Assert.AreEqual("UY", s.CountryID);
            Assert.AreEqual("aName", s.NeutralName);
            Assert.AreEqual(1000, s.StateID);
        }

        #endregion
    }
}