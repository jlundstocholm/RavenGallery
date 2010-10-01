﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client;
using RavenGallery.Core.Documents;

namespace RavenGallery.Core.Views
{
    public class ImageBrowseViewFactory : IViewFactory<ImageBrowseInputModel, ImageBrowseView>
    {
        private IDocumentSession documentSession;

        public ImageBrowseViewFactory(IDocumentSession documentSession)
        {
            this.documentSession = documentSession;
        }

        public ImageBrowseView Load(ImageBrowseInputModel input)
        {
            // Adjust the model appropriately
            input.PageSize = input.PageSize == 0 || input.PageSize > 20 ? 20 : input.PageSize;

            var items = documentSession.Query<ImageDocument>("Raven/DocumentsByEntityName")
                .Customize(x=>x.Where("Tag:ImageDocuments"))
                .Skip(input.Page * input.PageSize)
                .Take(input.PageSize)
                .ToArray()
                .Select(x => new ImageBrowseItem(x.Title, x.Filename));
               
            return new ImageBrowseView(
                input.Page,
                input.PageSize,
                items);
        }
    }
}
