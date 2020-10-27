using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace CPTech.Security
{
	public class RsaUtil
	{
		private readonly RSA privateKeyRsaProvider;
		private readonly RSA publicKeyRsaProvider;

        public RSAType RsaType { get; set; }
		public RSAKeyType RsaKeyType { get; set; }
		public Encoding EncodingType { get; set; } = Encoding.UTF8;

		public RsaUtil(string privateKey, string publicKey = null, RSAType rsaType = RSAType.RSA2,
			RSAKeyType rsaKeyType = RSAKeyType.PKCS8)
        {
			this.RsaType = rsaType;
			this.RsaKeyType = rsaKeyType;

			if (!string.IsNullOrWhiteSpace(privateKey)) privateKeyRsaProvider = CreateRsaProviderFromPrivateKey(privateKey);

			if (!string.IsNullOrWhiteSpace(publicKey)) publicKeyRsaProvider = CreateRsaProviderFromPublicKey(publicKey);
		}

		#region 加密
		public string Encrypt(string text)
		{
			_ = publicKeyRsaProvider ?? throw new Exception("_publicKeyRsaProvider is null");

			return Convert.ToBase64String(publicKeyRsaProvider.Encrypt(EncodingType.GetBytes(text), RSAEncryptionPadding.Pkcs1));
		}
		#endregion

		#region 解密
		public string Decrypt(string cipherText)
		{
			_ = privateKeyRsaProvider ?? throw new Exception("_privateKeyRsaProvider is null");

			return EncodingType.GetString(privateKeyRsaProvider.Decrypt(Convert.FromBase64String(cipherText), RSAEncryptionPadding.Pkcs1));
		}
		#endregion

		#region 使用私钥签名
		public string Sign(string data)
		{
			return Convert.ToBase64String(privateKeyRsaProvider.SignData(EncodingType.GetBytes(data),
				RsaType == RSAType.RSA2 ? HashAlgorithmName.SHA256 : HashAlgorithmName.SHA1,
				RSASignaturePadding.Pkcs1));
		}
		#endregion

		#region 使用公钥验证签名
		public bool Verify(string data,string sign)
		{
			return publicKeyRsaProvider.VerifyData(EncodingType.GetBytes(data),
				Convert.FromBase64String(sign),
				RsaType == RSAType.RSA2 ? HashAlgorithmName.SHA256 : HashAlgorithmName.SHA1,
				RSASignaturePadding.Pkcs1);
		}
		#endregion

		#region 密钥算法
		public RSA CreateRsaProviderFromPrivateKey(string privateKey)
		{
			var privateKeyBits = Convert.FromBase64String(privateKey);
			if (RsaKeyType == RSAKeyType.PKCS8) privateKeyBits = ConvertToPkcs1(privateKeyBits);

			var rsa = RSA.Create();
			var rsaParameters = new RSAParameters();

			using (BinaryReader reader = new BinaryReader(new MemoryStream(privateKeyBits)))
			{
				ushort twobytes = reader.ReadUInt16();
				if (twobytes == 0x8130)
					reader.ReadByte();
				else if (twobytes == 0x8230)
					reader.ReadInt16();
				else
					throw new Exception("Unexpected value read binr.ReadUInt16()");

				twobytes = reader.ReadUInt16();
				if (twobytes != 0x0102) throw new Exception("Unexpected version");

				byte bt = reader.ReadByte();
				if (bt != 0x00) throw new Exception("Unexpected value read binr.ReadByte()");

				rsaParameters.Modulus = reader.ReadBytes(GetIntegerSize(reader));
				rsaParameters.Exponent = reader.ReadBytes(GetIntegerSize(reader));
				rsaParameters.D = reader.ReadBytes(GetIntegerSize(reader));
				rsaParameters.P = reader.ReadBytes(GetIntegerSize(reader));
				rsaParameters.Q = reader.ReadBytes(GetIntegerSize(reader));
				rsaParameters.DP = reader.ReadBytes(GetIntegerSize(reader));
				rsaParameters.DQ = reader.ReadBytes(GetIntegerSize(reader));
				rsaParameters.InverseQ = reader.ReadBytes(GetIntegerSize(reader));
			}

			rsa.ImportParameters(rsaParameters);
			return rsa;
		}

		public RSA CreateRsaProviderFromPublicKey(string publicKeyString)
		{
			// encoded OID sequence for  PKCS #1 rsaEncryption szOID_RSA_RSA = "1.2.840.113549.1.1.1"
			byte[] seqOid = { 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00 };
			
			var x509Key = Convert.FromBase64String(publicKeyString);

            using BinaryReader reader = new BinaryReader(new MemoryStream(x509Key));

			ushort twobytes = reader.ReadUInt16();
            if (twobytes == 0x8130) //data read as little endian order (actual data order for Sequence is 30 81)
                reader.ReadByte();    //advance 1 byte
            else if (twobytes == 0x8230)
                reader.ReadInt16();   //advance 2 bytes
            else
                return null;

			byte[] seq = reader.ReadBytes(15);       //read the Sequence OID
            if (!CompareBytearrays(seq, seqOid)) return null;

            twobytes = reader.ReadUInt16();
            if (twobytes == 0x8103) //data read as little endian order (actual data order for Bit String is 03 81)
                reader.ReadByte();    //advance 1 byte
            else if (twobytes == 0x8203)
                reader.ReadInt16();   //advance 2 bytes
            else
                return null;

			byte bt = reader.ReadByte();
            if (bt != 0x00) return null;

            twobytes = reader.ReadUInt16();
            if (twobytes == 0x8130) //data read as little endian order (actual data order for Sequence is 30 81)
                reader.ReadByte();    //advance 1 byte
            else if (twobytes == 0x8230)
                reader.ReadInt16();   //advance 2 bytes
            else
                return null;

            twobytes = reader.ReadUInt16();
            byte lowbyte = 0x00;
            byte highbyte = 0x00;

            if (twobytes == 0x8102) //data read as little endian order (actual data order for Integer is 02 81)
                lowbyte = reader.ReadByte();  // read next bytes which is bytes in modulus
            else if (twobytes == 0x8202)
            {
                highbyte = reader.ReadByte(); //advance 2 bytes
                lowbyte = reader.ReadByte();
            }
            else
                return null;

            byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };   //reverse byte order since asn.1 key uses big endian order
            int modsize = BitConverter.ToInt32(modint, 0);

            int firstbyte = reader.PeekChar();
            if (firstbyte == 0x00)
            {   //if first byte (highest order) of modulus is zero, don't include it
                reader.ReadByte();    //skip this null byte
                modsize -= 1;   //reduce modulus buffer size by 1
            }

            byte[] modulus = reader.ReadBytes(modsize);   //read the modulus bytes

            if (reader.ReadByte() != 0x02) return null;

            int expbytes = reader.ReadByte();        // should only need one byte for actual exponent data (for all useful values)
            byte[] exponent = reader.ReadBytes(expbytes);

            // ------- create RSACryptoServiceProvider instance and initialize with public key -----
            var rsa = RSA.Create();
            RSAParameters rsaKeyInfo = new RSAParameters
            {
                Modulus = modulus,
                Exponent = exponent
            };
            rsa.ImportParameters(rsaKeyInfo);

            return rsa;
        }

		private byte[] ConvertToPkcs1(byte[] pkcs8)
		{
			byte[] SeqOID = { 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00 };
			
			MemoryStream mem = new MemoryStream(pkcs8);
			int lenstream = (int) mem.Length;
			using BinaryReader reader = new BinaryReader(mem);    //wrap Memory Stream with BinaryReader for easy reading
			
			ushort twobytes = reader.ReadUInt16();
			if (twobytes == 0x8130)    //data read as little endian order (actual data order for Sequence is 30 81)
				reader.ReadByte();    //advance 1 byte
			else if (twobytes == 0x8230)
				reader.ReadInt16();    //advance 2 bytes
			else
				return null;

			byte bt = reader.ReadByte();
			if (bt != 0x02) return null;

			twobytes = reader.ReadUInt16();

			if (twobytes != 0x0001) return null;

			byte[] seq = reader.ReadBytes(15);        //read the Sequence OID
			if (!CompareBytearrays(seq, SeqOID))    //make sure Sequence for OID is correct
				return null;

			bt = reader.ReadByte();
			if (bt != 0x04) return null;

			bt = reader.ReadByte();        //read next byte, or next 2 bytes is  0x81 or 0x82; otherwise bt is the byte count
			if (bt == 0x81)
				reader.ReadByte();
			else if (bt == 0x82)
				reader.ReadUInt16();
			//------ at this stage, the remaining sequence should be the RSA private key

			return reader.ReadBytes((int)(lenstream - mem.Position));
		}

		private int GetIntegerSize(BinaryReader reader)
		{
			byte bt = reader.ReadByte();
			if (bt != 0x02) return 0;
			bt = reader.ReadByte();

			int count;
			if (bt == 0x81)
				count = reader.ReadByte();
			else if (bt == 0x82)
			{
				var highbyte = reader.ReadByte();
				var lowbyte = reader.ReadByte();
				byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };
				count = BitConverter.ToInt32(modint, 0);
			}
			else
				count = bt;

			while (reader.ReadByte() == 0x00)
			{
				count -= 1;
			}
			reader.BaseStream.Seek(-1, SeekOrigin.Current);

			return count;
		}

		private bool CompareBytearrays(byte[] a, byte[] b)
		{
			if (a.Length != b.Length) return false;

			int i = 0;
			foreach (byte c in a)
			{
				if (c != b[i]) return false;
				i++;
			}

			return true;
		}
		#endregion
	}
}