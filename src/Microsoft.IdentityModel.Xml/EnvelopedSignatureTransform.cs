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

using System.Security.Cryptography;
using System.Xml;
using static Microsoft.IdentityModel.Logging.LogHelper;
using static Microsoft.IdentityModel.Xml.XmlSignatureConstants;
using static Microsoft.IdentityModel.Xml.XmlUtil;

namespace Microsoft.IdentityModel.Xml
{
    public sealed class EnvelopedSignatureTransform : Transform
    {
        private string _prefix = Prefix;

        /// <summary>
        /// Creates an EnvelopedSignatureTransform
        /// </summary>
        public EnvelopedSignatureTransform()
        {
            Algorithm = Algorithms.EnvelopedSignature;
        }

        /// <summary>
        /// Sets the reader to exclude the &lt;Signature> element
        /// </summary>
        /// <param name="reader"><see cref="XmlTokenStreamReader"/>to set.</param>
        /// <returns><see cref="XmlTokenStreamReader"/>with exclusion set.</returns>
        public override XmlTokenStreamReader Process(XmlTokenStreamReader reader)
        {
            if (reader == null)
                LogArgumentNullException(nameof(reader));

            reader.XmlTokens.SetElementExclusion(Elements.Signature, Namespace);
            return reader;
        }

        /// <summary>
        /// Not supported for EnvelopeSignatureTransform
        /// </summary>
        /// <param name="reader">not applicable</param>
        /// <param name="hash">not applicable</param>
        /// <returns></returns>
        public override byte[] ProcessAndDigest(XmlTokenStreamReader reader, HashAlgorithm hash)
        {
            throw LogReadException(LogMessages.IDX21104);
        }

        /// <summary>
        /// Reads and populates a <see cref="EnvelopedSignatureTransform"/> from XML
        /// </summary>
        /// <param name="reader"><see cref="XmlReader"/>positioned at a TransForm element.</param>
        public override void ReadFrom(XmlReader reader)
        {
            reader.MoveToContent();
            string algorithm = XmlUtil.ReadEmptyElementAndRequiredAttribute(reader,
                Elements.Transform, Namespace, Attributes.Algorithm, out _prefix);

            if (algorithm != Algorithm)
                throw LogReadException(LogMessages.IDX21105, Algorithm, algorithm);
        }

        public override void WriteTo(XmlWriter writer)
        {
            writer.WriteStartElement(_prefix, Elements.Transform, Namespace);
            writer.WriteAttributeString(Attributes.Algorithm, null, Algorithm);
            writer.WriteEndElement();
        }
    }
}
