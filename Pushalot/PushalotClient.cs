using System.Linq;

namespace TIKSN.Pushalot
{
    public class PushalotClient
    {
        private const string API_SEND_MESSAGE_URL = "https://pushalot.com/api/sendmessage";
        private const string JSON_FIELD_NAME_AUTHORIZATION_TOKEN = "AuthorizationToken";
        private const string JSON_FIELD_NAME_TITLE = "Title";
        private const string JSON_FIELD_NAME_BODY = "Body";
        private const string JSON_FIELD_NAME_LINK_TITLE = "LinkTitle";
        private const string JSON_FIELD_NAME_LINK = "Link";
        private const string JSON_FIELD_NAME_IS_IMPORTANT = "IsImportant";
        private const string JSON_FIELD_NAME_IS_SILENT = "IsSilent";
        private const string JSON_FIELD_NAME_IMAGE = "Image";
        private const string JSON_FIELD_NAME_SOURCE = "Source";
        private const string JSON_FIELD_NAME_TIME_TO_LIVE = "TimeToLive";
        private const string REQUEST_CONTENT_TYPE = "application/json";

        //private AuthorizationToken authorizationToken;

        private System.Collections.Generic.HashSet<AuthorizationToken> generalSubscribers;

        public PushalotClient()
        {
            this.generalSubscribers = new System.Collections.Generic.HashSet<AuthorizationToken>();
        }

        public bool Subscribe(AuthorizationToken authorizationToken)
        {
            return this.generalSubscribers.Add(authorizationToken);
        }

        public bool Unsubscribe(AuthorizationToken authorizationToken)
        {
            return this.generalSubscribers.Remove(authorizationToken);
        }

        public async System.Threading.Tasks.Task SendMessage(Message message)
        {
            foreach (var generalSubscriber in this.generalSubscribers)
            {
                await SendMessage(message, generalSubscriber);
            }
        }

        protected static async System.Threading.Tasks.Task SendMessage(Message message, AuthorizationToken authorizationToken)
        {
            string jsonText = GetJsonText(message, authorizationToken);

            System.Diagnostics.Debug.WriteLine("JSON: {0}", jsonText);

            using (var client = new System.Net.Http.HttpClient())
            {
                using (var content = new System.Net.Http.StringContent(jsonText, System.Text.Encoding.UTF8, REQUEST_CONTENT_TYPE))
                {
                    using (var response = await client.PostAsync(API_SEND_MESSAGE_URL, content))
                    {
                        System.Diagnostics.Debug.WriteLine("Response Message: {0}", response.Content.ReadAsStringAsync().Result);

                        switch (response.StatusCode)
                        {
                            case System.Net.HttpStatusCode.OK:
                                break;
                            case System.Net.HttpStatusCode.BadRequest:
                                throw new System.Exception("Input data validation failed. Check result information Description field for detailed information. Report about this issue by visiting https://pushalot.codeplex.com/workitem/list/basic.");
                            case System.Net.HttpStatusCode.MethodNotAllowed:
                                throw new System.Exception("Method POST is required. Report about this issue by visiting https://pushalot.codeplex.com/workitem/list/basic.");
                            case System.Net.HttpStatusCode.NotAcceptable:
                                throw new System.Exception("Message throttle limit hit. Check result information Description field for information which limit was exceeded. See limits (https://pushalot.com/api#limits) to learn more about what limits are enforced.");
                            case System.Net.HttpStatusCode.Gone:
                                throw new System.Exception("The AuthorizationToken is no longer valid and no more messages should be ever sent again using that token.");
                            case System.Net.HttpStatusCode.InternalServerError:
                                throw new System.Exception("Something is broken. Please contact Pushalot.com (https://pushalot.com/support) so we can investigate.");
                            case System.Net.HttpStatusCode.ServiceUnavailable:
                                throw new System.Exception("Pushalot.com servers are currently overloaded with requests. Try again later.");
                            default:
                                throw new System.Exception("Unknown error.");
                        }
                    }
                }
            }
        }

        private static string GetJsonText(Message message, AuthorizationToken authorizationToken)
        {
            var jsonDictionary = new System.Collections.Generic.Dictionary<string, object>();

            jsonDictionary.Add(JSON_FIELD_NAME_AUTHORIZATION_TOKEN, authorizationToken.Token);

            if (!string.IsNullOrEmpty(message.Title))
            {
                jsonDictionary.Add(JSON_FIELD_NAME_TITLE, message.Title);
            }

            jsonDictionary.Add(JSON_FIELD_NAME_BODY, message.Body);

            if (message.Link != null)
            {
                if (!string.IsNullOrEmpty(message.Link.Title))
                {
                    jsonDictionary.Add(JSON_FIELD_NAME_LINK_TITLE, message.Link.Title);
                }

                jsonDictionary.Add(JSON_FIELD_NAME_LINK, message.Link.Link.AbsoluteUri);
            }

            jsonDictionary.Add(JSON_FIELD_NAME_IS_IMPORTANT, message.IsImportant);
            jsonDictionary.Add(JSON_FIELD_NAME_IS_SILENT, message.IsSilent);

            if (message.Image != null)
            {
                jsonDictionary.Add(JSON_FIELD_NAME_IMAGE, message.Image.Image.AbsoluteUri);
            }

            if (!string.IsNullOrEmpty(message.Source))
            {
                jsonDictionary.Add(JSON_FIELD_NAME_SOURCE, message.Source);
            }

            if (message.TimeToLive.HasValue)
            {
                jsonDictionary.Add(JSON_FIELD_NAME_TIME_TO_LIVE, message.TimeToLive.Value);
            }

            string jsonText = Newtonsoft.Json.JsonConvert.SerializeObject(jsonDictionary);

            return jsonText;
        }
    }

    public class PushalotClient<T> : PushalotClient
    {
        private System.Lazy<System.Linq.ILookup<T, AuthorizationToken>> specialSubscribersLookup;
        private System.Collections.Generic.HashSet<System.Tuple<T, AuthorizationToken>> specialSubscribersSet;

        public PushalotClient()
            : base()
        {
            this.specialSubscribersSet = new System.Collections.Generic.HashSet<System.Tuple<T, AuthorizationToken>>();

            this.ResetSpecialSubscribersLookup();
        }

        public bool Subscribe(T tag, AuthorizationToken authorizationToken)
        {
            return this.specialSubscribersSet.Add(new System.Tuple<T, AuthorizationToken>(tag, authorizationToken));
        }

        public bool Unsubscribe(T tag, AuthorizationToken authorizationToken)
        {
            return this.specialSubscribersSet.Remove(new System.Tuple<T, AuthorizationToken>(tag, authorizationToken));
        }

        private void ResetSpecialSubscribersLookup()
        {
            this.specialSubscribersLookup = new System.Lazy<System.Linq.ILookup<T, AuthorizationToken>>(this.InitializeSpecialSubscribersLookup);
        }

        private System.Linq.ILookup<T, AuthorizationToken> InitializeSpecialSubscribersLookup()
        {
            return this.specialSubscribersSet.ToLookup(item => item.Item1, item => item.Item2);
        }

        public async System.Threading.Tasks.Task SendMessage(Message message, params T[] tags)
        {
            await this.SendMessage(message);

            foreach (var tag in tags)
            {
                foreach (var specialSubscriber in this.specialSubscribersLookup.Value[tag])
                {
                    await SendMessage(message, specialSubscriber);
                }
            }
        }
    }
}
