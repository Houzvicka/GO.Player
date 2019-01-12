//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Protection.PlayReady;
using PlayReadyUAP;

namespace PlayReadyUAP
{

    sealed public class LicenseManagement 
    {
        private LicenseManagement() { }

        static public void DumpLicenseValues(PlayReadyLicense license)
        {
            Console.WriteLine(" " );
            Console.WriteLine("License values:" );
            
            Console.WriteLine("FullyEvaluated  :" + license.FullyEvaluated.ToString() );
            Console.WriteLine("UsableForPlay   :" + license.UsableForPlay.ToString() );

            if( license.ExpirationDate == null )
            {
                Console.WriteLine("Expiration date  : Not specified" );
            }
            else
            {
                Console.WriteLine("Expiration date  :" + license.ExpirationDate.ToString() );
            }
            Console.WriteLine("Expiration period after first play  :" + license.ExpireAfterFirstPlay );
            
            Console.WriteLine("DomainAccountId :" + license.DomainAccountID.ToString() );
            Console.WriteLine("ChainDepth      :" + license.ChainDepth );
            for( uint i = 0; i < license.ChainDepth; i++ )
            {
                Guid keyId = license.GetKIDAtChainDepth(i);
                Console.WriteLine(String.Format(System.Globalization.CultureInfo.CurrentCulture, 
                                      "KeyId at chain depth ( {0} ) : {1}", i, keyId.ToString() ));
            }
            Console.WriteLine(" " );
            
        }
        
        static public  PlayReadyLicense FindSingleLicense( Guid keyId, string keyIdString, bool bFullyEvaluated )
        {
            Console.WriteLine("Enter LicenseManagement.FindSingleLicense()" );
            
            PlayReadyContentHeader contentHeader = new PlayReadyContentHeader(
                                                                                keyId,
                                                                                keyIdString,
                                                                                PlayReadyEncryptionAlgorithm.Aes128Ctr,
                                                                                null,
                                                                                null,
                                                                                String.Empty, 
                                                                                Guid.Empty);
            
            Console.WriteLine("Creating PlayReadyLicenseIterable..." );
            PlayReadyLicenseIterable licenseIterable = new PlayReadyLicenseIterable( contentHeader, bFullyEvaluated );
            foreach( PlayReadyLicense lic in licenseIterable )
            {
                DumpLicenseValues( lic );
            }
            
            PlayReadyLicense license = null;
            IEnumerable<IPlayReadyLicense> licenseEnumerable = licenseIterable;
            
            int licenseCount = Enumerable.Count<IPlayReadyLicense>( licenseEnumerable );
            Console.WriteLine("License count  :" + licenseCount );
            if( licenseCount > 0 )
            {
                license = Enumerable.ElementAt<IPlayReadyLicense>( licenseEnumerable, 0 ) as PlayReadyLicense;
            }
            
            Console.WriteLine("Leave LicenseManagement.FindSingleLicense()" );
            
            return license;
        }
        
        static public  IPlayReadyLicense[] FindMultipleLicenses( Guid keyId, string keyIdString, bool bFullyEvaluated )
        {
            Console.WriteLine("Enter LicenseManagement.FindMultipleLicenses()" );
            
            PlayReadyContentHeader contentHeader = new PlayReadyContentHeader(
                                                                                keyId,
                                                                                keyIdString,
                                                                                PlayReadyEncryptionAlgorithm.Aes128Ctr,
                                                                                null,
                                                                                null,
                                                                                String.Empty, 
                                                                                Guid.Empty);
            
            Console.WriteLine("Creating PlayReadyLicenseIterable..." );
            PlayReadyLicenseIterable licenseIterable = new PlayReadyLicenseIterable( contentHeader, bFullyEvaluated );
            foreach( PlayReadyLicense lic in licenseIterable )
            {
                DumpLicenseValues( lic );
            }
            
            IPlayReadyLicense[] licenses = null;
            IEnumerable<IPlayReadyLicense> licenseEnumerable = licenseIterable;
            
            int licenseCount = Enumerable.Count<IPlayReadyLicense>( licenseEnumerable );
            Console.WriteLine("License count  :" + licenseCount );
            if( licenseCount > 0 )
            {
                licenses = Enumerable.ToArray<IPlayReadyLicense>( licenseEnumerable );
            }
            
            Console.WriteLine("Leave LicenseManagement.FindMultipleLicenses()" );
            
            return licenses;
        }

        static public async  Task DeleteLicenses( Guid keyId, string keyIdString, PlayReadyEncryptionAlgorithm algorithm )
        {
            Console.WriteLine("Enter LicenseManagement.DeleteLicenses()" );
            Console.WriteLine("PlayReadyEncryptionType = " + algorithm.ToString() );
            
            PlayReadyContentHeader contentHeader = new PlayReadyContentHeader(
                                                                                keyId,
                                                                                keyIdString,
                                                                                algorithm,
                                                                                null,
                                                                                null,
                                                                                String.Empty, 
                                                                                Guid.Empty);
            
            Console.WriteLine("Deleting licenses..." );
            await PlayReadyLicenseManagement.DeleteLicenses( contentHeader );
            
            Console.WriteLine("Leave LicenseManagement.DeleteLicenses()" );
            
        }
    }
}
