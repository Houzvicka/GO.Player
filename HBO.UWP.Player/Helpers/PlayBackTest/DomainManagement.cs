//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Protection.PlayReady;
using PlayReadyUAP;

namespace PlayReadyUAP
{

    sealed public class DomainManagement 
    {
        private DomainManagement() { }

        static public void DumpDomainValues(PlayReadyDomain domain)
        {
            Console.WriteLine(" " );
            Console.WriteLine("Domain values:" );
            
            Console.WriteLine("AccountId  :" + domain.AccountId.ToString() );
            Console.WriteLine("ServiceId   :" + domain.ServiceId.ToString() );
            
            Console.WriteLine("Revision  :" + domain.Revision );
            Console.WriteLine("FriendlyName  :" + domain.FriendlyName );

            Uri uri = domain.DomainJoinUrl;
            if( uri != null )
            {
                Console.WriteLine("DomainJoinUrl :" + uri.ToString() );
            }
            Console.WriteLine(" " );
            
        }
        
        static public  PlayReadyDomain FindSingleDomain( Guid guidAccountId )
        {
            Console.WriteLine("Enter DomainManagement.FindSingleDomain()" );
                        
            Console.WriteLine("Creating PlayReadyDomainIterable..." );
            PlayReadyDomainIterable domainIterable = new PlayReadyDomainIterable( guidAccountId );
            foreach( PlayReadyDomain dom in domainIterable )
            {
                DumpDomainValues( dom );
            }
            
            PlayReadyDomain domain = null;
            IEnumerable<IPlayReadyDomain> domainEnumerable = domainIterable;
            
            int domainCount = Enumerable.Count<IPlayReadyDomain>( domainEnumerable );
            Console.WriteLine("domain count  :" + domainCount );
            if( domainCount > 0 )
            {
                domain = Enumerable.ElementAt<IPlayReadyDomain>( domainEnumerable, 0 ) as PlayReadyDomain;
            }
            
            Console.WriteLine("Leave DomainManagement.FindSingleDomain()" );
            
            return domain;
        }
        
        static public  IPlayReadyDomain[] FindMultipleDomains( Guid guidAccountId )
        {
            Console.WriteLine("Enter DomainManagement.FindMultipleDomains()" );
            
            Console.WriteLine("Creating PlayReadyDomainIterable..." );
            PlayReadyDomainIterable domainIterable = new PlayReadyDomainIterable( guidAccountId );
            foreach( PlayReadyDomain dom in domainIterable )
            {
                DumpDomainValues( dom );
            }
            
            IPlayReadyDomain[] domains = null;
            IEnumerable<IPlayReadyDomain> domainEnumerable = domainIterable;
            
            int domainCount = Enumerable.Count<IPlayReadyDomain>( domainEnumerable );
            Console.WriteLine("domain count  :" + domainCount );
            if( domainCount > 0 )
            {
                domains = Enumerable.ToArray<IPlayReadyDomain>( domainEnumerable );
            }
            
            Console.WriteLine("Leave DomainManagement.FindMultipleDomains()" );
            
            return domains;
        }

    }


}
