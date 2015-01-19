using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
 * contains the last few articles that were finished
 */

namespace NeSt
{
    class FinishedArticles
    {

        private List<Article> finishedArticles;
        public readonly int capacity = 3; // how many articles can be shown

        /// <summary>
        /// constructor
        /// </summary>
        public FinishedArticles()
        {
            finishedArticles = new List<Article>();
        }

        /// <summary>
        /// Get the article list
        /// </summary>
        public List<Article> getList()
        {
            return finishedArticles;
        }

        /// <summary>
        /// Add an article to the list
        /// </summary>
        /// <param name="article">The article to add</param>
        public void add(Article article)
        {
            if (finishedArticles.Count >= capacity)
            {
                finishedArticles[0].setStatus(0);
                finishedArticles.RemoveAt(0);
            }
            finishedArticles.Add(article);

            string headlines = "";
            for (int i = 0; i < finishedArticles.Count; i++)
            {
                headlines += finishedArticles[finishedArticles.Count - i - 1].textItem.Content;
                headlines += Environment.NewLine;
                headlines += Environment.NewLine;
            }

            foreach (Player player in Control.Instance.playerList.players) {
                player.island.updateArticles();
            }
            //Control.Instance.playerList.players[0].island.finishedArticles.Content = headlines;
            //Control.Instance.playerList.players[1].island.finishedArticles.Content = headlines;
            //Control.Instance.playerList.players[2].island.finishedArticles.Content = headlines;
            //Control.Instance.playerList.players[3].island.finishedArticles.Content = headlines;
        }

        /// <summary>
        /// reset the List to an empty one
        /// </summary>
        public void reset()
        {
            finishedArticles = new List<Article>();
        }

    }
}
