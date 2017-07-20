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

#if EncryptedTokens

using System.Xml;

namespace Microsoft.IdentityModel.Xml
{
    internal class EncryptionMethodElement
    {
        public string Algorithm { get; set; }

        public string Parameters { get; set; }

        public void ReadXml(XmlDictionaryReader reader)
        {
            XmlUtil.CheckReaderOnEntry(reader, XmlEncryptionConstants.Elements.EncryptionMethod, XmlEncryptionConstants.Namespace);

            Algorithm = reader.GetAttribute(XmlEncryptionConstants.Attributes.Algorithm, null);
            if (!reader.IsEmptyElement)
            {
                //
                // Trace unread missing element
                //

                string xml = reader.ReadOuterXml();
            }
            else
            {
                //
                // Read to the next element
                //
                reader.Read();
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            // <EncryptionMethod>
            writer.WriteStartElement(XmlEncryptionConstants.Prefix, XmlEncryptionConstants.Elements.EncryptionMethod, XmlEncryptionConstants.Namespace);

            writer.WriteAttributeString(XmlEncryptionConstants.Attributes.Algorithm, null, Algorithm);

            // </EncryptionMethod>
            writer.WriteEndElement();
        }

    }
}
#endif