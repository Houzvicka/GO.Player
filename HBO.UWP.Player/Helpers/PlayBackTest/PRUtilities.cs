//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Text;

using Windows.Foundation;
using Windows.Foundation.Collections;

using Windows.Storage;
using Windows.Storage.Streams;

using Windows.Media.Protection;
using Windows.Media.Protection.PlayReady;

using Windows.UI.Xaml.Controls;

using PlayReadyUAP;

namespace PlayReadyUAP
{

    public class PRUtilities
    {
        public string GuidToBase64(Guid guid)
        {
            return Convert.ToBase64String(guid.ToByteArray());
        }

        public Nullable<uint> ActionParamConvertToUint(string str)
        {
            
            Nullable<uint> uintRet = null;
            if (!String.IsNullOrEmpty(str) && str != "null")
            {
                uintRet = System.UInt32.Parse(str);
            }

            return uintRet;
        }

        public bool ActionParamConvertToBool(string str)
        {
            bool bRet = false;
            if (!String.IsNullOrEmpty(str) && str != "null" && str.ToLower() == "true")
            {
                bRet = true;
            }
            return bRet;
        }

        public string ActionParamConvertToString(string str)
        {
            string strRet = string.Empty;
            if (!String.IsNullOrEmpty(str) && str != "null")
            {
                strRet = str;
            }
            return strRet;
        }

        public Uri ActionParamConvertToUri(string strUri)
        {
            Uri uri = null;
            if (!String.IsNullOrEmpty(strUri) && strUri != "null" )
            {
                uri = new Uri(strUri);
            }
            return uri;
        }

        public Guid ActionParamConvertToGuid(string strGuid)
        {
            Guid guid = Guid.Empty;
            if (!String.IsNullOrEmpty(strGuid) && strGuid != "null" )
            {
                guid = new Guid(strGuid);
            }
            return guid;
        }

        public PlayReadyEncryptionAlgorithm ActionParamConvertToPlayReadyEncryptionAlgorithm(string strEncryptionAlgorithm)
        {
            PlayReadyEncryptionAlgorithm encryptionAlgorithm = PlayReadyEncryptionAlgorithm.Unprotected;
            if (!String.IsNullOrEmpty(strEncryptionAlgorithm) && strEncryptionAlgorithm != "null")
            {
                encryptionAlgorithm = (PlayReadyEncryptionAlgorithm)Enum.Parse(typeof(PlayReadyEncryptionAlgorithm), strEncryptionAlgorithm, true);
            }
            return encryptionAlgorithm;
        }

        private void TestActionFinished(bool bResult, object context)
        {
            if (_meteringAndReportResult != null)
            {
            }
           
            if (bResult)
            {
                Debug.WriteLine("TestActionFinished successfully");
            }
            else
            {
                Debug.WriteLine("TestActionFinished with error!!!");
            }
        }

        IndivAndReportResult _indiv = null;
        public void Test_Indiv()
        {
            _indiv = new IndivAndReportResult(new ReportResultDelegate(TestActionFinished));
            _indiv.IndivProactively();
        }

        LAAndReportResult _licenseAcquisition = null;
        public void Test_LicenseAcquisition( Guid [] GuidKids,
                                             string strEncryptionAlgorithm,
                                             string strLAURL,
                                             string strServiceID,
                                             string strCustomData,
                                             string useManualEnabling,
                                             string errorExpected)
        {

            Debug.WriteLine("Enter Test_LicenseAcquisition()");

            ServiceRequestConfigData requestConfigData = new ServiceRequestConfigData();
            requestConfigData.KeyIds = GuidKids;
            requestConfigData.EncryptionAlgorithm = ActionParamConvertToPlayReadyEncryptionAlgorithm(strEncryptionAlgorithm);
            requestConfigData.Uri = ActionParamConvertToUri(strLAURL);
            requestConfigData.DomainServiceId = ActionParamConvertToGuid(strServiceID);
            requestConfigData.ChallengeCustomData = ActionParamConvertToString(strCustomData);

            if (useManualEnabling.ToLower() == "true")
            {
                requestConfigData.ManualEnabling = true;
            }

            _licenseAcquisition = new LAAndReportResult(new ReportResultDelegate(TestActionFinished));
            _licenseAcquisition.RequestConfigData = requestConfigData;
            _licenseAcquisition.ExpectedError = ActionParamConvertToString(errorExpected);
            _licenseAcquisition.AcquireLicenseProactively();

            Debug.WriteLine("Leave Test_LicenseAcquisition()");
        }

        PlaybackAndReportResult _playbackAndReportResult = null;
        public void Test_Play(MediaElement mediaElement,
                              string mediaName,
                              string strLAURL,
                              string strCustomData,
                              string strDJURL,
                              string strServiceId,
                              string strAccountId,
                              string useManualEnabling)
        {

            Debug.WriteLine("Enter Test_Play()");

            ServiceRequestConfigData requestConfigData = new ServiceRequestConfigData();
            requestConfigData.Uri = ActionParamConvertToUri(strLAURL);
            requestConfigData.ChallengeCustomData = ActionParamConvertToString(strCustomData);
            requestConfigData.DomainUri = ActionParamConvertToUri(strDJURL);
            requestConfigData.DomainServiceId = ActionParamConvertToGuid(strServiceId);
            requestConfigData.DomainAccountId = ActionParamConvertToGuid(strAccountId);
            requestConfigData.ManualEnabling = ActionParamConvertToBool(useManualEnabling);

            _playbackAndReportResult = new PlaybackAndReportResult(mediaElement, new ReportResultDelegate(TestActionFinished));
            _playbackAndReportResult.RequestConfigData = requestConfigData;
            _playbackAndReportResult.FullPlayback(mediaElement, mediaName);

            Debug.WriteLine("Leave Test_Play()");
        }

        public void Test_LoadMedia(MediaElement mediaElement, 
                                   string mediaName,
                                   string strLAURL,
                                   string strCustomData,
                                   string strDJURL,
                                   string strServiceId,
                                   string strAccountId,
                                   string useManualEnabling)
        {

            Debug.WriteLine("Enter Test_LoadMedia()");

            ServiceRequestConfigData requestConfigData = new ServiceRequestConfigData();
            requestConfigData.Uri = ActionParamConvertToUri(strLAURL);
            requestConfigData.ChallengeCustomData = ActionParamConvertToString(strCustomData);
            requestConfigData.DomainUri = ActionParamConvertToUri(strDJURL);
            requestConfigData.DomainServiceId = ActionParamConvertToGuid(strServiceId);
            requestConfigData.DomainAccountId = ActionParamConvertToGuid(strAccountId);
            requestConfigData.ManualEnabling = ActionParamConvertToBool(useManualEnabling);

            _playbackAndReportResult = new PlaybackAndReportResult(mediaElement, new ReportResultDelegate(TestActionFinished));
            _playbackAndReportResult.RequestConfigData = requestConfigData;
            _playbackAndReportResult.LoadMedia(mediaElement, mediaName, false);

            Debug.WriteLine("Leave Test_LoadMedia()");
        }

        public void Test_SetSource(MediaElement mediaElement, string strMediaPath)
        {
            Debug.WriteLine("Enter Test_SetSource()");

            if (mediaElement == null)
            {
                Debug.WriteLine("mediaElement is closed ");
                TestActionFinished(false, null);
                return;
            }

            mediaElement.Source = new Uri(strMediaPath);
            Debug.WriteLine("Exit Test_SetSource()");
            TestActionFinished(true, null);
        }

        public void Test_getSource(MediaElement mediaElement, string ExpectedMediaSource)
        {
            Debug.WriteLine("Enter Test_CheckSourceValue()");

            bool bResult = false;

            if (mediaElement == null)
            {
                Debug.WriteLine("mediaElement is closed ");
                TestActionFinished(false, null);
                return;
            }

            if (string.IsNullOrEmpty(ExpectedMediaSource) || ExpectedMediaSource == "null")
            {
                if (mediaElement.Source == null)
                {
                    bResult = true;
                }
            }
            else
            {
                if (mediaElement.Source != null)
                {
                    if (mediaElement.Source.ToString().ToLower() == ExpectedMediaSource.ToLower())
                    {
                        bResult = true;
                    }
                }
            }

            if (bResult)
            {
                TestActionFinished(true, null);
            }
            else
            {
                TestActionFinished(false, null);
            }

            Debug.WriteLine("Exit Test_CheckSourceValue()");
        }

        public async void Test_PlayDuration(string strDurationInSeconds)
        {

            Debug.WriteLine("Enter Test_PlayDuration()");

            Debug.WriteLine("Playing " + strDurationInSeconds + " seconds...");
            if (strDurationInSeconds == "infinite")
            {
                _playbackAndReportResult.Play(true);
            }
            else
            {
                _playbackAndReportResult.Play(false);

                uint? uintSec = ActionParamConvertToUint(strDurationInSeconds);

                await Task.Delay((int)uintSec.Value * 1000);
            }

            Debug.WriteLine("Leave Test_PlayDuration()");
        }

        public void Test_Seek(MediaElement mediaElement, string strPositionInSeconds)
        {
            Debug.WriteLine("Enter Test_Seek()");

            if (mediaElement == null)
            {
                Debug.WriteLine("mediaElement is closed ");
                TestActionFinished(false, null);
                return;
            }

            Debug.WriteLine("seek to " + strPositionInSeconds + " seconds");

            uint? uintSec = ActionParamConvertToUint(strPositionInSeconds);

            TimeSpan ts = new TimeSpan(0, 0, 0, 0, (int)uintSec);

            mediaElement.Position = ts;

            Debug.WriteLine("Exit Test_Seek()");
            TestActionFinished(true, null);
        }
        public void Test_Stop()
        {

            Debug.WriteLine("Enter Test_Stop()");
            if (_playbackAndReportResult != null)
            {
                _playbackAndReportResult.Stop();
                TestActionFinished(true, null);
            }
            else
            {
                TestActionFinished(false, null);
            }
            Debug.WriteLine("Leave Test_Stop()");
        }

        public void Test_Pause()
        {

            Debug.WriteLine("Enter Test_Pause()");
            if (_playbackAndReportResult != null)
            {
                _playbackAndReportResult.Pause();
                TestActionFinished(true, null);
            }
            else
            {
                TestActionFinished(false, null);
            }
            Debug.WriteLine("Leave Test_Pause()");
        }

        public void Test_PlayExpectLAFailure(MediaElement mediaElement,
                                   string mediaName,
                                   string strLAURL,
                                   string strCustomData,
                                   string strExpectedLAErrorCode,
                                   string strExpectedPlaybackErrorCode,
                                   string useManualEnabling
                                   )
        {

            Debug.WriteLine("Enter Test_PlayExpectLAFailure()");

            ServiceRequestConfigData requestConfigData = new ServiceRequestConfigData();
            requestConfigData.Uri = ActionParamConvertToUri(strLAURL);
            requestConfigData.ChallengeCustomData = ActionParamConvertToString(strCustomData);
            requestConfigData.ExpectedLAErrorCode = ActionParamConvertToString(strExpectedLAErrorCode);
            requestConfigData.ManualEnabling = ActionParamConvertToBool(useManualEnabling);

            _playbackAndReportResult = new PlaybackAndReportResult(
                                                                    mediaElement,
                                                                    new ReportResultDelegate(TestActionFinished),
                                                                    ActionParamConvertToString(strExpectedPlaybackErrorCode)
                                                                    );
            _playbackAndReportResult.RequestConfigData = requestConfigData;
            _playbackAndReportResult.FullPlayback(mediaElement, mediaName);

            Debug.WriteLine("Leave Test_PlayExpectLAFailure()");
        }


        public void Test_PlayWithExpectedError(MediaElement mediaElement, 
                                               string mediaName,
                                               string strLAURL,
                                               string strCustomData,
                                               string strServiceId,
                                               string strAccountId,
                                               string strExpectedError)
        {

            Debug.WriteLine("Enter Test_PlayWithExpectedError()");

            ServiceRequestConfigData requestConfigData = new ServiceRequestConfigData();
            requestConfigData.Uri = ActionParamConvertToUri(strLAURL);
            requestConfigData.ChallengeCustomData = ActionParamConvertToString(strCustomData);
            requestConfigData.DomainUri = requestConfigData.Uri;
            requestConfigData.DomainServiceId = ActionParamConvertToGuid(strServiceId);
            requestConfigData.DomainAccountId = ActionParamConvertToGuid(strAccountId);

            _playbackAndReportResult = new PlaybackAndReportResult(
                                                mediaElement,
                                                new ReportResultDelegate(TestActionFinished),
                                                ActionParamConvertToString(strExpectedError)
                                                );
            _playbackAndReportResult.RequestConfigData = requestConfigData;
            _playbackAndReportResult.FullPlayback(mediaElement, mediaName);

            Debug.WriteLine("Leave Test_PlayWithExpectedError()");
        }

        DomainJoinAndReportResult _domainJoinAndReportResult = null;
        public void Test_DomainJoin(string strAccountId,
                                    string strServiceId,
                                    string djURL,
                                    string useManualEnabling)
        {
            Debug.WriteLine("Enter Test_DomainJoin()");

            ServiceRequestConfigData requestConfigData = new ServiceRequestConfigData();
            requestConfigData.DomainUri = ActionParamConvertToUri(djURL);
            requestConfigData.DomainServiceId = ActionParamConvertToGuid(strServiceId);
            requestConfigData.DomainAccountId = ActionParamConvertToGuid(strAccountId);
            requestConfigData.ManualEnabling = ActionParamConvertToBool(useManualEnabling);

            _domainJoinAndReportResult = new DomainJoinAndReportResult(new ReportResultDelegate(TestActionFinished));
            _domainJoinAndReportResult.RequestConfigData = requestConfigData;
            _domainJoinAndReportResult.DomainJoinProactively();

            Debug.WriteLine("Leave Test_DomainJoin()");
        }

        DomainLeaveAndReportResult _domainLeaveAndReportResult = null;
        public void Test_DomainLeave(string strAccountId,
                                     string strServiceId,
                                     string dlURL,
                                     string useManualEnabling)
        {
            Debug.WriteLine("Enter Test_DomainLeave()");

            ServiceRequestConfigData requestConfigData = new ServiceRequestConfigData();
            requestConfigData.DomainUri = ActionParamConvertToUri(dlURL);
            requestConfigData.DomainServiceId = ActionParamConvertToGuid(strServiceId);
            requestConfigData.DomainAccountId = ActionParamConvertToGuid(strAccountId);
            requestConfigData.ManualEnabling = ActionParamConvertToBool(useManualEnabling);

            _domainLeaveAndReportResult = new DomainLeaveAndReportResult(new ReportResultDelegate(TestActionFinished));
            _domainLeaveAndReportResult.RequestConfigData = requestConfigData;
            _domainLeaveAndReportResult.DomainLeaveProactively();

            Debug.WriteLine("Leave Test_DomainLeave()");
        }

        void VerifyLicenseShouldExist(Guid keyIdGuid, string strKeyIdString, bool bFullyEvaluated)
        {
            PlayReadyLicense license = LicenseManagement.FindSingleLicense(keyIdGuid, strKeyIdString, bFullyEvaluated);
            if (license == null)
            {
                Debug.WriteLine("License not found!!!");
                TestActionFinished(false,  null);
            }
            else
            {
                Guid keyIdFromLicense = license.GetKIDAtChainDepth(0);
                if (keyIdFromLicense == keyIdGuid || keyIdFromLicense == new Guid(System.Convert.FromBase64String(strKeyIdString)))
                {
                    Debug.WriteLine("Matching license found in license store!!!");
                    TestActionFinished(true, null);
                }
                else
                {
                    Debug.WriteLine("Matching license not found in license store!!!");
                    TestActionFinished(false, null);
                }
            }
        }

        void VerifyLicenseShouldNotExist(Guid keyIdGuid, string strKeyIdString, bool bFullyEvaluated)
        {
            PlayReadyLicense license = LicenseManagement.FindSingleLicense(keyIdGuid, strKeyIdString, bFullyEvaluated);
            if (license == null)
            {
                Debug.WriteLine("License not found!!!");
                TestActionFinished(true, null);
            }
            else
            {
                Debug.WriteLine("License found!!!");
                TestActionFinished(false, null);
            }
        }

        public void Test_VerifyLicense(string keyId,
                                       string keyIdString,
                                       string evaluated,
                                       string shouldExist
                                       )
        {
            Debug.WriteLine("Enter Test_VerifyLicense()");

            Guid keyIdGuid = ActionParamConvertToGuid(keyId);
            keyIdString = ActionParamConvertToString(keyIdString);

            bool bFullyEvaluated = ActionParamConvertToBool(evaluated);
            bool bLicenseShouldExist = ActionParamConvertToBool(shouldExist);

            if (bLicenseShouldExist)
            {
                VerifyLicenseShouldExist(keyIdGuid, keyIdString, bFullyEvaluated);
            }
            else
            {
                VerifyLicenseShouldNotExist(keyIdGuid, keyIdString, bFullyEvaluated);
            }

            Debug.WriteLine("Leave Test_VerifyLicense()");
        }

        void VerifyLicensesShouldExist(Guid keyIdGuid, bool bFullyEvaluated, uint? uintCount)
        {
            IPlayReadyLicense[] licenses = LicenseManagement.FindMultipleLicenses(keyIdGuid, string.Empty, bFullyEvaluated);
            if (licenses == null)
            {
                Debug.WriteLine("Licenses not found!!!");
                TestActionFinished(false, null);
            }
            else
            {
                if (!uintCount.HasValue)
                {
                    TestActionFinished(true, null);
                }
                else
                {
                    if (licenses.Length == uintCount.Value)
                    {
                        Debug.WriteLine("License count matched!!!");
                        TestActionFinished(true, null);
                    }
                    else
                    {
                        Debug.WriteLine("License count not matching!!!");
                        TestActionFinished(false, null);
                    }
                }
            }
        }

        void VerifyLicensesShouldNotExist(Guid keyIdGuid, bool bFullyEvaluated)
        {
            IPlayReadyLicense[] licenses = LicenseManagement.FindMultipleLicenses(keyIdGuid, string.Empty, bFullyEvaluated);
            if (licenses == null)
            {
                Debug.WriteLine("Licenses not found!!!");
                TestActionFinished(true, null);
            }
            else
            {
                Debug.WriteLine("Licenses found. Count = " + licenses.Length);
                TestActionFinished(false, null);
            }
        }

        public void Test_VerifyMultipleLicenses(string strKeyId,
                                                string strKeyIdString,
                                                string strEvaluated,
                                                string strShouldExist,
                                                string strCount)
        {
            Debug.WriteLine("Enter Test_VerifyMultipleLicenses()");

            Guid keyIdGuid = ActionParamConvertToGuid(strKeyId);
            string keyIdString = ActionParamConvertToString(strKeyIdString);

            bool bFullyEvaluated = ActionParamConvertToBool(strEvaluated);
            bool bLicenseShouldExist = ActionParamConvertToBool(strShouldExist);
            Nullable<uint> uintCount = ActionParamConvertToUint(strCount);

            if (bLicenseShouldExist)
            {
                VerifyLicensesShouldExist(keyIdGuid, bFullyEvaluated, uintCount);
            }
            else
            {
                VerifyLicensesShouldNotExist(keyIdGuid, bFullyEvaluated);
            }

            Debug.WriteLine("Leave Test_VerifyMultipleLicenses()");
        }

        public async void Test_DeleteLicenses(string strKeyId, string strEncryptionType, string strExpectedError)
        {
            Debug.WriteLine("Enter Test_DeleteLicenses()");

            strExpectedError = ActionParamConvertToString(strExpectedError);
            bool bTestActionResult = true;
            try
            {
                Guid keyIdGuid = ActionParamConvertToGuid(strKeyId);
                PlayReadyEncryptionAlgorithm alg = ActionParamConvertToPlayReadyEncryptionAlgorithm(strEncryptionType);
                await LicenseManagement.DeleteLicenses(keyIdGuid, string.Empty, alg);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Test_DeleteLicenses Exception = " + ex.Message);
                if (strExpectedError != null && ex.Message.ToLower().Contains(strExpectedError.ToLower()))
                {
                    Debug.WriteLine("'" + ex.Message + "' Contains " + strExpectedError + "  as expected");
                    bTestActionResult = true;
                }
                else
                {
                    bTestActionResult = false;
                }
            }

            Debug.WriteLine("Leave Test_DeleteLicenses()");
            TestActionFinished(bTestActionResult, null);
        }

        public void Test_ContentResolver(string strContentHeader, string strExpectServiceRequest, string strRootKid, string strLeafKid)
        {
            Debug.WriteLine("Enter Test_ContentResolver()");

            Debug.WriteLine("Root Kid = " + strRootKid);
            Debug.WriteLine("Leaf Kid = " + strLeafKid);

            bool bTestActionResult = false;
            bool bExpectServiceRequest = ActionParamConvertToBool(strExpectServiceRequest);

            Debug.WriteLine(strContentHeader);
            PlayReadyContentHeader contentHeader = new PlayReadyContentHeader(new System.Text.UnicodeEncoding().GetBytes(strContentHeader));

            LicenseAcquisition.DumpContentHeaderValues(contentHeader);

            Debug.WriteLine("Using PlayReadyContentResolver...");
            IPlayReadyServiceRequest serviceRequest = PlayReadyContentResolver.ServiceRequest(contentHeader);
            if (serviceRequest == null && !bExpectServiceRequest)
            {
                Debug.WriteLine("ServiceRequest not needed!!!");
                bTestActionResult = true;
            }
            else
            {
                Debug.WriteLine("Inspecting servicing request...");
                PlayReadyLicenseAcquisitionServiceRequest licenseServiceRequest = serviceRequest as PlayReadyLicenseAcquisitionServiceRequest;

                if (licenseServiceRequest == null)
                {
                    Debug.WriteLine("!!!!!!!!!!!!!!! servicing request is null !!!!!!!!!!");
                }
                else
                {
                    Debug.WriteLine("licenseServiceRequest.ContentHeader.KeyId = " + licenseServiceRequest.ContentHeader.KeyId.ToString());
                    if (licenseServiceRequest.ContentHeader.KeyId == new Guid(strRootKid))
                    {
                        Debug.WriteLine("KeyId is equal to expired Root KeyId, as expected");
                        bTestActionResult = true;
                    }
                }
            }

            TestActionFinished(bTestActionResult, null);
            Debug.WriteLine("Leave Test_ContentResolver()");
        }


        public void Test_CocktailContentResolver(string strContentHeader, string strLAURL, string strExpectServiceRequest)
        {
            Debug.WriteLine("Enter Test_CocktailContentResolver()");

            bool bExpectServiceRequest = ActionParamConvertToBool(strExpectServiceRequest);
            Uri uri = ActionParamConvertToUri(strLAURL);

            Debug.WriteLine(strContentHeader);
            PlayReadyContentHeader contentHeader = new PlayReadyContentHeader(
                                                            new System.Text.UnicodeEncoding().GetBytes(strContentHeader),
                                                            uri,
                                                            uri,
                                                            String.Empty,
                                                            Guid.Empty
                                                            );

            LicenseAcquisition.DumpContentHeaderValues(contentHeader);

            Debug.WriteLine("Using PlayReadyContentResolver...");
            IPlayReadyServiceRequest serviceRequest = PlayReadyContentResolver.ServiceRequest(contentHeader);
            if (serviceRequest == null && !bExpectServiceRequest)
            {
                Debug.WriteLine("ServiceRequest not needed!!!");
                TestActionFinished(true, null);
            }
            else
            {
                Debug.WriteLine("Servicing request...");
                PlayReadyLicenseAcquisitionServiceRequest licenseServiceRequest = serviceRequest as PlayReadyLicenseAcquisitionServiceRequest;

                LAAndReportResult licenseAcquisition = new LAAndReportResult(new ReportResultDelegate(TestActionFinished), null);
                licenseAcquisition.AcquireLicenseReactively(serviceRequest as PlayReadyLicenseAcquisitionServiceRequest);
            }

            Debug.WriteLine("Leave Test_CocktailContentResolver()");
        }

        void VerifyDomainShouldExist(Guid guidAccountId, Guid guidServiceId)
        {
            PlayReadyDomain domain = DomainManagement.FindSingleDomain(guidAccountId);
            if (domain == null)
            {
                Debug.WriteLine("Domain not found!!!");
                TestActionFinished(false, null);
            }
            else
            {
                if (domain.ServiceId == guidServiceId)
                {
                    Debug.WriteLine("Matching domain found in license store!!!");
                    TestActionFinished(true,null);
                }
                else
                {
                    Debug.WriteLine("Matching domain not found in license store!!!");
                    TestActionFinished(false, null);
                }
            }
        }

        void VerifyDomainShouldNotExist(Guid guidAccountId)
        {
            PlayReadyDomain domain = DomainManagement.FindSingleDomain(guidAccountId);
            if (domain == null)
            {
                Debug.WriteLine("Domain not found!!!");
                TestActionFinished(true, null);
            }
            else
            {
                Debug.WriteLine("Domain found!!!");
                TestActionFinished(false, null);
            }
        }

        public void Test_VerifyDomain(string strAccountId, string strServiceId, string strShouldExist)
        {
            Debug.WriteLine("Enter Test_VerifyDomain()");

            Guid guidAccountId = ActionParamConvertToGuid(strAccountId);
            Guid guidServiceId = ActionParamConvertToGuid(strServiceId);
            bool bShouldExist = ActionParamConvertToBool(strShouldExist);

            if (bShouldExist)
            {
                VerifyDomainShouldExist(guidAccountId, guidServiceId);
            }
            else
            {
                VerifyDomainShouldNotExist(guidAccountId);
            }

            Debug.WriteLine("Leave Test_VerifyDomain()");
        }

        void VerifyDomainsShouldExist(Guid guidAccountId, uint? uintCount)
        {
            IPlayReadyDomain[] domains = DomainManagement.FindMultipleDomains(guidAccountId);
            if (domains == null)
            {
                Debug.WriteLine("Domains not found!!!");
                TestActionFinished(false, null);
            }
            else
            {
                if (!uintCount.HasValue)
                {
                    TestActionFinished(true, null);
                }
                else
                {
                    if (domains.Length == uintCount.Value)
                    {
                        Debug.WriteLine("Domain count matched!!!");
                        TestActionFinished(true, null);
                    }
                    else
                    {
                        Debug.WriteLine("Domain count not matching!!!");
                        TestActionFinished(false, null);
                    }
                }
            }
        }

        void VerifyDomainsShouldNotExist(Guid guidAccountId)
        {
            IPlayReadyDomain[] domains = DomainManagement.FindMultipleDomains(guidAccountId);
            if (domains == null)
            {
                Debug.WriteLine("Domains not found!!!");
                TestActionFinished(true, null);
            }
            else
            {
                Debug.WriteLine("Domains found. Count = " + domains.Length);
                TestActionFinished(false, null);
            }
        }

        public void Test_VerifyMultipleDomains(
                                        string strAccountId,
                                        string strShouldExist,
                                        string strCount
                                        )
        {
            Debug.WriteLine("Enter Test_VerifyMultipleDomains()");

            Guid guidAccountId = ActionParamConvertToGuid(strAccountId);
            bool bLicenseShouldExist = ActionParamConvertToBool(strShouldExist);
            Nullable<uint> uintCount = ActionParamConvertToUint(strCount);

            if (bLicenseShouldExist)
            {
                VerifyDomainsShouldExist(guidAccountId, uintCount);
            }
            else
            {
                VerifyDomainsShouldNotExist(guidAccountId);
            }

            Debug.WriteLine("Leave Test_VerifyMultipleDomains()");
        }


        MeteringAndReportResult _meteringAndReportResult = null;
        public async void Test_ReportMeteringData(string strCertName,
                                                  string strLAUrl,
                                                  string strCustomData,
                                                  string strManualEnabling,
                                                  string strExpectError,
                                                  string strPlayCount)
        {
            Debug.WriteLine("Enter Test_ReportMeteringData()");

            bool bExpectError = ActionParamConvertToBool(strExpectError);
            uint? playCount = ActionParamConvertToUint(strPlayCount);

            Debug.WriteLine("Reading metering cert...");
            
            if(strCertName == null)
            {
                throw new ArgumentException("certName is null");
            }

            Uri uriCertFile = new Uri("ms-appx:///PRFiles/" + strCertName);

            StorageFile certfile = await StorageFile.GetFileFromApplicationUriAsync(uriCertFile);

            if (certfile == null)
            {
                throw new Exception("MeteringCertFile can't be found");
            }

            
            IBuffer buffer = await FileIO.ReadBufferAsync(certfile);
            byte[] meteringCertBytes = new byte[buffer.Length];

            // Use a dataReader object to read from the buffer
            using (DataReader dataReader = DataReader.FromBuffer(buffer))
            {
                dataReader.ReadBytes(meteringCertBytes);
                // Perform additional tasks
            }

            ServiceRequestConfigData requestConfigData = new ServiceRequestConfigData();
            requestConfigData.ManualEnabling = ActionParamConvertToBool(strManualEnabling);

            _meteringAndReportResult = new MeteringAndReportResult(
                                                    new ReportResultDelegate(TestActionFinished),
                                                    bExpectError,
                                                    playCount == null ? 0 : playCount.Value
                                                    );
            _meteringAndReportResult.RequestConfigData = requestConfigData;
            _meteringAndReportResult.SetMeteringCertificate(meteringCertBytes);
            _meteringAndReportResult.MeteringReportProactively();
            
            Debug.WriteLine("Leave Test_ReportMeteringData()");
        }

        SecureStopAndReportResult _secureStopAndReportResult = null;
        public async void Test_ReportSecureStopData(string strCertName,
                                                    string strUrl,
                                                    string strCustomData,
                                                    string strManualEnabling,
                                                    string strExpectError)
        {
            Debug.WriteLine("Enter Test_ReportSecureStopData()");

            bool bExpectError = ActionParamConvertToBool(strExpectError);

            Debug.WriteLine("Reading secure stop cert...");

            if (strCertName == null)
            {
                throw new ArgumentException("certName is null");
            }

            Uri uriCertFile = new Uri("ms-appx:///PRFiles/" + strCertName);

            StorageFile certfile = await StorageFile.GetFileFromApplicationUriAsync(uriCertFile);

            if (certfile == null)
            {
                throw new Exception("SecureStopCertFile can't be found");
            }

            IBuffer buffer = await FileIO.ReadBufferAsync(certfile);
            byte[] secureStopCertBytes = new byte[buffer.Length];

            // Use a dataReader object to read from the buffer
            using (DataReader dataReader = DataReader.FromBuffer(buffer))
            {
                dataReader.ReadBytes(secureStopCertBytes);
                // Perform additional tasks
            }

            ServiceRequestConfigData requestConfigData = new ServiceRequestConfigData();
            requestConfigData.ManualEnabling = ActionParamConvertToBool(strManualEnabling);
            requestConfigData.Uri = new Uri(strUrl);

            _secureStopAndReportResult = new SecureStopAndReportResult(
                                                    new ReportResultDelegate(TestActionFinished),
                                                    bExpectError);
            _secureStopAndReportResult.RequestConfigData = requestConfigData;
            _secureStopAndReportResult.SetSecureStopCertificate(secureStopCertBytes);
            _secureStopAndReportResult.SecureStopProactively();

            Debug.WriteLine("Leave Test_ReportSecureStopData()");
        }

        public async void Test_HeaderWithEmbeddedUpdates(string strKeyId,
                                                         string strEncryptionAlgorithm,
                                                         string strLAURL,
                                                         string strServiceID,
                                                         string strExpectFailure)
        {
            Debug.WriteLine(" ");
            Debug.WriteLine("Enter Test_HeaderWithEmbeddedUpdates()");
            bool bActionSucceeded = false;

            Guid keyIdGuid = ActionParamConvertToGuid(strKeyId);
            Uri uriLA = ActionParamConvertToUri(strLAURL);
            Guid guidServiceId = ActionParamConvertToGuid(strServiceID);

            PlayReadyEncryptionAlgorithm encryptionAlgorithm = ActionParamConvertToPlayReadyEncryptionAlgorithm(strEncryptionAlgorithm);

            Debug.WriteLine("Creating PlayReadyContentHeader..");
            PlayReadyContentHeader contentHeader = new PlayReadyContentHeader(
                                                                                keyIdGuid,
                                                                                string.Empty,
                                                                                encryptionAlgorithm,
                                                                                uriLA,
                                                                                uriLA,
                                                                                String.Empty,
                                                                                guidServiceId);

            Debug.WriteLine("Getting HeaderWithEmbeddedUpdates..");
            PlayReadyContentHeader contentHeaderWithEmbeddedUpdates = contentHeader.HeaderWithEmbeddedUpdates;
            if (contentHeaderWithEmbeddedUpdates == null)
            {
                Debug.WriteLine("HeaderWithEmbeddedUpdates not available");
                if (strExpectFailure.ToLower() == "true")
                {
                    bActionSucceeded = true;
                }
            }
            else
            {
                byte[] headerBytes = contentHeaderWithEmbeddedUpdates.GetSerializedHeader();

                Debug.WriteLine("HeaderWithEmbeddedUpdates:");

                Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                StorageFile HeaderWithEmbeddedLicenseFile = await localFolder.CreateFileAsync("HeaderWithEmbeddedLicense.bin",
                                                                                                CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteBytesAsync(HeaderWithEmbeddedLicenseFile, headerBytes);

                string strHeader = Encoding.UTF8.GetString(headerBytes, 0, headerBytes.Length);

                if (strHeader.Contains("EST") && strHeader.Contains("XMR"))
                {
                    Debug.WriteLine("Header contains EST and XMR");
                    bActionSucceeded = true;
                }
                else
                {
                    Debug.WriteLine("Header doesn't contains EST and XMR");
                }
            }

            TestActionFinished(bActionSucceeded, null);
            Debug.WriteLine("Leave Test_HeaderWithEmbeddedUpdates()");

        }

        void DumpPlayReadySettings()
        {
            Debug.WriteLine("Dumping PlayReadySettings...");

            Windows.Storage.ApplicationData appData = Windows.Storage.ApplicationData.Current;

            Windows.Storage.StorageFolder storageFolder = appData.LocalFolder;
            Debug.WriteLine("AppData LocalFolder Path = " + storageFolder.Path);

            IReadOnlyDictionary<string, ApplicationDataContainer> dictionary = appData.LocalSettings.Containers;
            Debug.WriteLine("Container count = " + dictionary.Count);
            foreach (string containerName in dictionary.Keys)
            {
                Debug.WriteLine("Container name = " + containerName);
            }

            Windows.Storage.ApplicationDataContainer playreadySettings = appData.LocalSettings.Containers["PlayReady"];

            Debug.WriteLine("Settings container Name = " + playreadySettings.Name);
            Windows.Foundation.Collections.IPropertySet propertySetValues = playreadySettings.Values;
            foreach (string strKey in propertySetValues.Keys)
            {
                string strValue = propertySetValues[strKey].ToString();
                Debug.WriteLine("Key     = " + strKey);
                Debug.WriteLine("Value   = " + strValue);
            }
        }

        public void Test_SetIndivServerUrl(string strUrl)
        {
            Debug.WriteLine("Enter Test_SetIndivServerUrl()");

            Windows.Storage.ApplicationDataContainer playreadySettings = Windows.Storage.ApplicationData.Current.LocalSettings.Containers["PlayReady"];
            playreadySettings.Values["IndivServerURL"] = strUrl;
            DumpPlayReadySettings();

            TestActionFinished(true, null);
            Debug.WriteLine("Leave Test_SetIndivServerUrl()");
        }

        public void Test_ResetServiceRequestStatistics()
        {
            Debug.WriteLine("Enter Test_ResetServiceRequestStatistics()");

            SerivceRequestStatistics.Reset();
            TestActionFinished(true, null);

            Debug.WriteLine("Leave Test_ResetServiceRequestStatistics()");
        }

        public void Test_VerifyServiceRequestStatistics(string strIndivCount, string strLACount)
        {
            Debug.WriteLine("Enter Test_VerifyServiceRequestStatistics()");

            uint? indivCount = ActionParamConvertToUint(strIndivCount);
            uint? LACount = ActionParamConvertToUint(strLACount);

            bool bTestActionResult = true;

            if (indivCount.HasValue)
            {
                uint actualIndivCount = SerivceRequestStatistics.GetIndivCount();
                Debug.WriteLine("Actual indiv count = " + actualIndivCount);
                if (indivCount.Value != actualIndivCount)
                {
                    bTestActionResult = false;
                }
            }

            if (LACount.HasValue)
            {
                uint ActualLicenseAcquisitionCount = SerivceRequestStatistics.GetLicenseAcquisitionCount();
                Debug.WriteLine("Actual license acquisition count = " + ActualLicenseAcquisitionCount);
                if (LACount.Value != ActualLicenseAcquisitionCount)
                {
                    bTestActionResult = false;
                }
            }

            TestActionFinished(bTestActionResult, null);

            Debug.WriteLine("Leave Test_VerifyServiceRequestStatistics()");
        }
    }

}
