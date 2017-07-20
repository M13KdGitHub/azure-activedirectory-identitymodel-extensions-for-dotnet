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


using System.Collections.Generic;
using System.Xml;

namespace Microsoft.IdentityModel.Xml
{
    public class XmlTokenStream
    {
        private List<XmlTokenEntry> _entries = new List<XmlTokenEntry>();
        private string _excludedElement;
        private string _excludedElementNamespace;

        public XmlTokenStream()
        {
        }

        public void Add(XmlNodeType type, string value)
        {
            _entries.Add(new XmlTokenEntry(type, value));
        }

        public void AddAttribute(string prefix, string localName, string namespaceUri, string value)
        {
            _entries.Add(new XmlTokenEntry(XmlNodeType.Attribute, prefix, localName, namespaceUri, value));
        }

        public void AddElement(string prefix, string localName, string namespaceUri, bool isEmptyElement)
        {
             _entries.Add(new XmlTokenEntry(XmlNodeType.Element, prefix, localName, namespaceUri, isEmptyElement));
        }

        public void SetElementExclusion(string excludedElement, string excludedElementNamespace)
        {
            _excludedElement = excludedElement;
            _excludedElementNamespace = excludedElementNamespace;
        }

        public void WriteTo(XmlDictionaryWriter writer)
        {
            var streamWriter = new XmlTokenStreamWriter(_entries, _excludedElement, _excludedElementNamespace);
            streamWriter.WriteTo(writer);
        }
    }
}
