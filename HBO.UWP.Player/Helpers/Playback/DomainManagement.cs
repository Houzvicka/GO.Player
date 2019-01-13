//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Windows.Media.Protection.PlayReady;

namespace HBO.UWP.Player.Helpers.Playback
{

    sealed public class DomainManagement 
    {
        private DomainManagement() { }

        static public void DumpDomainValues(PlayReadyDomain domain)
        {
            Debug.WriteLine(" " );
            Debug.WriteLine("Domain values:" );
            
            Debug.WriteLine("AccountId  :" + domain.AccountId.ToString() );
            Debug.WriteLine("ServiceId   :" + domain.ServiceId.ToString() );
            
            Debug.WriteLine("Revision  :" + domain.Revision );
            Debug.WriteLine("FriendlyName  :" + domain.FriendlyName );

            Uri uri = domain.DomainJoinUrl;
            if( uri != null )
            {
                Debug.WriteLine("DomainJoinUrl :" + uri.ToString() );
            }
            Debug.WriteLine(" " );
            
        }
        
        static public  PlayReadyDomain FindSingleDomain( Guid guidAccountId )
        {
            Debug.WriteLine("Enter DomainManagement.FindSingleDomain()" );
                        
            Debug.WriteLine("Creating PlayReadyDomainIterable..." );
            PlayReadyDomainIterable domainIterable = new PlayReadyDomainIterable( guidAccountId );
            foreach( PlayReadyDomain dom in domainIterable )
            {
                DumpDomainValues( dom );
            }
            
            PlayReadyDomain domain = null;
            IEnumerable<IPlayReadyDomain> domainEnumerable = domainIterable;
            
            int domainCount = Enumerable.Count<IPlayReadyDomain>( domainEnumerable );
            Debug.WriteLine("domain count  :" + domainCount );
            if( domainCount > 0 )
            {
                domain = Enumerable.ElementAt<IPlayReadyDomain>( domainEnumerable, 0 ) as PlayReadyDomain;
            }
            
            Debug.WriteLine("Leave DomainManagement.FindSingleDomain()" );
            
            return domain;
        }
        
        static public  IPlayReadyDomain[] FindMultipleDomains( Guid guidAccountId )
        {
            Debug.WriteLine("Enter DomainManagement.FindMultipleDomains()" );
            
            Debug.WriteLine("Creating PlayReadyDomainIterable..." );
            PlayReadyDomainIterable domainIterable = new PlayReadyDomainIterable( guidAccountId );
            foreach( PlayReadyDomain dom in domainIterable )
            {
                DumpDomainValues( dom );
            }
            
            IPlayReadyDomain[] domains = null;
            IEnumerable<IPlayReadyDomain> domainEnumerable = domainIterable;
            
            int domainCount = Enumerable.Count<IPlayReadyDomain>( domainEnumerable );
            Debug.WriteLine("domain count  :" + domainCount );
            if( domainCount > 0 )
            {
                domains = Enumerable.ToArray<IPlayReadyDomain>( domainEnumerable );
            }
            
            Debug.WriteLine("Leave DomainManagement.FindMultipleDomains()" );
            
            return domains;
        }

    }


}
