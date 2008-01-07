using System.Threading;

namespace Rainbow.Framework.Providers.Geographic
{
    public class State
    {
        int stateID;
        string neutralName;
        string countryID;

        /// <summary>
        /// Gets or sets the state ID.
        /// </summary>
        /// <value>The state ID.</value>
        public int StateID
        {
            get { return stateID; }
            set { stateID = value; }
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
        /// Gets or sets the country ID.
        /// </summary>
        /// <value>The country ID.</value>
        public string CountryID
        {
            get { return countryID; }
            set { countryID = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="State"/> class.
        /// </summary>
        public State() : this(0, string.Empty, string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="State"/> class.
        /// </summary>
        /// <param name="stateID">The state ID.</param>
        /// <param name="countryID">The country ID.</param>
        /// <param name="neutralName">Neutral Name.</param>
        public State(int stateID, string countryID, string neutralName)
        {
            this.stateID = stateID;
            this.countryID = countryID;
            this.neutralName = neutralName;
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
                    GeographicProvider.Current.GetStateDisplayName(stateID,
                                                                   Thread.CurrentThread.
                                                                       CurrentCulture);
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

            State otherState = (State) obj;
            return (countryID == otherState.countryID) &&
                   (neutralName == otherState.neutralName) &&
                   (stateID == otherState.stateID);
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
            return stateID.GetHashCode();
        }
    }
}
