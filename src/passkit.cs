//
// PassKit bindings
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2012, 2015 Xamarin Inc. All rights reserved.
//

using System;
using XamCore.ObjCRuntime;
using XamCore.Foundation;
using XamCore.UIKit;
#if !WATCH
using XamCore.AddressBook;
#endif
using XamCore.Contacts;

namespace XamCore.PassKit {
	[iOS (9,0)]
	[BaseType (typeof(NSObject))]
	interface PKContact : NSSecureCoding
	{
		[NullAllowed, Export ("name", ArgumentSemantic.Retain)]
		NSPersonNameComponents Name { get; set; }
	
#if XAMCORE_2_0 // The Contacts framework (CNPostalAddress) uses generics heavily, which is only supported in Unified (for now at least)
		[NullAllowed, Export ("postalAddress", ArgumentSemantic.Retain)]
		CNPostalAddress PostalAddress { get; set; }
#endif // XAMCORE_2_0
	
		[NullAllowed, Export ("emailAddress", ArgumentSemantic.Retain)]
		string EmailAddress { get; set; }
	
#if XAMCORE_2_0 // The Contacts framework (CNPhoneNumber) uses generics heavily, which is only supported in Unified (for now at least)
		[NullAllowed, Export ("phoneNumber", ArgumentSemantic.Retain)]
		CNPhoneNumber PhoneNumber { get; set; }
#endif // XAMCORE_2_0

		[iOS (9,2)]
		[NullAllowed, Export ("supplementarySubLocality", ArgumentSemantic.Retain)]
		string SupplementarySubLocality { get; set; }
	}
	
	[Since (6,0)]
	[BaseType (typeof (NSObject))]
	interface PKPassLibrary {
		[Static][Export ("isPassLibraryAvailable")]
		bool IsAvailable { get; }

		[Export ("containsPass:")]
		bool Contains (PKPass pass);

		[Export ("passes")]
		PKPass[] GetPasses ();

		[Export ("passWithPassTypeIdentifier:serialNumber:")]
		PKPass GetPass (string identifier, string serialNumber);

		[iOS (8,0)]
		[Export ("passesOfType:")]
		PKPass [] GetPasses (PKPassType passType);

		[Export ("removePass:")]
		void Remove (PKPass pass);

		[Export ("replacePassWithPass:")]
		bool Replace (PKPass pass);

		[Since (7,0)]
		[Export ("addPasses:withCompletionHandler:")]
		void AddPasses (PKPass[] passes, [NullAllowed] Action<PKPassLibraryAddPassesStatus> completion);

		[Field ("PKPassLibraryDidChangeNotification")]
		[Notification]
		NSString DidChangeNotification { get; }

		[iOS (9,0)]
		[Field ("PKPassLibraryRemotePaymentPassesDidChangeNotification")]
		[Notification]
		NSString RemotePaymentPassesDidChangeNotification { get; }

		[iOS (8,0)]
		[Static,Export ("isPaymentPassActivationAvailable")]
		[Availability (Deprecated = Platform.iOS_9_0, Message = "On iOS 9 and higher, use the library's instance IsLibraryPaymentPassActivationAvailable property instead")]
		bool IsPaymentPassActivationAvailable { get; }

		[iOS (9,0)]
		[Export ("isPaymentPassActivationAvailable")]
		bool IsLibraryPaymentPassActivationAvailable { get; }

		[NoWatch]
		[iOS (8,0)]
		[Export ("activatePaymentPass:withActivationData:completion:")]
		void ActivatePaymentPass (PKPaymentPass paymentPass, NSData activationData, [NullAllowed] Action<bool, NSError> completion);

		[NoWatch]
		[iOS (8,0)]
		[Export ("activatePaymentPass:withActivationCode:completion:")]
		void ActivatePaymentPass (PKPaymentPass paymentPass, string activationCode, [NullAllowed] Action<bool, NSError> completion);

		[NoWatch]
		[iOS (8,3)]
		[Export ("openPaymentSetup")]
		void OpenPaymentSetup ();

		[iOS (9,0)]
		[Export ("canAddPaymentPassWithPrimaryAccountIdentifier:")]
		bool CanAddPaymentPass (string primaryAccountIdentifier);

		[NoWatch]
		[iOS(9,0)]
		[Static]
		[Export ("endAutomaticPassPresentationSuppressionWithRequestToken:")]
		void EndAutomaticPassPresentationSuppression (nuint requestToken);

		[NoWatch]
		[iOS(9,0)]
		[Static]
		[Export ("isSuppressingAutomaticPassPresentation")]
		bool IsSuppressingAutomaticPassPresentation { get; }

		[iOS (9,0)]
		[Export ("remotePaymentPasses")]
		PKPaymentPass[] RemotePaymentPasses { get; }

#if !WATCH
		[NoWatch]
		[iOS(9,0)]
		[Static]
		[Export ("requestAutomaticPassPresentationSuppressionWithResponseHandler:")]
		nuint RequestAutomaticPassPresentationSuppression (Action<PKAutomaticPassPresentationSuppressionResult> responseHandler);
#endif		
	}

	[Since (6,0)]
	[Static]
	interface PKPassLibraryUserInfoKey
	{
		[Field ("PKPassLibraryAddedPassesUserInfoKey")]
		NSString AddedPasses { get; }

		[Field ("PKPassLibraryReplacementPassesUserInfoKey")]
		NSString ReplacementPasses { get; }

		[Field ("PKPassLibraryRemovedPassInfosUserInfoKey")]
		NSString RemovedPassInfos { get; }

		[Field ("PKPassLibraryPassTypeIdentifierUserInfoKey")]
		NSString PassTypeIdentifier { get; }

		[Field ("PKPassLibrarySerialNumberUserInfoKey")]
		NSString SerialNumber { get; }
	}

#if !WATCH
	[iOS (8,0)]
	[BaseType (typeof (NSObject))]
	interface PKPayment {
		[Export ("token")]
		PKPaymentToken Token { get; }

		[Export ("billingAddress")]
		[Availability (Deprecated = Platform.iOS_9_0, Message = "On iOS 9 and higher, use BillingContact instead")]
		ABRecord BillingAddress { get; }

		[Export ("shippingAddress")]
		[Availability (Deprecated = Platform.iOS_9_0, Message = "On iOS 9 and higher, use ShippingContact instead")]
		ABRecord ShippingAddress { get; }

		[Export ("shippingMethod")]
		PKShippingMethod ShippingMethod { get; }

		
		[iOS (9,0)]
		[NullAllowed, Export ("shippingContact")]
		PKContact ShippingContact { get; }

		[iOS (9,0)]
		[NullAllowed, Export ("billingContact")]
		PKContact BillingContact { get; }
	}

	public delegate void PKPaymentShippingAddressSelected (PKPaymentAuthorizationStatus status, PKShippingMethod [] shippingMethods, PKPaymentSummaryItem [] summaryItems);
	public delegate void PKPaymentShippingMethodSelected (PKPaymentAuthorizationStatus status, PKPaymentSummaryItem[] summaryItems);

#if !XAMCORE_2_0
	delegate void PKPaymentAuthorizationHandler (PKPaymentAuthorizationStatus status);
#endif

	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface PKPaymentAuthorizationViewControllerDelegate {
		[Export ("paymentAuthorizationViewController:didAuthorizePayment:completion:")]
		[EventArgs ("PKPaymentAuthorization")]
		[Abstract]
		void DidAuthorizePayment (PKPaymentAuthorizationViewController controller, PKPayment payment, 
#if XAMCORE_2_0
			Action<PKPaymentAuthorizationStatus> completion);
#else
			PKPaymentAuthorizationHandler completion);
#endif

		[Export ("paymentAuthorizationViewControllerDidFinish:")]
		[Abstract]
		void PaymentAuthorizationViewControllerDidFinish (PKPaymentAuthorizationViewController controller);

		[Export ("paymentAuthorizationViewController:didSelectShippingMethod:completion:")]
		[EventArgs ("PKPaymentShippingMethodSelected")]
		void DidSelectShippingMethod (PKPaymentAuthorizationViewController controller, PKShippingMethod shippingMethod, PKPaymentShippingMethodSelected completion);

		[Export ("paymentAuthorizationViewController:didSelectShippingAddress:completion:")]
		[EventArgs ("PKPaymentShippingAddressSelected")]
		void DidSelectShippingAddress (PKPaymentAuthorizationViewController controller, ABRecord address, PKPaymentShippingAddressSelected completion);

		[iOS (8,3)]
		[Export ("paymentAuthorizationViewControllerWillAuthorizePayment:")]
#if !XAMCORE_4_0
		[Abstract]
#endif
		void WillAuthorizePayment (PKPaymentAuthorizationViewController controller);

		[iOS (9,0)]
		[Export ("paymentAuthorizationViewController:didSelectShippingContact:completion:")]
		[EventArgs ("PKPaymentSelectedContact")]
		void DidSelectShippingContact (PKPaymentAuthorizationViewController controller, PKContact contact, PKPaymentShippingAddressSelected completion);

		[iOS (9,0)]
		[Export ("paymentAuthorizationViewController:didSelectPaymentMethod:completion:")]
		[EventArgs ("PKPaymentMethodSelected")]
		void DidSelectPaymentMethod (PKPaymentAuthorizationViewController controller, PKPaymentMethod paymentMethod, Action<PKPaymentSummaryItem[]> completion);
	}

	[iOS (8,0)]
	[BaseType (typeof (UIViewController), Delegates=new string []{"Delegate"}, Events=new Type [] {typeof(PKPaymentAuthorizationViewControllerDelegate)})]
	interface PKPaymentAuthorizationViewController {
		[DesignatedInitializer]
		[Export ("initWithPaymentRequest:")]
		IntPtr Constructor (PKPaymentRequest request);

		[Export ("delegate", ArgumentSemantic.UnsafeUnretained)]
		[NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		PKPaymentAuthorizationViewControllerDelegate Delegate { get; set; }

		[Static, Export ("canMakePayments")]
		bool CanMakePayments { get; }

		// These are the NSString constants
		[Static, Export ("canMakePaymentsUsingNetworks:")]
		bool CanMakePaymentsUsingNetworks (NSString [] paymentNetworks);

		[iOS (9,0)]
		[Static]
		[Export ("canMakePaymentsUsingNetworks:capabilities:")]
		bool CanMakePaymentsUsingNetworks (string[] supportedNetworks, PKMerchantCapability capabilties);
	}

	[iOS (8,0)]
	[BaseType (typeof (NSObject))]
	public interface PKPaymentSummaryItem {
		[NullAllowed] // by default this property is null
		[Export ("label")]
		string Label { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("amount", ArgumentSemantic.Copy)]
		NSDecimalNumber Amount { get; set; }

		[Static, Export ("summaryItemWithLabel:amount:")]
		PKPaymentSummaryItem Create (string label, NSDecimalNumber amount);

		[iOS (9,0)]
		[Export ("type", ArgumentSemantic.Assign)]
		PKPaymentSummaryItemType Type { get; set; }

		[iOS (9,0)]
		[Static]
		[Export ("summaryItemWithLabel:amount:type:")]
		PKPaymentSummaryItem Create (string label, NSDecimalNumber amount, PKPaymentSummaryItemType type);
	}

	[iOS (8,0)]
	[BaseType (typeof (PKPaymentSummaryItem))]
	public interface PKShippingMethod {
		[NullAllowed] // by default this property is null
		[Export ("identifier")]
		string Identifier { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("detail")]
		string Detail { get; set; }
	}

	[iOS (8,0)]
	[BaseType (typeof (NSObject))]
	interface PKPaymentRequest {
		[NullAllowed] // by default this property is null
		[Export ("merchantIdentifier")]
		string MerchantIdentifier { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("countryCode")]
		string CountryCode { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("supportedNetworks", ArgumentSemantic.Copy)]
		NSString [] SupportedNetworks { get; set; }

		[Export ("merchantCapabilities", ArgumentSemantic.UnsafeUnretained)]
		PKMerchantCapability MerchantCapabilities { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("paymentSummaryItems", ArgumentSemantic.Copy)]
		PKPaymentSummaryItem [] PaymentSummaryItems { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("currencyCode")]
		string CurrencyCode { get; set; }

		[Export ("requiredBillingAddressFields", ArgumentSemantic.UnsafeUnretained)]
		PKAddressField RequiredBillingAddressFields { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("billingAddress", ArgumentSemantic.UnsafeUnretained)]
		[Availability (Deprecated = Platform.iOS_9_0, Message = "On iOS 9 and higher, use BillingContact instead")]
		ABRecord BillingAddress { get; set; }

		[Export ("requiredShippingAddressFields", ArgumentSemantic.UnsafeUnretained)]
		PKAddressField RequiredShippingAddressFields { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("shippingAddress", ArgumentSemantic.UnsafeUnretained)]
		[Availability (Deprecated = Platform.iOS_9_0, Message = "On iOS 9 and higher, use ShippingContact instead")]
		ABRecord ShippingAddress { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("shippingMethods", ArgumentSemantic.Copy)]
		PKShippingMethod [] ShippingMethods { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("applicationData", ArgumentSemantic.Copy)]
		NSData ApplicationData { get; set; }

		[iOS (8,3)]
		[Export ("shippingType", ArgumentSemantic.Assign)]
		PKShippingType ShippingType { get; set; }

		[iOS (9,0)]
		[NullAllowed, Export ("shippingContact", ArgumentSemantic.Retain)]
		PKContact ShippingContact { get; set; }

		[iOS (9,0)]
		[NullAllowed, Export ("billingContact", ArgumentSemantic.Retain)]
		PKContact BillingContact { get; set; }
	}

	[iOS (8,0)]
	[BaseType (typeof (NSObject))]
	interface PKPaymentToken {
		[Export ("paymentInstrumentName")]
		[Availability (Deprecated = Platform.iOS_9_0, Message = "On iOS 9 and higher, use PaymentMethod instead")]
		string PaymentInstrumentName { get; }

		[Export ("paymentNetwork")]
		[Availability (Deprecated = Platform.iOS_9_0, Message = "On iOS 9 and higher, use PaymentMethod instead")]
		string PaymentNetwork { get; }

		[Export ("transactionIdentifier")]
		string TransactionIdentifier { get; }

		[Export ("paymentData")]
		NSData PaymentData { get; }

		[iOS (9,0)]
		[Export ("paymentMethod")]
		PKPaymentMethod PaymentMethod { get; }		
	}

	[Since (6,0)]
	[BaseType (typeof (UIViewController), Delegates = new string [] {"WeakDelegate"}, Events = new Type [] { typeof (PKAddPassesViewControllerDelegate) })]
	// invalid null handle for default 'init'
	[DisableDefaultCtor]
	interface PKAddPassesViewController {

		[Export ("initWithNibName:bundle:")]
		[PostGet ("NibBundle")]
		IntPtr Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);

		[Export ("initWithPass:")]
		IntPtr Constructor (PKPass pass);

		[Since (7,0)]
		[Export ("initWithPasses:")]
		IntPtr Constructor (PKPass[] pass);

		[iOS (8,0)]
		[Static]
		[Export ("canAddPasses")]
		bool CanAddPasses { get;}
			
		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		PKAddPassesViewControllerDelegate Delegate { get; set;  }
	}

	[Since (6,0)]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface PKAddPassesViewControllerDelegate {
		[Export ("addPassesViewControllerDidFinish:")]
		void Finished (PKAddPassesViewController controller);
	}

	[iOS (9,0)]
	[BaseType (typeof(NSObject))]
	interface PKAddPaymentPassRequest : NSSecureCoding
	{
		[NullAllowed, Export ("encryptedPassData", ArgumentSemantic.Copy)]
		NSData EncryptedPassData { get; set; }
	
		[NullAllowed, Export ("activationData", ArgumentSemantic.Copy)]
		NSData ActivationData { get; set; }
	
		[NullAllowed, Export ("ephemeralPublicKey", ArgumentSemantic.Copy)]
		NSData EphemeralPublicKey { get; set; }
	
		[NullAllowed, Export ("wrappedKey", ArgumentSemantic.Copy)]
		NSData WrappedKey { get; set; }
	}

	[iOS (9,0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface PKAddPaymentPassRequestConfiguration : NSSecureCoding
	{
		[DesignatedInitializer]
		[Export ("initWithEncryptionScheme:")]
		IntPtr Constructor (NSString encryptionScheme);
	
		[Export ("encryptionScheme")]
		NSString EncryptionScheme { get; }
	
		[NullAllowed, Export ("cardholderName")]
		string CardholderName { get; set; }
	
		[NullAllowed, Export ("primaryAccountSuffix")]
		string PrimaryAccountSuffix { get; set; }
	
		[NullAllowed, Export ("localizedDescription")]
		string LocalizedDescription { get; set; }
	
		[NullAllowed, Export ("primaryAccountIdentifier")]
		string PrimaryAccountIdentifier { get; set; }
	
		[NullAllowed, Export ("paymentNetwork")]
		string PaymentNetwork { get; set; }
	}

	[iOS (9,0)]
	[BaseType (typeof(UIViewController))]
	[DisableDefaultCtor]
	interface PKAddPaymentPassViewController
	{
		[Static]
		[Export ("canAddPaymentPass")]
		bool CanAddPaymentPass { get; }
	
		[DesignatedInitializer]
		[Export ("initWithRequestConfiguration:delegate:")]
		IntPtr Constructor (PKAddPaymentPassRequestConfiguration configuration, [NullAllowed] PKAddPaymentPassViewControllerDelegate viewControllerDelegate);
	
		[Wrap ("WeakDelegate")]
		[NullAllowed, Protocolize]
		PKAddPaymentPassViewControllerDelegate Delegate { get; set; }
	
		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }
	}
	
	[Protocol, Model]
	[BaseType (typeof(NSObject))]
	interface PKAddPaymentPassViewControllerDelegate
	{
		[Abstract]
		[Export ("addPaymentPassViewController:generateRequestWithCertificateChain:nonce:nonceSignature:completionHandler:")]
		void GenerateRequestWithCertificateChain (PKAddPaymentPassViewController controller, NSData[] certificates, NSData nonce, NSData nonceSignature, Action<PKAddPaymentPassRequest> handler);
	
		[Abstract]
		[Export ("addPaymentPassViewController:didFinishAddingPaymentPass:error:")]
		void DidFinishAddingPaymentPass (PKAddPaymentPassViewController controller, [NullAllowed] PKPaymentPass pass, [NullAllowed] NSError error);
	}
#endif // !WATCH
		
	[Since (6,0)]
	[BaseType (typeof (PKObject))]
	interface PKPass : NSSecureCoding, NSCopying {
		[Export ("initWithData:error:")]
		IntPtr Constructor (NSData data, out NSError error);

		[Export ("authenticationToken", ArgumentSemantic.Copy)]
		string AuthenticationToken { get; }

		[NoWatch]
		[Export ("icon", ArgumentSemantic.Copy)]
		UIImage Icon { get; }

		[Export ("localizedDescription", ArgumentSemantic.Copy)]
		string LocalizedDescription { get; }

		[Export ("localizedName", ArgumentSemantic.Copy)]
		string LocalizedName { get; }

		[Export ("organizationName", ArgumentSemantic.Copy)]
		string OrganizationName { get; }

		[Export ("passTypeIdentifier", ArgumentSemantic.Copy)]
		string PassTypeIdentifier { get; }

		[Export ("passURL", ArgumentSemantic.Copy)]
		NSUrl PassUrl { get; }

		[Export ("relevantDate", ArgumentSemantic.Copy)]
		NSDate RelevantDate { get; }

		[Export ("serialNumber", ArgumentSemantic.Copy)]
		string SerialNumber { get; }

		[Export ("webServiceURL", ArgumentSemantic.Copy)]
		NSUrl WebServiceUrl { get; }

		[Export ("localizedValueForFieldKey:")]
		NSObject GetLocalizedValue (NSString key); // TODO: Should be enum for PKPassLibraryUserInfoKey

		[Field ("PKPassKitErrorDomain")]
		NSString ErrorDomain { get; }

		[Since (7,0)]
		[Export ("userInfo", ArgumentSemantic.Copy)]
		NSDictionary UserInfo { get; }

		[iOS (8,0)]
		[Export ("passType")]
		PKPassType PassType { get; }

		[iOS (8,0)]
		[Export ("paymentPass")]
		PKPaymentPass PaymentPass { get; }

		[iOS (9,0)]
		[Export ("remotePass")]
		bool RemotePass { [Bind ("isRemotePass")] get; }

		[iOS (9,0)]
		[Export ("deviceName")]
		string DeviceName { get; }		
	}

#if !WATCH
	[iOS (9,0)]
	[BaseType (typeof(NSObject))]
	interface PKPaymentMethod : NSSecureCoding
	{
		[NullAllowed, Export ("displayName")]
		string DisplayName { get; }
	
		[NullAllowed, Export ("network")]
		string Network { get; }
	
		[Export ("type")]
		PKPaymentMethodType Type { get; }

		[NullAllowed, Export ("paymentPass")]
		PKPaymentPass PaymentPass { get; }
	}
#endif // !WATCH
	
	[iOS (8,0)]
	[BaseType (typeof (PKPass))]
	interface PKPaymentPass {

		[Export ("primaryAccountIdentifier")]
		string PrimaryAccountIdentifier { get; }

		[Export ("primaryAccountNumberSuffix")]
		string PrimaryAccountNumberSuffix { get; }

		[Export ("deviceAccountIdentifier")]
		string DeviceAccountIdentifier { get; }

		[Export ("deviceAccountNumberSuffix")]
		string DeviceAccountNumberSuffix { get; }

		[Export ("activationState")]
		PKPaymentPassActivationState ActivationState { get; }
	}
	
	[iOS (8,0)]
	[BaseType (typeof (NSObject))]
	public partial interface PKObject : NSCoding, NSSecureCoding, NSCopying {
		//Empty class in header file
	}

	[Static]
	[iOS (8,0)]
	interface PKPaymentNetwork {
		[Field ("PKPaymentNetworkAmex")]
		NSString Amex { get; }

		[iOS (9,2)]
		[Watch (2,2)]
		[Field ("PKPaymentNetworkChinaUnionPay")]
		NSString ChinaUnionPay { get; }

		[iOS (9,2)]
		[Watch (2,2)]
		[Field ("PKPaymentNetworkInterac")]
		NSString Interac { get; }

		[Field ("PKPaymentNetworkMasterCard")]
		NSString MasterCard { get; }

		[Field ("PKPaymentNetworkVisa")]
		NSString Visa { get; }

		[iOS (9,0)]
		[Field ("PKPaymentNetworkDiscover")]
		NSString Discover { get; }

		[iOS (9,0)]
		[Field ("PKPaymentNetworkPrivateLabel")]
		NSString PrivateLabel { get; }
	}

#if !WATCH
	[iOS (8,3)]
	[BaseType (typeof (UIButton))]
	[DisableDefaultCtor]
	interface PKPaymentButton {

		[Static]
		[Export ("buttonWithType:style:")]
		// note: named like UIButton method
		PKPaymentButton FromType (PKPaymentButtonType buttonType, PKPaymentButtonStyle buttonStyle);

		[iOS (9,0)]
		[Export ("initWithPaymentButtonType:paymentButtonStyle:")]
		[DesignatedInitializer]
		IntPtr Constructor (PKPaymentButtonType type, PKPaymentButtonStyle style);
	}

	[iOS (9,0)]
	[BaseType (typeof (UIButton))]
	[DisableDefaultCtor]
	interface PKAddPassButton {
		[Static]
		[Export ("addPassButtonWithStyle:")]
		PKAddPassButton Create (PKAddPassButtonStyle addPassButtonStyle);

		[Export ("initWithAddPassButtonStyle:")]
		[DesignatedInitializer]
		IntPtr Constructor (PKAddPassButtonStyle style);

		[Appearance]
		[Export ("addPassButtonStyle", ArgumentSemantic.Assign)]
		PKAddPassButtonStyle Style { get; set; }
	}
#endif // !WATCH

	[iOS(9,0)]
	[Static]
	interface PKEncryptionScheme {
		[Field ("PKEncryptionSchemeECC_V2")]
		NSString Ecc_V2 { get; }
	}
	
}