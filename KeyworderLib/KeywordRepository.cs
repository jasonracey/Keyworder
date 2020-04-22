using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace KeyworderLib
{
    public class KeywordRepository
    {
        private readonly string path;

        public KeywordRepository(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));

            this.path = path;
        }

        public void CreateKeyword(string categoryId, string keywordId)
        {
            if (string.IsNullOrWhiteSpace(categoryId))
                throw new ArgumentNullException(nameof(categoryId));
            if (string.IsNullOrWhiteSpace(keywordId))
                throw new ArgumentNullException(nameof(keywordId));

            var document = XDocument.Load(path);

            var category = document.Descendants("Category")
                .Single(d => d.Attribute("CategoryId").Value == categoryId);
            if (category.Descendants("Keyword")
                .Count(d => d.Attribute("KeywordId").Value == keywordId) == 0)
            {
                category.Add(new XElement("Keyword", new XAttribute("KeywordId", keywordId)));
                document.Save(path);
            }
        }

        public void CreateCategory(string categoryId)
        {
            if (string.IsNullOrWhiteSpace(categoryId))
                throw new ArgumentNullException(nameof(categoryId));

            var document = XDocument.Load(path);

            if (document.Descendants("Category")
                .Count(d => d.Attribute("CategoryId").Value == categoryId) == 0)
            {
                document.Root?.Add(new XElement("Category", new XAttribute("CategoryId", categoryId)));
                document.Save(path);
            }
        }

        public void DeleteKeyword(string categoryId, string keywordId)
        {
            if (string.IsNullOrWhiteSpace(categoryId))
                throw new ArgumentNullException(nameof(categoryId));
            if (string.IsNullOrWhiteSpace(keywordId))
                throw new ArgumentNullException(nameof(keywordId));

            var document = XDocument.Load(path);

            if (document.Descendants("Category")
                    .Single(d => d.Attribute("CategoryId").Value == categoryId)
                    .Descendants("Keyword")
                    .Count(d => d.Attribute("KeywordId").Value == keywordId) > 0)
            {
                document.Descendants("Category")
                    .Single(d => d.Attribute("CategoryId").Value == categoryId)
                    .Descendants("Keyword")
                    .Single(d => d.Attribute("KeywordId").Value == keywordId)
                    .Remove();
                document.Save(path);
            }
        }

        public void DeleteCategory(string categoryId)
        {
            if (string.IsNullOrWhiteSpace(categoryId))
                throw new ArgumentNullException(nameof(categoryId));

            var document = XDocument.Load(path);

            if (document.Descendants("Category")
                .Count(d => d.Attribute("CategoryId").Value == categoryId) > 0)
            {
                document.Descendants("Category")
                    .Single(d => d.Attribute("CategoryId").Value == categoryId)
                    .Remove();
                document.Save(path);
            }
        }

        public SortedSet<Category> GetCategories()
        {
            var document = XDocument.Load(path);

            var categories = new SortedSet<Category>(new CategoryComparer());

            foreach (var categoryNode in document.Descendants("Category"))
            {
                var categoryId = categoryNode.Attribute("CategoryId").Value;
                var category = new Category(categoryId);
                foreach (var keywordNode in categoryNode.Elements("Keyword"))
                {
                    var keywordId = keywordNode.Attribute("KeywordId").Value;
                    category.Keywords.Add(new Keyword(categoryId, keywordId));
                }
                categories.Add(category);
            }
            
            return categories;
        }

        public void UpdateKeyword(string categoryId, string oldKeywordId, string newKeywordId)
        {
            if (string.IsNullOrWhiteSpace(categoryId))
                throw new ArgumentNullException(nameof(categoryId));
            if (string.IsNullOrWhiteSpace(oldKeywordId))
                throw new ArgumentNullException(nameof(oldKeywordId));
            if (string.IsNullOrWhiteSpace(newKeywordId))
                throw new ArgumentNullException(nameof(newKeywordId));

            var document = XDocument.Load(path);

            var keyword = document.Descendants("Category")
                .Single(d => d.Attribute("CategoryId").Value == categoryId)
                .Descendants("Keyword")
                .SingleOrDefault(d => d.Attribute("KeywordId").Value == oldKeywordId);
            if (keyword == null)
                throw new ArgumentNullException("keyword not found", oldKeywordId);
            keyword.SetAttributeValue("KeywordId", newKeywordId);

            document.Save(path);
        }

        public void UpdateCategory(string oldCategoryId, string newCategoryId)
        {
            if (string.IsNullOrWhiteSpace(oldCategoryId))
                throw new ArgumentNullException(nameof(oldCategoryId));
            if (string.IsNullOrWhiteSpace(newCategoryId))
                throw new ArgumentNullException(nameof(newCategoryId));

            var document = XDocument.Load(path);

            var category = document.Descendants("Category")
                .SingleOrDefault(d => d.Attribute("CategoryId")
                .Value == oldCategoryId);
            if (category == null)
                throw new ArgumentNullException("category not found", oldCategoryId);
            category.SetAttributeValue("CategoryId", newCategoryId);

            document.Save(path);
        }
    }
}