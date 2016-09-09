namespace InspurOA.Identity.Owin
{
    /// <summary>
    /// Possible results from a sign in attempt
    /// </summary>
    public enum InspurSignInStatus
    {
        /// <summary>
        /// Sign in was successful
        /// </summary>
        Success,

        /// <summary>
        /// User is locked out
        /// </summary>
        LockedOut,

        /// <summary>
        /// Sign in requires addition verification (i.e. two factor)
        /// </summary>
        RequiresVerification,

        /// <summary>
        /// Sign in failed
        /// </summary>
        Failure
    }
}
