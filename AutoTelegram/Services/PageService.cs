using System;
using System.Windows.Controls;

namespace AutoTelegram.Services
{
    public class PageService
    {
        public event Action<Page> OnPageChanged;

        public void Navigate(Page page)
        {
            OnPageChanged?.Invoke(page);
        }
    }
}
