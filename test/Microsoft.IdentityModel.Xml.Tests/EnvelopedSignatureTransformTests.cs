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

using System;
using System.Xml;
using Microsoft.IdentityModel.Tests;
using Microsoft.IdentityModel.Xml;
using Xunit;

namespace Microsoft.IdentityModel.Xml.Tests
{
    public class EnvelopedSignatureTransformTests
    {
        [Fact]
        public void Constructor()
        {
            TestUtilities.WriteHeader($"{this}", "Constructor", true);
            var transform = new EnvelopedSignatureTransform();
        }

        [Fact]
        public void ProcessAndDigest()
        {
            TestUtilities.WriteHeader($"{this}", "ProcessAndDigest", true);
            var transform = new EnvelopedSignatureTransform();
            var expectedException = new ExpectedException(typeof(XmlReadException), "IDX21104:");
            try
            {
                transform.ProcessAndDigest(null, null);
                expectedException.ProcessNoException();
            }
            catch (Exception ex)
            {
                expectedException.ProcessException(ex);
            }
        }

#pragma warning disable CS3016 // Arrays as attribute arguments is not CLS-compliant
        [Theory, MemberData("ReadFromTheoryData")]
#pragma warning restore CS3016 // Arrays as attribute arguments is not CLS-compliant
        public void ReadFrom(EnvelopedSignatureTransformTheoryData theoryData)
        {
            TestUtilities.WriteHeader($"{this}.ReadFrom", theoryData);
            var context = new CompareContext($"{this}.ReadFrom, {theoryData.TestId}");
            var reader = XmlUtilities.CreateDictionaryReader(theoryData.Xml);
            try
            {
                var transform = new EnvelopedSignatureTransform();
                transform.ReadFrom(reader);
                theoryData.ExpectedException.ProcessNoException(context);
                IdentityComparer.AreTransformsEqual(transform, theoryData.Transform, context);
            }
            catch (Exception ex)
            {
                theoryData.ExpectedException.ProcessException(ex, context.Diffs);
            }

            TestUtilities.AssertFailIfErrors(context);
        }

        public static TheoryData<EnvelopedSignatureTransformTheoryData> ReadFromTheoryData
        {
            get
            {
                // uncomment to view exception displayed to user
                ExpectedException.DefaultVerbose = true;

                return new TheoryData<EnvelopedSignatureTransformTheoryData>
                {
                    new EnvelopedSignatureTransformTheoryData
                    {                      
                        First = true,
                        TestId = nameof(ReferenceXml.EnvelopedSignatureTransform_Valid_WithPrefix),
                        Transform = new EnvelopedSignatureTransform
                        {
                            Algorithm = XmlSignatureConstants.Algorithms.EnvelopedSignature,
                            Prefix = XmlSignatureConstants.Prefix
                        },
                        Xml = ReferenceXml.EnvelopedSignatureTransform_Valid_WithPrefix.Xml
                    },
                    new EnvelopedSignatureTransformTheoryData
                    {
                        TestId = nameof(ReferenceXml.EnvelopedSignatureTransform_Valid_WithOutPrefix),
                        Transform = new EnvelopedSignatureTransform
                        {
                            Algorithm = XmlSignatureConstants.Algorithms.EnvelopedSignature,
                            Prefix = String.Empty
                        },
                        Xml = ReferenceXml.EnvelopedSignatureTransform_Valid_WithOutPrefix.Xml
                    },
                    new EnvelopedSignatureTransformTheoryData
                    {
                        ExpectedException = new ExpectedException(typeof(XmlReadException), "IDX21105:"),
                        TestId = nameof(ReferenceXml.EnvelopedSignatureTransform_AlgorithmNotEnveloped),
                        Xml = ReferenceXml.EnvelopedSignatureTransform_AlgorithmNotEnveloped.Xml
                    },
                    new EnvelopedSignatureTransformTheoryData
                    {
                        ExpectedException = new ExpectedException(typeof(XmlReadException), "IDX21013:"),
                        TestId = nameof(ReferenceXml.EnvelopedSignatureTransform_AlgorithmMissing),
                        Xml = ReferenceXml.EnvelopedSignatureTransform_AlgorithmMissing.Xml
                    },
                    new EnvelopedSignatureTransformTheoryData
                    {
                        ExpectedException = new ExpectedException(typeof(XmlReadException), "IDX21013:"),
                        TestId = nameof(ReferenceXml.EnvelopedSignatureTransform_AlgorithmNull),
                        Xml = ReferenceXml.EnvelopedSignatureTransform_AlgorithmNull.Xml
                    },
                    new EnvelopedSignatureTransformTheoryData
                    {
                        ExpectedException = ExpectedException.ArgumentNullException(),
                        TestId = "XmlReaderNull",
                    }
                };
            }
        }
    }

    public class EnvelopedSignatureTransformTheoryData : TheoryDataBase
    {
        public EnvelopedSignatureTransform Transform { get; set; }

        public string Xml { get; set; }

        public XmlTokenStreamReader XmlTokenStreamReader { get; set; }

        public XmlWriter XmlWriter { get; set; }

        public override string ToString()
        {
            return $"{TestId}, {ExpectedException}";
        }
    }
}
