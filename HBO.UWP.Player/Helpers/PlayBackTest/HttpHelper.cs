using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Windows.Foundation.Collections;
using Windows.Media.Protection.PlayReady;

namespace PlayReadyUAP
{
    public class HttpHelper
    {
        protected IPlayReadyServiceRequest _serviceRequest = null;
        Uri  _uri = null;

        private List<KeyValuePair<string, string>> customHeaders;

        public HttpHelper( IPlayReadyServiceRequest serviceRequest, List<KeyValuePair<string, string>> customHeaders = null)
        {
            _serviceRequest = serviceRequest;
            this.customHeaders = customHeaders;
        }
        
        public async Task GenerateChallengeAndProcessResponse()
        {
            Debug.WriteLine(" " );
            Debug.WriteLine("Enter HttpHelper.GenerateChallengeAndProcessResponse()" );

            Debug.WriteLine("Generating challenge.." );
            PlayReadySoapMessage soapMessage = _serviceRequest.GenerateManualEnablingChallenge();
            if( _uri == null )
            {
                _uri = soapMessage.Uri;
            }

            Debug.WriteLine("Getting message body.." );
            byte[] messageBytes = soapMessage.GetMessageBody();
            HttpContent httpContent = new ByteArrayContent( messageBytes );

            IPropertySet propertySetHeaders = soapMessage.MessageHeaders;
            Debug.WriteLine("Http Headers:-");
            foreach( string strHeaderName in propertySetHeaders.Keys )
            {
                string strHeaderValue = propertySetHeaders[strHeaderName].ToString();
                Debug.WriteLine(strHeaderName + " : " + strHeaderValue);
                
                // The Add method throws an ArgumentException try to set protected headers like "Content-Type"
                // so set it via "ContentType" property
                if ( strHeaderName.Equals( "Content-Type", StringComparison.OrdinalIgnoreCase ) )
                {
                    httpContent.Headers.ContentType = MediaTypeHeaderValue.Parse(strHeaderValue);
                }
                else
                {
                    httpContent.Headers.Add(strHeaderName, strHeaderValue);
                }
                
            }

            if (customHeaders != null && customHeaders.Count >= 1)
            {
                foreach (KeyValuePair<string, string> header in customHeaders)
                {
                    httpContent.Headers.Add(header.Key, header.Value);
                }
            }

            Debug.WriteLine("Http Body:-" );
            Debug.WriteLine(new System.Text.UTF8Encoding().GetString( messageBytes, 0, messageBytes.Length ));

            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/64.0.3282.140 Safari/537.36 Edge/17.17134");
            HttpResponseMessage response = await httpClient.PostAsync( _uri, httpContent );
            string strResponse = await response.Content.ReadAsStringAsync();
            
            Debug.WriteLine("Processing Response.." );
            Exception exResult = _serviceRequest.ProcessManualEnablingResponse( await response.Content.ReadAsByteArrayAsync());

            if ( exResult != null)
            {
                throw exResult;
            }
            
            Debug.WriteLine("Leave HttpHelper.GenerateChallengeAndProcessResponse()" );
        }


    }


}
