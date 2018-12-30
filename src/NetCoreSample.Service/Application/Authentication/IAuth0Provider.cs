namespace NetCoreSample.Service.Application.Authentication
{
    public interface IAuth0Provider
    {
        /// <summary>
        /// The access token provided by Auth0 to access other
        /// resource providers.
        /// </summary>
        string AccessToken { get; }
    }
}
