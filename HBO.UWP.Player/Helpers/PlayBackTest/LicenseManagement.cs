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
            Debug.WriteLine(" " );
            Debug.WriteLine("License values:" );
            
            Debug.WriteLine("FullyEvaluated  :" + license.FullyEvaluated.ToString() );
            Debug.WriteLine("UsableForPlay   :" + license.UsableForPlay.ToString() );

            if( license.ExpirationDate == null )
            {
                Debug.WriteLine("Expiration date  : Not specified" );
            }
            else
            {
                Debug.WriteLine("Expiration date  :" + license.ExpirationDate.ToString() );
            }
            Debug.WriteLine("Expiration period after first play  :" + license.ExpireAfterFirstPlay );
            
            Debug.WriteLine("DomainAccountId :" + license.DomainAccountID.ToString() );
            Debug.WriteLine("ChainDepth      :" + license.ChainDepth );
            for( uint i = 0; i < license.ChainDepth; i++ )
            {
                Guid keyId = license.GetKIDAtChainDepth(i);
                Debug.WriteLine(String.Format(System.Globalization.CultureInfo.CurrentCulture, 
                                      "KeyId at chain depth ( {0} ) : {1}", i, keyId.ToString() ));
            }
            Debug.WriteLine(" " );
            
        }
        
        static public  PlayReadyLicense FindSingleLicense( Guid keyId, string keyIdString, bool bFullyEvaluated )
        {
            Debug.WriteLine("Enter LicenseManagement.FindSingleLicense()" );
            
            PlayReadyContentHeader contentHeader = new PlayReadyContentHeader(
                                                                                keyId,
                                                                                keyIdString,
                                                                                PlayReadyEncryptionAlgorithm.Aes128Ctr,
                                                                                null,
                                                                                null,
                                                                                String.Empty, 
                                                                                Guid.Empty);
            
            Debug.WriteLine("Creating PlayReadyLicenseIterable..." );
            PlayReadyLicenseIterable licenseIterable = new PlayReadyLicenseIterable( contentHeader, bFullyEvaluated );
            foreach( PlayReadyLicense lic in licenseIterable )
            {
                DumpLicenseValues( lic );
            }
            
            PlayReadyLicense license = null;
            IEnumerable<IPlayReadyLicense> licenseEnumerable = licenseIterable;
            
            int licenseCount = Enumerable.Count<IPlayReadyLicense>( licenseEnumerable );
            Debug.WriteLine("License count  :" + licenseCount );
            if( licenseCount > 0 )
            {
                license = Enumerable.ElementAt<IPlayReadyLicense>( licenseEnumerable, 0 ) as PlayReadyLicense;
            }
            
            Debug.WriteLine("Leave LicenseManagement.FindSingleLicense()" );
            
            return license;
        }
        
        static public  IPlayReadyLicense[] FindMultipleLicenses( Guid keyId, string keyIdString, bool bFullyEvaluated )
        {
            Debug.WriteLine("Enter LicenseManagement.FindMultipleLicenses()" );
            
            PlayReadyContentHeader contentHeader = new PlayReadyContentHeader(
                                                                                keyId,
                                                                                keyIdString,
                                                                                PlayReadyEncryptionAlgorithm.Aes128Ctr,
                                                                                null,
                                                                                null,
                                                                                String.Empty, 
                                                                                Guid.Empty);
            
            Debug.WriteLine("Creating PlayReadyLicenseIterable..." );
            PlayReadyLicenseIterable licenseIterable = new PlayReadyLicenseIterable( contentHeader, bFullyEvaluated );
            foreach( PlayReadyLicense lic in licenseIterable )
            {
                DumpLicenseValues( lic );
            }
            
            IPlayReadyLicense[] licenses = null;
            IEnumerable<IPlayReadyLicense> licenseEnumerable = licenseIterable;
            
            int licenseCount = Enumerable.Count<IPlayReadyLicense>( licenseEnumerable );
            Debug.WriteLine("License count  :" + licenseCount );
            if( licenseCount > 0 )
            {
                licenses = Enumerable.ToArray<IPlayReadyLicense>( licenseEnumerable );
            }
            
            Debug.WriteLine("Leave LicenseManagement.FindMultipleLicenses()" );
            
            return licenses;
        }

        static public async  Task DeleteLicenses( Guid keyId, string keyIdString, PlayReadyEncryptionAlgorithm algorithm )
        {
            Debug.WriteLine("Enter LicenseManagement.DeleteLicenses()" );
            Debug.WriteLine("PlayReadyEncryptionType = " + algorithm.ToString() );
            
            PlayReadyContentHeader contentHeader = new PlayReadyContentHeader(
                                                                                keyId,
                                                                                keyIdString,
                                                                                algorithm,
                                                                                null,
                                                                                null,
                                                                                String.Empty, 
                                                                                Guid.Empty);
            
            Debug.WriteLine("Deleting licenses..." );
            await PlayReadyLicenseManagement.DeleteLicenses( contentHeader );
            
            Debug.WriteLine("Leave LicenseManagement.DeleteLicenses()" );
            
        }
    }
}
