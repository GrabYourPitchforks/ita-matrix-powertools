using System;
using System.Collections;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;

namespace ita_matrix_helper
{
    public class united_hipmunk : HttpTaskAsyncHandler
    {
        public override bool IsReusable
        {
            get { return true; }
        }

        public override async Task ProcessRequestAsync(HttpContext context)
        {
            string hipmunkUrl = context.Request.QueryString["url"];
            if (String.IsNullOrEmpty(hipmunkUrl))
            {
                throw new HttpException(400, "Bad request.");
            }

            // Validate that the Hipmunk URI is well-formed.
            Uri hipmunkUrlAsUri;
            try
            {
                hipmunkUrlAsUri = new Uri(hipmunkUrl, UriKind.Absolute);
                if (hipmunkUrlAsUri.Scheme != "https" || hipmunkUrlAsUri.Host != "www.hipmunk.com" || hipmunkUrlAsUri.AbsolutePath != "/bookjs")
                {
                    throw new HttpException(400, "Bad request.");
                }
            }
            catch
            {
                throw new HttpException(400, "Bad request.");
            }

            // Make the web service call
            var client = new HttpClient();
            var response = await client.GetAsync(hipmunkUrlAsUri);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Bad status code.");
            }

            // Deserialize the JSON response payload.
            var deserialized = new JavaScriptSerializer().DeserializeObject(await response.Content.ReadAsStringAsync());
            var redirectUrl = Convert.ToString(((IDictionary)deserialized)["jquery"], CultureInfo.InvariantCulture);

            // Parse the redirect URL result.
            const string splitToken = @".call(jQuery,""";
            var tokenIdx = redirectUrl.IndexOf(splitToken, StringComparison.Ordinal);
            if (tokenIdx < 0)
            {
                throw new Exception("Bad response message.");
            }

            redirectUrl = redirectUrl.Substring(tokenIdx + splitToken.Length, redirectUrl.Length - tokenIdx - splitToken.Length - 2 /* trailing '")' */);
            context.Response.Redirect(redirectUrl);
        }
    }
}
