//-----------------------------------------------------------------------
// <copyright file="Entropy.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.IdentityModel.Protocols.WSTrust
{
    using System.IdentityModel.Tokens;

    /// <summary>
    /// The Entropy used in both token request message and token response message. 
    /// </summary>
    public class Entropy : ProtectedKey
    {
        /// <summary>
        /// Use this constructor to create an Entropy object with some randomly generated bytes.
        /// </summary>
        /// <param name="entropySizeInBits">The entropySizeInBits of the key material inside the entropy.</param>
        public Entropy( int entropySizeInBits )
            : this( CryptoHelper.GenerateRandomBytes( entropySizeInBits ) )
        {
        }

        /// <summary>
        /// Constructor for sending entropy in binary secret format.
        /// </summary>
        /// <param name="secret">The key material.</param>
        public Entropy( byte[] secret )
            : base( secret )
        {
        }

        /// <summary>
        /// Constructor for sending entropy in encrypted key format.
        /// </summary>
        /// <param name="secret">The key material.</param>
        /// <param name="wrappingCredentials">The encrypting credentials used to encrypt the key material.</param>
        public Entropy( byte[] secret, EncryptingCredentials wrappingCredentials )
            : base( secret, wrappingCredentials )
        {
        }

        /// <summary>
        /// Constructs an entropy instance with the protected key.
        /// </summary>
        /// <param name="protectedKey">The protected key which can be either binary secret or encrypted key.</param>
        public Entropy( ProtectedKey protectedKey )
            : base( GetKeyBytesFromProtectedKey( protectedKey ), GetWrappingCredentialsFromProtectedKey( protectedKey ) )
        {
        }

        static byte[] GetKeyBytesFromProtectedKey( ProtectedKey protectedKey )
        {
            if ( protectedKey == null )
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull( "protectedKey" );
            }

            return protectedKey.GetKeyBytes();
        }

        static EncryptingCredentials GetWrappingCredentialsFromProtectedKey( ProtectedKey protectedKey )
        {
            if ( protectedKey == null )
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull( "protectedKey" );
            }

            return protectedKey.WrappingCredentials;
        }
    }
}