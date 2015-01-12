﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Surface.Presentation.Controls;
using System.Windows;

/*
 * List of articles
 */

namespace PhotoPaint
{
    class ArticleList
    {

        public List<Article> articles;

        /// <summary>
        /// constructor
        /// </summary>
        public ArticleList() 
        {
            articles = new List<Article>();
        }

        /// <summary>
        /// start animation for all elements
        /// </summary>
        public void animateAll() {
            foreach (Article article in articles)
            {
                article.moveItem(article.imageItem);
                article.moveItem(article.textItem);
            }
        }

        /// <summary>
        /// start animation for all elements
        /// </summary>
        public void stopAll()
        {
            foreach (Article article in articles)
            {
                article.imageStoryboard.Stop(Control.Instance.window1);
                article.textStoryboard.Stop(Control.Instance.window1);
            }
        }

        /// <summary>
        /// checks if a given image and a given text belong to the same article
        /// </summary>
        /// <param name="image">Image to check</param>
        /// <param name="text">Text/Headline to check</param>
        public bool belongsTogether(ScatterViewItem image, ScatterViewItem text)
        {
            foreach (Article article in articles)
            {
                if (article.imageItem.Equals(image))
                {
                    if (article.textItem.Equals(text))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// get the text item that belongs to an image
        /// </summary>
        /// <param name="image">Image to find the corresponding text for</param>
        public ScatterViewItem getTextItem(ScatterViewItem image)
        {
            foreach (Article article in articles)
            {
                if (article.imageItem.Equals(image))
                {
                    return article.textItem;
                }
            }
            return null;
        }

        /// <summary>
        /// get the image item that belongs to a text
        /// </summary>
        /// <param name="text">Text/Headline to find the corresponding image for</param>
        public ScatterViewItem getImageItem(ScatterViewItem text)
        {
            foreach (Article article in articles)
            {
                if (article.textItem.Equals(text))
                {
                    return article.imageItem;
                }
            }
            return null;
        }

    }
}