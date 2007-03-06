using System;

namespace Rainbow.Core.Tests
{
    using Rainbow.Core;

    /// <summary>
    /// Summary description for Class1
    /// </summary>
    public class PageTest
    {
        class EventListener
        {
            private Page testPage;

            public EventListener(Page page)
            {
                testPage = page;
                // Add "PageChanged" to the Changed event on "testPage".
                testPage.Changed += new ChangedEventHandler(PageChanged);
            }

            // This will be called whenever the page changes.
            private void PageChanged(object sender, EventArgs e)
            {
                Console.WriteLine("This is called when the event fires.");
            }

            public void Detach()
            {
                // Detach the event and delete the list
                testPage.Changed -= new ChangedEventHandler(PageChanged);
                testPage = null;
            }
        }

        /// <summary>
        /// Test the ListWithChangedEvent class.
        /// </summary>
        public PageTest()
        {
            // Create a new list.
            Page page = new Page();

            // Create a class that listens to the page's change event.
            EventListener listener = new EventListener(page);

            // Add and remove items from the list.
            page.Id = 1234;
            page.Id = 0;

            listener.Detach();
        }
    }
}