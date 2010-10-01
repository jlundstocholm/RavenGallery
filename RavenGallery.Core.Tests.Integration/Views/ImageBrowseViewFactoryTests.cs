﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using RavenGallery.Core.Views;
using Raven.Client;
using RavenGallery.Core.Entities;
using RavenGallery.Core.Documents;

namespace RavenGallery.Core.Tests.Integration.Views
{
    [TestFixture]
    public class ImageBrowseViewFactoryTests : LocalRavenTest
    {
        public ImageBrowseViewFactory ViewFactory { get; set; }
        public IDocumentSession DocumentSession { get; set; }

        [SetUp]
        public void SetupObjects()
        {
            DocumentSession = this.Store.OpenSession();
            this.ViewFactory = new ImageBrowseViewFactory(this.DocumentSession);
        }

        [Test]
        [TestCase(0)]
        [TestCase(21)]
        public void WhenLoadIsInvokedWithInvalidPageSizePageSizeIsResetToSensibleDefault(int pageSize)
        {
            var results = this.ViewFactory.Load(new ImageBrowseInputModel()
            {
                Page = 0,
                PageSize = pageSize
            });

            Assert.AreEqual(20, results.PageSize);
        }

        [Test]
        [TestCase(0, 10)]
        [TestCase(5, 10)]
        [TestCase(2, 5)]
        public void WhenLoadIsInvokedWithSuppliedArgumentsExpectedResultsAreReturned(int page, int pageSize)
        {
            PopulateStore();
            var results = this.ViewFactory.Load(new ImageBrowseInputModel()
            {
                 Page = page,
                 PageSize = pageSize
            });

            Assert.AreEqual(pageSize, results.Items.Count());
            int pageStart = page * pageSize;
            for (int x = pageStart; x < pageStart + pageSize; x++)
            {
                var item = results.Items.ElementAt(x - pageStart);
                Assert.AreEqual(string.Format("Title{0}", x), item.Title);
                Assert.AreEqual(string.Format("Filename{0}", x), item.Filename);
            }
        }

        private void PopulateStore()
        {
 	        for(int x = 0; x < 100 ; x++){
                DocumentSession.Store(
                    new ImageDocument()
                    {
                        Title = string.Format("Title{0}", x),
                        Filename = string.Format("Filename{0}", x)
                    });
            }
            DocumentSession.SaveChanges();
            DocumentSession.Clear();
            WaitForIndexing();
        }

        [TearDown]
        public void DestroyObjects()
        {
            this.DocumentSession.Dispose();
        }
    }
}
