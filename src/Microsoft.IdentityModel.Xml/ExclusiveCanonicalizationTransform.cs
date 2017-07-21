//------------------------------------------------------------------------------
//
// Copyright (c) Microsoft Corporation.
// All rights reserved.
//
// This code is licensed under the MIT License.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files(the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and / or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions :
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
//------------------------------------------------------------------------------

using System.IO;
using System.Security.Cryptography;
using System.Xml;
using static Microsoft.IdentityModel.Logging.LogHelper;

namespace Microsoft.IdentityModel.Xml
{
    /// <summary>
    /// Canonicalization algorithms are found in &lt;SignedInfo> and &lt;Transform>.
    /// The element name can be: CanonicalizationMethod or Transform the actions are the same.
    /// </summary>
    public sealed class ExclusiveCanonicalizationTransform : Transform
    {
        public ExclusiveCanonicalizationTransform()
            : this(false)
        {
        }

        public ExclusiveCanonicalizationTransform(bool isCanonicalizationMethod)
            : this(isCanonicalizationMethod, false)
        {
        }

        public ExclusiveCanonicalizationTransform(bool isCanonicalizationMethod, bool includeComments)
        {
            ElementName = isCanonicalizationMethod ? XmlSignatureConstants.Elements.CanonicalizationMethod : XmlSignatureConstants.Elements.Transform;
            IncludeComments = includeComments;
            Algorithm = includeComments ? XmlSignatureConstants.ExclusiveC14nWithComments : XmlSignatureConstants.ExclusiveC14n;
            Prefix = XmlSignatureConstants.Prefix;
        }

        public string ElementName
        {
            get;
            private set;
        }

        public bool IncludeComments
        {
            get;
            private set;
        }

        public override XmlTokenStreamReader Process(XmlTokenStreamReader reader)
        {
            if (reader == null)
                throw LogArgumentNullException(nameof(reader));

            throw XmlUtil.LogReadException(LogMessages.IDX21106);
        }

        public override byte[] ProcessAndDigest(XmlTokenStreamReader reader, HashAlgorithm hash)
        {
            if (reader == null)
                LogArgumentNullException(nameof(reader));

            if (hash == null)
                LogArgumentNullException(nameof(hash));

            using (var stream = new MemoryStream())
            {
                reader.MoveToContent();
                WriteCanonicalStream(stream, reader, IncludeComments);
                stream.Flush();
                stream.Position = 0;
                return hash.ComputeHash(stream);
            }
        }

        public static void WriteCanonicalStream(Stream canonicalStream, XmlTokenStreamReader reader, bool includeComments)
        {
            XmlDictionaryWriter writer = XmlDictionaryWriter.CreateTextWriter(Stream.Null);
            writer.StartCanonicalization(canonicalStream, includeComments, null);
            reader.XmlTokens.WriteTo(writer);

            writer.Flush();
            writer.EndCanonicalization();

#if DESKTOPNET45 || DESKTOPNET451
            // TODO - what to use for net 1.4
            writer.Close();
#endif
        }

        public override void ReadFrom(XmlReader reader)
        {
            XmlUtil.CheckReaderOnEntry(reader, ElementName, XmlSignatureConstants.Namespace);

            Prefix = reader.Prefix;
            bool isEmptyElement = reader.IsEmptyElement;
            Algorithm = reader.GetAttribute(XmlSignatureConstants.Attributes.Algorithm, null);
            if (string.IsNullOrEmpty(Algorithm))
                throw XmlUtil.LogReadException(LogMessages.IDX21013, XmlSignatureConstants.Elements.Signature, XmlSignatureConstants.Attributes.Algorithm);

            if (Algorithm == XmlSignatureConstants.ExclusiveC14nWithComments)
                IncludeComments = true;
            else if (Algorithm == XmlSignatureConstants.ExclusiveC14n)
                IncludeComments = false;
            else
                throw XmlUtil.LogReadException(LogMessages.IDX21100, Algorithm, XmlSignatureConstants.ExclusiveC14nWithComments, XmlSignatureConstants.ExclusiveC14n);

            reader.Read();
            reader.MoveToContent();
            if (!isEmptyElement)
            {
                if (reader.IsStartElement(XmlSignatureConstants.ExclusiveC14nInclusiveNamespaces, XmlSignatureConstants.ExclusiveC14n))
                    throw XmlUtil.LogReadException(LogMessages.IDX21107);

                // </Transform>
                reader.MoveToContent();
                reader.ReadEndElement();
            }
        }

        public override void WriteTo(XmlWriter writer)
        {
            writer.WriteStartElement(Prefix, ElementName, XmlSignatureConstants.Namespace);
            writer.WriteAttributeString(XmlSignatureConstants.Attributes.Algorithm, null, Algorithm);
            writer.WriteEndElement();
        }
    }
}
