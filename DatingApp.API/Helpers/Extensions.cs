using Microsoft.AspNetCore.Http;

namespace DatingApp.API.Helpers
{
    public static class Extensions
    {
        public static void AddApplicationError(this HttpResponse httpResponse, string message){
            httpResponse.Headers.Add("Application-Error",message);
            httpResponse.Headers.Add("Access-Control-Expose-Headers","Application-Error");
            httpResponse.Headers.Add("Access-Control-Allow-Origin","*");
        }
    }
}