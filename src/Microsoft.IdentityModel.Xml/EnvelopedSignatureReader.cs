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

using System.Xml;
using static Microsoft.IdentityModel.Logging.LogHelper;

namespace Microsoft.IdentityModel.Xml
{
    /// <summary>
    /// Wraps a reader pointing to a root element of XML that may contain a signature.
    /// If a &lt; is found, <see cref="Signature"/> will be populated and <see cref="Signature.TokenSource"/> will
    /// be set to a <see cref="XmlTokenStreamReader"/> for future validation.
    /// </summary>
    public class EnvelopedSignatureReader : DelegatingXmlDictionaryReader
    {
        private int _elementCount;

        /// <summary>
        /// Initializes an instance of <see cref="EnvelopedSignatureReader"/>
        /// </summary>
        /// <param name="reader">a <see cref="XmlReader"/> pointing to XML that may contain an enveloped signature.</param>
        /// <remarks>If a &lt;Signature> element is found, a <see cref="Signature"/> will be popluated.</remarks>
        /// <exception cref="ArgumentNullException">'reader' is null.</exception>
        public EnvelopedSignatureReader(XmlReader reader)
        {
            if (reader == null)
                throw LogArgumentNullException(nameof(reader));

            TokenStreamReader = new XmlTokenStreamReader(reader);
            InnerReader = TokenStreamReader;
        }

        /// <summary>
        /// Gets the <see cref="XmlTokenStreamReader"/> that contains the XML nodes that were read.
        /// </summary>
        protected XmlTokenStreamReader TokenStreamReader
        {
            get;
        }

        /// <summary>
        /// Called after the root element has been completly read.
        /// Attaches a <see cref="XmlTokenStreamReader"/> to the signature for future processing if
        /// a signature was found.
        /// </summary>
        protected virtual void OnEndOfRootElement()
        {
            if (Signature != null)
                Signature.TokenSource = TokenStreamReader;
        }

        /// <summary>
        /// Gets the <see cref="Signature"/> that was found inside the XML.
        /// </summary>
        /// <remarksMmay be null.</remarks>
        public Signature Signature { get; private set; }

        /// <summary>
        /// Keeps track of the Xml Element count. If a signature is detected it is read.
        /// </summary>
        /// <returns>'true' if the next node was read successfully; 'false' if there are no more nodes.</returns>
        /// <exception cref="XmlReadException">if more than one signature is found.</exception>
        /// <exception cref="XmlReadException">if a &lt;Reference> element was not found in the &lt;SignedInfo>.</exception>
        public override bool Read()
        {
            if ((NodeType == XmlNodeType.Element) && (!base.IsEmptyElement))
                _elementCount++;

            if (NodeType == XmlNodeType.EndElement)
            {
                _elementCount--;
                if (_elementCount == 0)
                    OnEndOfRootElement();
            }

            bool result = InnerReader.Read();
            if (result
                && InnerReader.IsLocalName(XmlSignatureConstants.Elements.Signature)
                && InnerReader.IsNamespaceUri(XmlSignatureConstants.Namespace))
            {
                if (Signature != null)
                    throw XmlUtil.LogReadException(LogMessages.IDX21019);

                ReadSignature();
            }

            return result;
        }

        private void ReadSignature()
        {
            Signature = new Signature(InnerReader);
            if (Signature.SignedInfo.Reference == null)
                throw XmlUtil.LogReadException(LogMessages.IDX21101);
        }
    }
}
