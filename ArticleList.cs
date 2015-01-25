using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Surface.Presentation.Controls;
using System.Windows;

/*
 * List of articles
 */

namespace NeSt
{
    class ArticleList
    {

        public List<Article> articles;

        public readonly int MAXON = 5; // How many articles should be available at the same time?
        private int appearIndex; // index on which new images start to appear

        /// <summary>
        /// constructor
        /// </summary>
        public ArticleList() 
        {
            articles = new List<Article>();
            appearIndex = MAXON;
        }

        /// <summary>
        /// initial setting for the game
        /// </summary>
        public void initialize()
        {
            for ( int i = 0; i < MAXON; i++ )
            {
                articles[i].setStatus(1);
                articles[i].setStatus(2);
            }
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

        /// <summary>
        /// let new article pieces appear
        /// </summary>
        public void showNext()
        {

            // show next two pieces

            Article article1 = null;
            Article article2 = null;

            // check requirement for next while loop
            if (articles.Count() < MAXON + 3 + 2)
            {
                MessageBox.Show("Not enough articles found");
                return;
            }

            // get first article to show a piece from
            while (article1 == null)
            {
                if (articles[appearIndex].getStatus() == 0 || articles[appearIndex].getStatus() == 1)
                {
                    article1 = articles[appearIndex];
                    article1.setStatus(article1.getStatus() + 1);
                }
                appearIndex = (appearIndex + 1) % articles.Count;
            }

            // get second article to show a piece from
            int index2 = (appearIndex + 1) % articles.Count;
            while (article2 == null)
            {
                if (articles[index2].getStatus() == article1.getStatus() - 1)
                {
                    article2 = articles[index2];
                    article2.setStatus(article2.getStatus() + 1);
                }
                index2 = (index2 + 1) % articles.Count;
            }

        }

    }
}
